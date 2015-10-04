using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class CssLinter : LinterBase
    {
        private static Regex _rx = new Regex(@"(?<file>.+): line (?<line>[0-9]+), col (?<column>[0-9]+), (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public CssLinter(ISettings settings) : base(settings, _rx)
        {
            Name = "CssLint";
            ConfigFileName = ".csslintrc";
            IsEnabled = Settings.CssLintEnable;
        }
        
        protected override string GetArguments(FileInfo[] files)
        {
            return $"--format=compact --ignore=known-properties,ids";
        }
    }
}
