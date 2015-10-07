using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableManager;
using WebLinter;

namespace WebLinterVsix
{
    class TableEntriesSnapshot : TableEntriesSnapshotBase
    {
        private ProjectItem _item;
        private readonly List<LintingError> _errors = new List<LintingError>();

        internal TableEntriesSnapshot(string filePath, IEnumerable<LintingError> errors)
        {
            FilePath = filePath;
            _errors.AddRange(errors);
        }

        public override int Count
        {
            get { return _errors.Count; }
        }

        public string FilePath { get; }

        public override int VersionNumber { get; } = 1;

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            content = null;

            if ((index >= 0) && (index < _errors.Count))
            {
                if (columnName == StandardTableKeyNames.DocumentName)
                {
                    content = FilePath;
                }
                else if (columnName == StandardTableKeyNames.ErrorCategory)
                {
                    content = Constants.VSIX_NAME;
                }
                else if (columnName == StandardTableKeyNames.ErrorSource)
                {
                    content = Constants.VSIX_NAME;
                }
                else if (columnName == StandardTableKeyNames.Line)
                {
                    content = _errors[index].LineNumber;
                }
                else if (columnName == StandardTableKeyNames.Column)
                {
                    content = _errors[index].ColumnNumber;
                }
                else if (columnName == StandardTableKeyNames.Text)
                {
                    content = _errors[index].Message;
                }
                else if (columnName == StandardTableKeyNames.ErrorSeverity)
                {
                    content = _errors[index].IsError ? __VSERRORCATEGORY.EC_ERROR : __VSERRORCATEGORY.EC_WARNING;
                }
                else if (columnName == StandardTableKeyNames.Priority)
                {
                    content = _errors[index].IsError ? vsTaskPriority.vsTaskPriorityHigh : vsTaskPriority.vsTaskPriorityMedium;
                }
                else if (columnName == StandardTableKeyNames.ErrorSource)
                {
                    content = ErrorSource.Other;
                }
                else if (columnName == StandardTableKeyNames.BuildTool)
                {
                    content = _errors[index].Provider;
                }
                else if (columnName == StandardTableKeyNames.ErrorCode)
                {
                    content = _errors[index].ErrorCode;
                }
                else if (columnName == StandardTableKeyNames.ProjectName)
                {
                    _item = _item ?? WebLinterPackage.Dte.Solution.FindProjectItem(_errors[index].FileName);

                    if (_item != null && _item.ContainingProject != null)
                        content = _item.ContainingProject.Name;
                }
                else if ((columnName == StandardTableKeyNames.ErrorCodeToolTip) || (columnName == StandardTableKeyNames.HelpLink))
                {
                    var error = _errors[index];
                    string url;
                    if (!string.IsNullOrEmpty(error.Provider.HelpLinkFormat))
                    {
                        url = string.Format(error.Provider.HelpLinkFormat, error.ErrorCode);
                    }
                    else
                    {
                        url = string.Format("http://www.bing.com/search?q={0} {1}", _errors[index].Provider.Name, _errors[index].ErrorCode);
                    }

                    content = Uri.EscapeUriString(url);
                }
            }

            return content != null;
        }
    }
}
