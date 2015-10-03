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
            VSPackage.Dispatcher.BeginInvoke(new Action(() =>
            {
                // bool hasError = false;

                foreach (var result in results)
                {
                    if (result.HasErrors)
                    {
                        //hasError = true;
                        ErrorList.AddErrors(result.FileName, result.Errors);
                        //VSPackage._dte.StatusBar.Text = $"Error compiling \"{Path.GetFileName(result.FileName)}\". See Error List for details";
                    }
                    else
                    {
                        ErrorList.CleanErrors(result.FileName);

                        //if (!hasError)
                        //    VSPackage._dte.StatusBar.Text = $"Done compiling \"{Path.GetFileName(result.FileName)}\"";
                    }
                }
            }), DispatcherPriority.ApplicationIdle, null);
        }
    }
}
