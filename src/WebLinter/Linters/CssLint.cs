using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class CssLinter : LinterBase
    {
        private static Regex _rx = new Regex(@": line (?<line>[0-9]+), col (?<column>[0-9]+), (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override string Name
        {
            get { return "CssLint"; }
        }

        public override LintingResult Lint(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            var result = new LintingResult(fileName);

            if (!file.Exists)
            {
                result.Errors.Add(new LintingError(file.FullName, "The file doesn't exist"));
                return result;
            }

            string args = $"--format=compact {FindConfigFile(file)}";
            string output, error;
            RunProcess(file, "csslint.cmd", out output, out error, args);

            if (!string.IsNullOrEmpty(output))
            {
                foreach (Match match in _rx.Matches(output))
                {
                    AddError(file, result, match);
                }
            }
            else if (!string.IsNullOrEmpty(error))
            {
                result.Errors.Add(new LintingError(file.FullName, error));
            }

            return result;
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
