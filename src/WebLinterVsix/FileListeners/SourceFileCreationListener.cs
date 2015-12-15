using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace WebLinterVsix.FileListeners
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("JavaScript")]
    [ContentType("Node.js")]
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

            // Both "Web Compiler" and "Bundler & Minifier" extensions add this property on their
            // generated output files. Generated output should be ignored from linting
            bool generated;
            if (textView.Properties.TryGetProperty("generated", out generated) && generated)
                return;

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out _document))
            {
                Task.Run(async () =>
                {
                    if (!LinterService.IsFileSupported(_document.FilePath))
                        return;

                    _document.FileActionOccurred += DocumentSaved;
                    textView.Properties.AddProperty("lint_filename", _document.FilePath);

                    // Don't run linter again if error list already contains errors for the file.
                    if (!TableDataSource.Instance.HasErrors(_document.FilePath))
                    {
                        await LinterService.LintAsync(false, _document.FilePath);
                    }
                });
            }
        }

        private void TextviewClosed(object sender, EventArgs e)
        {
            IWpfTextView view = (IWpfTextView)sender;

            System.Threading.ThreadPool.QueueUserWorkItem((o) =>
            {
                string fileName;

                if (view != null && view.Properties.TryGetProperty("lint_filename", out fileName))
                {
                    TableDataSource.Instance.CleanErrors(new[] { fileName });
                }
            });

            if (view != null)
                view.Closed -= TextviewClosed;
        }

        private async void DocumentSaved(object sender, TextDocumentFileActionEventArgs e)
        {
            if (e.FileActionType == FileActionTypes.ContentSavedToDisk)
            {
                await LinterService.LintAsync(false, e.FilePath);
            }
        }
    }
}
