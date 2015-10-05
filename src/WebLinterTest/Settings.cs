using System.ComponentModel;
using System.IO;
using WebLinter;

namespace WebLinterTest
{
    class Settings : ISettings
    {
        private static Settings _settings;

        public static Settings Instance
        {
            get
            {
                if (_settings == null)
                    _settings = new Settings();

                return _settings;
            }
        }

        public static string CWD
        {
            get { return new FileInfo("../../artifacts/").FullName; }
        }

        public bool CoffeeLintEnable { get; set; } = true;
        public bool CssLintEnable { get; set; } = true;
        public bool ESLintEnable { get; set; } = true;
        public bool TSLintEnable { get; set; } = true;
    }
}
