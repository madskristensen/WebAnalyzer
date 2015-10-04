using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Threading;
using WebLinter;

namespace WebLinterVsix
{
    class ErrorListService
    {
        public static void ProcessLintingResults(IEnumerable<LintingResult> results)
        {
            //VSPackage.Dispatcher.BeginInvoke(new Action(() =>
            //{
                foreach (var result in results)
                {
                    if (result.HasErrors)
                    {
                        ErrorList.AddErrors(result.Errors);
                    }
                    else
                    {
                        ErrorList.CleanErrors(result.Errors.Select(e => e.FileName));
                    }
                }
           // }), DispatcherPriority.ApplicationIdle, null);
        }
    }
}
