using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTail.Messages
{
    public class FileError : FileMessageBase
    {
        public FileError(string name, string fullPath, Exception ex)
            : base(name, fullPath)
        {
            this.ExceptionInfo = new ExceptionInformation(ex);
        }

        public ExceptionInformation ExceptionInfo
        {
            get;
            private set;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Error in file {0}", this.Name);
            sb.AppendLine();
            var exInfo = this.ExceptionInfo;
            while (exInfo != null)
            {
                sb.AppendFormat(" because {0}", exInfo.Message);
                sb.AppendLine();
                exInfo = exInfo.InnerExceptionInfo;
            }
            return sb.ToString();
        }

        public class ExceptionInformation
        {
            public ExceptionInformation(Exception ex)
            {
                if (ex != null)
                {
                    this.Message = ex.Message;
                    this.StackTrace = ex.StackTrace;

                    if (ex.InnerException != null)
                    {
                        this.InnerExceptionInfo = new ExceptionInformation(ex.InnerException);
                    }
                }
            }

            public string Message
            {
                get;
                private set;
            }

            public string StackTrace
            {
                get;
                private set;
            }

            public ExceptionInformation InnerExceptionInfo
            {
                get;
                private set;
            }
        }
    }
}