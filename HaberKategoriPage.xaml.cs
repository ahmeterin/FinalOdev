using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace VizeOdev
{
    public partial class HaberKategoriPage : ContentPage
    {
        public HaberKategoriPage()
        {
            InitializeComponent();

            lstMenu.ItemsSource = Kategori.liste;

            // Fetch and parse the RSS feed for the first category as an example (you can modify this based on the selected category)
            lstMenu.SelectionChanged += async (sender, e) =>
            {
                if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
                {
                    var selectedCategory = (Kategori)e.CurrentSelection[0];
                    var rssLink = selectedCategory.Link;

                    if (!string.IsNullOrEmpty(rssLink))
                    {
                        var haberList = await GetHaberListAsync(rssLink);
                        lstHaber.ItemsSource = haberList;
                    }
                }
            };
        }


        private async Task InitializeHaberListAsync(string rssLink)
        {
            try
            {
                var haberList = await GetHaberListAsync(rssLink);
                lstHaber.ItemsSource = haberList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing haber list: {ex.Message}");
            }
        }

        private async Task<List<Haber>> GetHaberListAsync(string rssLink)
        {
            var haberList = new List<Haber>();

            try
            {
                var reader = XmlReader.Create(rssLink);
                var feed = SyndicationFeed.Load(reader);

                foreach (var item in feed.Items)
                {
                    // Use HtmlAgilityPack to extract text content from HTML
                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(item.Summary.Text);

                    var haber = new Haber
                    {
                        Title = item.Title.Text,
                        Detay = htmlDoc.DocumentNode.InnerText, // Extract text content
                        Link = item.Links.FirstOrDefault()?.Uri?.AbsoluteUri
                    };

                    haberList.Add(haber);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching RSS feed: {ex.Message}");
                throw; // Rethrow the exception to propagate it to the caller
            }

            return haberList;
        }


        private void lstMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                var selectedCategory = (Kategori)e.CurrentSelection[0];
                Console.WriteLine("Category selected: " + selectedCategory.Baslik);
                // Handle selection change for lstMenu
            }
        }

        private void lstHaber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection != null && e.CurrentSelection.Count > 0)
            {
                var selectedNews = (Haber)e.CurrentSelection[0];
                Console.WriteLine("News item selected: " + selectedNews.Title);
                // Handle selection change for lstHaber
            }
        }

        class Kategori
        {
            public string Baslik { get; set; }
            public string Link { get; set; }

            public static List<Kategori> liste = new List<Kategori>()
            {
                new Kategori() { Baslik = "Manşet", Link = "https://www.trthaber.com/manset_articles.rss"},
                new Kategori() { Baslik = "Eğitim", Link = "https://www.trthaber.com/egitim_articles.rss"},
                new Kategori() { Baslik = "Spor", Link = "https://www.trthaber.com/spor_articles.rss"},
                new Kategori() { Baslik = "Bilim Teknoloji", Link = "https://www.trthaber.com/bilim_teknoloji_articles.rss"},
                new Kategori() { Baslik = "Güdem", Link = "https://www.trthaber.com/gundem_articles.rss"}
            };
        }

        class Haber
        {
            public string Detay { get; set; }
            public string Link { get; set; }
            public string Title { get; set; }
        }
    }
}
