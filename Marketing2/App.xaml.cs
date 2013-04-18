using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ApplicationSettings;
using Marketing2.Common;
using System.Collections.Generic;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Marketing2
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
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                Marketing2.Common.SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                FeedDataSource feedDataSource = (FeedDataSource)App.Current.Resources["feedDataSource"];
                //DateTime lastRefreshedTime = (DateTime)App.Current.Resources["lastRefreshedTime"];

                //var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                //if (connectionProfile != null)
                //{
                //    FeedDataSource feedDataSource = (FeedDataSource)App.Current.Resources["feedDataSource"];
                //    if (feedDataSource != null)
                //    {
                //        if (feedDataSource.Feeds.Count == 0)
                //        {
                //            await feedDataSource.GetFeedsAsync();
                //        }
                //    }
                //}
                //else
                //{
                //    var messageDialog = new Windows.UI.Popups.MessageDialog("An internet connection is needed to download feeds. Please check your connection and restart the app.");
                //    var result = messageDialog.ShowAsync();
                //}

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                    await Marketing2.Common.SuspensionManager.RestoreAsync();
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                if (!rootFrame.Navigate(typeof(ItemsPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }

                //Frame frame = new Frame();
                //frame.Navigate(typeof(ExtendedSplash), args.SplashScreen);
                //Window.Current.Content = frame;
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
                settings.ShowFlyout(new AboutUs());
            });

            args.Request.ApplicationCommands.Add(about);

            // Add a Privacy Policy command
            var privacypolicy = new SettingsCommand("privacypolicy", "Privacy Policy", (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new PrivacyControl());
            });

            args.Request.ApplicationCommands.Add(privacypolicy);

            // Add a Other Apps command
            //var otherapps = new SettingsCommand("otherapps", "Other Apps by DaksaTech", (handler) =>
            //{
            //    var settings = new SettingsFlyout();
            //    settings.ShowFlyout(new OtherApps());
            //});

            //args.Request.ApplicationCommands.Add(otherapps);

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
            //TODO: Save application state and stop any background activity
            await Marketing2.Common.SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }


    /// <summary>
    /// Declaring and initializing the sessionData Variables
    /// </summary>
    public static class sessionData
    {
        public static List<FeedData> currentFeeds { get; set; }
        public static void resetValues()
        {
            sessionData.currentFeeds = null;            
        }
    }
}