using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Threading;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace WebLinterVsix.FileListeners
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("JavaScript")]
    [ContentType("TypeScript")]
    [ContentType("CoffeeScript")]
    [ContentType("JSX")]
    [ContentType("CSS")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class SourceFileCreationListener : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        private ITextDocument _document;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            textView.Closed += TextviewClosed;

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out _document))
            {
                if (!LinterService.IsFileSupported(_document.FilePath))
                    return;

                WebLinterPackage.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _document.FileActionOccurred += DocumentSaved;
                    textView.Properties.AddProperty("lint_filename", _document.FilePath);

                    // Don't run linter again if error list already contains errors for the file.
                    if (!ErrorList.Instance.HasErrors(_document.FilePath) && RunOnOpen(_document.FilePath))
                        LinterService.Lint(false, _document.FilePath);

                }), DispatcherPriority.ApplicationIdle, null);
            }
        }

        private static bool RunOnOpen(string fileName)
        {
            var patterns = WebLinterPackage.Settings.GetIgnorePatterns();

            if (patterns.Any(p => fileName.Contains(p)))
                return false;

            return true;
        }

        private void TextviewClosed(object sender, EventArgs e)
        {
            IWpfTextView view = (IWpfTextView)sender;
            string fileName;

            if (view.Properties.TryGetProperty("lint_filename", out fileName))
            {
                System.Threading.ThreadPool.QueueUserWorkItem((o) =>
                {
                    ErrorList.Instance.CleanErrors(new[] { fileName });
                });
            }

            if (view != null)
                view.Closed -= TextviewClosed;
        }

        private void DocumentSaved(object sender, TextDocumentFileActionEventArgs e)
        {
            if (e.FileActionType == FileActionTypes.ContentSavedToDisk && LinterService.IsFileSupported(e.FilePath))
            {
                LinterService.Lint(false, e.FilePath);
            }
        }
    }
}
