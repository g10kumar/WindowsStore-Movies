using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using QuotesOfWisdom.Data;
using System.Xml.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Core;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using QuotesOfWisdom.Common;

//using Windows.Foundation.Collections;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.ApplicationModel;

namespace QuotesOfWisdom
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoriteQuotes : Page
    {
        public FavoriteQuotes()
        {
            this.InitializeComponent();
            Windows.Storage.ApplicationData.Current.DataChanged += new TypedEventHandler<ApplicationData, object>(DataChangeHandler);
            Window.Current.SizeChanged += Current_SizeChanged;            
        }

        void DataChangeHandler(Windows.Storage.ApplicationData appData, object o)
        {
            // TODO: Refresh your data
        }

        /// <summary>
        /// Window Size Change event method
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
            string visualState = Utilities.DetermineVisualState(ApplicationView.Value);

            if (visualState == "Snapped")
            {
                itemGridView.Visibility = Visibility.Collapsed;
                itemListView.Visibility = Visibility.Visible;
                itemListView.SelectedIndex = itemGridView.SelectedIndex;
                backButton.Style = App.Current.Resources["SnappedBackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["SnappedPageHeaderTextStyle"] as Style;
            }
            else
            {
                itemGridView.Visibility = Visibility.Visible;
                itemListView.Visibility = Visibility.Collapsed;
                itemGridView.SelectedIndex = itemListView.SelectedIndex;
                backButton.Style = App.Current.Resources["BackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["PageHeaderTextStyle"] as Style;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            #region Load Favorite Quotes
            LoadFavQuotes();
            #endregion

            // Calls the ChangeWindowState method
            ChangeWindowState();
        }

        /// <summary>
        /// LoadQuotes method
        /// </summary>
        private async void LoadFavQuotes()
        {
            List<Quotations> listFav = new List<Quotations>();
            if (sessionData.currentFavoriteQuotes == null)
            {
                StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
                StorageFile sampleFile = await roamingFolder.GetFileAsync("favQuotes.xml");
                string fileContent = await FileIO.ReadTextAsync(sampleFile);

                XElement xe = XElement.Parse(fileContent);

                // Execute the query
                var query = (from quotes in xe.Elements("q")
                             orderby System.Guid.NewGuid()
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

                itemGridView.ItemsSource = sessionData.currentFavoriteQuotes.ToList();
                itemListView.ItemsSource = sessionData.currentFavoriteQuotes.ToList();
            }
            else
            {
                itemGridView.ItemsSource = sessionData.currentFavoriteQuotes.ToList();
                itemListView.ItemsSource = sessionData.currentFavoriteQuotes.ToList();
            }
        }

        /// <summary>
        /// Click event of the Back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
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
            ChangeBackground();
        }
        /// <summary>
        /// Method of changing background
        /// </summary>
        private void ChangeBackground()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            {
                LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
            }

            #region Custom Image selection
            /*
            if (ApplicationData.Current.RoamingSettings.Values["Settings"] != null)
            {
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
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
            }
            */
            #endregion
        }

        /// <summary>
        /// Delete Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string returnValue = "";
            string visualState  = Utilities.DetermineVisualState(ApplicationView.Value);
            if (visualState == "Snapped")
            {
                for (int i = 0; i < itemListView.SelectedItems.Count; i++)
                {
                    Quotations q = itemListView.SelectedItems[i] as Quotations;
                    returnValue = await DeleteFavs(q);
                }
            }
            else
            {
                for (int i = 0; i < itemGridView.SelectedItems.Count; i++)
                {
                    Quotations q = itemGridView.SelectedItems[i] as Quotations;
                    returnValue = await DeleteFavs(q);
                }
            }

            #region Load Favorite Quotes
            LoadFavQuotes();
            #endregion

        }

        /// <summary>
        /// Delete Favorite Quotes method
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        async public Task<string> DeleteFavs(Quotations q)
        {
            string returnValue = "";

            try
            {
                string tmpreturnValue = "";
                
                const string filename = "favQuotes.xml";
                StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;
                StorageFile file = await roamingFolder.GetFileAsync(filename);
                
                if (file != null)
                {
                    using (IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        Stream outStream = Task.Run(() => readStream.AsStreamForWrite()).Result;
                        await outStream.FlushAsync();
                        XDocument doc;
                        doc = XDocument.Load(outStream);

                        // Execute the query
                        IEnumerable<XElement> qteExists =
                                from el in doc.Descendants("q")
                                where el.Attribute("id").Value == q.QuoteId.ToString()
                                select el;

                        // Checks the Quote already exists based on the quote id
                        if (qteExists.Count() == 0)
                        {
                            tmpreturnValue = "Exists";

                            returnValue = tmpreturnValue;
                        }
                        else
                        {
                            doc.Element("root").Descendants("q").Where(x => (string)x.Attribute("id").Value == q.QuoteId.ToString()).Remove();
                            outStream.SetLength(0);
                            outStream.Seek(0, SeekOrigin.Begin);
                            doc.Save(outStream);
                            await outStream.FlushAsync();
                            sessionData.currentFavoriteQuotes.Remove(q);
                            tmpreturnValue = "DELETED";

                            returnValue = tmpreturnValue;
                        }
                    }
                }
            }
            catch { }

            return returnValue;
        }

        /// <summary>
        /// method for showing popups
        /// </summary>
        /// <param name="msg"></param>
        async void ShowMessage(string msg)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }
    }
}
