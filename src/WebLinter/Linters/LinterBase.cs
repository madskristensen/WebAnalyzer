using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WebLinter
{
    public abstract class LinterBase
    {
        public LinterBase(ISettings settings)
        {
            Settings = settings;
        }

        public LintingResult Run(string fileName)
        {
            Result = new LintingResult(fileName);

            if (!IsEnabled)
                return Result;

            FileInfo file = new FileInfo(fileName);

            if (!file.Exists)
            {
                Result.Errors.Add(new LintingError(file.FullName, "The file doesn't exist"));
                return Result;
            }

            return Lint(file);
        }

        protected abstract LintingResult Lint(FileInfo file);

        public abstract string Name { get; }

        public abstract bool IsEnabled { get; }

        protected ISettings Settings { get; }

        protected LintingResult Result { get; private set; }

        protected void RunProcess(FileInfo file, string command, out string output, out string error, string arguments = "")
        {
            ProcessStartInfo start = new ProcessStartInfo
            {
                WorkingDirectory = file.Directory.FullName,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe",
                Arguments = $"/c \"\"{Path.Combine(LinterFactory.ExecutionPath, $"node_modules\\.bin\\{command}")}\" {arguments} \"{file.FullName}\"\"",
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

        protected void AddError(FileInfo file, Match match, bool isError)
        {
            var e = new LintingError(file.FullName, match.Groups["message"].Value);
            e.LineNumber = int.Parse(match.Groups["line"].Value);
            e.ColumnNumber = int.Parse(match.Groups["column"].Value);
            e.IsError = isError;
            e.Provider = Name;
            Result.Errors.Add(e);
        }
    }
}
