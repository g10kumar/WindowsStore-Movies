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
using System.Xml;
using System.Text;
using System.Xml.Linq;
using Windows.Data.Xml;

using Windows.UI.ApplicationSettings;
// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227
using LoveQuotes.Common;


namespace LoveQuotes
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public const string AppName = "1,000 Love Quotes";        

        //public const DispatcherTimer dtautoPlay = new DispatcherTimer();
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
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application               
            }

            // Create a Frame to act navigation context and navigate to the first page
            //var rootFrame = new Frame();
            var rootFrame = CreateApplicationFrame();
            rootFrame.Navigate(typeof(BlankPage));
            //rootFrame.Navigate(typeof(SettingsSample));

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();

            // Register handler for CommandsRequested events from the settings pane
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
            
        }

        private static Frame CreateApplicationFrame()
        {
            var rootFrame = new Frame();
            rootFrame.Navigated += rootFrame_Navigated;
            return rootFrame;
        }

        static void rootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri == null)
            {
                if (e.SourcePageType != null)
                {
                    string uri = "/" + e.SourcePageType.FullName;
                    //AnalyticsHelper.TrackPageView(uri);
                }
            }
            else
            {
               // AnalyticsHelper.TrackPageView(e.Uri.ToString());
            }
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


        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        void OnSuspending(object sender, SuspendingEventArgs e)
        {
          

            //TODO: Save application state and stop any background activity
        }
    }

    /// <summary>
    /// Declaring and initializing the sessionData Variables
    /// </summary>
    public static class sessionQuotes
    {
        public static List<Quotation> currentQuotes { get; set; }
        public static List<Quotation> currentFavsQuotes { get; set; }
        public static int currentQuoteIndex { get; set; }
        public static bool isFavLoaded { get; set; }
        public static int currentFavsQuoteIndex { get; set; }
        public static int quotesPerPage = 25;
        public static int currentPage = 1;
        public static string colorValue { get; set; }
       
        public static void resetValues()
        {
            sessionQuotes.isFavLoaded = false;
            sessionQuotes.currentQuotes = null;
            sessionQuotes.currentFavsQuotes = null;
            sessionQuotes.currentQuoteIndex = 0;
            sessionQuotes.currentFavsQuoteIndex = 0;
            sessionQuotes.colorValue = "";
           

        }
    }

    /// <summary>
    /// Quotation Class definition
    /// </summary>
    public class Quotation
    {
        public string QuoteId { get; set; }
        public string Quote { get; set; }
        public string Author { get; set; }

    }
}
