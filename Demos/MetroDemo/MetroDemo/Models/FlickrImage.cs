//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace MetroDemo.Models
{
    using System;
    using Newtonsoft.Json;

    public class FlickrImage
    {       
        [JsonProperty("media")]
        public M m { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        public Uri DownloadableItem
        {
            get
            {
                return new Uri(Media.Replace("_m.jpg", "_b.jpg"));
            }
        }

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
