using System;
using System.Threading;
using System.Windows.Threading;
using WebLinter;

namespace WebLinterVsix
{
    internal static class LinterService
    {
        static LinterService()
        {
            LinterFactory.Initializing += delegate { StatusText("Extracting latest version of the linters..."); };
            LinterFactory.Initialized += delegate { VSPackage.Dte.StatusBar.Clear(); };
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
