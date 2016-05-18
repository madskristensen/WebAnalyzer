using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
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

                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    PromptToUpgrade();
                }), DispatcherPriority.ApplicationIdle, null);
            }
        }

        private static void PromptToUpgrade()
        {
            if (WebLinterPackage.Settings.ShowPromptToUpgrade)
            {
                try
                {
                    var hwnd = new IntPtr(WebLinterPackage.Dte.MainWindow.HWnd);
                    var window = (Window)HwndSource.FromHwnd(hwnd).RootVisual;

                    string msg = "The Web Analyzer extension has now been built in to Visual Studio in the .NET Core Tooling Preview 1.\r\rPlease uninstall Web Analyzer and install .NET Core Tooling Preview 1.\r\rDo you wish to go to the download page?";
                    var answer = MessageBox.Show(window, msg, Vsix.Name, MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (answer == MessageBoxResult.Yes)
                        Process.Start("https://www.microsoft.com/net/download#tools");
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
                finally
                {
                    WebLinterPackage.Settings.ShowPromptToUpgrade = false;
                    WebLinterPackage.Settings.SaveSettingsToStorage();
                }
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
