using System.Collections.Generic;

namespace WebLinter
{
    public class LintingResult
    {
        public LintingResult(string fileName)
        {
            FileName = fileName; ;
        }

        public string FileName { get; set; }

        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }

        public IList<LintingError> Errors { get; } = new List<LintingError>();
    }
}