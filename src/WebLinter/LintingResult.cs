using System.Collections.Generic;

namespace WebLinter
{
    public class LintingResult
    {
        public LintingResult(params string[] fileNames)
        {
            FileNames = fileNames;
        }

        public IEnumerable<string> FileNames { get; set; }

        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }

        public IList<LintingError> Errors { get; } = new List<LintingError>();
    }
}