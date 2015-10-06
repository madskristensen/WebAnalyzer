using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableManager;
using WebLinter;

namespace WebLinterVsix
{
    class LintingErrorSnapshot : TableEntriesSnapshotBase
    {
        private readonly string _filePath;

        public readonly List<LintingError> Errors = new List<LintingError>();

        internal LintingErrorSnapshot(string filePath, IEnumerable<LintingError> errors)
        {
            _filePath = filePath;
            Errors.AddRange(errors);
        }

        public override int Count
        {
            get { return Errors.Count; }
        }

        public override int VersionNumber { get; } = 1;

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            content = null;

            if ((index >= 0) && (index < Errors.Count))
            {
                if (columnName == StandardTableKeyNames.DocumentName)
                {
                    content = _filePath;
                  }
                else if (columnName == StandardTableKeyNames.ErrorCategory)
                {
                    content = Constants.VSIX_NAME;
                 }
                else if (columnName == StandardTableKeyNames.ErrorSource)
                {
                    content = "Spelling";
                }
                else if (columnName == StandardTableKeyNames.Line)
                {
                    content = Errors[index].LineNumber;
                }
                else if (columnName == StandardTableKeyNames.Column)
                {
                    content = Errors[index].ColumnNumber;
                }
                else if (columnName == StandardTableKeyNames.Text)
                {
                    content = Errors[index].Message;
                }
                else if (columnName == StandardTableKeyNames.ErrorSeverity)
                {
                    content = Errors[index].IsError ? __VSERRORCATEGORY.EC_ERROR : __VSERRORCATEGORY.EC_WARNING;
                }
                else if (columnName == StandardTableKeyNames.ErrorSource)
                {
                    content = ErrorSource.Other;
                }
                else if (columnName == StandardTableKeyNames.BuildTool)
                {
                    content = Errors[index].Provider;
                }
                else if (columnName == StandardTableKeyNames.ErrorCode)
                {
                    content = Errors[index].ErrorCode;
                }
                else if ((columnName == StandardTableKeyNames.ErrorCodeToolTip) || (columnName == StandardTableKeyNames.HelpLink))
                {
                    string url = string.Format("http://www.bing.com/search?q={0} {1}", Errors[index].Provider, Errors[index].ErrorCode);
                    content = Uri.EscapeUriString(url);
                }

                // We should also be providing values for StandardTableKeyNames.Project & StandardTableKeyNames.ProjectName but that is
                // beyond the scope of this sample.
            }

            return content != null;
        }
    }
}
