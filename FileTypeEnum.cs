using System;

namespace CodeFileTools
{
    public enum FileTypeEnum
    {
        Controllers = 1,
        IService = 2,
        Service = 3,
        Repository=4,//仓储接口类 和实现类
        Entity = 5, //实体，里面包含Map

        Entity_GDSJG=6//广东省监管 实体生成
    }

    public class FileTypeAttribute : Attribute
    {
        public FileTypeEnum FileType;

        public FileTypeAttribute(FileTypeEnum fileType)
        {
            FileType = fileType;
        }
    }
}
