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
using TopMovies.Views;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using TopMovies.Common;
using DT.GoogleAnalytics.Metro;
using Syncfusion.UI.Xaml.Controls.Notification;
using Syncfusion.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TopMovies
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : TopMovies.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += Current_SizeChanged;
        }


        private void manageViewState()
        {
            string visualState = DetermineVisualState(ApplicationView.Value);

            if (visualState == "Snapped")
            {
                mainStack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                snappedStack.Visibility = Windows.UI.Xaml.Visibility.Visible;
                backButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                pageTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                mainStack.Visibility = Windows.UI.Xaml.Visibility.Visible;
                snappedStack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                backButton.Style = App.Current.Resources["BackButtonStyle"] as Style;
                pageTitle.Style = App.Current.Resources["PageHeaderTextStyle"] as Style;

            }
        }

        void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            manageViewState();
        }

        protected override string DetermineVisualState(ApplicationViewState viewState)
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


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            //List<string> category = new List<string>();            
            //category.Add("TopEnglish");
            //category.Add("TopForeign");
            //category.Add("TopBollywood");
            //category.Add("TopAsian");

            //Passin the asset folder name with the number of image files inside the folder to populate the hub tile with the images . 
            TopEnglish.ImageList = GetImages("TopEnglish",185);
            TopForeign.ImageList = GetImages("TopForeign",235);
            TopBollywood.ImageList = GetImages("TopBollywood",103);
            TopAsian.ImageList = GetImages("TopAsian",112);
            

            AnalyticsHelper.TrackPageView("/MainPage");
            if (((App)(App.Current)).countryCode == "")
            {
                //Variable to get the Location of the user . This is to check if this is the first run or not . 
                ((App)(App.Current)).countryCode = new Windows.Globalization.GeographicRegion().DisplayName;             
                
            }


            //this.LoadState(e.Parameter, null);
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

        //protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        //{
        //    // Restore values stored in session state.
        //    //if (pageState != null && pageState.ContainsKey("greetingOutputText"))
        //    //{
        //    //    greetingOutput.Text = pageState["greetingOutputText"].ToString();
        //    //}

        //    // Restore values stored in app data.
        //    Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
        //    if (roamingSettings.Values.ContainsKey("lastEnglishMovieIndex"))
        //    {
        //        sessionData.lastEnglishMovieIndex = roamingSettings.Values["lastEnglishMovieIndex"].ToString();
        //    }
        //    if (roamingSettings.Values.ContainsKey("lastBollywoodMovieIndex"))
        //    {
        //        sessionData.lastEnglishMovieIndex = roamingSettings.Values["lastBollywoodMovieIndex"].ToString();
        //    }
        //    if (roamingSettings.Values.ContainsKey("lastForeignMovieIndex"))
        //    {
        //        sessionData.lastEnglishMovieIndex = roamingSettings.Values["lastForeignMovieIndex"].ToString();
        //    }
        //}

        //<summary>This function is executed on clicking the English Section </summary>
        private void btnTopEnglishMovies_Click(object sender, RoutedEventArgs e)
        {               
            sessionData.selectCategory = "TopEnglish";
            // Navigates to Movies page
            this.Frame.Navigate(typeof(Movies));
        }

        //<summary>This function is executed on clicking the International Section </summary>
        private void btnTopForeignMovies_Click(object sender, RoutedEventArgs e)
        {           

            sessionData.selectCategory = "TopForeign";
            // Navigates to Movies page
            this.Frame.Navigate(typeof(Movies));
        }

        //<summary>This function is executed on clicking the Bollywood Section </summary>
        private void btnTopBollywoodmovies_Click(object sender, RoutedEventArgs e)
        {            

            sessionData.selectCategory = "TopBollywood";
            // Navigates to Movies page
            this.Frame.Navigate(typeof(Movies));
        }

        //<summary>This function is executed on clicking the Asian Section </summary>
        private void btnTopAsianmovies_Click(object sender, RoutedEventArgs e)
        {           

            sessionData.selectCategory = "TopAsian";
            // Navigates to Movies page
            this.Frame.Navigate(typeof(Movies));

        }

        //<summary>This function populates the Hubtile element on the Main page </summary>
        private ImageList GetImages(string category,int lenght)
        {
            ImageList list = new ImageList();
            Random r = new Random();
            for (int i = 1; i < 20; i++)
            {
                int pic = r.Next(1, lenght);
                list.Add("Assets/" + category + "/" + pic + ".jpg");
            }

            return list;

            
        }
        
    }
}
