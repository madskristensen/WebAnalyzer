using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class EsLinter : LinterBase
    {
        private static Regex _rx = new Regex(@"(?<file>.+): line (?<line>[0-9]+), col (?<column>[0-9]+), .+ - (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public EsLinter(ISettings settings) : base(settings, _rx)
        {
            Name = "ESLint";
            ConfigFileName = ".eslintrc";
            IsEnabled = Settings.ESLintEnable;
        }

        protected override string GetArguments(FileInfo[] files)
        {
            return $"--format=compact {FindConfigFile(files.FirstOrDefault())}";
        }

        private string FindConfigFile(FileInfo file)
        {
            var dir = file.Directory;

            while (dir != null)
            {
                string rc = Path.Combine(dir.FullName, ConfigFileName);
                if (File.Exists(rc))
                    return $"--config=\"{rc}\"";

                dir = dir.Parent;
            }

            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string result = Path.Combine(userFolder, ConfigFileName);

            return $"--config=\"{result}\""; ;
        }
    }
}
