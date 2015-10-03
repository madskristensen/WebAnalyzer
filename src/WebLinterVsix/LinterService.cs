using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using WebLinter;

namespace WebLinterVsix
{
    internal static class LinterService
    {
        static LinterService()
        {
            LinterFactory.Initializing += InitiliazingLinters;
        }

        private static void InitiliazingLinters(object sender, EventArgs e)
        {
            StatusText("Installing latest version of the linters...");
        }

        public static void Lint(string fileName)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    ErrorList.CleanErrors(fileName);
                    var result = LinterFactory.Lint(fileName);

                    ErrorListService.ProcessLintingResults(new[] { result });
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            });
        }

        private static void StatusText(string message)
        {
            VSPackage.Dispatcher.BeginInvoke(new Action(() =>
            {
                VSPackage.Dte.StatusBar.Text = message;
            }), DispatcherPriority.ApplicationIdle, null);
        }
    }
}
