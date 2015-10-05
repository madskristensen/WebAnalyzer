using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using WebLinter;

namespace WebLinterVsix
{
    class ErrorList
    {
        private static Dictionary<string, ErrorListProvider> _providers = new Dictionary<string, ErrorListProvider>();

        public static void AddErrors(IEnumerable<LintingError> errors)
        {
            CleanErrors(errors.Select(e => e.FileName));

            List<Task> list = new List<Task>(errors.Select(e => CreateTask(e)));

            foreach (var file in list.GroupBy(t => t.Document))
            {
                var provider = new ErrorListProvider(VSPackage.Package);
                provider.SuspendRefresh();

                foreach (var task in file.ToArray())
                {
                    provider.Tasks.Add(task);
                }

                provider.ResumeRefresh();
                _providers.Add(file.Key, provider);
            }
        }

        public static void CleanErrors(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                if (_providers.ContainsKey(file))
                {
                    _providers[file].Tasks.Clear();
                    _providers[file].Dispose();
                    _providers.Remove(file);
                }

            }
        }

        public static void CleanAllErrors()
        {
            foreach (string file in _providers.Keys)
            {
                var provider = _providers[file];
                if (provider != null)
                {
                    provider.Tasks.Clear();
                    provider.Dispose();
                }
            }

            _providers.Clear();
        }

        public static bool HasErrors()
        {
            return _providers.Count > 0;
        }

        public static bool HasErrors(string fileName)
        {
            return _providers.ContainsKey(fileName);
        }

        private static ErrorTask CreateTask(LintingError error)
        {
            ErrorTask task = new ErrorTask()
            {
                Line = error.LineNumber - 1,
                Column = error.ColumnNumber,
                ErrorCategory = error.IsError ? TaskErrorCategory.Error : TaskErrorCategory.Warning,
                Category = TaskCategory.Html,
                Document = error.FileName,
                Priority = TaskPriority.Normal,
                Text = $"({error.Provider}) {error.Message}",
            };

            EnvDTE.ProjectItem item = VSPackage.Dte.Solution.FindProjectItem(error.FileName);

            if (item != null && item.ContainingProject != null)
                AddHierarchyItem(task, item.ContainingProject);

            task.Navigate += Task_Navigate;

            return task;
        }

        private static void Task_Navigate(object sender, EventArgs e)
        {
            var task = (ErrorTask)sender;
            VSPackage.Dte.ItemOperations.OpenFile(task.Document);
            var doc = (EnvDTE.TextDocument)VSPackage.Dte.ActiveDocument.Object("textdocument");
            doc.Selection.MoveToLineAndOffset(task.Line + 1, Math.Max(task.Column, 1), false);
        }

        private static void AddHierarchyItem(ErrorTask task, EnvDTE.Project project)
        {
            IVsSolution solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;

            if (solution == null || project == null)
                return;

            try
            {
                IVsHierarchy hierarchyItem = null;
                if (solution.GetProjectOfUniqueName(project.FullName, out hierarchyItem) == 0)
                {
                    task.HierarchyItem = hierarchyItem;
                }
            }
            catch (COMException ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
