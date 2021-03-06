﻿using System;
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
using Windows.ApplicationModel.Resources;
using System.Threading.Tasks;

// Below are the reference library namespace to be used in the Application . 
using DT.GoogleAnalytics.Metro;



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
        public bool youtubeReachable = false;

        public App()
        {
            this.InitializeComponent();
            App.Current.RequestedTheme = ApplicationTheme.Light;
            this.Suspending += OnSuspending;

            UnhandledException += App_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            //DebugSettings.EnableFrameRateCounter = true;

            //DebugSettings.IsOverdrawHeatMapEnabled = true;

          // DebugSettings.IsBindingTracingEnabled = true;

        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            AnalyticsHelper.Track("TrackException", "TaskException",e.Exception.InnerException.Message);
        }

        void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            AnalyticsHelper.Track("TrackException", "UnhandledException", e.Exception.InnerException.Message);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            //var timeSpent = new AutoTimedEventActivity("ApplicationLifecycle", "User_Time_Spent");       // This is to mark the application usage time.
                                  
            Frame rootFrame = Window.Current.Content as Frame;
                    
               
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
                if (roamingSettings.Values.ContainsKey("lastEnglishMovie"))
                {
                    sessionData.lastEnglishMovie = roamingSettings.Values["lastEnglishMovie"].ToString();
                }
                if (roamingSettings.Values.ContainsKey("lastBollywoodMovie"))
                {
                    sessionData.lastBollywoodMovie = roamingSettings.Values["lastBollywoodMovie"].ToString();
                }
                if (roamingSettings.Values.ContainsKey("lastForeignMovie"))
                {
                    sessionData.lastForeignMovie = roamingSettings.Values["lastForeignMovie"].ToString();
                }
                if (roamingSettings.Values.ContainsKey("lastAsianMovie"))
                {
                    sessionData.lastAsianMovie = roamingSettings.Values["lastAsianMovie"].ToString();
                }
                if (roamingSettings.Values["userCountrySetting"] != null)
                {
                    sessionData.userCountrySetting = roamingSettings.Values["userCountrySetting"].ToString();       // Uploading the user country setting from last session.
                }

                if (sessionData.userCountrySetting != null)                                                         // Condition to check if the user has made some setting or not . 
                {
                    countryCode = sessionData.userCountrySetting.ToString();
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


            AnalyticsHelper.Setup();
            AnalyticsHelper.Track("ApplicationLifecycle", "Start");
            Window.Current.Activate();

            // Register handler for CommandsRequested events from the settings pane
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;

        }

        void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            //Add a preference apps command

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var Country_Settings = loader.GetString("region");

            SettingsCommand preference = new SettingsCommand("region_setting", Country_Settings, (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new Regionsetting());
            });

            args.Request.ApplicationCommands.Add(preference);

            var About_Us = loader.GetString("aboutus");

            // Add a About command
            var about = new SettingsCommand("about", About_Us, (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new AboutUsUserControl());
            });

            args.Request.ApplicationCommands.Add(about);

            var Privacy_Policy = loader.GetString("privacypolicy");

            // Add a Privacy Policy command
            var privacypolicy = new SettingsCommand("privacypolicy", Privacy_Policy, (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new PrivacyPolicyUserControl());
            });
            args.Request.ApplicationCommands.Add(privacypolicy);
            
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
            //sessionData.resetValues();
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            
            AnalyticsHelper.Track("ApplicationLifecycle", "Stop");          

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
        public static string lastEnglishMovie { get; set; }
        public static string lastForeignMovie { get; set; }
        public static string lastBollywoodMovie { get; set; }
        public static string lastAsianMovie { get; set; }
        public static string userCountrySetting { get; set; }
        public static int sortOrder { get; set; }
        public static string filterLang { get; set; }
        public static string filterGenere { get; set; }
        public static int genreIndex { get; set; }
        public static int asianLangIndex{get;set;}
        public static int langIndex { get; set; }

public static void resetValues()
        {
            sessionData.selectCategory = "";
            //sessionData.lastMovieIndex = "";
            sessionData.lastEnglishMovie = "";
            sessionData.lastForeignMovie = "";
            sessionData.lastBollywoodMovie = "";
            sessionData.lastAsianMovie = "";
            sessionData.userCountrySetting = "";
        }
    }
}
