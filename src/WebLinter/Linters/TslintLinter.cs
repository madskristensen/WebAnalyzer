using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class TsLintLinter : LinterBase
    {
        private static Regex _rx = new Regex(@"\.ts\[(?<line>[0-9]+), (?<column>[0-9]+)\]: (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public TsLintLinter(ISettings settings) : base(settings)
        { }

        public override string Name
        {
            get { return "TSLint"; }
        }

        public override bool IsEnabled
        {
            get { return Settings.TSLintEnable; }
        }

        protected override LintingResult Lint(FileInfo file)
        {
            string output, error;
            RunProcess(file, "tslint.cmd", out output, out error);

            if (!string.IsNullOrEmpty(output))
            {
                foreach (Match match in _rx.Matches(output))
                {
                    AddError(file, match, Settings.TSLintAsErrors);
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
