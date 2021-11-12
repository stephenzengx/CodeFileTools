using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeFileTools
{
    [FileTypeAttribute(FileTypeEnum.IService)]
    public class IServiceProcess : IFileProcess
    {
        public void CreateFile(List<string> prefix, CreateOptions option, Tuple<List<TbDesc>, List<IndexDesc>> ret)
        {
            if (prefix.Count != 3)
                throw new ArgumentException("Excel SheetName ErrorFormat!");

            var csName = prefix[1];
            var csSummaryName = prefix[2];

            string fullFilePath = Path.Combine(option.Abspath, $"I{csName}Service.cs");
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            Console.WriteLine($" ----开始生成服务接口类 'I{csName}Service.cs', 文件存放路径：'{option.Abspath}' ----");
            
            using (File.Create(fullFilePath))
            {

            }
            Console.WriteLine("    --生成成功--   ");

            var content = new StringBuilder();
            //using
            content.Append(
                "using System;\r\n" +
                "using InternetHospital.Core.Dto;\r\n" +
                "using System.Threading.Tasks;\r\n" +
                "using InternetHospital.DataContext.AccountContext;\r\n");

            content.Append("\r\n");

            #region namespace
            content.Append($"namespace {option.Space}\r\n");
            content.Append("{\r\n");

            #region IService Class
            content.Append(GetTabContent("/// <summary>", 1));
            content.Append(GetTabContent($"/// {csSummaryName}服务接口", 1));
            content.Append(GetTabContent("/// </summary>", 1));

            content.Append(GetTabContent($"public interface I{csName}Service", 1));
            content.Append(GetTabContent("{", 1));

            //增/删/改/查/分页
            content.Append(GetTabContent($"Task<Result<PageList<{csName}>>> QueryPageAsync(int pageNo, int pageSize);", 2));
            content.Append(GetTabContent($"Task<Result> AddAsync({csName} input);", 2));
            content.Append(GetTabContent($"Task<Result> EditAsync(string id, {csName} input);", 2));
            content.Append(GetTabContent("Task<Result> DetailAsync(string id);", 2));
            content.Append(GetTabContent("Task<Result> DelAsync(string id);", 2));

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
