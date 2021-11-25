using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeFileTools
{
    [FileTypeAttribute(FileTypeEnum.Entity_GDSJG)]
    public class EntityProcess_GDSJG : IFileProcess
    {
        public void CreateFile(List<string> prefix, CreateOptions option, Tuple<List<TbDesc>, List<IndexDesc>> ret)
        {
            var csName = prefix[1];
            var tbDesc = ret.Item1;

            //string fullFilePath = Path.Combine(option.Abspath, csName , ".cs"); //Error

            string fullFilePath = Path.Combine(option.Abspath, csName + "Input.cs");
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            Console.WriteLine($" ----开始生成实体类 '{csName}.cs', 文件存放路径：'{option.Abspath}' ----");
            using (File.Create(fullFilePath))
            {

            }
            Console.WriteLine("    --生成成功--   ");

            var content = new StringBuilder();
            //using
            content.Append(
            "using System;\r\n" +
            "using System.ComponentModel.DataAnnotations;\r\n" +
            "using System.Collections.Generic;\r\n" +
            "using XYS.HISNetApi.Web.Core.Enums;\r\n");

            #region namespace
            content.Append($"namespace {option.Space}\r\n");
            content.Append("{\r\n");

            #region EntityClass
            content.Append(GetTabContent($"public class {csName}Input : ProviSuperviseBase_GDSJG<BaseOut>", 1));
            content.Append(GetTabContent("{", 1));

            //构造方法
            content.Append(GetTabContent("public override Dictionary<HospitalCode, Type> MapperTypeDic => new Dictionary<HospitalCode, Type>", 2));
            content.Append(GetTabContent("{", 2));
            content.Append(GetTabContent("{HospitalCode.GDSJG,typeof(ProviSuperviseNetApi.GD.Application." + csName+")}", 3));
            content.Append(GetTabContent("};", 2));
            content.Append("\r\n");

            #region Class Content
            foreach (var fItem in tbDesc)
            {
                // summary
                content.Append(GetTabContent("/// <summary>", 2));
                content.Append(GetTabContent($"/// {fItem.Remark}", 2));
                content.Append(GetTabContent("/// </summary>", 2));

                //if (fItem.CLRTypeString == "decimal")
                //{
                //    content.Append(GetTabContent($"[Column(TypeName = \"{fItem.DbFieldType}\")]", 2));
                //}

                //if (fItem.CLRTypeString == "string" && fItem.MaxLength.HasValue) //MaxLength标签
                //{
                //    content.Append(GetTabContent($"[MaxLength({fItem.MaxLength.Value}," + "ErrorMessage = \"{0}最大长度{1}\")]", 2));
                //}

                content.Append(GetTabContent($"public {fItem.CLRTypeString}{(!fItem.IsNeeded && fItem.CLRTypeString != "string" ? "?" : "")} {fItem.FieldName} " + "{ get; set; }", 2));

                content.Append("\r\n");
            }

            #endregion

            content.Append(GetTabContent("}", 1));
            #endregion

            content.Append("\r\n");

            content.Append("}\r\n");
            #endregion

            File.AppendAllText(fullFilePath, content.ToString());
        }

        public static string GetTabContent(string content, int tabCount)
        {
            for (var i = 0; i < tabCount * 4; i++)
            {
                content = " " + content;
            }

            content += "\r\n";

            return content;
        }
    }
}
