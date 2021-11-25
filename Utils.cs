using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Excel;
using Excel.OpenXml;
using Excel.Utils;
using Microsoft.Extensions.Configuration;

namespace CodeFileTools
{
    public static class Utils
    {
        public static IConfigurationRoot Config;
        public static string ContextName;
        public static List<CreateOptions> Options;
        public static string IgnoreFields;
        public static Dictionary<FileTypeEnum, IFileProcess> dicType = new Dictionary<FileTypeEnum, IFileProcess>();

        static Utils()
        {
            InitDicType();

            Reload();
        }

        public static void InitDicType()
        {
            var types = new List<Type>();
            AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach(ass =>
            {
                types.AddRange(ass.GetTypes().Where(t => t.GetInterface(nameof(IFileProcess)) != null));
            });

            foreach (var type in types)
            {
                var attrs = type.GetCustomAttributes<FileTypeAttribute>();
                var fileTypeAttributes = attrs as FileTypeAttribute[] ?? attrs.ToArray();
                if (!fileTypeAttributes.Any())
                    continue;
                dicType.Add(fileTypeAttributes.First().FileType, Activator.CreateInstance(type) as IFileProcess);
            }
        }

        /// <summary>
        /// 重载配置文件
        /// </summary>
        public static void Reload()
        {
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("AppSettings.Json");
            Config = configurationBuilder.Build();
            ContextName = Config["ContextName"];
            Options = Config.GetSection("ConfigOptions").Get<List<CreateOptions>>();
            IgnoreFields = Config["IgnoreFields"];
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="filePath"></param>
        public static void StartCreateFile(string filePath)
        {
            //获取第一个文件
            var files = new DirectoryInfo(filePath).GetFiles().Where(m => !m.Name.Contains("$")).ToList();
            var file = files[0];
            var sheetNames = MyMiniExcel.GetSheetNames(file.FullName);

            Console.WriteLine($"----开始检查Excel表 '{file.Name}' ----");

            using (FileStream stream = Helpers.OpenSharedRead(file.FullName))
            {
                var xmlSheetReader = new ExcelOpenXmlSheetReader(stream);

                foreach (var sheetName in sheetNames)
                {
                    if (sheetName.Contains("~"))
                    {
                        Console.WriteLine($"sheet表：'{sheetName}'，跳过生成!");
                        continue;
                    }

                    Console.WriteLine($"开始检查sheet表 '{sheetName}': ");
                    //数据库表名
                    var neededItems = new List<string>();
                    var sheetSpitArray = sheetName.Split("-").ToList();
                    neededItems.Add(sheetSpitArray[0]);

                    var tbSpitArray = sheetSpitArray[0].Split("_").ToList();

                    var sbCsName = new StringBuilder();
                    var flag = true;
                    foreach (var item in tbSpitArray)
                    {
                        if (flag)
                        {
                            flag = false;
                            continue;
                        }

                        sbCsName.Append(item.FirstLeterUpper());
                    }

                    neededItems.Add(sbCsName.ToString());
                    neededItems.Add(sheetSpitArray[1]);
                    var queryRet = xmlSheetReader.Query(true, sheetName, "A1", null);

                    //得到表字段属性，以及索引属性--
                    var ret = GetExcelResolveRet(queryRet);

                    //初始化字段 然后通过接口导入
                    foreach (var option in Options.Where(m=>m.IsCreate))
                    {
                        dicType[option.Sort].CreateFile(neededItems, option, ret);
                    }
                }
            }
        }

        //处理数据库设计表格数据
        public static Tuple<List<TbDesc>, List<IndexDesc>> GetExcelResolveRet(IEnumerable<IDictionary<string, object>> dicList)
        {
            var ret = new Tuple<List<TbDesc>, List<IndexDesc>>(new List<TbDesc>(),new List<IndexDesc>());
            var flag = 1;//1：数据字段，2：索引

            var rowIndex = 2;

            //var remarkList = new List<string>();
            var noMergeFlag = true;
            var lastModel = new TbDesc();
            foreach (var row in dicList)
            {
                if (flag == 1)
                {
                    var model1 = new TbDesc();
                    var addFlag = true;

                    foreach (var rowItem in row)
                    {
                        object itemValue = rowItem.Value;

                        //索引表头列跳过
                        if (itemValue != null && itemValue.ToString().Equals("序号"))
                        {
                            flag = 2;
                            addFlag = false;
                            break;
                        }

                        // 主键，默认值 不判断
                        if (rowItem.Key=="主键" || rowItem.Key == "默认值")
                        {
                            continue;
                        }

                        if (rowItem.Key == "字段名") //字段名
                        {
                            if (itemValue == null)
                            {
                                if (row["序号"] != null)
                                {
                                    throw new Exception($"第{rowIndex}行，字段名不能为空");
                                }

                                addFlag = false;
                                noMergeFlag = false;
                                continue;
                            }

                            //公共字段不判断--
                            if (IgnoreFields.Contains(itemValue.ToString().ToUpper()))
                            {
                                addFlag = false;
                                break;
                            }

                            noMergeFlag = true;
                            lastModel = model1;
                            model1.FieldName = itemValue.ToString();
                        }
                        else if (rowItem.Key == "类型") //类型
                        {
                            if (itemValue == null)
                            {
                                if (row["序号"] != null)
                                {
                                    throw new Exception($"第{rowIndex}行，类型不能为空");
                                }

                                addFlag = false;
                                continue;
                            }

                            model1.CLRTypeString = GetCLRTypeString(itemValue.ToString());
                            model1.DbFieldType = itemValue.ToString();
                        }
                        else if (rowItem.Key == "长度" && itemValue!=null)//长度
                        {
                            model1.MaxLength = Convert.ToInt32(itemValue.ToString());
                        }
                        else if (rowItem.Key == "允许空")//允许空
                        {
                            if (itemValue == null || string.IsNullOrEmpty(itemValue.ToString()))
                            {
                                model1.IsNeeded = true;
                            }
                        }
                        else if (rowItem.Key == "字段说明" && itemValue != null)//字段说明
                        {
                            if (noMergeFlag)
                                model1.Remark = itemValue.ToString();
                            else
                                lastModel.Remark += itemValue.ToString();
                        }
                    }

                    if(addFlag && noMergeFlag)
                        ret.Item1.Add(model1);
                }
                else if (flag == 2)
                {
                    var model2 = new IndexDesc();
                    var indexColumnIndex = 1;
                    foreach (var rowItem in row)
                    {
                        object itemValue = rowItem.Value;
                        if (indexColumnIndex == 2)
                        {
                            if (itemValue == null)
                                throw new Exception($"第{rowIndex}行，索引名不能为空");

                            model2.IndexName= rowItem.Value.ToString();
                        }
                        else if (indexColumnIndex == 5)
                        {
                            if (itemValue == null)
                                throw new Exception($"第{rowIndex}行，字段名不能为空");
                            model2.FieldName = rowItem.Value.ToString();
                        }

                        indexColumnIndex++;
                    }
                    ret.Item2.Add(model2);
                }

                rowIndex++;
            }

            return ret;
        }

        /// <summary>
        /// 获取CLR 数据类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Type GetCLRType(string dbType)
        {
            Type type = typeof(string);
            if (dbType.Equals("longtext"))
            {
                type = typeof(string);
            }
            else if (dbType.StartsWith("varchar"))
            {
                type = typeof(string);
            }
            else if (dbType.StartsWith("decimal"))
            {
                type = typeof(decimal);
            }
            else if (dbType.StartsWith("int"))
            {
                type = typeof(int);
            }
            else if (dbType.StartsWith("bit") || dbType.StartsWith("tinyint"))
            {
                type = typeof(bool);
            }
            else if (dbType.StartsWith("datetime") || dbType.StartsWith("date"))
            {
                type = typeof(DateTime);
            }

            return type;
        }

        /// <summary>
        /// 获取CLR 数据类型 字符串
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string GetCLRTypeString(string dbType)
        {
            string type = string.Empty;
            dbType = dbType.ToLower();
            if (dbType.StartsWith("varchar"))
            {
                type = "string";
            }
            else if (dbType.StartsWith("decimal"))
            {
                type = "decimal";
            }
            else if (dbType.StartsWith("int") || dbType.StartsWith("tinyint"))
            {
                type = "int";
            }
            else if (dbType.StartsWith("datetime") || dbType.StartsWith("date"))
            {
                type = "DateTime";
            }

            return type;
        }

        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FirstLeterLower(this string s)
        {
            return s.Substring(0, 1).ToLower() + s.Substring(1);
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FirstLeterUpper(this string s)
        {
            return s.Substring(0, 1).ToUpper() + s.Substring(1);
        }

        /// <summary>
        /// 生成文件GDSJG
        /// </summary>
        /// <param name="filePath"></param>
        public static void StartCreateFile_GDSJG(string filePath)
        {
            //获取第一个文件
            var files = new DirectoryInfo(filePath).GetFiles().Where(m => !m.Name.Contains("$")).ToList();
            var file = files[0];
            var sheetNames = MyMiniExcel.GetSheetNames(file.FullName);

            Console.WriteLine($"----开始检查Excel表 '{file.Name}' ----");

            using (FileStream stream = Helpers.OpenSharedRead(file.FullName))
            {
                var xmlSheetReader = new ExcelOpenXmlSheetReader(stream);

                foreach (var sheetName in sheetNames)
                {
                    if (sheetName.Contains("~"))
                    {
                        Console.WriteLine($"sheet表：'{sheetName}'，跳过生成!");
                        continue;
                    }

                    Console.WriteLine($"开始检查sheet表 '{sheetName}': ");

                    //数据库表名
                    var neededItems = new List<string>();
                    var sheetSpitArray = sheetName.Split("-").ToList();

                    neededItems.Add(sheetSpitArray[0]);
                    neededItems.Add(sheetSpitArray[1]);
                    var queryRet = xmlSheetReader.Query(true, sheetName, "A1", null);

                    //得到表字段属性，以及索引属性--
                    var ret = GetExcelResolveRet_GDSJG(queryRet);

                    //初始化字段 然后通过接口导入
                    foreach (var option in Options.Where(m => m.IsCreate && m.Sort == FileTypeEnum.Entity_GDSJG))
                    {
                        dicType[option.Sort].CreateFile(neededItems, option, ret);
                    }
                }
            }
        }

        //处理数据库设计表格数据-GDSJG
        public static Tuple<List<TbDesc>, List<IndexDesc>> GetExcelResolveRet_GDSJG(IEnumerable<IDictionary<string, object>> dicList)
        {
            var ret = new List<TbDesc>();
            var flag = true;
            foreach (var row in dicList)
            {
                if (flag)
                {
                    flag = false;
                    continue;
                }

                var model1 = new TbDesc();
                //公共字段跳过
                if (IgnoreFields.Contains(row["字段名称"].ToString()))
                    continue;
                /*
                 数据项	字段名称	数据类型	长 度	填报要求	说明	值域
                机构标识	JGDM	字符串	30	应填	见机构信息表机构标识。	
                */

                model1.CLRTypeString = GetCLRTypeString_GDSJG(row["数据类型"].ToString());

                model1.FieldName = row["字段名称"].ToString();

                if (row["说明"] == null)
                {
                    continue;
                }

                model1.Remark = row["填报要求"]?.ToString().Replace("\n", "") + " / " +
                                row["数据项"]?.ToString().Replace("\n", "") + " / " + row["说明"]?.ToString().Replace("\n",""); 
                if(!string.IsNullOrEmpty(row["值域"]?.ToString()))
                    model1.Remark+=" / " + row["值域"]?.ToString().Replace("\n", "");


                if (row["数据类型"].ToString().Contains("字符串"))//长度
                {
                    model1.MaxLength = Convert.ToInt32(row["长度"].ToString());
                }
                ret.Add(model1);
            }

            return new Tuple<List<TbDesc>, List<IndexDesc>>(item1:ret,item2:new List<IndexDesc>());
        }

        /// <summary>
        /// 获取CLR 数据类型 字符串-GDSJG
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static string GetCLRTypeString_GDSJG(string dbType)
        {
            string type = string.Empty;
            dbType = dbType.ToLower();
            if (dbType.Contains("字符"))
            {
                type = "string";
            }
            else if (dbType.StartsWith("数值"))
            {
                type = "decimal";
            }
            //else if (dbType.StartsWith("int") || dbType.StartsWith("tinyint"))
            //{
            //    type = "int";
            //}
            else if (dbType.Contains("日期"))
            {
                type = "DateTime";
            }

            return type;
        }
    }
}
