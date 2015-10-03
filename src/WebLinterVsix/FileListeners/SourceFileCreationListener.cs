using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Threading;
using EnvDTE;
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

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out _document))
            {
                _document.FileActionOccurred += DocumentSaved;

                VSPackage.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (IsValid(_document.FilePath))
                    {
                        textView.Properties.AddProperty("lint_filename", _document.FilePath);
                        LinterService.Lint(_document.FilePath);
                    }

                }), DispatcherPriority.ApplicationIdle, null);
            }

            textView.Closed += TextviewClosed;
        }

        private void TextviewClosed(object sender, EventArgs e)
        {
            IWpfTextView view = (IWpfTextView)sender;
            string fileName;

            if (view.Properties.TryGetProperty("lint_filename", out fileName))
            {
                ErrorList.CleanErrors(fileName);
            }

            if (view != null)
                view.Closed -= TextviewClosed;

            if (_document != null)
                _document.FileActionOccurred -= DocumentSaved;
        }

        private void DocumentSaved(object sender, TextDocumentFileActionEventArgs e)
        {
            if (e.FileActionType == FileActionTypes.ContentSavedToDisk && IsValid(e.FilePath))
            {
                LinterService.Lint(e.FilePath);
            }
        }

        private bool IsValid(string fileName)
        {
            // Check if filename is absolute because when debugging, script files are sometimes dynamically created.
            if (string.IsNullOrEmpty(fileName) || !Path.IsPathRooted(fileName))
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
    }
}
