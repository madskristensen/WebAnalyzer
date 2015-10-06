using System.Collections.Generic;
using System.Linq;
using WebLinter;

namespace WebLinterVsix
{
    class ErrorListService
    {
        public static void ProcessLintingResults(IEnumerable<LintingResult> results, bool showErrorList)
        {
            var errors = results.Where(r => r.HasErrors).SelectMany(r => r.Errors);
            var clean = results.Where(r => !r.HasErrors).SelectMany(r => r.FileNames);

            if (errors.Any())
            {
                ErrorList.Instance.AddErrors(errors);
                if (showErrorList)
                    ErrorList.Instance.BringToFront();
            }

            ErrorList.Instance.CleanErrors(clean);
        }
    }
}
