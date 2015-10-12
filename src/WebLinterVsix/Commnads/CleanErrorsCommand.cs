using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using WebLinter;

namespace WebLinterVsix
{
    internal sealed class CleanErrorsCommand
    {
        private readonly Package _package;
        private readonly BuildEvents _events;

        private CleanErrorsCommand(Package package)
        {
            _package = package;
            _events = WebLinterPackage.Dte.Events.BuildEvents;
            _events.OnBuildBegin += OnBuildBegin;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            var menuCommandID = new CommandID(PackageGuids.WebLinterCmdSet, PackageIds.CleanErrorsCommand);
            var menuItem = new OleMenuCommand(CleanErrors, menuCommandID);
            menuItem.BeforeQueryStatus += BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            if (Action == vsBuildAction.vsBuildActionClean || Action == vsBuildAction.vsBuildActionRebuildAll)
            {
                if (WebLinterPackage.Settings.CleanErrorsOnBuild)
                    TableDataSource.Instance.CleanAllErrors();
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
