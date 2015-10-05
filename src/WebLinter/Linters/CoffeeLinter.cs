using System.IO;
using System.Text.RegularExpressions;

namespace WebLinter
{
    internal class CoffeeLinter : LinterBase
    {
        private static Regex _rx = new Regex(@"^(?<file>.+),(?<line>[0-9]+),(?<column>[0-9]+)?,.+,(?<message>[^,\[]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public CoffeeLinter(ISettings settings, string workingDirectory) : base(settings, workingDirectory, _rx)
        {
            Name = "CoffeeLint";
            ConfigFileName = "coffeelint.json";
            IsEnabled = Settings.CoffeeLintEnable;
        }

        protected override string GetArguments(FileInfo[] files)
        {
            return $"--reporter=csv";
        }
    }
}
