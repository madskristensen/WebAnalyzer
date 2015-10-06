using System.IO;
using Newtonsoft.Json.Linq;

namespace WebLinter
{
    internal class EsLinter : LinterBase
    {
        public EsLinter(ISettings settings) : base(settings)
        {
            Name = "ESLint";
            ConfigFileName = ".eslintrc";
            ErrorMatch = "Error";
            IsEnabled = Settings.ESLintEnable;
        }

        protected override void ParseErrors(string output)
        {
            var array = JArray.Parse(output);

            foreach (JObject obj in array)
            {
                string fileName = obj["filePath"].Value<string>();

                foreach (JObject error in obj["messages"])
                {
                    var e = new LintingError(fileName, error["message"].Value<string>());
                    e.LineNumber = error["line"].Value<int>() - 1;
                    e.ColumnNumber = error["column"].Value<int>() - 1;
                    e.IsError = error["severity"].Value<int>() == 2;
                    e.ErrorCode = error["ruleId"].Value<string>();
                    e.Provider = Name;
                    Result.Errors.Add(e);
                }
            }
        }

        protected override string GetArguments(FileInfo[] files)
        {
            return $"--format=json";
        }
    }
}
