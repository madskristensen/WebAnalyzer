using System;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace WebLinter
{
    public static class LinterFactory
    {
        public static readonly string ExecutionPath = Path.Combine(Path.GetTempPath(), "WebLinter" + Constants.VERSION);

        private static string[] _supported = new string[] { ".JS", ".JSX", ".TS", ".COFFEE", ".LITCOFFEE", ".ICED", ".CSS" };
        private static object _syncRoot = new object();

        public static bool IsFileSupported(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToUpperInvariant();

            return _supported.Contains(extension);
        }

        public static LintingResult Lint(ISettings settings, params string[] fileNames)
        {
            string extension = Path.GetExtension(fileNames.First()).ToUpperInvariant();
            LinterBase linter = null;

            switch (extension)
            {
                case ".JS":
                case ".JSX":
                    linter = new EsLinter(settings);
                    break;

                case ".TS":
                    linter = new TsLintLinter(settings);
                    break;

                case ".COFFEE":
                case ".LITCOFFEE":
                case ".ICED":
                    linter = new CoffeeLinter(settings);
                    break;

                case ".CSS":
                    linter = new CssLinter(settings);
                    break;
            }

            if (linter != null)
            {
                lock (_syncRoot)
                {
                    Initialize();
                }

                return linter.Run(fileNames);
            }

            return null;
        }

        /// <summary>
        /// Initializes the Node environment.
        /// </summary>
        public static void Initialize()
        {
            var node_modules = Path.Combine(ExecutionPath, "node_modules");
            var node_exe = Path.Combine(ExecutionPath, "node.exe");
            var log_file = Path.Combine(ExecutionPath, "log.txt");

            if (!Directory.Exists(node_modules) || !File.Exists(node_exe) || !File.Exists(log_file) || (Directory.Exists(node_modules) && Directory.GetDirectories(node_modules).Length < 36))
            {
                OnInitializing();

                if (Directory.Exists(ExecutionPath))
                    Directory.Delete(ExecutionPath, true);

                Directory.CreateDirectory(ExecutionPath);
                SaveResourceFile(ExecutionPath, "WebLinter.Node.node.7z", "node.7z");
                SaveResourceFile(ExecutionPath, "WebLinter.Node.node_modules.7z", "node_modules.7z");
                SaveResourceFile(ExecutionPath, "WebLinter.Node.7z.exe", "7z.exe");
                SaveResourceFile(ExecutionPath, "WebLinter.Node.7z.dll", "7z.dll");
                SaveResourceFile(ExecutionPath, "WebLinter.Node.prepare.cmd", "prepare.cmd");

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

                OnInitialized();
            }
        }

        private static void SaveResourceFile(string path, string resourceName, string fileName)
        {
            using (Stream stream = typeof(LinterFactory).Assembly.GetManifestResourceStream(resourceName))
            using (FileStream fs = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                for (int i = 0; i < stream.Length; i++)
                    fs.WriteByte((byte)stream.ReadByte());
            }
        }

        private static void OnInitializing()
        {
            if (Initializing != null)
            {
                Initializing(null, EventArgs.Empty);
            }
        }

        private static void OnInitialized()
        {
            if (Initialized != null)
            {
                Initialized(null, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires when the compilers are about to be initialized.
        /// </summary>
        public static event EventHandler<EventArgs> Initializing;

        /// <summary>
        /// Fires when the compilers have been initialized.
        /// </summary>
        public static event EventHandler<EventArgs> Initialized;
    }
}
