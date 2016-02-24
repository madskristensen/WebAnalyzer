using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebLinter
{
    public static class LinterFactory
    {
        public static readonly string ExecutionPath = Path.Combine(Path.GetTempPath(), Constants.CACHE_NAME + Constants.VERSION);
        private static string[] _supported = new string[] { ".JS", ".ES6", ".JSX", ".TS", ".TSX", ".COFFEE", ".LITCOFFEE", ".ICED", ".CSS" };
        private static object _syncRoot = new object();
        private static AsyncLock _mutex = new AsyncLock();

        public static bool IsFileSupported(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToUpperInvariant();

            return _supported.Contains(extension);
        }

        public static async Task<LintingResult[]> LintAsync(ISettings settings, params string[] fileNames)
        {
            if (fileNames.Length == 0)
                return new LintingResult[0];

            string extension = Path.GetExtension(fileNames[0]).ToUpperInvariant();
            var groupedFiles = fileNames.GroupBy(f => Path.GetExtension(f).ToUpperInvariant());
            Dictionary<LinterBase, IEnumerable<string>> dic = new Dictionary<LinterBase, IEnumerable<string>>();

            foreach (var group in groupedFiles)
            {
                switch (group.Key)
                {
                    case ".JS":
                    case ".JSX":
                    case ".ES6":
                        AddLinter(dic, new EsLinter(settings), group);
                        break;

                    case ".TS":
                    case ".TSX":
                        AddLinter(dic, new TsLintLinter(settings), group);
                        break;

                    case ".COFFEE":
                    case ".LITCOFFEE":
                    case ".ICED":
                        AddLinter(dic, new CoffeeLinter(settings), group);
                        break;

                    case ".CSS":
                        AddLinter(dic, new CssLinter(settings), group);
                        break;
                }
            }

            if (dic.Count != 0)
            {
                await InitializeAsync();

                return await Task.WhenAll(dic.Select(group => group.Key.Run(group.Value.ToArray())));
            }

            return new LintingResult[0];
        }

        private static void AddLinter(Dictionary<LinterBase, IEnumerable<string>> dic, LinterBase linter, IEnumerable<string> files)
        {
            if (dic.ContainsKey(linter))
            {
                dic[linter] = dic[linter].Union(files);
            }
            else
            {
                dic.Add(linter, files);
            }
        }

        /// <summary>
        /// Initializes the Node environment.
        /// </summary>
        public static async Task InitializeAsync()
        {
            using (await _mutex.LockAsync())
            {
                var node_modules = Path.Combine(ExecutionPath, "node_modules");
                var log_file = Path.Combine(ExecutionPath, "log.txt");

                if (!Directory.Exists(node_modules) || !File.Exists(log_file) || (Directory.Exists(node_modules) && Directory.GetDirectories(node_modules).Length < 235))
                {
                    if (Directory.Exists(ExecutionPath))
                        Directory.Delete(ExecutionPath, recursive: true);

                    Directory.CreateDirectory(ExecutionPath);

                    var tasks = new List<Task>
                    {
                        SaveResourceFileAsync(ExecutionPath, "WebLinter.Node.node_modules.7z", "node_modules.7z"),
                        SaveResourceFileAsync(ExecutionPath, "WebLinter.Node.7z.exe", "7z.exe"),
                        SaveResourceFileAsync(ExecutionPath, "WebLinter.Node.7z.dll", "7z.dll"),
                        SaveResourceFileAsync(ExecutionPath, "WebLinter.Node.prepare.cmd", "prepare.cmd"),
                        SaveResourceFileAsync(ExecutionPath, "WebLinter.Node.server.js", "server.js"),
                        SaveResourceFileAsync(ExecutionPath, "WebLinter.Node.node.7z", "node.7z"),
                    };

                    await Task.WhenAll(tasks.ToArray());

                    ProcessStartInfo start = new ProcessStartInfo
                    {
                        WorkingDirectory = ExecutionPath,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = "/c prepare.cmd"
                    };

                    Process p = Process.Start(start);
                    await p.WaitForExitAsync();

                    // If this file is written, then the initialization was successful.
                    using (var writer = new StreamWriter(log_file))
                    {
                        await writer.WriteAsync(DateTime.Now.ToLongDateString());
                    }
                }
            }
        }

        private static async Task SaveResourceFileAsync(string path, string resourceName, string fileName)
        {
            using (Stream stream = typeof(LinterFactory).Assembly.GetManifestResourceStream(resourceName))
            using (FileStream fs = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }
        }
    }
}
