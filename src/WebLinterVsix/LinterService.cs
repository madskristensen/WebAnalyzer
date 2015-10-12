using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EnvDTE;
using WebLinter;

namespace WebLinterVsix
{
    internal static class LinterService
    {
        private static bool _defaultsCreated;

        public static bool IsFileSupported(string fileName)
        {
            // Check if filename is absolute because when debugging, script files are sometimes dynamically created.
            if (string.IsNullOrEmpty(fileName) || !Path.IsPathRooted(fileName))
                return false;

            if (!LinterFactory.IsFileSupported(fileName))
                return false;

            string extension = Path.GetExtension(fileName);

            var patterns = WebLinterPackage.Settings.GetIgnorePatterns();

            if (patterns.Any(p => fileName.Contains(p)))
                return false;

            // Ignore nested files
            if (WebLinterPackage.Settings.IgnoreNestedFiles && WebLinterPackage.Dte.Solution != null)
            {
                var item = WebLinterPackage.Dte.Solution.FindProjectItem(fileName);

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

        public static async Task LintAsync(bool showErrorList, params string[] fileNames)
        {
            try
            {
                WebLinterPackage.Dte.StatusBar.Text = "Analyzing...";
                WebLinterPackage.Dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationGeneral);

                await EnsureDefaultsAsync();

                var result = await LinterFactory.LintAsync(WebLinterPackage.Settings, fileNames);

                if (result != null)
                    ErrorListService.ProcessLintingResults(result, showErrorList);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {
                WebLinterPackage.Dte.StatusBar.Clear();
                WebLinterPackage.Dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationGeneral);
            }
        }

        public static async Task EnsureDefaultsAsync(bool force = false)
        {
            if (!_defaultsCreated || force)
            {
                string sourceFolder = GetVsixFolder();
                string destFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                try
                {
                    foreach (string sourceFile in Directory.EnumerateFiles(sourceFolder))
                    {
                        string fileName = Path.GetFileName(sourceFile);
                        string destFile = Path.Combine(destFolder, fileName);

                        if (force || !File.Exists(destFile))
                        {
                            using (var source = File.Open(sourceFile, FileMode.Open))
                            using (var dest = File.Create(destFile))
                            {
                                await source.CopyToAsync(dest);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

                _defaultsCreated = true;
            }
        }

        private static string GetVsixFolder()
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            string root = Path.GetDirectoryName(assembly);
            return Path.Combine(root, "Resources\\Defaults");
        }
    }
}
