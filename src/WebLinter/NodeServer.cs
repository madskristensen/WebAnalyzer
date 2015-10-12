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

        public async Task<string> CallServerAsync(string path, object postData)
        {
            await EnsureInitializedAsync();

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

        private async Task EnsureInitializedAsync()
        {
            if (_process == null || _process.HasExited)
            {
                try
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

                    await SendPingAsync();
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    Down();
                }
            }
        }

        private async Task SendPingAsync()
        {
            using (WebClient client = new WebClient())
            {
                string url = $"{BASE_URL}:{BasePort}/ping";
                string ping = await client.DownloadStringTaskAsync(url);
                Debug.WriteLine(ping);
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

            if (!string.IsNullOrEmpty(toolsDir))
            {
                string parent = Directory.GetParent(toolsDir).Parent.FullName;
                return Path.Combine(parent, @"IDE\Extensions\Microsoft\Web Tools\External\Node");
            }

            return string.Empty;
        }
    }
}
