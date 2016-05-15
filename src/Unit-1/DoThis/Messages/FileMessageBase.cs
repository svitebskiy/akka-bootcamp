using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTail.Messages
{
    public class FileMessageBase
    {
        public FileMessageBase(string name, string fullPath)
        {
            this.Name = name;
            this.FullPath = fullPath;
        }

        public string Name
        {
            get;
            private set;
        }

        public string FullPath
        {
            get;
            private set;
        }
    }
}
