using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class JshintLinter : LinterBase
    {
        private static Regex _rx = new Regex(": line (?<line>[0-9]+), col (?<column>[0-9]+), (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public JshintLinter(ISettings settings) : base(settings)
        { }

        public override string Name
        {
            get { return "JSHint"; }
        }

        public override bool IsEnabled
        {
            get { return Settings.JSHintEnable; }
        }

        protected override LintingResult Lint(FileInfo file)
        {
            string output, error;
            RunProcess(file, "jshint.cmd", out output, out error);

            if (!string.IsNullOrEmpty(output))
            {
                foreach (Match match in _rx.Matches(output))
                {
                    AddError(file, match, Settings.JSHintAsErrors);
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
