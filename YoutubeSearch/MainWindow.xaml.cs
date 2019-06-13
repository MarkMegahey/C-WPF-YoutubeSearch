using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using YoutubeSearch.Models;


namespace YoutubeSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<YTVideo> videos;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void YoutubeSearchButton_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            await LoadDataCollection();
            YoutubeSearchDisplay.ItemsSource = videos;
        }

        private async Task LoadDataCollection()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyB1OOSpTREs85WUMvIgJvLTZKye4BVsoFU",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = YoutubeSearchBar_Textbox.Text;
            searchListRequest.MaxResults = 50;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            videos = new List<YTVideo>();

            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        YTVideo video = new YTVideo();
                        video.Title = searchResult.Snippet.Title;
                        video.Author = searchResult.Snippet.ChannelTitle;
                        video.URL = $"https://www.youtube.com/watch?v={searchResult.Id.VideoId}";
                        byte[] imageBytes = new WebClient().DownloadData(searchResult.Snippet.Thumbnails.Default__.Url);
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            var imageSource = new BitmapImage();
                            imageSource.BeginInit();
                            imageSource.StreamSource = ms;
                            imageSource.EndInit();

                            // Assign the Source property of your image
                            video.Thumbnail = new Image { Source = imageSource };
                        }
                        videos.Add(video);
                        break;
                }
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            YTVideo video = (YTVideo)YoutubeSearchDisplay.SelectedItem;

            if (video == null)
                return;

            Process.Start(video.URL);
        }
    }
}
