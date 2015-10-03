using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class CssLinter : LinterBase
    {
        private static Regex _rx = new Regex(@": line (?<line>[0-9]+), col (?<column>[0-9]+), (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public CssLinter(ISettings settings) : base(settings)
        { }

        public override string Name
        {
            get { return "CssLint"; }
        }

        public override bool IsEnabled
        {
            get { return Settings.CssLintEnable; }
        }

        protected override LintingResult Lint(FileInfo file)
        {
            string args = $"--format=compact {FindConfigFile(file)}";
            string output, error;
            RunProcess(file, "csslint.cmd", out output, out error, args);

            if (!string.IsNullOrEmpty(output))
            {
                foreach (Match match in _rx.Matches(output))
                {
                    AddError(file, match, Settings.CssLintAsErrors);
                }
            }
            else if (!string.IsNullOrEmpty(error))
            {
                Result.Errors.Add(new LintingError(file.FullName, error));
            }

            return Result;
        }

        private static string FindConfigFile(FileInfo file)
        {
            return "--ignore=known-properties,ids";
            // return here until the npm package is updated to support the --config flag

            var dir = file.Directory;

            while (dir != null)
            {
                string rc = Path.Combine(dir.FullName, ".csslintrc");
                if (File.Exists(rc))
                    return $"--config=\"{rc}\"";

                dir = dir.Parent;
            }

            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string result = Path.Combine(userFolder, ".csslintrc");

            return $"--config=\"{result}\""; ;
        }
    }
}
