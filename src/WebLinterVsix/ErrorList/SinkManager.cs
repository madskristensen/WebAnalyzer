using Microsoft.VisualStudio.Shell.TableManager;
using System.Linq;
using System;
using System.Collections.Generic;
using WebLinter;

namespace WebLinterVsix
{
    class SinkManager : IDisposable
    {
        private readonly ITableDataSink _sink;
        private ErrorList _errorList;
        private List<LintingErrorSnapshot> _snapshots = new List<LintingErrorSnapshot>();

        internal SinkManager(ErrorList errorList, ITableDataSink sink)
        {
            _sink = sink;
            _errorList = errorList;

            errorList.AddSinkManager(this);
        }

        internal void Clear()
        {
            _sink.RemoveAllSnapshots();
        }

        internal void UpdateSink(IEnumerable<LintingErrorSnapshot> snapshots)
        {
            foreach (var snapshot in snapshots)
            {
                var existing = _snapshots.FirstOrDefault(s => s.FilePath == snapshot.FilePath);

                if (existing != null)
                {
                    _snapshots.Remove(existing);
                    _sink.ReplaceSnapshot(existing, snapshot);
                }
                else
                {
                    _sink.AddSnapshot(snapshot);
                }

                _snapshots.Add(snapshot);
            }
        }

        internal void RemoveSnapshots(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                var existing = _snapshots.FirstOrDefault(s => s.FilePath == file);

                if (existing != null)
                {
                    _snapshots.Remove(existing);
                    _sink.RemoveSnapshot(existing);
                }
            }
        }

        public void Dispose()
        {
            // Called when the person who subscribed to the data source disposes of the cookie (== this object) they were given.
            _errorList.RemoveSinkManager(this);
        }
    }
}
