using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using EnvDTE;
using WebLinter;

namespace WebLinterVsix
{
    internal static class LinterService
    {
        private static bool _defaultsCreated;

        static LinterService()
        {
            LinterFactory.Initializing += delegate { StatusText("Extracting latest version of the linters..."); };
            LinterFactory.Initialized += delegate { VSPackage.Dte.StatusBar.Clear(); };
        }

        public static bool IsFileSupported(string fileName)
        {
            return LinterFactory.IsFileSupported(fileName);
        }

        public static void Lint(string fileName)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    ErrorList.CleanErrors(new[] { fileName });
                    EnsureDefaults();

                    string workingDirectory = GetWorkingDirectory(fileName);
                    var result = LinterFactory.Lint(workingDirectory, VSPackage.Settings, fileName);

                    if (result != null)
                        ErrorListService.ProcessLintingResults(new[] { result });
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            });
        }

        private static string GetWorkingDirectory(string fileName)
        {
            ProjectItem item = VSPackage.Dte.Solution?.FindProjectItem(fileName);

            if (item == null || item.ContainingProject == null || item.ContainingProject.Properties == null)
                return Path.GetDirectoryName(fileName);

            return item.ContainingProject.GetRootFolder();
        }


        private static void StatusText(string message)
        {
            VSPackage.Dispatcher.BeginInvoke(new Action(() =>
            {
                VSPackage.Dte.StatusBar.Text = message;
            }), DispatcherPriority.ApplicationIdle, null);
        }

        private static void EnsureDefaults()
        {
            if (!_defaultsCreated)
            {
                string assembly = Assembly.GetExecutingAssembly().Location;
                string root = Path.GetDirectoryName(assembly);
                string sourceFolder = Path.Combine(root, "Resources\\Defaults");
                string destFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                foreach (string sourceFile in Directory.EnumerateFiles(sourceFolder))
                {
                    string fileName = Path.GetFileName(sourceFile);
                    string destFile = Path.Combine(destFolder, fileName);

                    if (!File.Exists(destFile))
                        File.Copy(sourceFile, destFile);
                }

                _defaultsCreated = true;
            }
        }
    }
}
