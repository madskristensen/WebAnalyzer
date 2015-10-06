using System.IO;
using Newtonsoft.Json.Linq;

namespace WebLinter
{
    internal class TsLintLinter : LinterBase
    {
        public TsLintLinter(ISettings settings) : base(settings)
        {
            Name = "TSLint";
            ConfigFileName = "tslint.json";
            IsEnabled = Settings.TSLintEnable;
        }

        protected override void ParseErrors(string output)
        {
            var array = JArray.Parse(output);

            foreach (JObject obj in array)
            {
                string fileName = obj["name"].Value<string>().Replace("/", "\\");

                var e = new LintingError(fileName, obj["failure"].Value<string>());
                e.LineNumber = obj["startPosition"]["line"].Value<int>();
                e.ColumnNumber = obj["startPosition"]["character"].Value<int>();
                e.IsError = false;
                e.ErrorCode = obj["ruleName"].Value<string>();
                e.Provider = Name;
                Result.Errors.Add(e);
            }
        }

        protected override string GetArguments(FileInfo[] files)
        {
            // See built-in reporters here https://www.npmjs.com/package/tslint#cli
            return $"--format=json";
        }
    }
}
