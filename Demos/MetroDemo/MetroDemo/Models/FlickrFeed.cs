//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace MetroDemo.Models
{
    using Newtonsoft.Json;

    public class FlickrFeed
    {
        [JsonProperty("items")]
        public FlickrImage[] Items { get; set; }
    }
}
