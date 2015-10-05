using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class CssLinter : LinterBase
    {
        private static Regex _rx = new Regex(@"(?<file>.+): line (?<line>[0-9]+), col (?<column>[0-9]+), (?<severity>Error|Warning) - (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public CssLinter(ISettings settings, string workingDirectory) : base(settings, workingDirectory, _rx)
        {
            Name = "CssLint";
            ConfigFileName = ".csslintrc";
            ErrorMatch = "Error";
            IsEnabled = Settings.CssLintEnable;
        }

        protected override string GetArguments(FileInfo[] files)
        {
            return $"--format=compact --ignore=known-properties,ids";
        }
    }
}
