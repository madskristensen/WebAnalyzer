using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class TsLintLinter : LinterBase
    {
        private static Regex _rx = new Regex(@"(?<file>.+)\[(?<line>[0-9]+), (?<column>[0-9]+)\]: (?<message>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public TsLintLinter(ISettings settings, string workingDirectory) : base(settings, workingDirectory, _rx)
        {
            Name = "TSLint";
            ConfigFileName = "tslint.json";
            IsEnabled = Settings.TSLintEnable;
        }
    }
}
