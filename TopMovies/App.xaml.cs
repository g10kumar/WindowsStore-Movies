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
using Windows.UI.ApplicationSettings;
using TopMovies.Common;
using Windows.Globalization;
using Windows.UI.Xaml.Media.Animation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace TopMovies
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public string countryCode = ""; // Global variable that is going to store the user selection for country .The same variable is going to be stored in the session.

        public App()
        {
            this.InitializeComponent();
            App.Current.RequestedTheme = ApplicationTheme.Light;
            this.Suspending += OnSuspending;

        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;
           
            //Variable to get the Location of the user . This is to check if this is the first run or not . 

            var geoGraphicRegion = new Windows.Globalization.GeographicRegion();

            var _countryCode = geoGraphicRegion.CodeTwoLetter;

            //if (countryCode == null). On the very first run the value of country code is going to be null , & in case the value of countryCode is unable to load from the lase session . 
                  
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //TopMovies.Common.SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                    //await TopMovies.Common.SuspensionManager.RestoreAsync();


                }

                Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                if (roamingSettings.Values.ContainsKey("lastEnglishMovieIndex"))
                {
                    sessionData.lastEnglishMovieIndex = roamingSettings.Values["lastEnglishMovieIndex"].ToString();
                }
                if (roamingSettings.Values.ContainsKey("lastBollywoodMovieIndex"))
                {
                    sessionData.lastBollywoodMovieIndex = roamingSettings.Values["lastBollywoodMovieIndex"].ToString();
                }
                if (roamingSettings.Values.ContainsKey("lastForeignMovieIndex"))
                {
                    sessionData.lastForeignMovieIndex = roamingSettings.Values["lastForeignMovieIndex"].ToString();
                }
                if (roamingSettings.Values.ContainsKey("lastAsianMovieIndex"))
                {
                    sessionData.lastAsianMovieIndex = roamingSettings.Values["lastAsianMovieIndex"].ToString();
                }
                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();

            // Register handler for CommandsRequested events from the settings pane
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }

        void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {

            // Add a About command
            var about = new SettingsCommand("about", "About Us", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new AboutUsUserControl());
            });

            args.Request.ApplicationCommands.Add(about);



            // Add a Privacy Policy command
            var privacypolicy = new SettingsCommand("privacypolicy", "Privacy Policy", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new PrivacyPolicyUserControl());
            });

            args.Request.ApplicationCommands.Add(privacypolicy);

            // Add a More Apps command
            var moreapps = new SettingsCommand("moreapps", "More Apps", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new MoreAppsUserControl());
            });

            args.Request.ApplicationCommands.Add(moreapps);

            //Add a preference apps command
            SettingsCommand preference = new SettingsCommand("region_setting", "Country setting", (handler) =>
                {

                    Popup popup = BuildSettingsItem(new Regionsetting(), 300);
                    popup.IsOpen = true;
                });

            args.Request.ApplicationCommands.Add(preference);
        }



        private Popup BuildSettingsItem(UserControl u, int w )
        {

            Popup p = new Popup();
            p.IsLightDismissEnabled = true;
            p.ChildTransitions = new TransitionCollection();
            p.ChildTransitions.Add(new PaneThemeTransition()

            {
                Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right) ?
                        EdgeTransitionLocation.Right :
                        EdgeTransitionLocation.Left

            });
            
            u.Width = w;
            u.Height = Window.Current.Bounds.Height;
            p.Child = u;
            p.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (Window.Current.Bounds.Width - w) : 0);
            p.SetValue(Canvas.TopProperty, 0);
            return p;

        }


        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity


            deferral.Complete();
        }



    }

    /// <summary>
    /// Declaring and initializing the sessionData Variables
    /// </summary>
    public static class sessionData 
    {
        public static string selectCategory { get; set; }
        //public static string lastMovieIndex { get; set; }
        public static string lastEnglishMovieIndex { get; set; }
        public static string lastForeignMovieIndex { get; set; }
        public static string lastBollywoodMovieIndex { get; set; }
        public static string lastAsianMovieIndex { get; set; }

        

 
        
        
public static void resetValues()
        {
            sessionData.selectCategory = "";
            //sessionData.lastMovieIndex = "";
            sessionData.lastEnglishMovieIndex = "";
            sessionData.lastForeignMovieIndex = "";
            sessionData.lastBollywoodMovieIndex = "";
            sessionData.lastAsianMovieIndex = "";

        }
    }
}
