namespace CodeFileTools
{
    public class TbDesc
    {
        public string FieldName { get; set; }
        public string DbFieldType { get; set; }//数据库字段类型

        public string CLRTypeString { get; set; }//字段类型对应C# 基础数据类型 字符串
        //public Type CLRType { get; set; }//字段类型对应C# 基础数据类型
        public bool IsNeeded { get; set; }
        public int? MaxLength { get; set; } //如果为varchar的最大长度

        public string Remark { get; set; }
    }

    public class IndexDesc
    {
        public string IndexName { get; set; }
        public string FieldName { get; set; }
    }
}
