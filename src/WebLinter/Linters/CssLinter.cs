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
                    var e = new LintingError(fileName, issue.Attributes["reason"].InnerText);
                    e.LineNumber = int.Parse(issue.Attributes["line"].InnerText) - 1;
                    e.ColumnNumber = int.Parse(issue.Attributes["char"].InnerText) - 1;
                    e.IsError = issue.Attributes["severity"].InnerText == "error";
                    e.Provider = Name;
                    Result.Errors.Add(e);
                }
            }
        }

        protected override string GetArguments(FileInfo[] files)
        {
            return $"--format=lint-xml --ignore=known-properties,ids";
        }
    }
}
