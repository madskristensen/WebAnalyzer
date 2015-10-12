using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using WebLinter;

namespace WebLinterVsix
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", WebLinter.Constants.VERSION, IconResourceID = 400)]
    [ProvideOptionPage(typeof(Settings), "Web", "Analyzer", 101, 111, true, new[] { "eslint", "tslint", "coffeelint", "csslint" }, ProvidesLocalizedCategoryName = false)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [Guid(PackageGuids.guidVSPackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class WebLinterPackage : Package
    {
        public static DTE2 Dte;
        public static Settings Settings;
        private SolutionEvents _events;

        protected override void Initialize()
        {
            Dte = GetService(typeof(DTE)) as DTE2;
            Settings = (Settings)GetDialogPage(typeof(Settings));

            _events = Dte.Events.SolutionEvents;
            _events.AfterClosing += delegate { TableDataSource.Instance.CleanAllErrors(); };

            Telemetry.SetDeviceName(Dte.Edition);
            Logger.Initialize(this, Constants.VSIX_NAME);

            LintFilesCommand.Initialize(this);
            CleanErrorsCommand.Initialize(this);
            EditConfigFilesCommand.Initialize(this);
            ResetConfigFilesCommand.Initialize(this);

            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LinterBase.Server.Down();
            }

            base.Dispose(true);
        }
    }

    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    public sealed class WebLinterInitPackage : Package
    {
        protected override void Initialize()
        {
            // Delay execution until VS is idle.
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                // Then execute in a background thread.
                System.Threading.Tasks.Task.Run(async () =>
                {
                    try
                    {
                        await LinterFactory.Initialize();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                });
            }), DispatcherPriority.ApplicationIdle, null);
        }
    }
}
