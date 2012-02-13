
namespace MetroDemo.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using AtomicMVVM;
    using ObservableCollectionExample;
    using Windows.Foundation.Collections;

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

        private string _search = "Windows8,Windows 8,Win8,WinRT";

        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                RaisePropertyChanged("Search");
            }
        }

        private string _ErrorMessage = "";

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                _ErrorMessage = value;
                RaisePropertyChanged("ErrorMessage");
            }
        }

        private bool _Errored = false;

        public bool Errored
        {
            get { return _Errored; }
            set
            {
                _Errored = value;
                RaisePropertyChanged("Errored");
            }
        }


        private bool _InProgress = false;

        public bool InProgress
        {
            get { return _InProgress; }
            set
            {
                _InProgress = value;
                RaisePropertyChanged("InProgress");
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
            InProgress = true;
            var query = "http://api.flickr.com/services/feeds/photos_public.gne?format=json&tagmode=any&tags=" + this.Search;
            Debug.WriteLine(query);
            var request = WebRequest.Create(query);
            request.BeginGetResponse(GotImages, request);
        }

        private void GotImages(IAsyncResult result)
        {
            InProgress = false;
            var request = result.AsyncState as WebRequest;
            WebResponse response;
            try
            {
                response = request.EndGetResponse(result);
            }
            catch (WebException)
            {
                ErrorMessage = "We cannot connect to Flickr, check your Internet connection, firewall settings and try again.";
                Errored = true;
                return;
            }

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
                Debug.WriteLine(image);
                images.Add(image);
                this.RaisePropertyChanged("Images");
                dataslugStart = raw.IndexOf(dataslug);
            }
        }
    }
}
