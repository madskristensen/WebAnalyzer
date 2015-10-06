using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace WebLinter
{
    internal class CoffeeLinter : LinterBase
    {
        public CoffeeLinter(ISettings settings) : base(settings)
        {
            Name = "CoffeeLint";
            ConfigFileName = "coffeelint.json";
            ErrorMatch = "error";
            IsEnabled = Settings.CoffeeLintEnable;
        }

        protected override void ParseErrors(string output)
        {
            var root = JObject.Parse(output);

            foreach (JProperty obj in root.Children<JProperty>())
            {
                string fileName = obj.Name;

                foreach (JObject error in obj.Value)
                {
                    if (error["name"] == null) // It's a compiler error
                        continue;

                    var e = new LintingError(fileName, error["message"].Value<string>());
                    e.LineNumber = error["lineNumber"].Value<int>() - 1;
                    e.ColumnNumber = 0;
                    e.IsError = error["level"].Value<string>() == "error";
                    e.ErrorCode = error["rule"].Value<string>();
                    e.Provider = Name;
                    Result.Errors.Add(e);
                }
            }
        }

        protected override string GetArguments(FileInfo[] files)
        {
            // See built-in reporters here https://github.com/clutchski/coffeelint/tree/master/src/reporters
            return $"--reporter=raw -f coffeelint.json";
        }
    }
}
