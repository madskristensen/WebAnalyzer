using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class JshintLinter : LinterBase
    {
        private static Regex _rx = new Regex(": line (?<line>[0-9]+), col (?<column>[0-9]+), (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override string Name
        {
            get { return "JSHint"; }
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

            string output, error;
            RunProcess(file, "jshint.cmd", out output, out error);

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
    }
}
