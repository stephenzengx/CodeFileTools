using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeFileTools
{
    [FileTypeAttr(FileTypeEnum.Repository)]
    public class RepositoryProcess : IFileProcess
    {
        public void CreateFile(List<string> prefix, CreateOptions option, Tuple<List<TbDesc>, List<IndexDesc>> ret)
        {
            if (prefix.Count != 3)
                throw new ArgumentException("Excel SheetName ErrorFormat!");

            var csName = prefix[1];
            var csSummaryName = prefix[2];

            string fullFilePath = Path.Combine(option.Abspath, $"{csName}Repostory.cs");
            if (!File.Exists(fullFilePath))
            {
                Console.WriteLine($" ----开始生成仓储类 '{csName}Repostory.cs', 文件存放路径：'{option.Abspath}' ----");
                using (File.Create(fullFilePath))
                {

                }
                Console.WriteLine("    --生成成功--   ");
            }

            var content = new StringBuilder();
            //using
            content.Append(
            "using InternetHospital.Core.EntityFrameworkCore;\r\n" +
            "using InternetHospital.Core.Repository;\r\n" +
            "using InternetHospital.Engine.Platform.Ioc;\r\n");

            content.Append("\r\n");

            #region namespace
            content.Append($"namespace {option.Space}\r\n");
            content.Append("{\r\n");

            #region IRepostory interface
            content.Append(GetTabContent("/// <summary>", 1));
            content.Append(GetTabContent($"/// {csSummaryName}仓储类接口", 1));
            content.Append(GetTabContent("/// </summary>", 1));
            content.Append(GetTabContent($"public interface I{csName}Repostory : IRepository<{csName}, string>, ITransientDependency", 1));
            content.Append(GetTabContent("{", 1));
            content.Append(GetTabContent("}", 1));
            #endregion

            #region Repostory Class
            content.Append("\r\n");
            content.Append(GetTabContent("/// <summary>", 1));
            content.Append(GetTabContent($"/// {csSummaryName}仓储类", 1));
            content.Append(GetTabContent("/// </summary>", 1));

            #region Class
            content.Append(GetTabContent($"public class {csName}Repostory : EfRepository<AccountContext, {csName}, string>, I{csName}Repostory", 1));
            content.Append(GetTabContent("{", 1));

            #region Method
            content.Append(GetTabContent($"public {csName}Repostory(IDbContextProvider dbContextProvider) : base(dbContextProvider)", 2));
            content.Append(GetTabContent("{", 2));
            content.Append(GetTabContent("}", 2));

            #endregion

            content.Append(GetTabContent("}", 1));
            #endregion

            #endregion

            content.Append("\r\n");

            #region 



            #endregion

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
