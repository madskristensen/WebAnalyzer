using System.ComponentModel;
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

        [Category("CoffeeLint")]
        [DisplayName("Enabled")]
        [DefaultValue(true)]
        public bool CoffeeLintEnable { get; set; } = true;

        [Category("CoffeeLint")]
        [DisplayName("Show warnings as errors")]
        [DefaultValue(true)]
        public bool CoffeeLintAsErrors { get; set; } = true;

        [Category("CSSLint")]
        [DisplayName("Enabled")]
        [DefaultValue(true)]
        public bool CssLintEnable { get; set; } = true;

        [Category("CssLint")]
        [DisplayName("Show warnings as errors")]
        [DefaultValue(true)]
        public bool CssLintAsErrors { get; set; } = true;

        [Category("JSHint")]
        [DisplayName("Enabled")]
        [DefaultValue(true)]
        public bool JSHintEnable { get; set; } = true;

        [Category("JSHint")]
        [DisplayName("Show warnings as errors")]
        [DefaultValue(true)]
        public bool JSHintAsErrors { get; set; } = true;

        [Category("TSLint")]
        [DisplayName("Enabled")]
        [DefaultValue(true)]
        public bool TSLintEnable { get; set; } = true;

        [Category("TSLint")]
        [DisplayName("Show warnings as errors")]
        [DefaultValue(true)]
        public bool TSLintAsErrors { get; set; } = true;
    }
}
