using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Xml.Linq;
using Windows.Data.Xml;

using System.Collections.ObjectModel;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using System.Threading.Tasks;
//using Microsoft.Phone.Tasks;
using System.Net.Http;
using Windows.Storage.Pickers;
//using Hammock.Web;
//using Hammock.Authentication.OAuth;
//using Hammock;
//using XAMLMetroAppIsolatedStorageHelper;
using Windows.UI.Notifications;
using LoveQuotes.NotificationsExtensions.TileContent;
using Windows.ApplicationModel.DataTransfer;
using LoveQuotes.Common;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.UI.Popups;
namespace LoveQuotes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage : Page
    {
        #region Objects
        string OAuthTokenKey = string.Empty;
        string tokenSecret = string.Empty;
        bool isExists = false;
        string accessToken = string.Empty;
        string accessTokenSecret = string.Empty;

        string userID = string.Empty;
        string userScreenName = string.Empty;

        StorageFile sampleFile = null;
        //StorageFile twitterFile = null;
        //string twitterContent = "";

        string fileContent = "";
        // Windows.Data.Xml.Dom.XmlDocument doc;
        StorageFolder roamingFolder = null;
        //List<Quotation> listFavQuotation;// = new List<Quotation>();
        //XAMLMetroAppIsolatedStorageHelper.StorageHelper<Quotation> objectStorageHelper;
        // private XmlSerializer serializer;
        public DataTransferManager datatransferManager;
        public DispatcherTimer dtautoPlay = new DispatcherTimer();
        int interval = 0;
        bool isPlay = true;
        //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
        //LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
        LicenseChangedEventHandler licenseChangeHandler = null;

        public const int DAYS_UNTIL_PROMPT = 0;
        public const int LAUNCHES_UNTIL_PROMPT = 2;
        double launch_count;
        #endregion

        public BlankPage()
        {
            this.InitializeComponent();

            //this.Initialize();            
            roamingFolder = ApplicationData.Current.RoamingFolder;
            ApplicationData.Current.RoamingSettings.Values["date_firstLaunch"] = null;

            ShareSourceLoad();
            
            Window.Current.SizeChanged += Current_SizeChanged;

            if (ApplicationData.Current.RoamingSettings.Values["launch_count"] != null)
            {
                launch_count = Convert.ToDouble(ApplicationData.Current.RoamingSettings.Values["launch_count"].ToString()) + 1;
            }
            else
            {
                launch_count = 1;
                ApplicationData.Current.RoamingSettings.Values["launch_count"] = launch_count;
            }

            if (ApplicationData.Current.RoamingSettings.Values["date_firstLaunch"] == null)
            {
                ApplicationData.Current.RoamingSettings.Values["date_firstLaunch"] = CurrentTimeMillis().ToString();
            }




        }


        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        private void myPopup_Loaded(object sender, RoutedEventArgs e)
        {
            PopupLoaded();
        }

        private void PopupLoaded()
        {
            //position popup in center of the window
            myPopup.HorizontalOffset = (Window.Current.Bounds.Width - popupStack.Width) / 2;
            myPopup.VerticalOffset = (Window.Current.Bounds.Height - popupStack.Height) / 2;
        }

        private async void btnRate_Click(object sender, RoutedEventArgs e)
        {
            // since user is rating, we dont want to bug him again.
            ApplicationData.Current.RoamingSettings.Values["dontshowagain"] = "dontshowagain";
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:PDP?PFN=DaksaTech.1000LoveQuotes_c7fyd19frge5m"));
            myPopup.IsOpen = false;
        }

        private void btnremind_Click(object sender, RoutedEventArgs e)
        {
            myPopup.IsOpen = false;
        }

        private void btnnothanks_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["dontshowagain"] = "dontshowagain";
            myPopup.IsOpen = false;
        }
        void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            string visualState = DetermineVisualState(ApplicationView.Value);
            //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["AdFreeVersion"];

            if (visualState == "Snapped")
            {
                adFull.Visibility = Visibility.Collapsed;

                if (productLicense.IsActive)
                {
                    adSnapped.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    adSnapped.Visibility = Visibility.Visible;
                }
                listQuotes.Visibility = Visibility.Collapsed;
                listQuotesSnapped.Visibility = Visibility.Visible;
                listQuotesSnapped.SelectedIndex = listQuotes.SelectedIndex;
                //if (sessionQuotes.currentQuotes != null)
                //{

                //    try
                //    {
                //        listQuotes.SelectedItem = sessionQuotes.currentQuotes[listQuotes.SelectedIndex];


                //        FlipViewItem p = listQuotes.ItemContainerGenerator.ContainerFromItem(listQuotes.SelectedItem) as FlipViewItem;

                //        if (p != null)
                //        {
                //            StackPanel s = FindVisualChild<StackPanel>(p) as StackPanel;

                //            StackPanel quoteSP = (StackPanel)s.FindName("QuotesListPanel");
                //            TextBlock quoteTB = (TextBlock)quoteSP.FindName("txtQuote");
                //            StackPanel authorSP = (StackPanel)s.FindName("authorListPanel");
                //            TextBlock authorTB = (TextBlock)authorSP.FindName("textQuoteAuthor");


                //            quoteTB.Style = App.Current.Resources["Style20"] as Style;
                //            authorTB.Style = App.Current.Resources["Style20"] as Style;


                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                //        dialog.ShowAsync();
                //    }
                //}
            }
            else
            {
                if (productLicense.IsActive)
                {
                    adFull.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    adFull.Visibility = Visibility.Visible;
                }

                adSnapped.Visibility = Visibility.Collapsed;
                listQuotes.Visibility = Visibility.Visible;
                listQuotesSnapped.Visibility = Visibility.Collapsed;
                listQuotes.SelectedIndex = listQuotesSnapped.SelectedIndex;
            }
        }

        protected string DetermineVisualState(ApplicationViewState viewState)
        {
            if (viewState == ApplicationViewState.Filled || viewState == ApplicationViewState.FullScreenLandscape)
            {
                // Allow pages to request that the Filled state be used only for landscape layouts narrower
                // than 1366 virtual pixels
                var windowWidth = Window.Current.Bounds.Width;
                viewState = windowWidth >= 1366 ? ApplicationViewState.FullScreenLandscape : ApplicationViewState.Filled;
            }
            return viewState.ToString();
        }

        //void listQuotes_LayoutUpdated(object sender, object e)
        //{
        //    TextBlock tb = (TextBlock)listQuotes.FindName("txtQuote");
        //    //listQuotes.SelectedIndex = sessionQuotes.currentQuoteIndex;
        //    //FlipViewItem p = listQuotes.ItemContainerGenerator.ContainerFromItem(listQuotes.SelectedItem) as FlipViewItem;
        //    //TextBlock tes = FindVisualChild<TextBlock>(p) as TextBlock;

        //    string cc = tb.Text;
        //}

        private void dtautoPlay_Tick(object sender, object e)
        {
            listQuotes.SelectedIndex += 1;
            sessionQuotes.currentQuoteIndex++;
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
        void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            btnAutoPlay.Style = App.Current.Resources["AutoPlayAppBarButtonStyle"] as Style;
            dtautoPlay.Stop();
            isPlay = true;

            Quotation q = sessionQuotes.currentQuotes[listQuotes.SelectedIndex];
            if (q.Quote.Length <= 25)
                e.Request.Data.Properties.Title = "A quote on love: " + q.Quote;
            else
                e.Request.Data.Properties.Title = "A quote on love: " + q.Quote.Substring(0, 25) + "....";

            e.Request.Data.Properties.Description = q.Quote + " - " + q.Author;
            e.Request.Data.SetText(q.Quote + " - " + q.Author);
        }

        /// <summary>
        /// Checks if 'sample.txt' already exists, if it does assign it to sampleFile
        /// </summary>
        async void Initialize()
        {
            try
            {

                //doc = await ScenarioInit("Xml", "QuotesLove.xml");
                //doc.GetXml();
                // string ss = Windows.Storage.KnownFolders.HomeGroup;

                Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");

                sampleFile = await storageFolder.GetFileAsync("QuotesLove.xml");

                //sampleFile = await Windows.Storage.KnownFolders.DocumentsLibrary.GetFileAsync("QuotesLove.xml");

                fileContent = await FileIO.ReadTextAsync(sampleFile);

                //StorageFolder myStore = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Favorites");

                //if (myStore == null)
                //{
                //    myStore = await Windows.ApplicationModel.Package.Current.InstalledLocation.CreateFolderAsync("Favorites");
                //}
                //StorageFile file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("FavQuote.xml");

                //if (file == null)
                //{
                //    file = await Windows.ApplicationModel.Package.Current.InstalledLocation.CreateFileAsync("FavQuote.xml");
                //}

                //Windows.Data.Xml.Dom.XmlLoadSettings loadSettings = new Windows.Data.Xml.Dom.XmlLoadSettings();
                //loadSettings.ProhibitDtd = false;
                //loadSettings.ResolveExternals = false;
                //loadSettings.ValidateOnParse = false;
                //doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file, loadSettings);

                //XElement xe = XElement.Parse(doc.GetXml());  
                //if (file == null)
                //{
                //    StorageFile file = await myStore.CreateFileAsync("FavQuote.xml", CreationCollisionOption.ReplaceExisting);
                //}

                //var stream = await file.OpenAsync(FileAccessMode.Read);

                //Stream s = await file.OpenStreamForWriteAsync();
                //XDocument xdoc = new XDocument(new XElement("root"));
                //xdoc.Save((Stream)stream);

                //Windows.Storage.Streams.IInputStream stream = await FileIO.ReadBufferAsync("FavQuotes.xml");
                //XDocument xdoc = new XDocument(new XElement("root"));
                //xdoc.Save(stream);

                //Windows.Storage.FileProperties. 
            }
            catch (FileNotFoundException)
            {
                // 'sample.txt' doesn't exist so scenario one must be run
            }
        }

        //private async Task<Windows.Data.Xml.Dom.XmlDocument> ScenarioInit(String folder, String file)
        //{
        //    Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(folder);
        //    Windows.Storage.StorageFile storageFile = await storageFolder.GetFileAsync(file);
        //    Windows.Data.Xml.Dom.XmlLoadSettings loadSettings = new Windows.Data.Xml.Dom.XmlLoadSettings();
        //    loadSettings.ProhibitDtd = false;
        //    loadSettings.ResolveExternals = false;
        //    return await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(storageFile, loadSettings);
        //}

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (launch_count >= LAUNCHES_UNTIL_PROMPT && Convert.ToInt32(ApplicationData.Current.RoamingSettings.Values["date_firstLaunch"].ToString()) + DAYS_UNTIL_PROMPT > System.DateTime.Now.Millisecond)
            //{
            if ((ApplicationData.Current.RoamingSettings.Values["dontshowagain"] == null) || (ApplicationData.Current.RoamingSettings.Values["dontshowagain"] != null & ApplicationData.Current.RoamingSettings.Values["dontshowagain"] != "dontshowagain"))
                {
                    //if ((string)ApplicationData.Current.RoamingSettings.Values["dontshowagain"].ToString() != "dontshowagain")
                    //{
                    if (launch_count >= LAUNCHES_UNTIL_PROMPT && long.Parse(ApplicationData.Current.RoamingSettings.Values["date_firstLaunch"].ToString()) + DAYS_UNTIL_PROMPT * 24 * 60 * 60 * 1000 < long.Parse(CurrentTimeMillis().ToString()))
                        {
                            myPopup.IsOpen = true;
                        }
                    //}
                }
                //else
                //{
                //    myPopup.IsOpen = true;
                //}
            //}

            await LoadInAppPurchaseProxyFileAsync();

            //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;

            var productLicense = licenseInformation.ProductLicenses["AdFreeVersion"];
            if (productLicense.IsActive)
            {
                //rootPage.NotifyUser("You can use Product 1.", NotifyType.StatusMessage);
                adFull.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                adSnapped.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                adFull.Visibility = Windows.UI.Xaml.Visibility.Visible;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //rootPage.NotifyUser("You don't own Product 1. You must buy Product 1 before you can use it.", NotifyType.ErrorMessage);
            }
            //if (ApplicationData.Current.RoamingSettings.Values["FontSize"] != null)
            //{
            //    fontSize.Text = ApplicationData.Current.RoamingSettings.Values["FontSize"].ToString();
            //}
            //else
            //{
            //    fontSize.Text = "36";
            //}

            //if (ApplicationData.Current.RoamingSettings.Values["FontColor"] != null)
            //{
            //    fontColor.Text = ApplicationData.Current.RoamingSettings.Values["FontColor"].ToString();
            //}
            //else
            //{
            //    fontColor.Text = "Red";
            //}

            loadQuotes();


            //loadNewQuote();

            #region Tile Notification

            try
            {
                //Uri polledUri = new Uri("http://apps.daksatech.com/quoteService/RetrieveTileXML.aspx?source=lovequotes");
                Uri polledUri = new Uri("http://apps.daksatech.com/quoteService/LoveQuotesTileXML.html");
                PeriodicUpdateRecurrence recurrence = PeriodicUpdateRecurrence.Daily;
                TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdate(polledUri, recurrence);
            }
            catch
            {
                //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                //dialog.ShowAsync();
            }

            #endregion
        }

        private async Task LoadInAppPurchaseProxyFileAsync()
        {
            //StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("Xml");
            //StorageFile proxyFile = await proxyDataFolder.GetFileAsync("in-app-purchase.xml");
            licenseChangeHandler = new LicenseChangedEventHandler(InAppPurchaseRefreshScenario);

            //CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
            CurrentApp.LicenseInformation.LicenseChanged += licenseChangeHandler;
            //await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);

            // setup application upsell message
            //ListingInformation listing = await CurrentAppSimulator.LoadListingInformationAsync();            
        }

        private void InAppPurchaseRefreshScenario()
        {
            //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["AdFreeVersion"];
            if (productLicense.IsActive)
            {
                //rootPage.NotifyUser("You can use Product 1.", NotifyType.StatusMessage);
                adFull.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                adSnapped.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                string visualState = DetermineVisualState(ApplicationView.Value);
                if (visualState == "Snapped")
                {
                    adSnapped.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    adFull.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    adFull.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    adSnapped.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //rootPage.NotifyUser("You don't own Product 1. You must buy Product 1 before you can use it.", NotifyType.ErrorMessage);
            }
        }



        /// <summary>
        /// Invoked when this page is about to unload
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (licenseChangeHandler != null)
            {
                //CurrentAppSimulator.LicenseInformation.LicenseChanged -= licenseChangeHandler;
                CurrentApp.LicenseInformation.LicenseChanged -= licenseChangeHandler;
            }
            base.OnNavigatingFrom(e);
        }

        private async void loadQuotes()
        {
            try
            {
                int randomQuotesToLoad = 1393;
                List<Quotation> list = new List<Quotation>();
                
                Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");

                sampleFile = await storageFolder.GetFileAsync("QuotesLove.xml");

                fileContent = await FileIO.ReadTextAsync(sampleFile);

                XElement xe = XElement.Parse(fileContent);

                var query = (from quotes in xe.Elements("q")
                             orderby System.Guid.NewGuid()
                             select new Quotation
                             {
                                 Quote = quotes.Element("d").Value,
                                 QuoteId = quotes.Attribute("id").Value,
                                 Author = quotes.Element("a").Value
                             }).Take(randomQuotesToLoad);



                foreach (Quotation q in query)
                {
                    list.Add(q);
                }


                sessionQuotes.currentQuotes = list;

                try
                {

                    //binding
                    listQuotes.ItemsSource = sessionQuotes.currentQuotes.ToList();
                    listQuotes.SelectedIndex = sessionQuotes.currentQuoteIndex;

                    listQuotesSnapped.ItemsSource = sessionQuotes.currentQuotes.ToList();
                    listQuotesSnapped.SelectedIndex = sessionQuotes.currentQuoteIndex;
                }
                catch (Exception ex)
                {
                    // Initializing Messagedialog object and calling pop up window
                    this.ShowMessage(ex.Message.ToString());
                }

                #region Commented on 21.05.2012
                /*
                if (sessionQuotes.currentQuotes != null || sessionQuotes.currentQuotes.Count > 0)
                {
                    string Quote = "";
                    string quoteIndex;
                    string author = "";

                    // Get the current showing quote id
                    quoteIndex = sessionQuotes.currentQuoteIndex.ToString();

                    // get the quote and category based on the quoteId from currentQuotes sessionData
                    if (quoteIndex != null)
                    {
                        Quote = sessionQuotes.currentQuotes[Convert.ToInt32(quoteIndex)].Quote;
                        author = sessionQuotes.currentQuotes[Convert.ToInt32(quoteIndex)].Author;
                    }

                    //textQuote.Text = Quote;
                    //textQuoteAuthor.Text = "\n- " + author;

                    // checking for the length of showing quote for hiding or showing
                    // Twitter option
                    if (textQuote.Text.Length <= 140)
                    {
                        btnTwitter.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else
                    {
                        btnTwitter.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                }
                else
                {
                    this.ShowMessage("No quotes");
                }
                */
                #endregion

            }
            catch
            {
                //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                //dialog.ShowAsync();
            }
        }


        #region Commented on 21.05.2012
        /*
        /// <summary>
        /// Method of showing Quotes
        /// </summary>
        private void loadNewQuote()
        {
            if (sessionQuotes.currentQuotes != null || sessionQuotes.currentQuotes.Count > 0)
            {
                string Quote = "";
                string quoteIndex;
                string author = "";

                // Get the current showing quote id
                quoteIndex = sessionQuotes.currentQuoteIndex.ToString();

                // get the quote and category based on the quoteId from currentQuotes sessionData
                if (quoteIndex != null)
                {
                    Quote = sessionQuotes.currentQuotes[Convert.ToInt32(quoteIndex)].Quote;
                    author = sessionQuotes.currentQuotes[Convert.ToInt32(quoteIndex)].Author;
                }

                textQuote.Text = Quote;
                textQuoteAuthor.Text = "\n- " + author;

                // checking for the length of showing quote for hiding or showing
                // Twitter option
                if (textQuote.Text.Length <= 140)
                {
                    btnTwitter.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else
                {
                    btnTwitter.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
            else
            {
                this.ShowMessage("No quotes");
            }

        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            // checks the currentQuoteIndex of sessionData and decrements its value by 1
            // calls the loadNewQuote() method
            if (sessionQuotes.currentQuoteIndex > 0)
            {
                sessionQuotes.currentQuoteIndex -= 1;
                loadNewQuote();
            }// else displays notification message based on the currentQuoteIndex
            else
            {
                //RichEditBox.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                //RichEditBox.Document.SetText(Windows.UI.Text.TextSetOptions.None, "No previous quote");
                //RichEditBox.IsReadOnly = true;
                // Windows.UI.Popups.PopupMenu pop = new Windows.UI.Popups.PopupMenu("test");
                //await pop.ShowAsync();
                //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("You're a Superstar!");
                //await dialog.ShowAsync();
                this.ShowMessage("No previous quote");
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            // checks the currentQuoteIndex + 1 of sessionData is equal to currentQuotes of session Data 
            // and loads the next set of quotes from database and not equal to fav quote loaded
            if (sessionQuotes.currentQuoteIndex + 1 < sessionQuotes.currentQuotes.Count())
            {
                // increments the currentQuoteIndex by 1
                sessionQuotes.currentQuoteIndex += 1;

                // Calls the New Quote load method
                loadNewQuote();
            }
            else
            {
                this.ShowMessage("You have reached the last of the love quotes.");
            }
        }
        */
        #endregion

        async void ShowMessage(string msg)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }

        private async Task<StorageFile> GetFileIfExistsAsync(StorageFolder folder, string fileName)
        {
            try
            {
                isExists = true;
                return await folder.GetFileAsync(fileName);

            }
            catch
            {
                return null;
            }
        }

        async void btnFavorite_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // get the current quote from currentQuotes sessionQuotes variable based on
                // quoteindex
                Quotation q = sessionQuotes.currentQuotes[sessionQuotes.currentQuoteIndex];

                // adds the current quote to Favorites
                //Windows.Data.Xml.Dom.XmlDocument xmlDoc;



                #region Method 1

                //string returnValue = "Added";
                const string filename = "favQuotes.xml";
                roamingFolder = ApplicationData.Current.RoamingFolder;
                StorageFile file;
                //Stream outStream1 = null;
                // IRandomAccessStream readStream1 = null;

                file = await GetFileIfExistsAsync(roamingFolder, filename);

                if (file == null)
                {
                    isExists = false;
                    file = await roamingFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                    //readStream1 = await file.OpenAsync(FileAccessMode.ReadWrite);
                    //outStream1 = Task.Run(() => readStream1.AsStreamForWrite()).Result;    
                    //using (IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    //{
                    //    //outStream = Task.Run(() => readStream.AsStreamForWrite()).Result;
                    //    //Stream tempStream = Task.Run(() => readStream.AsStreamForWrite()).Result;
                    //    //XDocument xdoc = new XDocument(new XElement("root"));
                    //    //xdoc.Save(tempStream);
                    //}
                }



                using (IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Stream inStream = Task.Run(() => readStream.AsStreamForRead()).Result;
                    Stream outStream = Task.Run(() => readStream.AsStreamForWrite()).Result;

                    #region Commented
                    //XDocument doc;
                    ////doc = (XDocument)serializer.Deserialize(inStream);
                    //doc = XDocument.Load(inStream);
                    ////stream.Seek(0, SeekOrigin.Begin);

                    //// execute the query
                    //IEnumerable<XElement> qteExists =
                    //        from el in doc.Descendants("q")
                    //        where el.Attribute("id").Value == q.QuoteId
                    //        select el;

                    //// Checks the Quote already exists based on the quote id
                    //if (qteExists.Count() == 0)
                    //{
                    //    XElement qElement = new XElement("q",
                    //        new XAttribute("id", q.QuoteId),
                    //        new XElement("d", q.Quote),
                    //        new XElement("a", q.Author)
                    //                );

                    //    doc.Element("root").Add(qElement);
                    //    doc.Save(outStream);
                    //    returnValue = "Added";

                    //}
                    //else
                    //{
                    //    returnValue = "Exists";
                    //}
                    //}
                    #endregion


                    if (isExists == true)
                    {
                        XDocument doc;
                        doc = XDocument.Load(inStream);

                        //await file.DeleteAsync();
                        //file = await roamingFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                        //outStream1 = Task.Run(() => readStream1.AsStreamForWrite()).Result;

                        // execute the query
                        IEnumerable<XElement> qteExists =
                                from el in doc.Descendants("q")
                                where el.Attribute("id").Value == q.QuoteId
                                select el;

                        // Checks the Quote already exists based on the quote id
                        if (qteExists.Count() == 0)
                        {
                            XElement qElement = new XElement("q",
                                new XAttribute("id", q.QuoteId),
                                new XElement("d", q.Quote),
                                new XElement("a", q.Author)
                                        );

                            doc.Element("root").Add(qElement);
                            //outStream.Seek(0, SeekOrigin.Begin);
                            doc.Save(outStream);
                        }
                    }
                    else
                    {
                        using (XmlWriter writer = XmlWriter.Create(outStream))
                        {
                            writer.WriteStartDocument();

                            writer.WriteStartElement("root");

                            writer.WriteStartElement("q");
                            writer.WriteAttributeString("id", q.QuoteId.ToString());
                            writer.WriteElementString("d", q.Quote);
                            writer.WriteElementString("a", q.Author);
                            writer.WriteEndElement();

                            writer.WriteEndElement();

                            writer.WriteEndDocument();
                        }
                    }

                }
                #endregion

                #region Method 2
                ////listFavQuotation.Add(new Quotation { QuoteId = q.QuoteId, Quote = q.Quote, Author = q.Author });
                //Quotation objQuotation = new Quotation { QuoteId = q.QuoteId, Quote = q.Quote, Author = q.Author };
                //var objectStorageHelper = new XAMLMetroAppIsolatedStorageHelper.ObjectStorageHelper<Quotation>(XAMLMetroAppIsolatedStorageHelper.StorageType.Roaming);
                //objectStorageHelper.SaveASync(objQuotation, "favQuotes");

                #endregion

                #region Method 3

                //const string filename = "favQuotes.xml";
                //roamingFolder = ApplicationData.Current.RoamingFolder;
                //StorageFile file;

                //file = await GetFileIfExistsAsync(roamingFolder, filename);

                //if (file == null)
                //{
                //    file = await roamingFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                //}

                //Windows.Data.Xml.Dom.XmlDocument xmlDoc;
                //using (var readStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                //{
                //    //Stream stream = readStream.GetInputStreamAt(0).AsStreamForRead();                        
                //    //stream.Seek(0, SeekOrigin.Begin);
                //    readStream.Seek(0);
                //    xmlDoc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file);

                //    Windows.Data.Xml.Dom.XmlElement root = xmlDoc.DocumentElement;

                //    Windows.Data.Xml.Dom.XmlElement title = xmlDoc.CreateElement("Title");
                //    title.InnerText = "title";
                //    Windows.Data.Xml.Dom.XmlElement desc = xmlDoc.CreateElement("Description");
                //    desc.InnerText = "description";

                //    root.AppendChild(title);
                //    root.AppendChild(desc);
                //}
                //if (xmlDoc != null)
                //    await xmlDoc.SaveToFileAsync(file);
                #endregion

            }
            catch (Exception ex)
            {
                this.ShowMessage(ex.Message.ToString());
            }

        }

        /// <summary>
        /// LayoutUpdted Event of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_LayoutUpdated(object sender, object e)
        {
            //listQuotes.LayoutUpdated += listQuotes_LayoutUpdated;
            // Calls the Background change method
            ChangeBackground();

            //listQuotes.SelectedIndex = listQuotes.SelectedIndex;
        }
        /// <summary>
        /// Method of changing background
        /// </summary>
        private void ChangeBackground()
        {
            if (sessionQuotes.currentQuotes != null)
            {

                try
                {
                    if (listQuotes.SelectedIndex != -1)
                    {
                        listQuotes.SelectedItem = sessionQuotes.currentQuotes[listQuotes.SelectedIndex];

                        if (listQuotes.SelectedItem != null)
                        {

                            //listQuotes.SelectedItem = sessionQuotes.currentQuotes[sessionQuotes.currentQuoteIndex];
                            FlipViewItem p = listQuotes.ItemContainerGenerator.ContainerFromItem(listQuotes.SelectedItem) as FlipViewItem;

                            if (p != null)
                            {
                                StackPanel s = FindVisualChild<StackPanel>(p) as StackPanel;

                                //StackPanel s = (StackPanel)listQuotes.FindName("mainStack");

                                StackPanel quoteSP = (StackPanel)s.FindName("QuotesListPanel");
                                TextBlock quoteTB = (TextBlock)quoteSP.FindName("txtQuote");
                                StackPanel authorSP = (StackPanel)s.FindName("authorListPanel");
                                TextBlock authorTB = (TextBlock)authorSP.FindName("textQuoteAuthor");

                                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("FontColor"))
                                {
                                    string colorvalue = (string)ApplicationData.Current.RoamingSettings.Values["FontColor"].ToString();
                                    //ComboBoxItem cbi = (ComboBoxItem)colorvalue;
                                    //SolidColorBrush sb = new SolidColorBrush();
                                    //sb = (SolidColorBrush)Utilities.HexColor(colorvalue);
                                    //quoteTB.Foreground = (SolidColorBrush)Utilities.HexColor(colorvalue);
                                    //new SolidColorBrush(Colors.Black);
                                    quoteTB.Foreground = new SolidColorBrush(Utilities.HexColor(colorvalue));// Utilities.HexColor(colorvalue);
                                    authorTB.Foreground = new SolidColorBrush(Utilities.HexColor(colorvalue));
                                }

                                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Font"))
                                {
                                    //int fontvalue = Convert.ToInt32(ApplicationData.Current.RoamingSettings.Values["FontSize"].ToString());
                                    //string ss = "";
                                    //try
                                    //{
                                    if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Font"))
                                    {
                                        //quoteTB.LineHeight = Convert.ToDouble(fontvalue);
                                        //quoteTB.FontSize = Convert.ToDouble(fontvalue);

                                        if ((string)ApplicationData.Current.RoamingSettings.Values["Font"].ToString() != "")
                                        {
                                            quoteTB.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Font"].ToString()] as Style;
                                            authorTB.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Font"].ToString()] as Style;

                                        }
                                        else
                                        {
                                            quoteTB.Style = App.Current.Resources["Style30"] as Style;
                                            authorTB.Style = App.Current.Resources["Style30"] as Style;
                                        }
                                    }
                                    else
                                    {
                                        quoteTB.Style = App.Current.Resources["Style30"] as Style;
                                    }
                                    //quoteTB.Style = "FontSize:" + fontvalue.ToString();
                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                                    //    dialog.ShowAsync();
                                    //}
                                }


                                //StackPanel tes1 = FindVisualChild<StackPanel>(s.FindName("QuotesListPanel")) as StackPanel;
                                //TextBlock tl = (TextBlock)tes1.FindName("txtQuote");
                                //string cc = tl.Text;
                                //StackPanel tes2 = FindVisualChild<StackPanel>(s.Children[1]) as StackPanel;
                                //TextBlock t2 = (TextBlock)tes2.FindName("textQuoteAuthor");
                                //string c = quoteTB.Text + " ---- " + authorTB.Text;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                    dialog.ShowAsync();
                }
            }

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Auto-play"))
            {
                interval = Convert.ToInt32(ApplicationData.Current.RoamingSettings.Values["Auto-play"].ToString());
            }
            else
            {
                interval = 30;
            }

            if (ApplicationData.Current.RoamingSettings.Values["Settings"] != null)
            {
                // Initialize the Radio button from roaming settings
                if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "Generic")
                {
                    SolidColorBrush s = new SolidColorBrush();
                    s.Color = Utilities.GetColor(sessionQuotes.colorValue);
                    LayoutRoot.Background = s;
                }
                else if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
                {
                    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                }
                else
                {
                    LayoutRoot.Style = App.Current.Resources["layoutBlockStyle_f2b100"] as Style;
                    //Style style = new Style(typeof(Grid));
                    //style.Setters.Add(new Setter(Grid.BackgroundProperty, ApplicationData.Current.RoamingSettings.Values["Settings"]));
                    //LayoutRoot.Style = style;
                }
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle_f2b100"] as Style;
            }
            //Style style = new Style(typeof(Grid));
            //style.Setters.Add(new Setter(Grid.BackgroundProperty, ApplicationData.Current.RoamingSettings.Values["Settings"]));
            //LayoutRoot.Style = style;           
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public void btnAutoPlay_Click(object sender, RoutedEventArgs e)
        {
            if (isPlay)
            {
                //btnAutoPlay.Content = "Stop";
                btnAutoPlay.Style = App.Current.Resources["StopAppBarButtonStyle"] as Style;
                isPlay = false;

                if (ApplicationData.Current.RoamingSettings.Values["Auto-play"] != null)
                {
                    if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Auto-play"))
                    {
                        interval = Convert.ToInt32(ApplicationData.Current.RoamingSettings.Values["Auto-play"].ToString());
                    }
                    else
                    {
                        interval = 30;
                    }
                }
                else
                {
                    interval = 30;
                }

                dtautoPlay.Interval = TimeSpan.FromSeconds(interval);
                dtautoPlay.Tick += dtautoPlay_Tick;
                dtautoPlay.Start();
            }
            else
            {
                btnAutoPlay.Style = App.Current.Resources["AutoPlayAppBarButtonStyle"] as Style;
                dtautoPlay.Stop();
                isPlay = true;
            }
        }

        //void btnTextSize_Click(object sender, RoutedEventArgs e)
        //{
        //    stackPopup.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //}

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //AppSettings app = new AppSettings();
            //app.FontSize = Convert.ToInt32(fontSize.Text);
            //ApplicationData.Current.RoamingSettings.Values["FontSize"] = Convert.ToInt32(fontSize.Text);

            // ComboBoxItem cbi = (ComboBoxItem)fontColor.SelectedItem;
            //ApplicationData.Current.RoamingSettings.Values["FontColor"] = fontColor.Text;
            //TextBlock tb = listQuotes.ItemContainerGenerator.ContainerFromItem("txtQuote") as TextBlock;

            //FlipViewItem p = listQuotes.ItemContainerGenerator.ContainerFromItem("txtQuote") as FlipViewItem;
            //FlipView tes = p.FindName("listQuotes") as FlipView;
            //FlipView tes = FindVisualChild(p, "listQuotes") as FlipView;
            //FlipView tes = FindVisualChild<FlipView>(p) as FlipView;
            //var selectedItem = tes.SelectedItem as Quotations;

            //TextBlock tb = listQuotes.FindName("txtQuote");
            //tb.FontSize = Convert.ToInt32(txtFontSize.Text);
            //stackPopup.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(BlankPage));

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();
            //listQuotes.SelectedIndex = listQuotes.SelectedIndex;
        }

        private async void btnBuyAdFree_Click_1(object sender, RoutedEventArgs e)
        {
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["AdFreeVersion"];
            if (!productLicense.IsActive)
            {
                try
                {
                    //await CurrentAppSimulator.RequestProductPurchaseAsync("AdFreeVersion", false);
                    await CurrentApp.RequestProductPurchaseAsync("AdFreeVersion", false);
                    if (productLicense.IsActive)
                    {
                        this.ShowMessage("You bought the Ad Free version.");
                    }
                }
                catch (Exception)
                {
                    this.ShowMessage("Unable to buy Ad Free version.");
                }
            }
            else
            {
                this.ShowMessage("You already own Ad Free version of app.");
            }
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            ImplementCopyContext();
        }

        //private void btnOtherApps_Click(object sender, RoutedEventArgs e)
        //{
        //    datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        //    //var frame = new Frame();
        //    //frame.Navigate(typeof(OtherApps));
        //    this.Frame.Navigate(typeof(OtherApps));
        //    //Window.Current.Content = frame;
        //    //Window.Current.Activate();
        //}

        private void ImplementCopyContext()
        {
            string copycontext = "";

            if (sessionQuotes.currentQuotes != null)
            {

                try
                {
                    listQuotes.SelectedItem = sessionQuotes.currentQuotes[listQuotes.SelectedIndex];

                    FlipViewItem p = listQuotes.ItemContainerGenerator.ContainerFromItem(listQuotes.SelectedItem) as FlipViewItem;

                    if (p != null)
                    {
                        StackPanel s = FindVisualChild<StackPanel>(p) as StackPanel;

                        StackPanel quoteSP = (StackPanel)s.FindName("QuotesListPanel");
                        TextBlock quoteTB = (TextBlock)quoteSP.FindName("txtQuote");

                        StackPanel authorSP = (StackPanel)s.FindName("authorListPanel");
                        TextBlock authorTB = (TextBlock)quoteSP.FindName("textQuoteAuthor");

                        copycontext = quoteTB.Text + " - " + authorTB.Text;

                        var dataPackage = new DataPackage();
                        dataPackage.SetText(copycontext);
                        Clipboard.SetContent(dataPackage);

                        #region Check for whether quote is copied into Clipboard or not

                        //var dataPackageView = Clipboard.GetContent();
                        //if (dataPackageView.Contains(StandardDataFormats.Text))
                        //{
                        //    try
                        //    {
                        //        var text = await dataPackageView.GetTextAsync();
                        //        this.ShowMessage("Text: " + Environment.NewLine + text);
                        //    }
                        //    catch (Exception ex)
                        //    {

                        //    }
                        //}

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                    dialog.ShowAsync();
                }
            }
        }

        // returns a rect for selected text
        // if selection is multiline then rect is unreliable
        // if no text is selected, returns caret location
        // textbox should not be empty
        //private Rect GetTextboxSelectionRect(TextBox textbox)
        //{
        //    Rect rectFirst, rectLast;
        //    if (textbox.SelectionStart == textbox.Text.Length)
        //    {
        //        rectFirst = textbox.GetRectFromCharacterIndex(textbox.SelectionStart - 1, true);
        //    }
        //    else
        //    {
        //        rectFirst = textbox.GetRectFromCharacterIndex(textbox.SelectionStart, false);
        //    }

        //    int lastIndex = textbox.SelectionStart + textbox.SelectionLength;
        //    if (lastIndex == textbox.Text.Length)
        //    {
        //        rectLast = textbox.GetRectFromCharacterIndex(lastIndex - 1, true);
        //    }
        //    else
        //    {
        //        rectLast = textbox.GetRectFromCharacterIndex(lastIndex, false);
        //    }

        //    GeneralTransform buttonTransform = textbox.TransformToVisual(null);
        //    Point point = buttonTransform.TransformPoint(new Point());

        //    return new Rect(point.X + rectFirst.Left,
        //        point.Y + rectFirst.Top,
        //        rectLast.Right - rectFirst.Left,
        //        rectLast.Bottom - rectFirst.Top);
        //}

        //private async void txtQuote_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        //{
        //    e.Handled = true;
        //    TextBox textbox = (TextBox)sender;
        //    if (textbox.SelectionLength > 0)
        //    {
        //        // Create a menu and add commands specifying an id value for each instead of a delegate.
        //        var menu = new PopupMenu();
        //        menu.Commands.Add(new UICommand("Copy", null, 1));

        //        Rect rect = GetTextboxSelectionRect(textbox);
        //        var chosenCommand = await menu.ShowForSelectionAsync(rect);
        //        if (chosenCommand != null)
        //        {
        //            switch ((int)chosenCommand.Id)
        //            {
        //                case 1:
        //                    String selectedText = ((TextBox)sender).SelectedText;
        //                    var dataPackage = new DataPackage();
        //                    dataPackage.SetText(selectedText);
        //                    Clipboard.SetContent(dataPackage);
        //                    break;
        //            }
        //        }
        //        else
        //        {

        //        }
        //    }
        //    else
        //    {

        //    }
        //}

        //private void listQuotes_LayoutUpdated(object sender, object e)
        //{
        //    StackPanel s = (StackPanel)listQuotes.FindName("mainStack");
        //    StackPanel quoteSP = (StackPanel)s.FindName("QuotesListPanel");
        //    TextBlock quoteTB = (TextBlock)quoteSP.FindName("txtQuote");
        //    StackPanel authorSP = (StackPanel)s.FindName("authorListPanel");
        //    TextBlock authorTB = (TextBlock)authorSP.FindName("textQuoteAuthor");
        //}

        /// <summary>
        /// Click event of the Back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region Commented on 05.04.2012
        //Windows.Data.Xml.Dom.XmlDocument c;
        //using (IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
        //{
        //    using (IRandomAccessStream readStream = await file.OpenReadAsync())
        //    {
        //        Stream stream = readStream.GetInputStreamAt(0).AsStreamForRead();
        //        XDocument doc;

        //        doc = XDocument.Load(stream);


        //        if (doc != null)
        //            doc.Save(stream);

        //    }

        //}

        //Windows.Data.Xml.Dom.XmlDocument xmlDoc;
        //using (var writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
        //{
        //    Stream stream = writeStream.GetInputStreamAt(0).AsStreamForRead();
        //    //using (IRandomAccessStream readStream = await file.OpenAsync())
        //    //{
        //    //    Stream stream = readStream.GetInputStreamAt(0).AsStreamForRead();
        //        //XDocument doc;

        //        //doc = XDocument.Load(stream);

        //        XDocument xdoc = new XDocument(new XElement("root"));
        //        xdoc.Save(stream);
        //    //}

        //}
        //if (xmlDoc != null)
        //    await xmlDoc.SaveToFileAsync(file);

        //objectStorageHelper = new StorageHelper<List<Quotation>>(StorageType.Roaming);

        //System.IO.Stream stream;
        //// get the IRandomAccessStream from the file
        //using (IRandomAccessStream writeStream = await file.OpenAsync(FileAccessMode.ReadWrite))
        //{
        //    // convert to IO.Stream
        //    stream = writeStream.AsStreamForWrite();

        //}

        // file = await roamingFolder.GetFileAsync("favQuotes.xml"); 

        //var stream = await file.OpenAsync(FileAccessMode.ReadWrite);

        //var writeStream = await file.OpenAsync(FileAccessMode.ReadWrite);

        // convert to IO.Stream
        //System.IO.Stream stream = (System.IO.Stream) writeStream.AsStreamForWrite();

        //XDocument doc;

        //doc = XDocument.Load(stream);
        //stream.Seek(0, SeekOrigin.Begin);

        //// execute the query
        //IEnumerable<XElement> qteExists =
        //        from el in doc.Descendants("q")
        //        where el.Attribute("id").Value == q.QuoteId
        //        select el;

        //// Checks the Quote already exists based on the quote id
        //if (qteExists.Count() == 0)
        //{
        //    XElement qElement = new XElement("q",
        //        new XAttribute("id", q.QuoteId),
        //        new XElement("d", q.Quote),
        //        new XElement("a", q.Author)
        //                );

        //    doc.Element("root").Add(qElement);
        //    doc.Save(stream);
        //    returnValue = "Added";
        //}
        //else
        //{
        //    returnValue = "Exists";
        //}

        // Windows.Data.Xml.Dom.XmlDocument xmlDoc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file);

        //Windows.Data.Xml.Dom.XmlDocument xmlDoc;
        // using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
        //{
        //xmlDoc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file);

        //Windows.Data.Xml.Dom.XmlElement root = xmlDoc.DocumentElement;
        ////root.Attributes id = xmlDoc.CreateAttribute("id");
        ////id.
        ////Windows.Data.Xml.Dom.XmlAttribute id = root.c.;
        //Windows.Data.Xml.Dom.XmlElement quote = xmlDoc.CreateElement("q");
        //Windows.Data.Xml.Dom.XmlAttribute id = xmlDoc.CreateAttribute("id");
        //id.Value = q.QuoteId;
        //Windows.Data.Xml.Dom.XmlElement d = xmlDoc.CreateElement("d");
        //d.InnerText = q.Quote;
        //Windows.Data.Xml.Dom.XmlElement desc = xmlDoc.CreateElement("a");
        //desc.InnerText = q.Author;

        //quote.AppendChild(d);
        //quote.AppendChild(desc);
        //root.AppendChild(quote);
        //root.AppendChild(d);
        //root.AppendChild(desc);


        //    XElement qElement = new XElement("q",
        //                new XAttribute("id", q.QuoteId),
        //                new XElement("d", q.Quote),
        //                new XElement("a", q.Author)
        //                );
        //}

        //Windows.Data.Xml.Dom.XmlDocument xmlDoc;
        //using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
        //{
        //    xmlDoc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file);
        //    stream.Seek(0);

        //    Windows.Data.Xml.Dom.XmlElement root = xmlDoc.DocumentElement;

        //    Windows.Data.Xml.Dom.XmlElement title = xmlDoc.CreateElement("Title");
        //    title.InnerText = "title";
        //    Windows.Data.Xml.Dom.XmlElement desc = xmlDoc.CreateElement("Description");
        //    desc.InnerText = "description";

        //    root.AppendChild(title);
        //    root.AppendChild(desc);
        //}
        //if (xmlDoc != null)
        //    await xmlDoc.SaveToFileAsync(file);

        //Windows.Data.Xml.Dom.XmlDocument xmlDoc;
        //using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
        //{
        //    IRandomAccessStream
        //    xmlDoc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file);

        //    Windows.Data.Xml.Dom.XmlElement root = xmlDoc.DocumentElement;

        //    Windows.Data.Xml.Dom.XmlElement title = xmlDoc.CreateElement("Title");
        //    title.InnerText = "title";
        //    Windows.Data.Xml.Dom.XmlElement desc = xmlDoc.CreateElement("Description");
        //    desc.InnerText = "description";

        //    root.AppendChild(title);
        //    root.AppendChild(desc);
        //}
        //if (xmlDoc != null)
        //    await xmlDoc.SaveToFileAsync(file);

        //if (xmlDoc != null)
        //    await xmlDoc.SaveToFileAsync(file);

        //await FileIO.WriteTextAsync((file, formatter.Format(DateTime.Now));
        // displays message based on the above result.

        //if (addResult == "Added")
        //    txtNotificationText.Text = "Quote added to your favorites";
        //else if (addResult == "Exists")
        //    txtNotificationText.Text = "Quote already exists in your favorites";

        //// shows the Notification stackPanel
        //stackNotification.Visibility = System.Windows.Visibility.Visible;
        #endregion
    }
}
