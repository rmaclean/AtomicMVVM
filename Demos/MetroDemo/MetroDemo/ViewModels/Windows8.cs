using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AtomicMVVM;
using ObservableCollectionExample;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MetroDemo.ViewModels
{
    public class Windows8 : CoreData
    {
        private ObservableCollection<string> images = new ObservableCollection<string>();
        public IObservableVector<string> Images
        {
            get
            {
                return new ObservableCollectionShim<string>(images);
            }
        }

        public Windows8()
        {
            GetImages();
        }

        public void Refresh()
        {
            GetImages();
        }

        private void GetImages()
        {
            var request = WebRequest.Create("http://api.flickr.com/services/feeds/photos_public.gne?format=json&tagmode=any&tags=Windows8,Windows 8,Win8,WinRT");
            request.BeginGetResponse(GotImages, request);
        }

        private void GotImages(IAsyncResult result)
        {
            var request = result.AsyncState as WebRequest;
            var response = request.EndGetResponse(result);
            string raw;
            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                raw = streamReader1.ReadToEnd();
            }

            var dataslug = "\"media\": {\"m\":\"";
            var dataslugStart = raw.IndexOf(dataslug);
            while (dataslugStart > -1)
            {
                raw = raw.Substring(dataslugStart + dataslug.Length);
                var dataslugEnd = raw.IndexOf('"');
                var image = raw.Substring(0, dataslugEnd);
                if (images.Count > 0)
                {
                    if (!images.Contains(image))
                    {
                        images.Insert(0, image);
                    }
                }
                else
                {
                    images.Add(image);
                }
                this.RaisePropertyChanged("Images");
                dataslugStart = raw.IndexOf(dataslug);
            }
        }
    }
}
