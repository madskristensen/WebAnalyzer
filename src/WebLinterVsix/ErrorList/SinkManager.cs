using Microsoft.VisualStudio.Shell.TableManager;
using System;
using System.Collections.Generic;
using WebLinter;

namespace WebLinterVsix
{
    class SinkManager : IDisposable
    {
        private readonly ITableDataSink _sink;
        private ErrorList _errorList;

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

        internal void UpdateSink(IEnumerable<ITableEntriesSnapshot> snapshots)
        {
            _sink.RemoveAllSnapshots();

            foreach (var snapshot in snapshots)
            {
                _sink.AddSnapshot(snapshot);
            }
        }

        public void Dispose()
        {
            // Called when the person who subscribed to the data source disposes of the cookie (== this object) they were given.
            _errorList.RemoveSinkManager(this);
        }
    }
}
