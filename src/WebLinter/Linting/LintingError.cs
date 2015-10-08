using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebLinter
{
    public class LintingError
    {
        public LintingError(string fileName)
        {
            FileName = fileName;
        }

        public LinterBase Provider { get; set; }
        public string FileName { get; set; }
        public string Message { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public bool IsError { get; set; } = true;
        public string ErrorCode { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
