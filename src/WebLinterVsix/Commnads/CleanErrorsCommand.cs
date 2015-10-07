using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using WebLinter;

namespace WebLinterVsix
{
    internal sealed class CleanErrorsCommand
    {
        private readonly Package _package;

        private CleanErrorsCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            _package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(PackageGuids.WebLinterCmdSet, PackageIds.CleanErrorsCommand);
                var menuItem = new OleMenuCommand(CleanErrors, menuCommandID);
                menuItem.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(menuItem);
            }
        }

        public static CleanErrorsCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new CleanErrorsCommand(package);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;

            button.Visible = TableDataSource.Instance.HasErrors();
        }

        private void CleanErrors(object sender, EventArgs e)
        {
            TableDataSource.Instance.CleanAllErrors();
            Telemetry.TrackEvent("VS Clean Errors");
        }
    }
}
