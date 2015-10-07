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
            HelpLinkFormat = "https://github.com/palantir/tslint?rule={0}#supported-rules";
        }

        protected override void ParseErrors(string output)
        {
            var array = JArray.Parse(output);

            foreach (JObject obj in array)
            {
                string fileName = obj["name"].Value<string>().Replace("/", "\\");

                var le = new LintingError(fileName);
                le.Message = obj["failure"].Value<string>();
                le.LineNumber = obj["startPosition"]["line"].Value<int>();
                le.ColumnNumber = obj["startPosition"]["character"].Value<int>();
                le.IsError = false;
                le.ErrorCode = obj["ruleName"].Value<string>();
                le.Provider = this;
                Result.Errors.Add(le);
            }
        }
    }
}
