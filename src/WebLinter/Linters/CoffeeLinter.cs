using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class CoffeeLinter : LinterBase
    {
        private static Regex _rx = new Regex(@"\[stdin\]:(?<line>[0-9]+):(?<column>[0-9]+): error: (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public CoffeeLinter(ISettings settings) : base(settings)
        { }

        public override string Name
        {
            get { return "CoffeeLint"; }
        }

        public override bool IsEnabled
        {
            get { return Settings.CoffeeLintEnable; }
        }

        protected override LintingResult Lint(FileInfo file)
        {
            string output, error;
            RunProcess(file, "coffeelint.cmd", out output, out error);

            if (!string.IsNullOrEmpty(output))
            {
                foreach (Match match in _rx.Matches(output))
                {
                    AddError(file, match, Settings.CoffeeLintAsErrors);
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
