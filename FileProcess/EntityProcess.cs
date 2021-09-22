using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeFileTools
{
    [FileTypeAttr(FileTypeEnum.Entity)]
    public class EntityProcess : IFileProcess
    {
        public void CreateFile(List<string> prefix, CreateOptions option, Tuple<List<TbDesc>, List<IndexDesc>> ret)
        {
            if (prefix.Count != 3)
                throw new ArgumentException("Excel SheetName ErrorFormat!");

            var tbName = prefix[0];
            var csName = prefix[1];
            var csSummaryName = prefix[2];

            var tbDesc = ret.Item1;
            var indexDesc = ret.Item2;

            //string fullFilePath = Path.Combine(option.Abspath, csName , ".cs"); //Error

            string fullFilePath = Path.Combine(option.Abspath, csName + ".cs");
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
            "using System.ComponentModel;\r\n" +
            "using System.ComponentModel.DataAnnotations;\r\n" +
            "using System.ComponentModel.DataAnnotations.Schema;\r\n" +
            "using InternetHospital.Core.Entities.Auditing;\r\n" +
            "using Microsoft.EntityFrameworkCore;\r\n" +
            "using InternetHospital.Core.EntityFrameworkCore;\r\n" +
            "using InternetHospital.Core.Repository;\r\n" +
            "using Microsoft.EntityFrameworkCore.Metadata.Builders;\r\n\r\n");

            #region namespace
            content.Append($"namespace {option.Space}\r\n");
            content.Append("{\r\n");

            #region EntityClass
            content.Append(GetTabContent("/// <summary>", 1));
            content.Append(GetTabContent($"/// {csSummaryName}实体类", 1));
            content.Append(GetTabContent("/// </summary>", 1));
            //[Table("T_DispenseMedicines")]
            content.Append(GetTabContent($"[Table(\"{tbName}\")]", 1));
            content.Append(GetTabContent($"public class {csName} : SimpleAuditedEntity<string, string>", 1));
            content.Append(GetTabContent("{", 1));

            #region Class Content
            foreach (var fItem in tbDesc)
            {
                /* 考虑decimal
                    [Column(TypeName = "decimal(9,2)")]
                    [Range(0, double.MaxValue, ErrorMessage ="{0}为非负数")]

                    以及可空(?)情况
                 */

                // summary
                content.Append(GetTabContent("/// <summary>", 2));
                content.Append(GetTabContent($"/// {fItem.Remark}", 2));
                content.Append(GetTabContent("/// </summary>", 2));

                content.Append(GetTabContent($"[Description(\"{fItem.Remark}\")]", 2));//Description标签
                if (fItem.IsNeeded) //Required 标签
                {
                    content.Append(GetTabContent("[Required]", 2));
                }

                if (fItem.CLRTypeString == "decimal")
                {
                    content.Append(GetTabContent($"[Column(TypeName = \"{fItem.DbFieldType}\")]", 2));
                }

                if (fItem.CLRTypeString == "string" && fItem.MaxLength.HasValue) //MaxLength标签
                {
                    content.Append(GetTabContent($"[MaxLength({fItem.MaxLength.Value}," + "ErrorMessage = \"{0}最大长度{1}\")]", 2));
                }

                content.Append(GetTabContent($"public {fItem.CLRTypeString}{(!fItem.IsNeeded && fItem.CLRTypeString != "string" ? "?" : "")} {fItem.FieldName} " + "{ get; set; }", 2));

                content.Append("\r\n");
            }

            #endregion

            content.Append(GetTabContent("}", 1));
            #endregion

            content.Append("\r\n");

            #region MapClass
            content.Append(GetTabContent("/// <summary>", 1));
            content.Append(GetTabContent($"/// {csSummaryName}实体类映射", 1));
            content.Append(GetTabContent("/// </summary>", 1));

            #region Class
            content.Append(GetTabContent($"public class {csName}Map : EntityMap<{csName}, string>", 1));
            content.Append(GetTabContent("{", 1));

            #region Method
            content.Append(GetTabContent($"public override void Configure(EntityTypeBuilder<{csName}> b)", 2));
            content.Append(GetTabContent("{", 2));

            content.Append(GetTabContent("b.Property(x => x.Id).HasMaxLength(36).HasValueGenerator<StringGuidValueGenerator>().ValueGeneratedOnAdd();", 3));
            foreach (var item in indexDesc)
            {
                content.Append(GetTabContent($"b.HasIndex(o => o.{item.FieldName}).HasName(\"{item.IndexName}\");", 3));
            }
            content.Append(GetTabContent("}", 2));

            #endregion

            content.Append(GetTabContent("}", 1));
            #endregion

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
