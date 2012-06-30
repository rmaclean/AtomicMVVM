
namespace MetroDemo.Models
{
    using Newtonsoft.Json;

    public class FlickrFeed
    {
        [JsonProperty("items")]
        public FlickrImage[] Items { get; set; }
    }
}
