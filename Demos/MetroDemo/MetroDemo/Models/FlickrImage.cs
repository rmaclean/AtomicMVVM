

namespace MetroDemo.Models
{
    using Newtonsoft.Json;

    public class FlickrImage
    {       
        [JsonProperty("media")]
        public M m { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        public string Media
        {
            get
            {
                return m.m;
            }
        }
    }

    public class M
    {
        [JsonProperty("m")]
        public string m { get; set; }
    }
}
