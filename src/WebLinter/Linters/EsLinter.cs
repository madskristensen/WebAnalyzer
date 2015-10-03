using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class EsLinter : LinterBase
    {
        private static Regex _rx = new Regex(@": line (?<line>[0-9]+), col (?<column>[0-9]+), .+ - (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public EsLinter(ISettings settings) : base(settings)
        { }

        public override string Name
        {
            get { return "ESLint"; }
        }

        public override bool IsEnabled
        {
            get { return Settings.ESLintEnable; }
        }

        protected override LintingResult Lint(FileInfo file)
        {
            string output, error;
            RunProcess(file, "eslint.cmd", out output, out error, "--format=compact");

            if (!string.IsNullOrEmpty(output))
            {
                foreach (Match match in _rx.Matches(output))
                {
                    AddError(file, match, Settings.ESLintAsErrors);
                }
            }
            else if (!string.IsNullOrEmpty(error))
            {
                Result.Errors.Add(new LintingError(file.FullName, error));
            }

            return Result;
        }
    }
}
