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

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace QuotesOfWisdom
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class AllCategories : QuotesOfWisdom.Common.LayoutAwarePage
    {
        public AllCategories()
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
            List<Categories> listCategory = new List<Categories>();

            if (sessionData.currentCategories == null)
            {
                Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");
                var sf = await storageFolder.GetFileAsync(@"Category.xml");
                var file = await sf.OpenAsync(FileAccessMode.Read);
                Stream inStream = file.AsStreamForRead();
                XElement xe = XElement.Load(inStream);

                var catGroups = (from category in xe.Elements("cat")
                                 orderby category.Value
                                 select new Categories
                                 {
                                     category = category.Value,
                                     ct = (int)category.Attribute("count")
                                 });
                var cGroups = catGroups.ToList();

                //loop the author details and binds to authorlist
                foreach (Categories c in cGroups)
                {
                    listCategory.Add(c);
                }

                //sessionData.currentCategoryQuotes = listCategory;
                sessionData.currentCategories = listCategory;
            }
            else
            {
                //listCategory = sessionData.currentCategoryQuotes.ToList();
                listCategory = sessionData.currentCategories.ToList();
            }
            itemGridView.ItemsSource = listCategory;
            itemListView.ItemsSource = listCategory;
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
            sessionData.isAllCat = true;
            Categories c = (Categories)e.ClickedItem;
            this.Frame.Navigate(typeof(ItemDetailPage), c.category);
        }
    }
}
