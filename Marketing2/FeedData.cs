using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using System.Xml.Serialization;
using System.Runtime.Serialization;
namespace Marketing2
{
    // FeedData
    // Holds info for a single blog feed, including a list of blog posts (FeedItem).

    [KnownType(typeof(FeedItem))] 
    [DataContract]
    public class FeedData
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime PubDate { get; set; }

        [DataMember]
        private List<FeedItem> _Items = new List<FeedItem>();

        public List<FeedItem> Items
        {
            get
            {
                return this._Items;
            }
        }

        
    }

    // FeedItem
    // Holds info for a single blog post. 
    [DataContract]
    public class FeedItem
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public DateTime PubDate { get; set; }
        [DataMember]
        public Uri Link { get; set; }
    }

    // FeedDataSource
    // Holds a collection of blog feeds (FeedData), and contains methods needed to
    // retreive the feeds.
    public class FeedDataSource
    {
        private ObservableCollection<FeedData> _Feeds = new ObservableCollection<FeedData>();
        public ObservableCollection<FeedData> Feeds
        {
            get
            {
                return this._Feeds;
            }
        }

        public async Task GetFeedsAsync()
        {
            Task<FeedData> feed1 =
                GetFeedAsync("http://feeds.feedburner.com/BBurg");
            Task<FeedData> feed2 =
                GetFeedAsync("http://sethgodin.typepad.com/seths_blog/atom.xml");
            Task<FeedData> feed3 =
                GetFeedAsync("http://feeds.feedburner.com/OnlineMarketingSEOBlog");
            Task<FeedData> feed4 =
                GetFeedAsync("http://feeds.feedburner.com/m4bmarketingblog/xIjV");
            Task<FeedData> feed5 =
                GetFeedAsync("http://feeds.feedburner.com/MarketingProfsDailyFix");
            Task<FeedData> feed6 =
                GetFeedAsync("http://blog.hubspot.com/DesktopModules/Ingeni-NewsArticles/Rss.aspx?TabID=6307&ModuleID=7298&MaxCount=10");
            Task<FeedData> feed7 =
                GetFeedAsync("http://feeds2.feedburner.com/SmallBusinessTrends");
            Task<FeedData> feed8 =
                GetFeedAsync("http://feeds.feedburner.com/ProbloggerHelpingBloggersEarnMoney");
            // There is no Atom feed for this blog, so use the RSS feed.
            Task<FeedData> feed9 =
                GetFeedAsync("http://feeds.feedburner.com/SocialMediaMarketing");
            Task<FeedData> feed10 =
                GetFeedAsync("http://feeds.feedburner.com/ChiefMarketingTechnologist");
            Task<FeedData> feed11 =
                GetFeedAsync("http://feeds.feedburner.com/chrisbrogandotcom");
            Task<FeedData> feed12 =
                GetFeedAsync("http://feeds.feedburner.com/hmnib");

            Task<FeedData> feed13 =
                GetFeedAsync("http://econsultancy.com/us/blog.atom");
            Task<FeedData> feed14 =
                GetFeedAsync("http://feeds.copyblogger.com/copyblogger");


            Task<FeedData> feed15 =
                GetFeedAsync("http://www.findandconvert.com/blog/feed/rss/");
            Task<FeedData> feed16 =
                GetFeedAsync("http://www.businessweek.com/smallbiz/running_small_business/newentrepreneur.rss");
            Task<FeedData> feed17 =
                GetFeedAsync("http://feeds.feedburner.com/Webbiquity");
            Task<FeedData> feed18 =
                GetFeedAsync("http://feeds.feedburner.com/BlueFocusMarketing");
            Task<FeedData> feed19 =
                GetFeedAsync("http://feeds.marketingpilgrim.com/marketing-pilgrim");
            Task<FeedData> feed20 =
                GetFeedAsync("http://feeds.feedblitz.com/dannybrown");
            Task<FeedData> feed21 =
                GetFeedAsync("http://feeds.feedburner.com/ducttapemarketing/VWSb");
            Task<FeedData> feed22 =
                GetFeedAsync("http://feeds.feedburner.com/ReveNewsOnlineRevenueBlogs");
            //Task<FeedData> feed23 =
            //    GetFeedAsync("http://feeds.feedburner.com/seomoz");
            Task<FeedData> feed23 =
                GetFeedAsync("http://feeds.adweek.com/adweek/adfreak");
        
            Task<FeedData> feed24 =
                GetFeedAsync("http://feeds2.feedburner.com/ConversationAgent");


            //Task<FeedData> feed1 =
            //    GetFeedAsync("http://windowsteamblog.com/windows/b/developers/atom.aspx");
            //Task<FeedData> feed2 =
            //    GetFeedAsync("http://windowsteamblog.com/windows/b/windowsexperience/atom.aspx");
            //Task<FeedData> feed3 =
            //    GetFeedAsync("http://windowsteamblog.com/windows/b/extremewindows/atom.aspx");
            //Task<FeedData> feed4 =
            //    GetFeedAsync("http://windowsteamblog.com/windows/b/business/atom.aspx");
            //Task<FeedData> feed5 =
            //    GetFeedAsync("http://windowsteamblog.com/windows/b/bloggingwindows/atom.aspx");
            //Task<FeedData> feed6 =
            //    GetFeedAsync("http://windowsteamblog.com/windows/b/windowssecurity/atom.aspx");
            //Task<FeedData> feed7 =
            //    GetFeedAsync("http://windowsteamblog.com/windows/b/springboard/atom.aspx");
            //Task<FeedData> feed8 =
            //    GetFeedAsync("http://windowsteamblog.com/windows/b/windowshomeserver/atom.aspx");
            //// There is no Atom feed for this blog, so use the RSS feed.
            //Task<FeedData> feed9 =
            //    GetFeedAsync("http://windowsteamblog.com/windows_live/b/windowslive/rss.aspx");
            //Task<FeedData> feed10 =
            //    GetFeedAsync("http://windowsteamblog.com/windows_live/b/developer/atom.aspx");
            //Task<FeedData> feed11 =
            //    GetFeedAsync("http://windowsteamblog.com/ie/b/ie/atom.aspx");
            //Task<FeedData> feed12 =
            //    GetFeedAsync("http://windowsteamblog.com/windows_phone/b/wpdev/atom.aspx");
            //Task<FeedData> feed13 =
            //    GetFeedAsync("http://windowsteamblog.com/windows_phone/b/wmdev/atom.aspx");
            //Task<FeedData> feed14 =
            //    GetFeedAsync("http://windowsteamblog.com/windows_phone/b/windowsphone/atom.aspx");

            this.Feeds.Add(await feed1);
            this.Feeds.Add(await feed2);
            this.Feeds.Add(await feed3);
            this.Feeds.Add(await feed4);
            this.Feeds.Add(await feed5);
            this.Feeds.Add(await feed6);
            this.Feeds.Add(await feed7);
            this.Feeds.Add(await feed8);
            this.Feeds.Add(await feed9);
            this.Feeds.Add(await feed10);
            this.Feeds.Add(await feed11);
            this.Feeds.Add(await feed12);
            this.Feeds.Add(await feed13);
            this.Feeds.Add(await feed14);
            this.Feeds.Add(await feed15);
            this.Feeds.Add(await feed16);
            this.Feeds.Add(await feed17);
            this.Feeds.Add(await feed18);
            this.Feeds.Add(await feed19);
            this.Feeds.Add(await feed20);
            this.Feeds.Add(await feed21);
            this.Feeds.Add(await feed22);
            this.Feeds.Add(await feed23);
            this.Feeds.Add(await feed24);
        }

        private async Task<FeedData> GetFeedAsync(string feedUriString)
        {
            Windows.Web.Syndication.SyndicationClient client = new SyndicationClient();
            Uri feedUri = new Uri(feedUriString);

            try
            {
                SyndicationFeed feed = await client.RetrieveFeedAsync(feedUri);

                // This code is executed after RetrieveFeedAsync returns the SyndicationFeed.
                // Process the feed and copy the data you want into the FeedData and FeedItem classes.
                FeedData feedData = new FeedData();

                if (feed.Title != null && feed.Title.Text != null)
                {
                    if (feed.Title.Text.IndexOf("Search Engine Land") != 0)
                    {
                        feedData.Title = feed.Title.Text;
                    }
                    else
                    {
                        feedData.Title = "Search Engine Land";
                    }

                }
                if (feed.Subtitle != null && feed.Subtitle.Text != null)
                {
                    if (feed.Subtitle.Text.IndexOf("Danny Brown") != 0)
                    {
                        if (feed.Subtitle.Text.Length > 75)
                        {
                            feedData.Description = feed.Subtitle.Text.Substring(0, 75) + "...";
                        }
                        else
                        {
                            feedData.Description = feed.Subtitle.Text;
                        }
                    }
                }
                if (feed.Items != null && feed.Items.Count > 0)
                {
                    // Use the date of the latest post as the last updated date.
                    feedData.PubDate = feed.Items[0].PublishedDate.DateTime;

                    foreach (SyndicationItem item in feed.Items)
                    {
                        FeedItem feedItem = new FeedItem();
                        if (item.Title != null && item.Title.Text != null)
                        {
                            feedItem.Title = item.Title.Text;
                        }
                        if (item.PublishedDate != null)
                        {
                            feedItem.PubDate = item.PublishedDate.DateTime;
                        }
                        if (item.Authors != null && item.Authors.Count > 0)
                        {
                            feedItem.Author = item.Authors[0].Name.ToString();
                        }
                        //else
                        //{
                        //    feedItem.Author = "Test";
                        //}
                        // Handle the differences between RSS and Atom feeds.
                        if (feed.SourceFormat == SyndicationFormat.Atom10)
                        {
                            if (item.Content != null && item.Content.Text != null)
                            {
                                feedItem.Content = item.Content.Text;
                            }
                            if (item.Links != null)
                            {
                                feedItem.Link = new Uri(item.Links[0].Uri.AbsoluteUri.ToString());
                            }
                            //if (item.Id != null)
                            //{
                            //    feedItem.Link = new Uri("http://windowsteamblog.com" + item.Id);
                            //}
                        }
                        else if (feed.SourceFormat == SyndicationFormat.Rss20)
                        {
                            if (item.Summary != null && item.Summary.Text != null)
                            {
                                feedItem.Content = item.Summary.Text;
                            }
                            if (item.Links != null && item.Links.Count > 0)
                            {
                                feedItem.Link = item.Links[0].Uri;
                            }
                        }
                        feedData.Items.Add(feedItem);
                    }
                }
                return feedData;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Returns the feed that has the specified title.
        public static FeedData GetFeed(string title)
        {
            // Simple linear search is acceptable for small data sets
            var _feedDataSource = App.Current.Resources["feedDataSource"] as FeedDataSource;
            try
            {
                var matches = _feedDataSource.Feeds.Where((feed) => feed.Title.Equals(title));
                if (matches.Count() == 1) return matches.First();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        // Returns the post that has the specified title.
        public static FeedItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var _feedDataSource = sessionData.currentFeeds;// App.Current.Resources["feedDataSource"] as FeedDataSource;
            //var _feeds = _feedDataSource.Feeds;

            var matches = _feedDataSource.SelectMany(group => group.Items).Where((item) => item.Title.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }
    }
}