using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using QuotesOfWisdom.Data;
using SQLite;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using QuotesOfWisdom.Common;
//using System.IO;
//using Windows.Foundation.Collections;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;
//using System.Xml.Linq;
//using QuotesOfWisdom.ServiceReference1;
//using QuotesOfWisdom.Common;
//using Windows.UI.Xaml.Media.Imaging;

namespace QuotesOfWisdom
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchResultsPage : QuotesOfWisdom.Common.LayoutAwarePage
    {

        #region Objects
        private UIElement _previousContent;
        private ApplicationExecutionState _previousExecutionState;
        List<Authors> listAuthor = new List<Authors>();
        List<Categories> listCategory = new List<Categories>();
        List<Quotations> Quoteslist = new List<Quotations>();
        string selectedAuthor = "";
        string selectedCat = "";
        public DataTransferManager datatransferManager;
        Quotes quote = new Quotes();
        #endregion

        public SearchResultsPage()
        {
            this.InitializeComponent();
            sessionData.isBackgroundChanged = true;
            //ShareSourceLoad();
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
            e.Request.Data.Properties.Title = "Quotes of Wisdom on Windows 8";
            e.Request.Data.Properties.Description = "Quotes of Wisdom ";
            e.Request.Data.SetText("Great Quotations app on Windows 8 - check out Quotes of Wisdom");
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
            //     * */
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
        /// Determines how best to support navigation back to the previous application state.
        /// </summary>
        public static void Activate(String queryText, ApplicationExecutionState previousExecutionState)
        {
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            if (frame != null)
            {
                // If the app is already running and uses top-level frame navigation we can just
                // navigate to the search results
                frame.Navigate(typeof(SearchResultsPage), queryText);
            }
            else
            {
                // Otherwise bypass navigation and provide the tools needed to emulate the back stack
                SearchResultsPage page = new SearchResultsPage();
                page._previousContent = previousContent;
                page._previousExecutionState = previousExecutionState;
                page.LoadState(queryText, null);
                Window.Current.Content = page;
            }

            // Either way, active the window
            Window.Current.Activate();
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
            var queryText = navigationParameter as String;

            // TODO: Application-specific searching logic.  The search process is responsible for
            //       creating a list of user-selectable result categories:
            //
            //       filterList.Add(new Filter("<filter name>", <result count>));
            //
            //       Only the first filter, typically "All", should pass true as a third argument in
            //       order to start in an active state.  Results for the active filter are provided
            //       in Filter_SelectionChanged below.

            var filterList = new List<Filter>();

            //filterList.Add(new Filter("All", 0, true));

            // Communicate results through the view model
            this.DefaultViewModel["QueryText"] = '\u201c' + queryText.Replace("&amp;", "&") + '\u201d';
            this.DefaultViewModel["CanGoBack"] = this._previousContent != null;

            #region Authors Search Locally
            LoadAuthors(queryText);
            if (sessionData.currentAuthorQuotes != null)
            {
                filterList.Add(new Filter("Author", sessionData.currentAuthorQuotes.Count(), true));
            }
            #endregion

            #region Cateogry Search Locally
            LoadCategories(queryText);
            if (sessionData.currentCategoryQuotes != null)
            {
                filterList.Add(new Filter("Category", sessionData.currentCategoryQuotes.Count()));
            }
            #endregion

            #region Quotes Search using WCF service
            //LoadQuotes(queryText);
            //if (sessionData.currentQuotes != null)
            //{
            //    filterList.Add(new Filter("Quotes", sessionData.currentQuotes.Count()));
            //}
            #endregion

            this.DefaultViewModel["Filters"] = filterList;
            this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;
        }

        /// <summary>
        /// Method for loading Authors
        /// </summary>
        private void LoadAuthors(string queryText)
        {
            using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
            {
                var query = (from q in db.Table<Authors>()
                             where q.Author.Contains(queryText) == true
                             select new Authors
                             {
                                 Author = q.Author.Replace("&amp;", "&"),
                                 ct = (int)q.ct
                             });

                //loop the author details and binds to authorlist
                foreach (Authors a in query)
                {
                    listAuthor.Add(a);
                }

                //quote.getSearchAuthors(queryText);
            }
            //order by Quote Count
            listAuthor = listAuthor.OrderByDescending(s => s.ct).ToList();

            // binds the authorlist to sessionData
            sessionData.currentAuthorQuotes = listAuthor;
        }

        /// <summary>
        /// Method for loading Categories
        /// </summary>
        private void LoadCategories(string queryText)
        {
            using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
            {
                var query = (from q in db.Table<Categories>()
                             where q.category.Contains(queryText) == true
                             select new Categories
                             {
                                 category = q.category.Replace("&amp;", "&"),
                                 ct = (int)q.ct
                             });

                foreach (Categories q in query)
                {
                    listCategory.Add(q);                    
                }
                
                //quote.getSearchCategtories(queryText);
            }

            // binds the categorylist to sessionData
            sessionData.currentCategoryQuotes = listCategory;
        }        

        /// <summary>
        /// Method for loading Quotes based on Keyword search
        /// </summary>
        /// <param name="keyword"></param>
        private void LoadQuotes(string keyword)
        {
            using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
            {
                var query = (from q in db.Table<Quotations>()
                             where (q.Cat.Contains(keyword) == true || q.Author.Contains(keyword) == true || q.Quote.Contains(keyword) == true)
                             select q).Take(50);

                Quoteslist = query.ToList();
            }
            sessionData.currentQuotes = Quoteslist;

        }

        /// <summary>
        /// Invoked when the back button is pressed.
        /// </summary>
        /// <param name="sender">The Button instance representing the back button.</param>
        /// <param name="e">Event data describing how the button was clicked.</param>
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            // Return the application to the state it was in before search results were requested
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else if (this._previousContent != null)
            {
                Window.Current.Content = this._previousContent;
            }
            else
            {
                // TODO: invoke the app's normal launch behavior, using this._previousExecutionState
                //       as appropriate.  Exact details can vary from app to app, which is why an
                //       implementation isn't included in the Search Contract template.  Typically
                //       this method and OnLaunched in App.xaml.cs can call a common method.
            }
        }

        /// <summary>
        /// Invoked when a filter is selected using the ComboBox in snapped view state.
        /// </summary>
        /// <param name="sender">The ComboBox instance.</param>
        /// <param name="e">Event data describing how the selected filter was changed.</param>
        void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as Filter;
            if (selectedFilter != null)
            {
                // Mirror the results into the corresponding Filter object to allow the
                // RadioButton representation used when not snapped to reflect the change
                selectedFilter.Active = true;

                // TODO: Respond to the change in active filter by setting this.DefaultViewModel["Results"]
                //       to a collection of items with bindable Image, Title, Subtitle, and Description properties

                if (selectedFilter.Name == "Author")
                {
                    //if (selectedFilter.Count != 0)
                    //{
                    listCats.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    //listQuotes.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    listAuthors.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    listAuthors.ItemsSource = sessionData.currentAuthorQuotes.ToList();
                    //}
                }
                if (selectedFilter.Name == "Category")
                {
                    //if (selectedFilter.Count != 0)
                    //{
                    listAuthors.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    //listQuotes.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    listCats.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    listCats.ItemsSource = sessionData.currentCategoryQuotes.ToList();
                    //}
                }

                if (selectedFilter.Name == "Quotes")
                {
                    //if (selectedFilter.Count != 0)
                    //{
                    listAuthors.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    listCats.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    //listQuotes.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    //listQuotes.ItemsSource = sessionData.currentQuotes.ToList();
                    //}
                }

                // Ensure results are found
                object results;
                ICollection resultsCollection;
                if (this.DefaultViewModel.TryGetValue("Results", out results) &&
                    (resultsCollection = results as ICollection) != null &&
                    resultsCollection.Count != 0)
                {
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    return;
                }
            }
            else
            {
                // Display informational text when there are no search results.
                VisualStateManager.GoToState(this, "NoResultsFound", true);
            }
        }

        /// <summary>
        /// Invoked when a filter is selected using a RadioButton when not snapped.
        /// </summary>
        /// <param name="sender">The selected RadioButton instance.</param>
        /// <param name="e">Event data describing how the RadioButton was selected.</param>
        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
            }
        }

        /// <summary>
        /// SelectionChanged event of the Authors list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listAuthors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sessionData.isSearch = true;

            // Checking for SelectedIndex of listAuthors listbox
            if (listAuthors.SelectedIndex == -1)
                return;

            // gets the selected author from the authors list box
            selectedAuthor = ((Authors)(listAuthors.Items[listAuthors.SelectedIndex])).Author;

            sessionData.searchWord = selectedAuthor;
            //datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = selectedAuthor;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);

            // Reset selected index to -1 (no selection)
            listAuthors.SelectedIndex = -1;

        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void AuthorItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            sessionData.isSearch = true;

            sessionData.isSearchAut = true;

            // gets the selected author from the authors list box
            selectedAuthor = ((Authors)(e.ClickedItem)).Author;

            sessionData.searchWord = selectedAuthor;
            //datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = selectedAuthor;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }

        /// <summary>
        /// SelectionChanged event of the Category list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listCats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            sessionData.isSearch = true;

            // Checking for SelectedIndex of listCats listbox
            if (listCats.SelectedIndex == -1)
                return;

            // gets the selected category from the categories list box
            selectedCat = ((Categories)(listCats.Items[listCats.SelectedIndex])).category;

            sessionData.searchWord = selectedCat;

            //datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = selectedCat;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);

            // Reset selected index to -1 (no selection)
            listCats.SelectedIndex = -1;

        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        async void CategoryItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            string error = "";

            try
            {
                sessionData.isSearch = true;

                // gets the selected category from the categories list box
                selectedCat = ((Categories)(e.ClickedItem)).category;

                sessionData.searchWord = selectedCat;

                sessionData.isSearchCat = true;

                //datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);

                // Navigate to the appropriate destination page, configuring the new page
                // by passing required information as a navigation parameter
                var itemId = selectedCat;
                this.Frame.Navigate(typeof(ItemDetailPage), itemId);
            }
            catch
            {
                error = "Oops!";                
            }

            if (error != "")
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Oops!");
                await dialog.ShowAsync();
            }
        }


        /// <summary>
        /// View model describing one of the filters available for viewing search results.
        /// </summary>
        private sealed class Filter : QuotesOfWisdom.Common.BindableBase
        {
            private String _name;
            private int _count;
            private bool _active;

            public Filter(String name, int count, bool active = false)
            {
                this.Name = name;
                this.Count = count;
                this.Active = active;
            }

            public override String ToString()
            {
                return Description;
            }

            public String Name
            {
                get { return _name; }
                set { if (this.SetProperty(ref _name, value)) this.OnPropertyChanged("Description"); }
            }

            public int Count
            {
                get { return _count; }
                set { if (this.SetProperty(ref _count, value)) this.OnPropertyChanged("Description"); }
            }

            public bool Active
            {
                get { return _active; }
                set { this.SetProperty(ref _active, value); }
            }

            public String Description
            {
                get { return String.Format("{0} ({1})", _name, _count); }
            }
        }
    }
}
