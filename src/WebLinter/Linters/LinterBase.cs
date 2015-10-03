using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace WebLinter
{
    public abstract class LinterBase
    {
        public abstract LintingResult Lint(string fileName);

        public abstract string Name { get; }

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

        protected void AddError(FileInfo file, LintingResult result, Match match, int lineAdjust = 0)
        {
            var e = new LintingError(file.FullName, match.Groups["message"].Value);
            e.LineNumber = int.Parse(match.Groups["line"].Value) + lineAdjust;
            e.ColumnNumber = int.Parse(match.Groups["column"].Value);
            e.Provider = Name;
            result.Errors.Add(e);
        }
    }
}
