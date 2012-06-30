
namespace MetroDemo.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using AtomicMVVM;
    using MetroDemo.Models;
    using Newtonsoft.Json;
    using Windows.ApplicationModel.DataTransfer;
    using Windows.ApplicationModel.Search;
    using Windows.Storage.Pickers;
    using Windows.Storage.Streams;
    using Windows.System;
    using Windows.UI.Popups;

    public class Windows8 : CoreData
    {
        private DownloadManager manager = DownloadManager.GetInstance();
        private string _search;
        private bool _InProgress = false;
        private FlickrImage selectedItem;

        public FlickrImage SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (value != selectedItem)
                {
                    selectedItem = value;
                    RaisePropertyChanged("SelectedItem");
                }
            }
        }

        public ObservableCollection<FlickrImage> Images { get; set; }

        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                RaisePropertyChanged("Search");
            }
        }

        public bool InProgress
        {
            get { return _InProgress; }
            set
            {
                _InProgress = value;
                RaisePropertyChanged("InProgress");
            }
        }

        public Windows8(string initialSearch)
        {
            Images = new ObservableCollection<FlickrImage>();
            this.Search = initialSearch;
            GetImages();

            var searchPane = SearchPane.GetForCurrentView();
            searchPane.QuerySubmitted += (sender, args) =>
                {
                    this.Search = args.QueryText;
                    GetImages();
                };

            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += (sender, args) =>
                {
                    if (this.SelectedItem == null)
                    {
                        args.Request.FailWithDisplayText("No image selected - select an image first");
                        return;
                    }

                    args.Request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(this.SelectedItem.Media)));
                    args.Request.Data.Properties.Description = this.SelectedItem.Link;
                    args.Request.Data.Properties.Title = this.SelectedItem.Title;
                };
        }

        public void Refresh()
        {
            GetImages();
        }

        public void ClearResults()
        {
            this.Images.Clear();
        }

        [ReevaluateProperty("SelectedItem")]
        public bool CanSaveSelected()
        {
            return this.SelectedItem != null;
        }

        public async void SaveSelected()
        {
            var picker = new FileSavePicker();
            picker.SuggestedFileName = this.SelectedItem.DownloadableItem.Segments.Last();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.CommitButtonText = "Save";
            picker.FileTypeChoices.Add("Image", new List<string>() { ".jpg" });
            var result = await picker.PickSaveFileAsync();
            if (result != null)
            {
                manager.AddDownload(this.SelectedItem.DownloadableItem, result);
            }
        }

        [ReevaluateProperty("SelectedItem")]
        public bool CanLaunchSelected()
        {
            return this.SelectedItem != null;
        }

        public void LaunchSelected()
        {
            Launcher.LaunchUriAsync(new System.Uri(this.SelectedItem.Link));
        }

        public void About()
        {
            Launcher.LaunchUriAsync(new System.Uri("https://bitbucket.org/rmaclean/atomicmvvm"));
        }

        private async void GetImages()
        {
            InProgress = true;
            var query = "http://api.flickr.com/services/feeds/photos_public.gne?format=json&tagmode=any&tags=" + this.Search;

            var request = WebRequest.Create(query);
            WebResponse response = null;
            try
            {
                response = await request.GetResponseAsync();
                InProgress = false;
            }
            catch (WebException ex)
            {
                InProgress = false;
                var dialog = new MessageDialog("We cannot connect to Flickr, check your Internet connection, firewall settings and try again.");
                dialog.ShowAsync();
                return;

            }

            string raw = string.Empty;
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                raw = streamReader.ReadToEnd();
            }

            var stripped = raw.Substring(15).Substring(0, raw.Length - 16);

            var results = JsonConvert.DeserializeObject<FlickrFeed>(stripped);

            foreach (var item in results.Items)
            {
                if (Images.Any(_ => _.Media == item.Media))
                {
                    continue;
                }

                Images.Add(item);
                this.RaisePropertyChanged("Images");
            }
        }
    }
}
