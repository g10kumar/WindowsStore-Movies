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


using CSharpAnalytics.Protocols.Urchin;
using CSharpAnalytics.WindowsStore;


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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await AutoAnalytics.StartAsync(new UrchinConfiguration("UA-38070832-4", "http://www.daksatech.com"));  // Tracking the application using Google Analytics
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

        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Restore values stored in session state.
            //if (pageState != null && pageState.ContainsKey("greetingOutputText"))
            //{
            //    greetingOutput.Text = pageState["greetingOutputText"].ToString();
            //}

            // Restore values stored in app data.
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (roamingSettings.Values.ContainsKey("lastEnglishMovieIndex"))
            {
                sessionData.lastEnglishMovieIndex = roamingSettings.Values["lastEnglishMovieIndex"].ToString();
            }
            if (roamingSettings.Values.ContainsKey("lastBollywoodMovieIndex"))
            {
                sessionData.lastEnglishMovieIndex = roamingSettings.Values["lastBollywoodMovieIndex"].ToString();
            }
            if (roamingSettings.Values.ContainsKey("lastForeignMovieIndex"))
            {
                sessionData.lastEnglishMovieIndex = roamingSettings.Values["lastForeignMovieIndex"].ToString();
            }
        }

        private void btnTopEnglishMovies_Click(object sender, RoutedEventArgs e)
        {                   

            sessionData.selectCategory = "TopEnglish";
            // Navigates to Movies page
            this.Frame.Navigate(typeof(Movies));
        }

        private void btnTopForeignMovies_Click(object sender, RoutedEventArgs e)
        {           

            sessionData.selectCategory = "TopForeign";
            // Navigates to Movies page
            this.Frame.Navigate(typeof(Movies));
        }

        private void btnTopBollywoodmovies_Click(object sender, RoutedEventArgs e)
        {            

            sessionData.selectCategory = "TopBollywood";
            // Navigates to Movies page
            this.Frame.Navigate(typeof(Movies));
        }


        private void btnTopAsianmovies_Click(object sender, RoutedEventArgs e)
        {           

            sessionData.selectCategory = "TopAsian";
            // Navigates to Movies page
            this.Frame.Navigate(typeof(Movies));

        }
        
    }
}
