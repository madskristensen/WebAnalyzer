using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace WebLinterVsix
{
    internal sealed class ResetConfigFilesCommand
    {
        private readonly Package _package;

        private ResetConfigFilesCommand(Package package)
        {
            _package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            var menuCommandID = new CommandID(PackageGuids.ConfigFileCmdSet, PackageIds.ResetConfigFiles);
            var menuItem = new OleMenuCommand(CleanErrors, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static ResetConfigFilesCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new ResetConfigFilesCommand(package);
        }

        private void CleanErrors(object sender, EventArgs e)
        {
            string msg = "This will reset the configuration for all the linters to their defaults.\n\nDo you wish to continue?";
            var result = MessageBox.Show(msg, Constants.VSIX_NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LinterService.EnsureDefaults(true);
                WebLinterPackage.Dte.StatusBar.Text = "Web Linter configuration files have been reset";
            }
        }
    }
}
