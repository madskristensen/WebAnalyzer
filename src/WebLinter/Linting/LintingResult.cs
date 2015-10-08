using System.Collections.Generic;

namespace WebLinter
{
    public class LintingResult
    {
        public LintingResult(params string[] fileNames)
        {
            FileNames.AddRange(fileNames);
        }

        public List<string> FileNames { get; set; } = new List<string>();

        public bool HasErrors
        {
            get { return Errors.Count > 0; }
        }

        public IList<LintingError> Errors { get; } = new List<LintingError>();
    }
}