using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeFileTools
{
    [FileTypeAttr(FileTypeEnum.Service)]
    public class ServiceProcess : IFileProcess
    {
        public void CreateFile(List<string> prefix, CreateOptions option, Tuple<List<TbDesc>, List<IndexDesc>> ret)
        {
            if (prefix.Count != 3)
                throw new ArgumentException("Excel SheetName ErrorFormat!");

            var csName = prefix[1];
            var csSummaryName = prefix[2];
            var depRepName = $"_{csName.FirstLeterLower()}Rep";

            string fullFilePath = Path.Combine(option.Abspath, $"{csName}Service.cs");
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            Console.WriteLine($" ---- 开始生成服务类 '{csName}Service.cs', 文件存放路径：'{option.Abspath}' ----");


            using (File.Create(fullFilePath))
            {

            }
            Console.WriteLine("    --生成成功--   ");

            var content = new StringBuilder();
            //using
            content.Append(
                "using System;\r\n" +
                "using System.Linq;\r\n" +

                "using System.Threading.Tasks;\r\n" +
                "using InternetHospital.Core.Dto;\r\n" +
                "using InternetHospital.DataContext.AccountContext;\r\n" +
                "using InternetHospital.Core.Service;\r\n" +
                "using Microsoft.Extensions.Logging;\r\n" +
                "using InternetHospital.Module.IApplet;\r\n");
            content.Append("\r\n");

            #region namespace
            content.Append($"namespace {option.Space}\r\n");
            content.Append("{\r\n");

            #region Service Class
            content.Append(GetTabContent("/// <summary>", 1));
            content.Append(GetTabContent($"/// {csSummaryName}服务类", 1));
            content.Append(GetTabContent("/// </summary>", 1));

            content.Append(GetTabContent($"public class {csName}Service : ServiceBase, I{csName}Service", 1));
            content.Append(GetTabContent("{", 1));

            //注入  
            content.Append(GetTabContent($"private readonly I{csName}Repostory {depRepName};", 2));
            content.Append(GetTabContent($"private readonly ILogger<{csName}Service> _logger;", 2));
            content.Append("\r\n");

            //构造方法
            content.Append(GetTabContent($"public {csName}Service(I{csName}Repostory {csName.FirstLeterLower()}Rep, ILogger<{csName}Service> logger)", 2));
            content.Append(GetTabContent("{", 2));
            content.Append(GetTabContent($"{depRepName} = {csName.FirstLeterLower()}Rep;", 3));
            content.Append(GetTabContent("_logger = logger;", 3));
            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

            //分页
            content.Append(GetTabContent($"public async Task<Result<PageList<{csName}>>> QueryPageAsync(int pageNo, int pageSize)", 2));
            content.Append(GetTabContent("{", 2));
            content.Append(GetTabContent($"return await {depRepName}.Readonly()" +
                                         ".OrderByDescending(m => m.XGSJ)" +
                                         $".ToPageResultFromExpressionQueryAsync<{csName}>(pageNo, pageSize);", 3));
            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

            //新增
            content.Append(GetTabContent($"public async Task<Result> AddAsync({csName} input)", 2));
            content.Append(GetTabContent("{", 2));

            content.Append(GetTabContent("//TODO 逻辑判断", 3));
            content.Append("\r\n");
            content.Append(GetTabContent($"await {depRepName}.InsertAsync(input);", 3));
            content.Append("\r\n");
            content.Append(GetTabContent("return Result.Ok();", 3));

            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

            //编辑
            content.Append(GetTabContent($"public async Task<Result> EditAsync(string id, {csName} input)", 2));
            content.Append(GetTabContent("{", 2));

            content.Append(GetTabContent("//TODO 逻辑判断", 3));
            content.Append(GetTabContent($"var model = await {depRepName}.FirstOrDefaultAsync(m => m.Id == id);", 3));
            content.Append(GetTabContent("if (model == null)", 3));
            content.Append(GetTabContent("return Result.FromError(\"记录不存在\");", 4));
            content.Append("\r\n");
            content.Append(GetTabContent($"await {depRepName}.UpdateAsync(model);//TODO", 3));
            content.Append("\r\n");
            content.Append(GetTabContent("return Result.Ok();", 3));

            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

            //详情
            content.Append(GetTabContent("public async Task<Result> DetailAsync(string id)", 2));
            content.Append(GetTabContent("{", 2));

            content.Append(GetTabContent($"var model = await {depRepName}.FirstOrDefaultAsync(m => m.Id == id);", 3));
            content.Append(GetTabContent("if (model == null)", 3));
            content.Append(GetTabContent("return Result.FromError(\"记录不存在\");", 4));
            content.Append("\r\n");
            content.Append(GetTabContent("return Result.FromData(model);", 3));

            content.Append(GetTabContent("}", 2));
            content.Append("\r\n");

            //删
            content.Append(GetTabContent("public async Task<Result> DelAsync(string id)", 2));
            content.Append(GetTabContent("{", 2));

            content.Append(GetTabContent("//TODO 逻辑判断", 3));
            content.Append(GetTabContent($"var model = await {depRepName}.FirstOrDefaultAsync(m => m.Id == id);", 3));
            content.Append(GetTabContent("if (model == null)", 3));
            content.Append(GetTabContent("return Result.FromError(\"记录不存在\");", 4));
            content.Append("\r\n");
            content.Append(GetTabContent("model.IsDeleted = true;", 3));
            content.Append(GetTabContent($"await {depRepName}.UpdateAsync(model);", 3));
            content.Append("\r\n");
            content.Append(GetTabContent("return Result.Ok();", 3));

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
