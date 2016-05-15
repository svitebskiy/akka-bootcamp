﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTail.Messages
{
    public class FileChange : FileMessageBase
    {
        public FileChange(string name, string fullPath)
            : base(name, fullPath)
        {
        }
    }
}
