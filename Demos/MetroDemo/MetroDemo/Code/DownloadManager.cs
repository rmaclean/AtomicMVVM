namespace MetroDemo
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.Networking.BackgroundTransfer;
    using Windows.Storage;
    public class DownloadManager
    {
        private static DownloadManager instance;

        private DownloadManager() { }

        public static DownloadManager GetInstance()
        {
            if (instance == null)
            {
                instance = new DownloadManager();
                instance.DiscoverActiveDownloads();
            }

            return instance;
        }

        private async void DiscoverActiveDownloads()
        {
            await DiscoverActiveDownloadsAsync();
        }

        // Enumerate the downloads that were going on in the background while the app was closed.
        private async Task DiscoverActiveDownloadsAsync()
        {
            IReadOnlyList<DownloadOperation> downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
            if (downloads.Count > 0)
            {
                foreach (DownloadOperation download in downloads)
                {
                    download.AttachAsync();
                }
            }
        }

        public void AddDownload(Uri uri, IStorageFile resultFile)
        {
            BackgroundDownloader downloader = new BackgroundDownloader();
            DownloadOperation download = downloader.CreateDownload(uri, resultFile);
            download.CostPolicy = BackgroundTransferCostPolicy.Always;
            download.StartAsync();
        }
    }
}