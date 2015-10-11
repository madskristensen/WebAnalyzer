using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebLinter
{
    public static class LinterFactory
    {
        public static readonly string ExecutionPath = Path.Combine(Path.GetTempPath(), Constants.CACHE_NAME + Constants.VERSION);
        private static string[] _supported = new string[] { ".JS", ".ES6", ".JSX", ".TS", ".TSX", ".COFFEE", ".LITCOFFEE", ".ICED", ".CSS" };
        private static object _syncRoot = new object();

        public static bool IsFileSupported(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToUpperInvariant();

            return _supported.Contains(extension);
        }

        public static async Task<IEnumerable<LintingResult>> Lint(ISettings settings, params string[] fileNames)
        {
            List<LintingResult> list = new List<LintingResult>();

            if (fileNames.Length == 0)
                return list;

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
                Initialize();
                var tasks = new List<Task<LintingResult>>();

                foreach (var linter in dic.Keys)
                {
                    var files = dic[linter].ToArray();
                    tasks.Add(linter.Run(files));
                }

                list.AddRange(await Task.WhenAll(tasks));
            }

            return list;
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

        public static void WarmUp()
        {
            Initialize();
            LinterBase.Server.EnsureInitialized();
        }

        /// <summary>
        /// Initializes the Node environment.
        /// </summary>
        private static void Initialize()
        {
            lock (_syncRoot)
            {
                var node_modules = Path.Combine(ExecutionPath, "node_modules");
                var log_file = Path.Combine(ExecutionPath, "log.txt");

                if (!Directory.Exists(node_modules) || !File.Exists(log_file) || (Directory.Exists(node_modules) && Directory.GetDirectories(node_modules).Length < 315))
                {
                    if (Directory.Exists(ExecutionPath))
                        Directory.Delete(ExecutionPath, true);

                    Directory.CreateDirectory(ExecutionPath);
                    SaveResourceFile(ExecutionPath, "WebLinter.Node.node_modules.7z", "node_modules.7z");
                    SaveResourceFile(ExecutionPath, "WebLinter.Node.7z.exe", "7z.exe");
                    SaveResourceFile(ExecutionPath, "WebLinter.Node.7z.dll", "7z.dll");
                    SaveResourceFile(ExecutionPath, "WebLinter.Node.prepare.cmd", "prepare.cmd");
                    SaveResourceFile(ExecutionPath, "WebLinter.Node.server.js", "server.js");

                    ProcessStartInfo start = new ProcessStartInfo
                    {
                        WorkingDirectory = ExecutionPath,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = "/c prepare.cmd"
                    };

                    Process p = Process.Start(start);
                    p.WaitForExit();

                    // If this file is written, then the initialization was successful.
                    File.WriteAllText(log_file, DateTime.Now.ToLongDateString());
                }
            }
        }

        private static void SaveResourceFile(string path, string resourceName, string fileName)
        {
            using (Stream stream = typeof(LinterFactory).Assembly.GetManifestResourceStream(resourceName))
            using (FileStream fs = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                stream.CopyTo(fs);
            }
        }
    }
}
