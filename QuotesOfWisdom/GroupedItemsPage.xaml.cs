using QuotesOfWisdom.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Storage;
using QuotesOfWisdom.Common;
using SQLite;
using System.Xml.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Core;

//using System.IO;
//using Windows.Foundation.Collections;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;
//using QuotesOfWisdom.NotificationsExtensions.TileContent;
//using Windows.Graphics.Imaging;
//using Windows.Storage.FileProperties;
//using Windows.Storage.Pickers;
//using Windows.UI.Xaml.Media.Imaging;


namespace QuotesOfWisdom
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : QuotesOfWisdom.Common.LayoutAwarePage
    {
        #region Objects
        public DataTransferManager datatransferManager;
        Random randNum = new Random();
        List<Quotations> list = new List<Quotations>();
        public DispatcherTimer dtrandomQuoteDisplay = new DispatcherTimer();
        private Syncfusion.UI.Xaml.Controls.Notification.HubTileTransitionCollection collection = new Syncfusion.UI.Xaml.Controls.Notification.HubTileTransitionCollection();

        private Syncfusion.UI.Xaml.Controls.SlideTransition slidetransition = new Syncfusion.UI.Xaml.Controls.SlideTransition();

        private Syncfusion.UI.Xaml.Controls.RotateTransition rotatetransition = new Syncfusion.UI.Xaml.Controls.RotateTransition();

        private Syncfusion.UI.Xaml.Controls.FadeTransition fadetransition = new Syncfusion.UI.Xaml.Controls.FadeTransition();
        #endregion

        public GroupedItemsPage()
        {
            this.InitializeComponent();
            ShareSourceLoad();            
            Window.Current.SizeChanged += Current_SizeChanged;            
        }

        /// <summary>
        /// Window Size Change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            // Calls the ChangeWindowState method
            ChangeWindowState();            
        }

        /// <summary>
        /// Change Window State method
        /// </summary>
        private void ChangeWindowState()
        {
            string visualState = DetermineVisualState(ApplicationView.Value);

            if (visualState == "Snapped")
            {
                txtOthers.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                favhubtile.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                hubtile.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                itemGridView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                itemListView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                backButton.Style = App.Current.Resources["SnappedBackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["SnappedPageHeaderTextStyle"] as Style;
                //btnFavoriteQuotes.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //btnRandomQuotes.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                txtOthers.Visibility = Windows.UI.Xaml.Visibility.Visible;
                favhubtile.Visibility = Windows.UI.Xaml.Visibility.Visible;
                hubtile.Visibility = Windows.UI.Xaml.Visibility.Visible;
                itemGridView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                itemListView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                backButton.Style = App.Current.Resources["BackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["PageHeaderTextStyle"] as Style;
                //btnFavoriteQuotes.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //btnRandomQuotes.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }        

        /// <summary>
        /// Method for Declaring and Initializing datatransfer object
        /// </summary>
        public void ShareSourceLoad()
        {
            datatransferManager = DataTransferManager.GetForCurrentView();
            datatransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }

        /// <summary>
        /// Async method for data sharing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            e.Request.Data.Properties.Title = "Quotes of Wisdom on Windows 8";
            e.Request.Data.Properties.Description = "Quotes of Wisdom ";
            e.Request.Data.SetText("Great Quotations app on Windows 8 - check out Quotes of Wisdom");
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            sessionData.isSearch = false;
            
            var quotesGroups = Quotes.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = quotesGroups;

            #region Tile Notification

            try
            {
                Uri polledUri = new Uri("http://apps.daksatech.com/quoteService/QuotesTileXML.html");
                PeriodicUpdateRecurrence recurrence = PeriodicUpdateRecurrence.Daily;
                TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdate(polledUri, recurrence);
            }
            catch
            {
                
            }
            datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            #endregion

            #region Load Ramdom Quotes
            LoadRandomQuotes();
            #endregion

            #region load Favorite Quotes
            LoadFavQuotes();
            #endregion            

            dtrandomQuoteDisplay.Interval = TimeSpan.FromSeconds(24.0);
            dtrandomQuoteDisplay.Tick += dtrandomQuoteDisplay_Tick;
            dtrandomQuoteDisplay.Start();
            
        }

        /// <summary>
        /// Method for checking whether storage file exists or not
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task<StorageFile> GetFileIfExistsAsync(StorageFolder folder, string fileName)
        {
            try
            {
                return await folder.GetFileAsync(fileName);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Favorite Quotes loading method
        /// </summary>
        private async void LoadFavQuotes()
        {
            List<Quotations> listFav = new List<Quotations>();

            StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
            StorageFile sampleFile; 

            sampleFile = await GetFileIfExistsAsync(roamingFolder, "favQuotes.xml");

            if (sampleFile != null)
            {
                try
                {
                    sampleFile = await roamingFolder.GetFileAsync("favQuotes.xml");
                    string fileContent = await FileIO.ReadTextAsync(sampleFile);
                    fileContent = fileContent.TrimStart('\0');
                    fileContent = fileContent.TrimEnd(' ');
                    XElement xe = XElement.Parse(fileContent.Trim());

                    var query = (from quotes in xe.Elements("q")
                                 select new Quotations
                                 {
                                     Quote = quotes.Element("d").Value,
                                     QuoteId = quotes.Attribute("id").Value,
                                     Author = quotes.Element("a").Value,
                                     Cat = quotes.Element("c").Value
                                 });



                    foreach (Quotations q in query)
                    {
                        listFav.Add(q);
                    }
                    sessionData.currentFavoriteQuotes = listFav.ToList();

                    if (listFav.Count == 0)
                    {
                        favoritetitle.Text = "No Favorites Yet";
                        favoriteseconarytitle.Text = "No Favorites Yet";
                    }
                    else
                    {
                        if (listFav.Count == 1)
                        {
                            favoritetitle.Text = Utilities.GetTeaser(listFav[0].Quote.ToString(), 200);
                            favoriteseconarytitle.Text = Utilities.GetTeaser(listFav[0].Quote.ToString(), 200);
                        }
                        else
                        {
                            favoritetitle.Text = Utilities.GetTeaser(listFav[randNum.Next(1, listFav.Count)].Quote.ToString(), 200);
                            favoriteseconarytitle.Text = Utilities.GetTeaser(listFav[randNum.Next(2, listFav.Count)].Quote.ToString(), 200);
                        }
                    }
                }
                catch
                {

                }
            }
            else
            {
                favoritetitle.Text = "No Favorites Yet";
                favoriteseconarytitle.Text = "No Favorites Yet";
            }
        }

        /// <summary>
        /// Random Quotes loading method
        /// </summary>
        private void LoadRandomQuotes()
        {
            try
            {
                string rand1 = "";
                string rand2 = "";
                using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
                {
                    rand1 = randNum.Next(1, 54000).ToString();
                    rand2 = randNum.Next(1, 54000).ToString();
                    var query = from q in db.Table<Quotations>()
                                where q.QuoteId == rand1 || q.QuoteId == rand2
                                select q;

                    list = query.ToList();
                }

                title.Text = Utilities.GetTeaser(list[0].Quote.ToString(), 200);
                secondarytitle.Text = Utilities.GetTeaser(list[1].Quote.ToString(), 200);
            }
            catch
            { }
        }

        /// <summary>
        /// Timer tick for random quotes display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtrandomQuoteDisplay_Tick(object sender, object e)
        {
            LoadRandomQuotes();
            LoadFavQuotes();
        }

        /// <summary>
        /// Loaded event of Random hub tile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hubtile_Loaded(object sender, RoutedEventArgs e)
        {
            hubtile.Interval = TimeSpan.FromSeconds(Double.Parse("3"));
            collection.Add(slidetransition);
            collection.Add(rotatetransition);
            collection.Add(fadetransition);
            hubtile.HubTileTransitions = collection;
        }

        /// <summary>
        /// Loaded event of Favorite hub tile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void favhubtile_Loaded(object sender, RoutedEventArgs e)
        {
            favhubtile.Interval = TimeSpan.FromSeconds(Double.Parse("3"));
            collection.Add(slidetransition);
            collection.Add(rotatetransition);
            collection.Add(fadetransition);
            favhubtile.HubTileTransitions = collection;
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            dtrandomQuoteDisplay.Stop();

            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(GroupDetailPage), ((QuotesGroup)group).Title);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            int i = itemGridView.SelectedIndex;
            datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            dtrandomQuoteDisplay.Stop();
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((QuotesItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }

        /// <summary>
        /// LayoutUpdted Event of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_LayoutUpdated(object sender, object e)
        {
            // Calls the Background change method
            ChangeBackground();
        }
        /// <summary>
        /// Method of changing background
        /// </summary>
        private void ChangeBackground()
        {
            ChangeWindowState();

            if (ApplicationData.Current.RoamingSettings.Values["Settings"] != null)
            {
                // Initialize the Radio button from roaming settings
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
                {
                    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                }
                else
                {
                    LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
                }

                #region Custom Image Selection
                /*
                if (ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() != "CustomImage")
                {
                    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                }
                else
                {
                    StorageFile file;

                    file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(ApplicationData.Current.RoamingSettings.Values["fileToken"].ToString());

                    ImageBrush ib = new ImageBrush();

                    BitmapImage src = new BitmapImage();
                    src.SetSource(await file.OpenAsync(FileAccessMode.Read));


                    ib.ImageSource = src;

                    LayoutRoot.Background = ib;

                }
                */
                #endregion
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
            }
        }

        /// <summary>
        /// GotFocus event of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageRoot_GotFocus_1(object sender, RoutedEventArgs e)
        {
            ChangeBackground();
        }

        /// <summary>
        /// Tapped event of Random Quotes Hub Tile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hubtile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            dtrandomQuoteDisplay.Stop();
            this.Frame.Navigate(typeof(RandomQuotes));
        }

        /// <summary>
        /// Tapped event of Favorite Quotes Hub Tile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void favhubtile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sessionData.currentFavoriteQuotes != null)
            {
                dtrandomQuoteDisplay.Stop();
                this.Frame.Navigate(typeof(FavoriteQuotes));
            }
            else
            {
                this.ShowMessage("You dont have any favorite quotes as yet.");
            }
        }

        async void ShowMessage(string msg)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }

        /// <summary>
        /// Click event of random quotes button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRandomQuotes_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RandomQuotes));
        }

        /// <summary>
        /// Click event of favorites quotes button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnFavoriteQuotes_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FavoriteQuotes));
        }

        /// <summary>
        /// Click event of all Cats app bar button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAllCats_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AllCategories));
        }

        /// <summary>
        /// Click event of all Auts app bar button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAllAuthors_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AllAuthors));
        }
    }
}
