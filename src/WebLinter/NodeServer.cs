using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebLinter
{
    public class NodeServer
    {
        private const string BASE_URL = "http://localhost";
        private static Process _process;
        private static object _syncRoot = new object();
        private static AsyncLock _mutex = new AsyncLock();

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
            if (_process != null)
            {
                try
                {
                    if (!_process.HasExited)
                        _process.Kill();
                }
                finally
                {
                    _process.Dispose();
                    _process = null;
                }
            }
        }

        private async Task EnsureInitializedAsync()
        {
            using (await _mutex.LockAsync())
            {
                if (_process != null && !_process.HasExited)
                    return;

                try
                {
                    Down();
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

                    // Give the node server some time to initialize
                    await Task.Delay(100);
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
            // Creates the Socket to send data over a TCP connection.

            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                    socket.Bind(endPoint);
                    IPEndPoint endPointUsed = (IPEndPoint)socket.LocalEndPoint;
                    BasePort = endPointUsed.Port;
                }
            }
            catch (SocketException)
            {
                /* Couldn't get an available IPv4 port */
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp))
                    {
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.IPv6Any, 0);
                        socket.Bind(endPoint);
                        IPEndPoint endPointUsed = (IPEndPoint)socket.LocalEndPoint;
                        BasePort = endPointUsed.Port;
                    }
                }
                catch (SocketException)
                { /* Couldn't get an available IPv6 port either */ }
            }
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
