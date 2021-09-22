using System;
using System.Collections.Generic;

namespace CodeFileTools
{
    public interface IFileProcess
    {
        void CreateFile(List<string> prefix, CreateOptions option, Tuple<List<TbDesc>, List<IndexDesc>> ret);
    }
}
