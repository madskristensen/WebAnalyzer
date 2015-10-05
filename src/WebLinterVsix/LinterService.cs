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
            LinterFactory.Progress += OnProgress;
        }

        private static void OnProgress(object sender, LintingEventArgs e)
        {
            // No reason to show progress for single files
            if (e.Total == 1) return;

            if (e.Total > e.AmountOfTotal)
                VSPackage.Dte.StatusBar.Progress(true, $"Running {e.ProviderName} on {e.Files} files...", e.AmountOfTotal + 1, e.Total + 1);
            else
                VSPackage.Dte.StatusBar.Progress(false, $"Web Linter completed", e.AmountOfTotal + 1, e.Total + 1);
        }

        public static bool IsFileSupported(string fileName)
        {
            // Check if filename is absolute because when debugging, script files are sometimes dynamically created.
            if (string.IsNullOrEmpty(fileName) || !Path.IsPathRooted(fileName))
                return false;

            if (!LinterFactory.IsFileSupported(fileName))
                return false;

            string extension = Path.GetExtension(fileName);

            // Minified files should be ignored
            if (fileName.EndsWith(".min" + extension, StringComparison.OrdinalIgnoreCase))
                return false;

            // Ignore nested files
            if (VSPackage.Dte.Solution != null)
            {
                var item = VSPackage.Dte.Solution.FindProjectItem(fileName);

                if (item == null)
                    return false;

                if (item.Collection != null && item.Collection.Parent != null)
                {
                    var parent = item.Collection.Parent as ProjectItem;

                    if (parent != null && parent.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
                        return false;
                }
            }

            return true;
        }

        public static void Lint(params string[] fileNames)
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    ErrorList.CleanErrors(fileNames);
                    EnsureDefaults();

                    string workingDirectory = GetWorkingDirectory(fileNames[0]);
                    var result = LinterFactory.Lint(workingDirectory, VSPackage.Settings, fileNames);

                    if (result != null)
                        ErrorListService.ProcessLintingResults(result);
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
