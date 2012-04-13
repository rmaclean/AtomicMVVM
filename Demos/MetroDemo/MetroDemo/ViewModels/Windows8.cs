
namespace MetroDemo.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using AtomicMVVM;

    public class Windows8 : CoreData
    {
        public ObservableCollection<string> Images {get;set;}     
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
            Images = new ObservableCollection<string>();
            GetImages();
        }

        public void Refresh()
        {
            GetImages();
        }

        private async void GetImages()
        {
            InProgress = true;
            var query = "http://api.flickr.com/services/feeds/photos_public.gne?format=json&tagmode=any&tags=" + this.Search;
            Debug.WriteLine(query);
            var request = WebRequest.Create(query);
            WebResponse response = null;
            try
            {
                response = await request.GetResponseAsync();
                InProgress = false;                
            }
            catch (WebException)
            {
                InProgress = false;
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
                Images.Add(image);
                this.RaisePropertyChanged("Images");
                dataslugStart = raw.IndexOf(dataslug);
            }
        }
    }
}
