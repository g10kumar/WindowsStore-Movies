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
using QuotesOfWisdom.Data;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.Storage;
using QuotesOfWisdom.Common;
using SQLite;
using System.Xml.Linq;
using Windows.UI.ViewManagement;
using Windows.UI.Core;

namespace QuotesOfWisdom
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class AllAuthors : QuotesOfWisdom.Common.LayoutAwarePage
    {
        public AllAuthors()
        {
            this.InitializeComponent();
            sessionData.isBackgroundChanged = true;
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
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            List<Authors> listAuthor = new List<Authors>();

            if (sessionData.currentAuthors == null)
            {
                Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");
                var sf = await storageFolder.GetFileAsync(@"Author.xml");
                var file = await sf.OpenAsync(FileAccessMode.Read);
                Stream inStream = file.AsStreamForRead();
                //XDocument xdoc = XDocument.Load(inStream);
                XElement xe = XElement.Load(inStream);

                var query = (from q in xe.Elements("auth")
                             where (int)q.Attribute("count") > 5
                             orderby q.Value
                             select new Authors
                             {
                                 Author = q.Value,
                                 ct = (int)q.Attribute("count")
                             });

                //loop the author details and binds to authorlist
                foreach (Authors a in query)
                {
                    listAuthor.Add(a);
                }

                //sessionData.currentAuthorQuotes = listAuthor;
                sessionData.currentAuthors = listAuthor;

            }
            else
            {
                //listAuthor = sessionData.currentAuthorQuotes.ToList();
                //foreach (var v in from a in sessionData.currentAuthors.ToList()
                //                  where a.ct > 2
                //                  select new Authors
                //                  {
                //                      Author = a.Author,
                //                      ct = a.ct
                //                  })
                //{
                //    listAuthor.Add(v);
                //}

                listAuthor = sessionData.currentAuthors.ToList();//.Where(l => l.ct > 10).ToList();
            }
            itemGridView.ItemsSource = listAuthor;
            itemListView.ItemsSource = listAuthor;
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
        }
        /// <summary>
        /// Method of changing background
        /// </summary>
        private void ChangeBackground()
        {
            ChangeWindowState();

            #region Commented on 11.06.2013
            //if (ApplicationData.Current.RoamingSettings.Values["Settings"] != null)
            //{
            //    // Initialize the Radio button from roaming settings
            //    if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            //    {
            //        LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
            //    }
            //    else
            //    {
            //        LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
            //    }

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
        /// Change Window State method
        /// </summary>
        private void ChangeWindowState()
        {
            string visualState = DetermineVisualState(ApplicationView.Value);

            if (visualState == "Snapped")
            {
                itemGridView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                itemListView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                backButton.Style = App.Current.Resources["SnappedBackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["SnappedPageHeaderTextStyle"] as Style;
            }
            else
            {
                itemGridView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                itemListView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                backButton.Style = App.Current.Resources["BackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["PageHeaderTextStyle"] as Style;
            }
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            sessionData.isAllAut = true;
            Authors a = (Authors)e.ClickedItem;
            this.Frame.Navigate(typeof(ItemDetailPage), a.Author);
        }
    }
}
