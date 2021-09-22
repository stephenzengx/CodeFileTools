using System;

namespace CodeFileTools
{
    public enum FileTypeEnum
    {
        Controllers = 1,
        IService = 2,
        Service = 3,
        Repository=4,//仓储接口类 和实现类
        Entity = 5 //实体，里面包含Map
    }

    public class FileTypeAttr : Attribute
    {
        public FileTypeEnum FileType;

        public FileTypeAttr(FileTypeEnum fileType)
        {
            FileType = fileType;
        }
    }
}
