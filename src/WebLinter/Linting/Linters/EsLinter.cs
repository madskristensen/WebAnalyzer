using System;
using Newtonsoft.Json.Linq;

namespace WebLinter
{
    internal class EsLinter : LinterBase
    {
        public EsLinter(ISettings settings) : base(settings)
        {
            Name = "ESLint";
            ConfigFileName = ".eslintrc";
            IsEnabled = Settings.ESLintEnable;
        }

        protected override void ParseErrors(string output)
        {
            var array = JArray.Parse(output);

            foreach (JObject obj in array)
            {
                string fileName = obj["filePath"]?.Value<string>();

                if (string.IsNullOrEmpty(fileName))
                    continue;

                foreach (JObject error in obj["messages"])
                {
                    var le = new LintingError(fileName);
                    le.Message = error["message"]?.Value<string>();
                    le.LineNumber = error["line"]?.Value<int>() - 1 ?? 0;
                    le.ColumnNumber = error["column"]?.Value<int>() - 1 ?? 0;
                    le.IsError = error["severity"]?.Value<int>() == 2;
                    le.ErrorCode = error["ruleId"]?.Value<string>();
                    le.HelpLink = GetHelpLink(le.ErrorCode);
                    le.Provider = this;
                    Result.Errors.Add(le);
                }
            }
        }

        private string GetHelpLink(string errorCode)
        {
            int slash = errorCode.IndexOf('/');

            if (slash == -1)
                return $"http://eslint.org/docs/rules/{errorCode}";

            string plugin = errorCode.Substring(0, slash).ToLowerInvariant();
            string error = errorCode.Substring(slash + 1);

            if (plugin == "react")
                return $"https://github.com/yannickcr/eslint-plugin-react/blob/master/docs/rules/{error}.md";

            return Uri.EscapeUriString($"http://www.bing.com/search?q={Name} {errorCode}");
        }
    }
}
