using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class EsLinter : LinterBase
    {
        private static Regex _rx = new Regex(@"(?<file>.+): line (?<line>[0-9]+), col (?<column>[0-9]+), (?<severity>Error|Warning) - (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public EsLinter(ISettings settings, string workingDirectory) : base(settings, workingDirectory, _rx)
        {
            Name = "ESLint";
            ConfigFileName = ".eslintrc";
            ErrorMatch = "Error";
            IsEnabled = Settings.ESLintEnable;
        }

        protected override string GetArguments(FileInfo[] files)
        {
            return $"--format=compact";
        }
    }
}
