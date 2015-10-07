using System.IO;
using System.Xml;

namespace WebLinter
{
    internal class CssLinter : LinterBase
    {
        public CssLinter(ISettings settings) : base(settings)
        {
            Name = "CssLint";
            ConfigFileName = ".csslintrc";
            ErrorMatch = "Error";
            IsEnabled = Settings.CssLintEnable;
            HelpLinkFormat = "https://github.com/CSSLint/csslint/wiki/Rules/#{0}";
        }

        protected override void ParseErrors(string output)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(output);

            foreach (XmlNode file in doc.SelectNodes("//file"))
            {
                string fileName = file.Attributes["name"].InnerText;

                foreach (XmlNode issue in file.SelectNodes("issue"))
                {
                    var le = new LintingError(fileName);
                    le.Message = issue.Attributes["reason"].InnerText;
                    le.LineNumber = int.Parse(issue.Attributes["line"].InnerText) - 1;
                    le.ColumnNumber = int.Parse(issue.Attributes["char"].InnerText) - 1;
                    le.IsError = issue.Attributes["severity"].InnerText == "error";
                    le.ErrorCode = issue.Attributes["rule"].InnerText;
                    le.Provider = this;
                    Result.Errors.Add(le);
                }
            }
        }

        protected override string GetArguments(FileInfo[] files)
        {
            return $"--format=vs-xml";
        }
    }
}
