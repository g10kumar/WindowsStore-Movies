using QuotesOfWisdom.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using SQLite;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Storage.Streams;
using System.Xml.Linq;
using System.Xml;
using System.Collections.ObjectModel;
using QuotesOfWisdom.Common;

//using Windows.Foundation.Collections;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media.Imaging;
//using System.Runtime.Serialization;
//using System.Xml.Serialization;
//using System.Text;

namespace QuotesOfWisdom
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage : QuotesOfWisdom.Common.LayoutAwarePage
    {
        #region Objects
        //bool firstTime = false;
        //string currentTitle = "";

        public DataTransferManager datatransferManager;
        //int i = 0;
        //int selectedQuoteIndex = 0;
        //FlipView fv;
        LicenseChangedEventHandler licenseChangeHandler = null;
        //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
        //LicenseInformation licenseInformation = CurrentApp.LicenseInformation;

        StorageFolder roamingFolder = null;
        bool isExists = true;
        List<Quotations> list = new List<Quotations>();
        
        #endregion
        public ItemDetailPage()
        {
            this.InitializeComponent();
            sessionData.isBackgroundChanged = true;
            //fv = flipView.FindName("listQuotes") as FlipView;            
            ShareSourceLoad();
            Window.Current.SizeChanged += Current_SizeChanged;

        }

        /// <summary>
        /// Method for when the window size is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            string visualState = DetermineVisualState(ApplicationView.Value);

            //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["Ad Free"];

            if (visualState == "Snapped")
            {
                ad1.Visibility = Visibility.Collapsed;

                if (productLicense.IsActive)
                {
                    ad2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    ad2.Visibility = Visibility.Visible;
                }

                //listQuotes.Visibility = Visibility.Collapsed;
                //listQuotesSnapped.Visibility = Visibility.Visible;
                //listQuotesSnapped.SelectedIndex = listQuotes.SelectedIndex;

            }
            else
            {
                if (productLicense.IsActive)
                {
                    ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    ad1.Visibility = Visibility.Visible;
                }

                ad2.Visibility = Visibility.Collapsed;
                //listQuotes.Visibility = Visibility.Visible;
                //listQuotesSnapped.Visibility = Visibility.Collapsed;
                //listQuotes.SelectedIndex = listQuotesSnapped.SelectedIndex;
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

        //protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        //{
        //    await LoadInAppPurchaseProxyFileAsync();

        //    //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
        //    LicenseInformation licenseInformation = CurrentApp.LicenseInformation;

        //    var productLicense = licenseInformation.ProductLicenses["Ad Free"];
        //    if (productLicense.IsActive)
        //    {
        //        ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //        ad2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //        btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        ad1.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //        btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //    }
        //}

        /// <summary>
        /// Method for loading App Purchase proxy file
        /// </summary>
        /// <returns></returns>
        private async Task LoadInAppPurchaseProxyFileAsync()
        {
            StorageFolder proxyDataFolder = await Package.Current.InstalledLocation.GetFolderAsync("Xml");
            StorageFile proxyFile = await proxyDataFolder.GetFileAsync("in-app-purchase.xml");
            licenseChangeHandler = new LicenseChangedEventHandler(InAppPurchaseRefreshScenario);

            //CurrentAppSimulator.LicenseInformation.LicenseChanged += licenseChangeHandler;
            CurrentApp.LicenseInformation.LicenseChanged += licenseChangeHandler;
            //await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);

        }

        /// <summary>
        /// Refresg method ofr App Purchase
        /// </summary>
        private void InAppPurchaseRefreshScenario()
        {
            //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["Ad Free"];
            if (productLicense.IsActive)
            {
                //rootPage.NotifyUser("You can use Product 1.", NotifyType.StatusMessage);
                ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ad2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                string visualState = DetermineVisualState(ApplicationView.Value);
                if (visualState == "Snapped")
                {
                    ad2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    ad1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ad2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //rootPage.NotifyUser("You don't own Product 1. You must buy Product 1 before you can use it.", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Click event of Buy Ad Free button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnBuyAdFree_Click_1(object sender, RoutedEventArgs e)
        {
            //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["Ad Free"];
            if (!productLicense.IsActive)
            {
                try
                {
                    //await CurrentAppSimulator.RequestProductPurchaseAsync("Ad Free", false);
                    await CurrentApp.RequestProductPurchaseAsync("Ad Free", false);
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

        /// <summary>
        /// Click event of the Copy button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            // Calls the copying method
            ImplementCopyContext();
        }

        /// <summary>
        /// Copy method
        /// </summary>
        private void ImplementCopyContext()
        {
            string copycontext = "";

            try
            {
                FlipViewItem p = flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem) as FlipViewItem;
                FlipView tes = FindVisualChild<FlipView>(p) as FlipView;
                var selectedItem = tes.SelectedItem as Quotations;

                copycontext = selectedItem.Quote.ToString() + " - " + selectedItem.Author.ToString();

                var dataPackage = new DataPackage();
                dataPackage.SetText(copycontext);
                Clipboard.SetContent(dataPackage);

                //#region Check for whether quote is copied into Clipboard or not

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

                //#endregion

            }
            catch (Exception ex)
            {
                this.ShowMessage(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Method for showing pop up messages
        /// </summary>
        /// <param name="msg"></param>
        async void ShowMessage(string msg)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
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

        /// <summary>
        /// Unloaded of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageRoot_Unloaded_1(object sender, RoutedEventArgs e)
        {
            datatransferManager = DataTransferManager.GetForCurrentView();
            datatransferManager.DataRequested -= DataRequested;
        }

        /// <summary>
        /// Async method for data sharing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {

            //var selectedItem = (QuotesItem)this.flipView.SelectedItem;

            FlipViewItem p = flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem) as FlipViewItem;
            //FlipView tes = p.FindName("listQuotes") as FlipView;
            //FlipView tes = FindVisualChild(p, "listQuotes") as FlipView;
            FlipView tes = FindVisualChild<FlipView>(p) as FlipView;
            var selectedItem = tes.SelectedItem as Quotations;

            //var selectedItem = (QuotesItem)fv.SelectedItem;
            //FlipView fv = (FlipView)this.flipView.FindName("listQuotes");
            //string ss = "";
            if (!selectedItem.Author.Contains("Proverb"))
                e.Request.Data.Properties.Title = "A quote on '" + selectedItem.Cat + "' by " + selectedItem.Author;
            else
                e.Request.Data.Properties.Title = "A quote on '" + selectedItem.Cat + "'";

            e.Request.Data.Properties.Description = selectedItem.Quote.ToString() + " - " + selectedItem.Author.ToString();
            e.Request.Data.SetText(selectedItem.Quote.ToString() + " - " + selectedItem.Author.ToString());

        }

        /// <summary>
        /// Method for finding child items of the control
        /// </summary>
        /// <typeparam name="childItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            sessionData.isBackgroundChanged = true;
            await LoadInAppPurchaseProxyFileAsync();

            //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            string visualState = DetermineVisualState(ApplicationView.Value);

            var productLicense = licenseInformation.ProductLicenses["Ad Free"];
            if (productLicense.IsActive)
            {
                ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                ad2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                if (visualState == "Snapped")
                {
                    ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    ad2.Visibility = Visibility.Visible;
                }
                else
                {
                    ad1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ad2.Visibility = Visibility.Collapsed;
                }
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            if (sessionData.isSearch)
            {
                navigationParameter = sessionData.searchWord;

                //if (sessionData.isSearchCat)
                //{
                    
                //}
                //else if (sessionData.isSearchAut)
                //{
                //    //getSearchAuthors((String)navigationParameter);
                //}
            }

            //var item = Quotes.GetItem((String)navigationParameter);

            var item = new QuotesItem("", 0, "", "", "", "", "", null, null);

            if (sessionData.isAllCat)
            {
                item = Quotes.GetCatItem((String)navigationParameter);
            }
            else if (sessionData.isAllAut)
            {
                item = Quotes.GetAuthItem((String)navigationParameter);
            }
            //else if (sessionData.isSearchCat)
            //{
                
            //////    //var groupCat = new QuotesGroup("Categories");
            //    item = Quotes.GetSearchCatItem((String)navigationParameter);
            //////    //item = new QuotesItem(sessionData.currentCategoryQuotes[0].category, sessionData.currentCategoryQuotes[0].ct, sessionData.currentCategoryQuotes[0].category, "Category", "", "", "", groupCat, null);
            //}
            //else if (sessionData.isSearchAut)
            //{
            //    item = Quotes.GetSearchAuthItem((String)navigationParameter);
            //}
            else
            {
                //flipView.ItemsSource = sessionData.currentCategoryQuotes.ToList();
                item = Quotes.GetItem((String)navigationParameter);
            }

            sessionData.title = item.Title;
            //currentTitle = item.Title;
            sessionData.subTitle = item.Subtitle;

            //List<Quotations> list = new List<Quotations>();

            // reads the quotes data based on Author
            list = LoadQuotes(item.Title, item.Subtitle);            

            item.Quotations = list;

            this.DefaultViewModel["Group"] = item.Group;
            this.DefaultViewModel["Items"] = item.Group.Items;

            this.flipView.SelectedItem = item;

            #region Commented on 14.02.13
            // sessionData.firstTime = true;
            //if (!firstTime)
            //{
            //    flipView.SelectionChanged -= flipView_SelectionChanged_1;
            //    firstTime = true;
            //}
            //sessionData.currentTitle = item.Title;
            #endregion
        }

        
        /// <summary>
        /// SelectionChange event of the FlipView control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flipView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            //if (flipView.SelectedIndex == 0)
            //    return;

            var item = (QuotesItem)this.flipView.SelectedItem;
            //var item = new QuotesItem(sessionData.currentCategoryQuotes[flipView.SelectedIndex].category, 0, sessionData.currentCategoryQuotes[flipView.SelectedIndex].category, "Category", "", "", "", groupCat, null);

            //if (item == null)
            //{
            //    if (sessionData.isSearchCat)
            //    {
            //        var groupCat = new QuotesGroup("Categories");
            //        //item = Quotes.GetSearchCatItem((String)navigationParameter);
            //        item = new QuotesItem(sessionData.currentCategoryQuotes[flipView.SelectedIndex].category, 0, sessionData.currentCategoryQuotes[flipView.SelectedIndex].category, "Category", "", "", "", groupCat, null);
            //    }
            //}
            if (sessionData.title != item.Title)
            {
                sessionData.title = item.Title;
                list.Clear();
                // reads the quotes data based on Author
                list = LoadQuotes(item.Title, item.Subtitle);
            }
            
            item.Quotations = list;

            //this.DefaultViewModel["Group"] = item.Group;
            //this.DefaultViewModel["Items"] = item.Group.Items;

            this.flipView.SelectedItem = item;

            #region Commented on 14.02.13
            //if(flipView.SelectedItem != flipView
            //flipView.SelectionChanged += flipView_SelectionChanged_1;

            //FlipViewItem p = flipView.ItemContainerGenerator.ContainerFromItem(sessionData.title) as FlipViewItem;
            //RichTextBlock tes = FindVisualChild<RichTextBlock>(p) as RichTextBlock;
            //var selectedItem = tes.SelectedItem as Quotations;
            //richTextBlock
            //TextBlock rtb = (TextBlock)flipView.FindName("test");
            //var item = Quotes.GetItem((String)sessionData.title);
            //List<Quotations> list = new List<Quotations>();
            //currentTitle = item.Title;

            //if (item != null)
            //{
            //}
            //sessionData.firstTime = false;
            #endregion
        }

        /// <summary>
        /// Quotes loading method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<Quotations> LoadQuotes(string name, string type)
        {
            List<Quotations> list = new List<Quotations>();
            try
            {
                if (type == "Category")
                {
                    using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
                    {
                        var query = from q in db.Table<Quotations>()
                                    //where q.Cat.Contains(name) == true
                                    where q.Cat == name
                                    select q;

                        list = query.ToList();
                    }
                }
                else if (type == "Author")
                {
                    using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
                    {
                        var query = from q in db.Table<Quotations>()
                                    where q.Author == name
                                    select q;

                        list = query.ToList();
                    }
                }

                Random rng = new Random();
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    Quotations value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
            catch (Exception ex)
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Oops! There was a problem. Please try again.");
                dialog.ShowAsync();
            }

            return list;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = (QuotesItem)this.flipView.SelectedItem;
            pageState["SelectedItem"] = selectedItem.UniqueId;
        }

        /// <summary>
        /// Click event of the Back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            sessionData.isAllCat = false;
            sessionData.isAllAut = false;

            sessionData.isSearchCat = false;
            sessionData.isSearchAut = false;

            flipView.SelectedIndex = 0;
            //sessionData.currentTitle = "";
            //this.flipView.SelectedItem = null;
            datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }

        //private void listQuotes_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        //{
        //    //var selectedItem = ((QuotesItem)this.flipView.SelectedItem).Quotations;
        //    //selectedQuoteIndex = selectedQuoteIndex + 1;
        //    //ad1.Refresh();
        //}

        /// <summary>
        /// LayoutUpdted Event of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_LayoutUpdated(object sender, object e)
        {
            // Calls the Background change method
            if (sessionData.isBackgroundChanged)
            {
                ChangeBackground();
                sessionData.isBackgroundChanged = false;
            }
        }


        //private async void btnLike_Click(object sender, RoutedEventArgs e)
        //{
        //    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:PDP?PFN=DaksaTech.QuotesofWisdom_c7fyd19frge5m"));
        //}

        //private async void btnDislike_Click(object sender, RoutedEventArgs e)
        //{
        //    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:PDP?PFN=DaksaTech.QuotesofWisdom_c7fyd19frge5m"));
        //}

        /// <summary>
        /// Method of changing background
        /// </summary>
        private void ChangeBackground()
        {
            #region Commented on 11.06.2013
            //if (ApplicationData.Current.RoamingSettings.Values["Settings"] != null)
            //{
            //    if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            //    {
            //        LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
            //    }
            //    else
            //    {
            //        LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
            //    }

            //    #region Custom Image Selection
            //    /*
            //    if (ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() != "CustomImage")
            //    {
            //        LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
            //    }
            //    else
            //    {
            //        StorageFile file;

            //        file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(ApplicationData.Current.RoamingSettings.Values["fileToken"].ToString());

            //        ImageBrush ib = new ImageBrush();

            //        BitmapImage src = new BitmapImage();
            //        src.SetSource(await file.OpenAsync(FileAccessMode.Read));


            //        ib.ImageSource = src;

            //        LayoutRoot.Background = ib;

            //    }
            //    */
            //    #endregion
            //}
            //else
            //{
            //    LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
            //}
            #endregion

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            {
                if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() != "dynamicStyle")
                {
                    //LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                    if ((string)ApplicationData.Current.RoamingSettings.Values["bgColor"].ToString() != "")
                    {
                        SolidColorBrush sbColorBrush = new SolidColorBrush(Utilities.HexColor(ApplicationData.Current.RoamingSettings.Values["bgColor"].ToString()));
                        LayoutRoot.Background = sbColorBrush;
                    }
                    else
                    {
                        SolidColorBrush sbColorBrush = new SolidColorBrush(Utilities.HexColor("#f2b100"));
                        LayoutRoot.Background = sbColorBrush;
                    }

                }
                else
                {
                    Utilities.dynamicBackgroundChange(LayoutRoot);
                }
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;

            }
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
        /// Click event of the Add To Fav button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void btnAddToFav_Click(object sender, RoutedEventArgs e)
        {
            string returnValue = "";

            // get the current quote from currentQuotes sessionQuotes variable based on
            // quoteindex

            FlipViewItem p = flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedItem) as FlipViewItem;
            FlipView tes = FindVisualChild<FlipView>(p) as FlipView;
            var q = tes.SelectedItem as Quotations;

            roamingFolder = ApplicationData.Current.RoamingFolder;

            #region Add to Favorites
            try
            {
                returnValue = await AddtoFavorites(q);



                if (returnValue == "Added")
                {
                    this.ShowMessage("Quote added to your favorites");
                }
                else if (returnValue == "Exists")
                {
                    this.ShowMessage("Quote already exists in your favorites");
                }
            }
            catch (Exception ex)
            {
                this.ShowMessage("The Quote would not be added to your favorites. Please try again later.");
            }
            #endregion
        }

        /// <summary>
        /// Method for adding Favorite Quotes
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        async public Task<string> AddtoFavorites(Quotations q)
        {
            string returnValue = "";
            string tmpreturnValue = "";
            const string filename = "favQuotes.xml";
            roamingFolder = ApplicationData.Current.RoamingFolder;
            StorageFile file;

            file = await GetFileIfExistsAsync(roamingFolder, filename);

            if (file == null)
            {
                isExists = false;
                file = await roamingFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            }
            else
            {
                isExists = true;
            }

            using (IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {

                IOutputStream ioutStream = readStream.GetOutputStreamAt(0);

                Stream outStream = Task.Run(() => readStream.AsStreamForWrite()).Result;

                if (isExists == true)
                {
                    XDocument doc;
                    doc = XDocument.Load(outStream);
                    outStream.Seek(0, SeekOrigin.Begin);
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
                            new XElement("a", q.Author),
                            new XElement("c", q.Cat)
                                    );

                        doc.Element("root").Add(qElement);
                        doc.Save(outStream);

                        tmpreturnValue = "Added";
                        returnValue = tmpreturnValue;

                    }
                    else
                    {
                        tmpreturnValue = "Exists";
                        returnValue = tmpreturnValue;
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
                        writer.WriteElementString("c", q.Cat);
                        writer.WriteEndElement();

                        writer.WriteEndElement();

                        writer.WriteEndDocument();

                        tmpreturnValue = "Added";
                        returnValue = tmpreturnValue;
                    }
                }
            }

            return returnValue;
        }
    }
}
