using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using WebLinter;

namespace WebLinterVsix
{
    class ErrorList : ITableDataSource
    {
        private static ErrorList _instance;
        private readonly ITableManager _manager;
        private readonly List<SinkManager> _managers = new List<SinkManager>();

        [Import]
        private ITableManagerProvider TableManagerProvider { get; set; } = null;

        private ErrorList()
        {
            var compositionService = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            compositionService.DefaultCompositionService.SatisfyImportsOnce(this);

            _manager = TableManagerProvider.GetTableManager(StandardTables.ErrorsTable);
            _manager.AddSource(this, StandardTableColumnDefinitions.DetailsExpander,
                                                   StandardTableColumnDefinitions.ErrorSeverity, StandardTableColumnDefinitions.ErrorCode,
                                                   StandardTableColumnDefinitions.ErrorSource, StandardTableColumnDefinitions.BuildTool,
                                                   StandardTableColumnDefinitions.ErrorSource, StandardTableColumnDefinitions.ErrorCategory,
                                                   StandardTableColumnDefinitions.Text, StandardTableColumnDefinitions.DocumentName, StandardTableColumnDefinitions.Line, StandardTableColumnDefinitions.Column);
        }

        public static ErrorList Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ErrorList();

                return _instance;
            }
        }

        #region ITableDataSource members
        public string SourceTypeIdentifier
        {
            get { return StandardTableDataSources.ErrorTableDataSource; }
        }

        public string Identifier
        {
            get { return PackageGuids.guidVSPackageString; }
        }

        public string DisplayName
        {
            get { return Constants.VSIX_NAME; }
        }

        public IDisposable Subscribe(ITableDataSink sink)
        {
            return new SinkManager(this, sink);
        }
        #endregion

        public void AddSinkManager(SinkManager manager)
        {
            // This call can, in theory, happen from any thread so be appropriately thread safe.
            // In practice, it will probably be called only once from the UI thread (by the error list tool window).
            lock (_managers)
            {
                _managers.Add(manager);
            }
        }

        public void RemoveSinkManager(SinkManager manager)
        {
            // This call can, in theory, happen from any thread so be appropriately thread safe.
            // In practice, it will probably be called only once from the UI thread (by the error list tool window).
            lock (_managers)
            {
                _managers.Remove(manager);
            }
        }

        public void UpdateAllSinks()
        {
            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.UpdateSink(_snapsnots.Values);
                }
            }
        }

        private static Dictionary<string, LintingErrorSnapshot> _snapsnots = new Dictionary<string, LintingErrorSnapshot>();

        public void AddErrors(IEnumerable<LintingError> errors)
        {
            if (!errors.Any())
                return;

            CleanErrors(errors.Select(e => e.FileName));

            foreach (var error in errors.GroupBy(t => t.FileName))
            {
                var snapshot = new LintingErrorSnapshot(error.Key, error);
                _snapsnots.Add(error.Key, snapshot);
            }

            UpdateAllSinks();
        }

        public void CleanErrors(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                if (_snapsnots.ContainsKey(file))
                {
                    _snapsnots[file].Dispose();
                    _snapsnots.Remove(file);
                }
            }

            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.RemoveSnapshots(files);
                }
            }

            UpdateAllSinks();
        }

        public void CleanAllErrors()
        {
            foreach (string file in _snapsnots.Keys)
            {
                var snapshot = _snapsnots[file];
                if (snapshot != null)
                {
                    snapshot.Dispose();
                }
            }

            _snapsnots.Clear();

            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.Clear();
                }
            }
        }

        public void BringToFront()
        {
            WebLinterPackage.Dte.ExecuteCommand("View.ErrorList");
        }

        public bool HasErrors()
        {
            return _snapsnots.Count > 0;
        }

        public bool HasErrors(string fileName)
        {
            return _snapsnots.ContainsKey(fileName);
        }
    }
}
