using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace CodeFileTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            AssemblyLearning();
        }

        private void BtnStartCreate_Click(object sender, EventArgs e)
        {
            Utils.StartCreateFile(TxtDirPath.Text);
        }

        private void AssemblyLearning()
        {
            //var loadAssembly = Assembly.Load("CodeFileTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            //var ins = (TbDesc)loadAssembly.CreateInstance("CodeFileTools.TbDesc");//命名空间加类名
            //ins?.TbDescSay();

            //检索包含Int32的程序集，并显示其名称和文件位置。
            //Assembly assemb = Assembly.GetAssembly(typeof(int));
            //Console.WriteLine($"{assemb.FullName} - {assemb.Location} - {assemb.CodeBase}");

            //AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach(o =>
            //    types.AddRange(o.GetTypes().Where(o => o.GetInterface(nameof(IRechargeOperation)) != null && o.IsInterface)));
            //_typeMaps = new Dictionary<(OrderTypeEnum, AppSourceType), Type>();
            //foreach (var type in types)
            //{
            //    var attrs = type.GetCustomAttributes<OrderTypeAttribute>();
            //    var orderTypeAttributes = attrs as OrderTypeAttribute[] ?? attrs.ToArray();
            //    if (!orderTypeAttributes.Any())
            //        continue;
            //    var isWp = type.GetInterface(nameof(IWholeProcessRecharge)) != null;
            //    AppSourceType sourceType = isWp ? AppSourceType.VisitDoctorWholeProcess : AppSourceType.InternetHospital; //应用来源
            //    foreach (var attr in orderTypeAttributes)
            //    {
            //        _typeMaps.Add((attr.OrderType, sourceType), type);
            //    }
            //}

            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            foreach (var assemb in assemblies)
            {
                if(!assemb.FullName.Contains("CodeFileTools"))
                    continue;

                Console.WriteLine($"{assemb.FullName}");//assemb.GetName()
                //foreach (var type in assemb.GetExportedTypes()) //获取此程序集中定义的公共类型，这些公共类型在程序集外可见。
                //{
                //    Console.WriteLine($"{type.FullName}");
                //}

                Console.WriteLine("-----");

                foreach (var type in assemb.GetTypes()) //获取此程序集中定义的类型。
                {
                    if (type.FullName.Contains("MyMiniExcel"))
                    {

                        Console.WriteLine($"{type.FullName}-----------------");
                        var Methods = type.GetMethods();
                        foreach (var method in Methods)
                        {
                            Console.WriteLine(method.Name);
                        }

                        //foreach (ParameterInfo Param in Params)
                        //{
                        //    Console.WriteLine("Param=" + Param.Name.ToString());
                        //    Console.WriteLine("  Type=" + Param.ParameterType.ToString());
                        //    Console.WriteLine("  Position=" + Param.Position.ToString());
                        //    Console.WriteLine("  Optional=" + Param.IsOptional.ToString());
                        //}
                    }
                }

                //var ret1 = assemb.GetManifestResourceNames();
                //foreach (var item1 in ret1)
                //{
                //    Console.Write($"-{item1}");
                //}
            }
        }
    }
}
