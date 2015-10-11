using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebLinter
{
    public class NodeServer
    {
        private const string BASE_URL = "http://localhost";
        private static Process _process;
        private static object _syncRoot = new object();

        public int BasePort { get; private set; }

        public async Task<string> CallServer(string path, object postData)
        {
            EnsureInitialized();

            string url = $"{BASE_URL}:{BasePort}/{path.ToLowerInvariant()}";

            try
            {
                using (WebClient client = new WebClient())
                {
                    string json = JsonConvert.SerializeObject(postData);
                    return await client.UploadStringTaskAsync(url, json);
                }
            }
            catch (WebException ex)
            {
                Telemetry.TrackException(ex);
                Down();
                return string.Empty;
            }
        }

        public void Down()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
                _process.Dispose();
                _process = null;
            }
        }

        internal void EnsureInitialized()
        {
            //try
            //{
            //    string url = $"{BASE_URL}:{BasePort}/ping";
            //    using (WebClient client = new WebClient())
            //    {
            //        string ping = client.DownloadString(url);

            //        if (ping == "1")
            //            return;
            //    }
            //}
            //catch (Exception)
            //{ /* Server isn't running */}

            lock (_syncRoot)
            {
                if (_process == null || _process.HasExited)
                {
                    SelectAvailablePort();

                    string node = Path.Combine(GetNodeDirectory(), "node");

                    ProcessStartInfo start = new ProcessStartInfo(node)
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = $"\"{Path.Combine(LinterFactory.ExecutionPath, "server.js")}\" {BasePort}",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    _process = Process.Start(start);
                }
            }
        }

        private void SelectAvailablePort()
        {
            Random rand = new Random();
            TcpConnectionInformation[] connections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();

            do
                BasePort = rand.Next(1024, 65535);
            while (connections.Any(t => t.LocalEndPoint.Port == BasePort));
        }

        private static string GetNodeDirectory()
        {
            string toolsDir = Environment.GetEnvironmentVariable("VS140COMNTOOLS");
            string parent = Directory.GetParent(toolsDir).Parent.FullName;
            return Path.Combine(parent, @"IDE\Extensions\Microsoft\Web Tools\External\Node");
        }
    }
}
