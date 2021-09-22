using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeFileTools
{
    public class ControllerProcess : IFileProcess
    {
        public void CreateFile(List<string> prefix, CreateOptions option, Tuple<List<TbDesc>, List<IndexDesc>> ret)
        {
            if (prefix.Count != 3)
                throw new ArgumentException("Excel SheetName ErrorFormat!");

            var csName = prefix[1];
            var csSummaryName = prefix[2];

            var depServiceName = $"_{csName.FirstLeterLower()}Service";
            string fullFilePath = Path.Combine(option.Abspath, $"{csName}Controller.cs");
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            Console.WriteLine($" ----开始生成控制器类 '{csName}Controller.cs', 文件存放路径：'{option.Abspath}' ----");
            using (File.Create(fullFilePath))
            {

            }
            Console.WriteLine("    --生成成功--   ");
            var content = new StringBuilder();
            //using
            content.Append(
                "using System.Threading.Tasks;\r\n" +
                "using System.ComponentModel.DataAnnotations;\r\n" +
                "using Microsoft.AspNetCore.Mvc;\r\n" +
                "using InternetHospital.Module.IApplet;\r\n" +
                "using InternetHospital.DataContext.AccountContext;\r\n" +
                "using InternetHospital.Core.Dto;\r\n"
            );
            content.Append("\r\n");

            #region namespace
            content.Append($"namespace {option.Space}\r\n");
            content.Append("{\r\n");

            #region Controller Class
            content.Append(GetTabContent("/// <summary>", 1));
            content.Append(GetTabContent($"/// {csSummaryName}控制器", 1));
            content.Append(GetTabContent("/// </summary>", 1));

            content.Append(GetTabContent($"public class {csName}Controller : BaseController", 1));
            content.Append(GetTabContent("{", 1));

            //注入  
            content.Append(GetTabContent($"private readonly I{csName}Service {depServiceName};", 2));
            content.Append("\r\n");

            //构造方法
            content.Append(GetTabContent($"public {csName}Controller(I{csName}Service {csName.FirstLeterLower()}Service)", 2));
            content.Append(GetTabContent("{", 2));
            content.Append(GetTabContent($"{depServiceName} = {csName.FirstLeterLower()}Service;", 3));
            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

            //分页
            content.Append(GetTabContent("/// <summary>", 2));
            content.Append(GetTabContent($"/// 分页", 2));
            content.Append(GetTabContent("/// </summary>", 2));
            content.Append(GetTabContent("/// <param name=\"pageNo\"></param>", 2));
            content.Append(GetTabContent("/// <param name=\"pageSize\"></param>", 2));
            content.Append(GetTabContent("/// <returns></returns>", 2));
            content.Append(GetTabContent("[HttpGet]", 2));
            content.Append(GetTabContent($"public async Task<Result<PageList<{csName}>>> QueryPageAsync(int pageNo, int pageSize)", 2));
            content.Append(GetTabContent("{", 2));
            content.Append(GetTabContent($"return await {depServiceName}.QueryPageAsync(pageNo,pageSize);", 3));
            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

            //增
            content.Append(GetTabContent("/// <summary>", 2));
            content.Append(GetTabContent($"/// 新增", 2));
            content.Append(GetTabContent("/// </summary>", 2));
            content.Append(GetTabContent("/// <param name=\"input\"></param>", 2));
            content.Append(GetTabContent("/// <returns></returns>", 2));
            content.Append(GetTabContent("[HttpPost]", 2));
            content.Append(GetTabContent($"public async Task<Result> AddAsync({csName} input)", 2));
            content.Append(GetTabContent("{", 2));
            content.Append(GetTabContent($"return await {depServiceName}.AddAsync(input);", 3));
            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

            //编辑
            content.Append(GetTabContent("/// <summary>", 2));
            content.Append(GetTabContent($"/// 编辑", 2));
            content.Append(GetTabContent("/// </summary>", 2));
            content.Append(GetTabContent("/// <param name=\"id\"></param>", 2));
            content.Append(GetTabContent("/// <param name=\"input\"></param>", 2));
            content.Append(GetTabContent("/// <returns></returns>", 2));
            content.Append(GetTabContent("[HttpPost]", 2));
            content.Append(GetTabContent($"public async Task<Result> EditAsync([Required]string id, {csName} input)", 2));
            content.Append(GetTabContent("{", 2));
            content.Append(GetTabContent($"return await {depServiceName}.EditAsync(id,input);", 3));
            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

            //详情
            content.Append(GetTabContent("/// <summary>", 2));
            content.Append(GetTabContent($"/// 详情", 2));
            content.Append(GetTabContent("/// </summary>", 2));
            content.Append(GetTabContent("/// <param name=\"id\"></param>", 2));
            content.Append(GetTabContent("/// <returns></returns>", 2));
            content.Append(GetTabContent("[HttpGet]", 2));
            content.Append(GetTabContent($"public async Task<Result> DetailAsync([Required]string id)", 2));
            content.Append(GetTabContent("{", 2));
            content.Append(GetTabContent($"return await {depServiceName}.DetailAsync(id);", 3));
            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

            //删除
            content.Append(GetTabContent("/// <summary>", 2));
            content.Append(GetTabContent($"/// 删除", 2));
            content.Append(GetTabContent("/// </summary>", 2));
            content.Append(GetTabContent("/// <param name=\"id\"></param>", 2));
            content.Append(GetTabContent("/// <returns></returns>", 2));
            content.Append(GetTabContent("[HttpPost]", 2));
            content.Append(GetTabContent($"public async Task<Result> DeleteAsync([Required]string id)", 2));
            content.Append(GetTabContent("{", 2));
            content.Append(GetTabContent($"return await {depServiceName}.DeleteAsync(id);", 3));
            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

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
