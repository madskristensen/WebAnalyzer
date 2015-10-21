using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebLinter
{
    public class ServerPostData
    {
        [JsonProperty("config")]
        public string Config { get; set; }

        [JsonProperty("files")]
        public IEnumerable<string> Files { get; set; }
    }
}
