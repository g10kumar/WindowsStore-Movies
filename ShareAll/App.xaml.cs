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
using Facebook;

using Windows.UI.ApplicationSettings;
using ShareAll.Common;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227
namespace ShareAll
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        FacebookClient _fb = new FacebookClient();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
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
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
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
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            // Add a About command
            var about = new SettingsCommand("about", loader.GetString("About Us"), (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new AboutUsUserControl());
            });

            args.Request.ApplicationCommands.Add(about);



            // Add a Privacy Policy command
            var privacypolicy = new SettingsCommand("privacypolicy", loader.GetString("Privacy Policy"), (handler) =>
            {
                var settings = new SettingsFlyout();
                settings.ShowFlyout(new PrivacyPolicyUserControl());
            });

            args.Request.ApplicationCommands.Add(privacypolicy);

            // Add a Other Apps command
            //var otherapps = new SettingsCommand("otherapps", "Other Apps", (handler) =>
            //{
            //    var settings = new SettingsFlyout();
            //    settings.ShowFlyout(new OtherAppsUserControl());
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
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        protected override void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            var rootFrame = new Frame();
            rootFrame.Navigate(typeof(ShareIt), args.ShareOperation);
            Window.Current.Content = rootFrame;
            Window.Current.Activate();

            // eMail_200x200.png
            //var rootFrame = new Frame();
            //rootFrame.Navigate(typeof(ShareItModified), args.ShareOperation);
            //Window.Current.Content = rootFrame;
            //Window.Current.Activate();
        }


    }

    /// <summary>
    /// Declaring and initializing the sessionData Variables
    /// </summary>
    //public static class sessionData
    //{
    //    //public static bool isFBConfigure = false;
    //    //public static bool isTweetConfigure = false;
    //    public static bool isEmailConfigure = false;

    //    public static void resetValues()
    //    {
    //       // sessionData.isFBConfigure = false;
    //        //sessionData.isTweetConfigure = false;
    //        sessionData.isEmailConfigure = false;
    //    }
    //}
}
