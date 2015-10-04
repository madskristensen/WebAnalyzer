using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using WebLinter;

namespace WebLinterVsix
{
    public class Settings : DialogPage, ISettings
    {
        [Category("CoffeeLint")]
        [DisplayName("Enabled")]
        [DefaultValue(true)]
        public bool CoffeeLintEnable { get; set; } = true;

        [Category("CoffeeLint")]
        [DisplayName("Show warnings as errors")]
        [DefaultValue(false)]
        public bool CoffeeLintAsErrors { get; set; } = false;

        [Category("CSSLint")]
        [DisplayName("Enabled")]
        [DefaultValue(true)]
        public bool CssLintEnable { get; set; } = true;

        [Category("CSSLint")]
        [DisplayName("Show warnings as errors")]
        [DefaultValue(false)]
        public bool CssLintAsErrors { get; set; } = false;

        [Category("JSHint")]
        [DisplayName("Enabled")]
        [DefaultValue(true)]
        public bool ESLintEnable { get; set; } = true;

        [Category("JSHint")]
        [DisplayName("Show warnings as errors")]
        [DefaultValue(false)]
        public bool ESLintAsErrors { get; set; } = false;

        [Category("TSLint")]
        [DisplayName("Enabled")]
        [DefaultValue(true)]
        public bool TSLintEnable { get; set; } = true;

        [Category("TSLint")]
        [DisplayName("Show warnings as errors")]
        [DefaultValue(false)]
        public bool TSLintAsErrors { get; set; } = false;
    }
}
