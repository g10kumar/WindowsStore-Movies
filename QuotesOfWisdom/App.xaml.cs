using QuotesOfWisdom.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
using Windows.ApplicationModel.Search;
using SQLite;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.ApplicationModel.DataTransfer;
using System.Threading;

namespace QuotesOfWisdom
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private Quotes _quotes;
        public Quotes Quotes { get { return _quotes; } }
        public DataTransferManager datatransferManager;

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }       

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            //if (_quotes == null)
            //{
            //    // Load the data before activating the search-results page
            //    _quotes = new Quotes();

            //    _quotes.QuotesLoaded += (s, e) =>
            //    {

            //    };

            //_quotes.GetCategories();
            //_quotes.GetAuthors();
            // }

            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            // Create a Frame to act as the navigation context and associate it with
            // a SuspensionManager key
            var rootFrame = new Frame();
            SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                await SuspensionManager.RestoreAsync();
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter 
                if (!rootFrame.Navigate(typeof(GroupedItemsPage), "AllGroups"))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();

            // Register handler for SuggestionsRequested events from the search pane
            SearchPane.GetForCurrentView().SuggestionsRequested += OnSuggestionsRequested;

            // Register handler for CommandsRequested events from the settings pane
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }

        void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            // Add a Settings command
            var preferences = new SettingsCommand("preferences", "Preferences", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new SettingsUserControl());
            });

            args.Request.ApplicationCommands.Add(preferences);

            // Add an About command
            var about = new SettingsCommand("about", "About", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new AboutUserControl());
            });

            args.Request.ApplicationCommands.Add(about);

            // Add an Privacy Policy command
            var privacy = new SettingsCommand("privacy", "Privacy Policy", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new PrivacyControl());
            });

            args.Request.ApplicationCommands.Add(privacy);



        }
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        void OnSuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            List<Quotations> list = new List<Quotations>();

            string query = args.QueryText.ToLower();
            string[] terms = { "Consulting", "pepper", "water", "egg", "vinegar", "flour", "rice", "oil" };

            using (SQLiteConnection db = new SQLiteConnection("thefile2.db", SQLiteOpenFlags.ReadOnly))
            {
                string Sql = "Select category as keyword from Categories union select Author as keyword from Authors";

                list = db.Query<Quotations>(Sql, "");                
                
            }

            foreach (var q in list)
            {
                if (q.keyword.ToLower().Contains(query))
                    args.Request.SearchSuggestionCollection.AppendQuerySuggestion(q.keyword.ToString());
            }

            //foreach (var term in terms)
            //{
            //    if (term.StartsWith(query))
            //        args.Request.SearchSuggestionCollection.AppendQuerySuggestion(term);
            //}
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            sessionData.searchKeyWord = args.QueryText;

            if (_quotes == null)
            {

                // Load the data before activating the search-results page
                _quotes = new Quotes();

                _quotes.QuotesLoaded += (s, e) =>
                {
                    SearchPane.GetForCurrentView().SuggestionsRequested += OnSuggestionsRequested;

                    var previousContent = Window.Current.Content;
                    var frame = previousContent as Frame;

                    if (frame == null)
                    {
                        frame = new Frame();
                        Window.Current.Content = frame;
                    }
                    
                    QuotesOfWisdom.SearchResultsPage.Activate(args.QueryText, args.PreviousExecutionState);
                    //Window.Current.Activate();
                };

            }
            else
            {
                // Activate immediately if data is available
                QuotesOfWisdom.SearchResultsPage.Activate(args.QueryText, args.PreviousExecutionState);
            }            

            ////if (args.QueryText == "")
            ////{
            ////    var rootFrame = new Frame();
            ////    rootFrame.Navigate(typeof(GroupedItemsPage), "AllGroups");
            ////    Window.Current.Content = rootFrame;
            ////    Window.Current.Activate();
            ////}
            ////else
            ////{
            ////    QuotesOfWisdom.SearchResultsPage.Activate(args.QueryText, args.PreviousExecutionState);
            ////}
        }
    }

    /// <summary>
    /// Declaring and initializing the sessionData Variables
    /// </summary>
    public static class sessionData
    {
        public static string currentCat { get; set; }
        public static string currentAuthor { get; set; }
        public static bool isFavLoaded { get; set; }
        public static bool isRandomLoaded { get; set; }
        public static string currentKeyword { get; set; }
        public static int currentQuoteIndex { get; set; }
        public static string searchWord { get; set; }
        public static string searchSen { get; set; }
        public static int quotesPerPage = 25;
        public static int currentPage = 1;
        public static List<Quotations> currentQuotes { get; set; }

        public static List<Quotations> currentServiceQuotes { get; set; }
        public static List<Quotations> currentRandomQuotes { get; set; }
        public static List<Quotations> currentFavoriteQuotes { get; set; }
        public static List<Categories> currentCategoryQuotes { get; set; }
        public static List<Categories> currentCategories { get; set; }
        public static List<Authors> currentAuthorQuotes { get; set; }
        public static List<Authors> currentAuthors { get; set; }
        public static List<Authors> currentAuthorQuoteslong { get; set; }
        public static int categoryQuotesCount { get; set; }
        public static int authorQuotesCount { get; set; }
        public static bool comingFromFav = false;
        public static bool isCancel = false;
        public static bool ramdom = false;
        public static bool isSearch = false;
        public static string colorValue { get; set; }
        public static string whichVersion { get; set; }
        public static bool isRandomQuoteAutomatic { get; set; }
        public static string title { get; set; }
        public static string subTitle { get; set; }
        public static bool firstTime = false;
        public static string currentTitle { get; set; }
        public static bool isAllCat = false;
        public static bool isAllAut = false;
        public static bool isSearchCat = false;
        public static bool isSearchAut = false;
        public static string searchKeyWord { get; set; }
        public static bool isSearchClicked = false;
        public static bool isBackgroundChanged = false;
        public static int curPageCount { get; set; }
        public static int noOfImages { get; set; }
        public static int totalImagesCount { get; set; }
        public static string genericURL { get; set; }
        public static CancellationTokenSource tokenSource { get; set; }
        public static List<QuotesOfWisdom.BGImages> currentBackgroundImages { get; set; }

        public static void resetValues()
        {
            sessionData.currentAuthor = "";
            sessionData.currentCat = "";
            sessionData.isFavLoaded = false;
            sessionData.isRandomLoaded = false;
            sessionData.currentQuotes = null;
            sessionData.currentRandomQuotes = null;
            sessionData.currentFavoriteQuotes = null;
            sessionData.currentServiceQuotes = null;
            sessionData.currentCategoryQuotes = null;
            sessionData.currentAuthors = null;
            sessionData.currentCategories = null;
            sessionData.currentAuthorQuotes = null;
            sessionData.currentAuthorQuoteslong = null;
            sessionData.currentQuoteIndex = 0;
            sessionData.currentPage = 1;
            sessionData.comingFromFav = false;
            sessionData.isCancel = false;
            sessionData.ramdom = false;
            sessionData.searchWord = "";
            sessionData.searchSen = "";
            sessionData.whichVersion = "";
            sessionData.categoryQuotesCount = 0;
            sessionData.authorQuotesCount = 0;
            sessionData.isSearch = false;
            sessionData.colorValue = "";
            sessionData.isRandomQuoteAutomatic = false;
            sessionData.title = "";
            sessionData.subTitle = "";
            sessionData.currentTitle = "";
            sessionData.firstTime = false;
            sessionData.isAllCat = false;
            sessionData.isAllAut = false;
            sessionData.isSearchCat = false;
            sessionData.isSearchAut = false;
            sessionData.searchKeyWord = "";
            sessionData.isSearchClicked = false;
            sessionData.curPageCount = 0;
            sessionData.noOfImages = 12;
            sessionData.totalImagesCount = 0;
            sessionData.genericURL = "";
            sessionData.tokenSource = null;
            sessionData.currentBackgroundImages = null;
            sessionData.isBackgroundChanged = false;

        }
    }
    /// <summary>
    /// CategoryDetailswithImage class definition
    /// </summary>
    public class Categories
    {
        public string Image { get; set; }
        public string category { get; set; }
        public int ct { get; set; }
    }

    /// <summary>
    /// AuthorDetails class definition
    /// </summary>
    public class Authors
    {
        public string Image { get; set; }
        public string Author { get; set; }
        public int ct { get; set; }

    }
}


