using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System;

namespace WebLinter
{
    public abstract class LinterBase
    {
        private Regex _rx;
        private string _cwd;

        public LinterBase(ISettings settings, string workingDirectory, Regex regex)
        {
            Settings = settings;
            _rx = regex;
            _cwd = workingDirectory;
        }

        public LintingResult Run(params string[] files)
        {
            Result = new LintingResult(files);

            if (!IsEnabled)
                return Result;

            List<FileInfo> fileInfos = new List<FileInfo>();

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                if (!fileInfo.Exists)
                {
                    Result.Errors.Add(new LintingError(fileInfo.FullName, "The file doesn't exist"));
                    return Result;
                }

                fileInfos.Add(fileInfo);
            }

            return Lint(fileInfos.ToArray());
        }

        protected LintingResult Lint(params FileInfo[] files)
        {
            string args = GetArguments(files);
            string output, error;
            RunProcess($"{Name}.cmd", out output, out error, args, files);

            if (!string.IsNullOrEmpty(output))
            {
                foreach (Match match in _rx.Matches(output))
                {
                    AddError(match, Settings.CssLintAsErrors);
                }
            }
            else if (!string.IsNullOrEmpty(error))
            {
                Result.Errors.Add(new LintingError(files.First().FullName, error));
            }

            return Result;
        }

        protected string Name { get; set; }

        protected virtual string ConfigFileName { get; set; }

        protected virtual bool IsEnabled { get; set; }

        protected ISettings Settings { get; }

        protected LintingResult Result { get; private set; }

        protected void RunProcess(string command, out string output, out string error, string arguments = "", params FileInfo[] files)
        {
            string fileArg = string.Join(" ", files.Select(f => $"\"{f.FullName}\""));

            ProcessStartInfo start = new ProcessStartInfo
            {
                WorkingDirectory = _cwd,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = $"/c \"\"{Path.Combine(LinterFactory.ExecutionPath, $"node_modules\\.bin\\{command}")}\" {arguments} {fileArg}\"",
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            start.EnvironmentVariables["PATH"] = LinterFactory.ExecutionPath + ";" + start.EnvironmentVariables["PATH"];

            Process p = Process.Start(start);
            var stdout = p.StandardOutput.ReadToEndAsync();
            var stderr = p.StandardError.ReadToEndAsync();
            p.WaitForExit();

            output = stdout.Result.Trim();
            error = stderr.Result.Trim();
        }

        protected virtual string GetArguments(FileInfo[] files)
        {
            return string.Empty;
        }

        protected virtual string FindConfigFile(FileInfo file)
        {
            var dir = file.Directory;

            while (dir != null)
            {
                string rc = Path.Combine(dir.FullName, ConfigFileName);
                if (File.Exists(rc))
                    return $"--config=\"{rc}\"";

                dir = dir.Parent;
            }

            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string result = Path.Combine(userFolder, ConfigFileName);

            return $"--config=\"{result}\""; ;
        }

        protected void AddError(Match match, bool isError)
        {
            string fileName = match.Groups["file"].Value;

            var e = new LintingError(fileName, match.Groups["message"].Value);
            e.LineNumber = int.Parse(match.Groups["line"].Value);
            e.ColumnNumber = int.Parse(match.Groups["column"].Success ? match.Groups["column"].Value : "0");
            e.IsError = isError;
            e.Provider = Name;
            Result.Errors.Add(e);
        }
    }
}
