using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using WebLinter;

namespace WebLinterVsix
{
    public class Settings : DialogPage, ISettings
    {
        // General
        [Category("General")]
        [DisplayName("Ignore patterns")]
        [Description("A comma-separated list of strings. Any file containing one of the strings in the path will be ignored.")]
        [DefaultValue(@"\node_modules\,\bower_components\,\typings\,\lib\")]
        public string IgnoreFolderNames { get; set; } = @"\node_modules\,\bower_components\,\typings\,\lib\";

        // Linters
        [Category("Linters")]
        [DisplayName("Enable CoffeeLint")]
        [Description("CoffeeLint is a linter for CoffeeScript files")]
        [DefaultValue(true)]
        public bool CoffeeLintEnable { get; set; } = true;

        [Category("Linters")]
        [DisplayName("Enable CSS Lint")]
        [Description("CoffeeLint is a linter for CSS files")]
        [DefaultValue(true)]
        public bool CssLintEnable { get; set; } = true;

        [Category("Linters")]
        [DisplayName("Enable ESLint")]
        [Description("CoffeeLint is a linter JavaScript and JSX files")]
        [DefaultValue(true)]
        public bool ESLintEnable { get; set; } = true;

        [Category("Linters")]
        [DisplayName("Enable TSLint")]
        [Description("CoffeeLint is a linter for TypeScript files")]
        [DefaultValue(true)]
        public bool TSLintEnable { get; set; } = true;

        public IEnumerable<string> GetIgnorePatterns()
        {
            var raw = IgnoreFolderNames.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pattern in raw)
            {
                yield return pattern;
            }
        }
    }
}
