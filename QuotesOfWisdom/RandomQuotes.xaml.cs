using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using SQLite;
using QuotesOfWisdom.Data;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using Windows.ApplicationModel.Store;
using Windows.ApplicationModel;
using QuotesOfWisdom.Common;

//using Windows.Foundation.Collections;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Media.Imaging;
//using System.Runtime.Serialization;
//using System.Xml.Serialization;
//using System.Text;

namespace QuotesOfWisdom
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RandomQuotes : Page
    {
        #region Objects
        public DispatcherTimer dtautoPlay = new DispatcherTimer();
        int interval = 0;
        bool isPlay = true;
        StorageFolder roamingFolder = null;
        bool isExists = true;
        Random randNum = new Random();
        List<Quotations> list = new List<Quotations>();
        //public DispatcherTimer dtrandomQuoteDisplay = new DispatcherTimer();
        LicenseChangedEventHandler licenseChangeHandler = null;
        public int currentQuoteIndex = -1;
        private int maxQuotesInList = 100;

        #endregion

        public RandomQuotes()
        {
            this.InitializeComponent();
            sessionData.isBackgroundChanged = true;
            Windows.Storage.ApplicationData.Current.DataChanged += new TypedEventHandler<ApplicationData, object>(DataChangeHandler);
            Window.Current.SizeChanged += Current_SizeChanged;

        }

        /// <summary>
        /// Window Size change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            ChangeWindowState();
        }

        /// <summary>
        /// Method for window size change
        /// </summary>
        private void ChangeWindowState()
        {
            string visualState = Utilities.DetermineVisualState(ApplicationView.Value);
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            var productLicense = licenseInformation.ProductLicenses["Ad Free"];

            if (visualState == "Snapped")
            {
                ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                //if (productLicense.IsActive)
                //{
                //    ad2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //}
                //else
                //{
                //    ad2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //}

                //listQuotes.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                listQuotesSnapped.Visibility = Windows.UI.Xaml.Visibility.Visible;
                //listQuotesSnapped.SelectedIndex = listQuotes.SelectedIndex;

                backButton.Style = App.Current.Resources["SnappedBackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["SnappedPageHeaderTextStyle"] as Style;
            }
            else
            {
                if (productLicense.IsActive)
                {
                    ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    //ad1.Visibility = Visibility.Visible;
                }

                //ad2.Visibility = Visibility.Collapsed;
                //listQuotes.Visibility = Visibility.Visible;
                listQuotesSnapped.Visibility = Visibility.Collapsed;
                //listQuotes.SelectedIndex = listQuotesSnapped.SelectedIndex;
                backButton.Style = App.Current.Resources["BackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["PageHeaderTextStyle"] as Style;
            }
        }

        /// <summary>
        /// Click event of Buy Ad Free button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnBuyAdFree_Click(object sender, RoutedEventArgs e)
        {
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

        void DataChangeHandler(Windows.Storage.ApplicationData appData, object o)
        {

            // TODO: Refresh your data
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            #region Load Ramdom Quotes
            showHideAds();
            LoadRandomQuotes("Next");
            #endregion

            ChangeWindowState();
        }

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
                //ad2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                string visualState = Utilities.DetermineVisualState(ApplicationView.Value);
                if (visualState == "Snapped")
                {
                    //ad2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    ad1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    //ad2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        /// <summary>
        /// Method Show or Hide Ads
        /// </summary>
        private async void showHideAds()
        {
            await LoadInAppPurchaseProxyFileAsync();

            //LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
            LicenseInformation licenseInformation = CurrentApp.LicenseInformation;

            var productLicense = licenseInformation.ProductLicenses["Ad Free"];
            if (productLicense.IsActive)
            {
                ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //ad2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                string visualState = Utilities.DetermineVisualState(ApplicationView.Value);
                if (visualState == "Snapped")
                {
                    ad1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    ad1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                //ad1.Visibility = Windows.UI.Xaml.Visibility.Visible;
                btnBuyAdFree.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        /// <summary>
        /// Method for loading Random Quotes
        /// </summary>
        /// <param name="mode"></param>
        private void LoadRandomQuotes(string mode)
        {
            #region Commented
            /*
            List<Quotations> list = new List<Quotations>();

            if (sessionData.currentRandomQuotes != null)
            {
                if (sessionData.currentRandomQuotes.Count == 0)
                {
                    using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
                    {
                        var query = from q in db.Table<Quotations>()
                                    select q;

                        list = query.ToList();
                    }

                    listQuotes.ItemsSource = list.ToList().Take(1);
                }
                else
                {
                    listQuotes.ItemsSource = sessionData.currentRandomQuotes.ToList().Take(1);
                }
            }
            * */

            #endregion

            try
            {

                if (mode == "Next")
                {

                    if (list.Count == currentQuoteIndex+1)
                    {

                        string rand1 = "";

                        using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
                        {
                            rand1 = randNum.Next(1, 54000).ToString();
                            var query = from q in db.Table<Quotations>()
                                        where q.QuoteId == rand1
                                        select q;


                            list.Add(query.First());

                            if (list.Count > maxQuotesInList)
                            {
                                list.RemoveAt(0);
                            }
                            else
                            {
                                currentQuoteIndex = currentQuoteIndex + 1;
                            }
                            
                        }
                    }
                    else
                    {
                        currentQuoteIndex = currentQuoteIndex + 1;
                    }

                }
                else
                {
                    currentQuoteIndex = currentQuoteIndex - 1;
                    
                }

                if (currentQuoteIndex == -1)
                {
                    ShowMessage("No previous quotes!");
                }
                else
                {
                    txtQuote.Text = list[currentQuoteIndex].Quote;
                    textQuoteAuthor.Text = list[currentQuoteIndex].Author;
                }
           
            }
            catch
            { }

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
        /// Click event of the Back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //sessionData.isRandomQuoteAutomatic = false;
            //dtrandomQuoteDisplay.Stop();
            dtautoPlay.Stop();
            // Use the navigation frame to return to the previous page
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }

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
            containerStack.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        }
        /// <summary>
        /// Method of changing background
        /// </summary>
        private void ChangeBackground()
        {
            #region Commented on 11.06.2013
            //if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            //{
            //    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
            //}
            //else
            //{
            //    LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
            //}

            //#region Custom Image selection
            ///*
            //if (ApplicationData.Current.RoamingSettings.Values["Settings"] != null)
            //{
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
            //}
            //else
            //{
            //    LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
            //}
            //*/
            //#endregion
            #endregion

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            {
                if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() != "dynamicStyle")
                {
                    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;

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
        /// Click event of the Auto Play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnAutoPlay_Click(object sender, RoutedEventArgs e)
        {
            #region Commented
            
            if (isPlay)
            {
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
                sessionData.isRandomQuoteAutomatic = false;
                btnAutoPlay.Style = App.Current.Resources["AutoPlayAppBarButtonStyle"] as Style;
                dtautoPlay.Stop();
                isPlay = true;
            }
            
            #endregion

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
        /// Click event of Add to Fav button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void btnAddToFav_Click(object sender, RoutedEventArgs e)
        {
            string returnValue = "";
            
            // get the current quote from flipview control

            Quotations q = null;

            //string visualState = DetermineVisualState(ApplicationView.Value);
            //LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
            //var productLicense = licenseInformation.ProductLicenses["Ad Free"];

            //if (visualState == "Snapped")
            //{

            //    q = listQuotesSnapped.SelectedItem as Quotations;
            //}
            //else
            //{
                //q = listQuotes.SelectedItem as Quotations;
                Quotations tq = new Quotations();
                tq.Author = list[currentQuoteIndex].Author;
                tq.QuoteId = list[currentQuoteIndex].QuoteId;
                tq.Quote = list[currentQuoteIndex].Quote;
                tq.Cat = list[currentQuoteIndex].Cat;
                q = tq;

            //}
            #region Add to Favorites

            returnValue = await AddtoFavorites(q);

            #endregion

            if (returnValue == "Added")
            {
                this.ShowMessage("Quote added to your favorites");
            }
            else if (returnValue == "Exists")
            {
                this.ShowMessage("Quote already exists in your favorites");
            }

        }

        /// <summary>
        /// Method for adding quotes to favorites
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
                        await outStream.FlushAsync();

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
        /// Method for showing pop ups mesages
        /// </summary>
        /// <param name="msg"></param>
        async void ShowMessage(string msg)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }

        /// <summary>
        /// Click event of the Auto Play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtautoPlay_Tick(object sender, object e)
        {
            //listQuotes.SelectedIndex += 1;
            LoadRandomQuotes("Next");
        }

       
        /// <summary>
        /// Pointer Presssed event of the stack panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StackPanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

            Windows.UI.Input.PointerPoint pt = e.GetCurrentPoint(containerStack);

            var windowWidth = Window.Current.Bounds.Width;
            if (pt.Position.X < windowWidth / 2)
            {
                if (currentQuoteIndex >= 0)
                {
                    LoadRandomQuotes("Previous");
                }
                else
                {
                    ShowMessage("No previous quotes!");
                }
            }
            else
            {
                LoadRandomQuotes("Next");
            }
            e.Handled = true;

        }

        /// <summary>
        /// ManipulationCompleted event of the stack panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void containerStack_ManipulationCompleted_1(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var velocities = e.Velocities;
        }

        
        /// <summary>
        /// Click event of the Previous button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void prevButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (currentQuoteIndex >= 0)
            {
                LoadRandomQuotes("Previous");
            }
            else
            {
                ShowMessage("No previous quotes!");
            }
        }

        /// <summary>
        /// Click event of the Next button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextButton_Click_1(object sender, RoutedEventArgs e)
        {
            LoadRandomQuotes("Next");
        }        

    }
}
