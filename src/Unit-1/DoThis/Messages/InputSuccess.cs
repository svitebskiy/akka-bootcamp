using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTail.Messages
{
    public class InputSuccess
    {
        public InputSuccess(string data)
        {
            this.Data = data;
        }

        public string Data
        {
            get;
            private set;
        }
    }
}
