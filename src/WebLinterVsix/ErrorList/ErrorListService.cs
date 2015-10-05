using System.Collections.Generic;
using System.Linq;
using WebLinter;

namespace WebLinterVsix
{
    class ErrorListService
    {
        public static void ProcessLintingResults(IEnumerable<LintingResult> results, bool showErrorList)
        {
            foreach (var result in results)
            {
                if (result.HasErrors)
                {
                    ErrorList.AddErrors(result.Errors);

                    if (showErrorList)
                        ErrorList.BringToFront();
                }
                else
                {
                    ErrorList.CleanErrors(result.Errors.Select(e => e.FileName));
                }
            }
        }
    }
}
