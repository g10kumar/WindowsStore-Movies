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
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace Marketing2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplash
    {
        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.

        private SplashScreen splash; // Variable to hold the splash screen object.

        //internal Frame rootFrame;

        FeedDataSource feedDataSource = new FeedDataSource();
        List<FeedData> lstfeedDataSource = new List<FeedData>();
        //private XmlSerializer serializer;

        public ExtendedSplash()
        {
            this.InitializeComponent();

            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This is important to ensure that the extended splash screen is formatted properly in response to snapping, unsnapping, rotation, etc...
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);
        }

        void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;
        }

        void ExtendedSplash_OnResize(Object sender, WindowSizeChangedEventArgs e)
        {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (splash != null)
            {
                // Update the coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (connectionProfile != null)
            {
                try
                {
                    lstfeedDataSource = await LocalStorage.RestoreAsync<FeedData>();
                }
                catch { }

                if (lstfeedDataSource == null)
                {
                    await feedDataSource.GetFeedsAsync();
                    lstfeedDataSource = feedDataSource.Feeds.ToList();

                }

            }
            else
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("An internet connection is needed to download feeds. Please check your connection and restart the app.");
                var result = messageDialog.ShowAsync();
            }

            // Retrieve splash screen object
            splash = (SplashScreen)(e.Parameter);

            if (splash != null)
            {
                // Register an event handler to be executed when the splash screen has been dismissed.
                splash.Dismissed += new TypedEventHandler<SplashScreen, object>(DismissedEventHandler);

                
                if (lstfeedDataSource != null)
                {
                    await LocalStorage.SaveAsync<FeedData>(lstfeedDataSource);
                    sessionData.currentFeeds = lstfeedDataSource.ToList();
                }

                Frame frame = new Frame();
                frame.Navigate(typeof(ItemsPage));
                Window.Current.Content = frame;
                // Ensure the current window is active
                Window.Current.Activate();

                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
                
            }
        }

        // Include code to be executed when the system has transitioned from the splash screen to the extended splash screen (application's first view).
        void DismissedEventHandler(SplashScreen sender, object e)
        {
            if (lstfeedDataSource != null && lstfeedDataSource.Count != 0)
            {
                dismissed = true;                
            }
            // rootFrame.Navigate(typeof(ItemsPage), this);
            // Navigate away from the app's extended splash screen after completing setup operations here...
            // This sample navigates away from the extended splash screen when the "Learn More" button is clicked.
        }
    }
}
