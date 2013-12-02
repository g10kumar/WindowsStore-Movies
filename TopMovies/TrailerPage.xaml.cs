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
using System.Text;
using Windows.UI.ViewManagement;
using Microsoft.Advertising.WinRT.UI;
using DT.GoogleAnalytics.Metro;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Resources;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TopMovies
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrailerPage : TopMovies.Common.LayoutAwarePage
    {
        ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();                // This is to get the resources defined in the resource.resw file . 
        bool leftAdvisible = true;
        bool rightAdvisible = true;

        public TrailerPage()
        {
            this.InitializeComponent();
        }

        void trailerHolder_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ProgressRing.IsBusy = false;
            trailerHolder.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AnalyticsHelper.Track(Regex.Match(trailerHeader.Text, @"(?<=:).*").ToString(), "Movie_Trailer");
            trailerHolder.LoadCompleted -= trailerHolder_LoadCompleted;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            AnalyticsHelper.TrackPageView("/Trailer");
            
            ProgressRing.IsBusy = true;
            if (sessionData.selectCategory == "TopEnglish")
            {
                trailerHeader.Text = loader.GetString("trailerHeader")+" : " + sessionData.lastEnglishMovie;
            }
            else if (sessionData.selectCategory == "TopForeign")
            {
                trailerHeader.Text = loader.GetString("trailerHeader") + " : " + sessionData.lastForeignMovie;
            }
            else if (sessionData.selectCategory == "TopBollywood")
            {
                trailerHeader.Text = loader.GetString("trailerHeader") + " : " + sessionData.lastBollywoodMovie;
            }
            else if (sessionData.selectCategory == "TopAsian")
            {
                trailerHeader.Text = loader.GetString("trailerHeader") + " : " + sessionData.lastAsianMovie;
            }

            YouTubeVideoId obj = new YouTubeVideoId();
            int position = (int)e.Parameter;
            //string videoWidth = "853";
            //string videoHeight = "450";
            string videoID = await obj.getVideoId(position);

            string videoSrc = "https://www.youtube.com/embed/" + videoID + @"?autoplay=1&vq=720p&controls=1&rel=0&autohide=1&showinfo=0&theme=light&enablejsapi=1";
            StringBuilder html = new StringBuilder();
            html.Append(@"<html><head></head><body style=""padding:0; margin:0"" >");
            //html.Append(@"<iframe frameborder=""0"" scrolling=""no"" style=""padding:auto; margin:auto"" width=""" + videoWidth + @""" height=""" + videoHeight + @""" src=" + videoSrc + " allowfullscreen=" + @"""1" + @"""></iframe>");
            html.Append(@"<iframe type=""text/html"" frameborder=""0"" scrolling=""no"" style=""padding:0; margin:0"" width=""" + "853" + @""" height=""" + "450" + @""" src=" + videoSrc + "></iframe>");
            html.Append(@"</body></html>");
            trailerHolder.NavigateToString(html.ToString());
        }

       


        private void GoBack(object sender, RoutedEventArgs e)
        {
            trailerHolder.NavigateToString(@"<html><head></head><body></body></html>");
            base.GoBack(sender, e);
        }

        /// <summary>
        /// Always add an adcontroler inside a grid or stackpanel.Changing the visibility or suspending or disabling auto refresh causes more error . 
        /// Remove the ad unit from the parent grid to get lest errors. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAdError(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
            
            if(((AdControl)sender).Name.Contains("verticle") && e.ErrorCode == MicrosoftAdvertising.ErrorCode.NoAdAvailable)
            {
                if(((AdControl)sender).Name == "verticleAdLeft")
                {
                    leftAdvisible = false;
                }
                else if (((AdControl)sender).Name == "verticleAdRight")
                {
                    rightAdvisible = false;
                }

            }
            if (!leftAdvisible && !rightAdvisible && horizontalAd.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                horizontalAd.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }

        private void AdRefresh(object sender, RoutedEventArgs e)
        {

            if (((AdControl)sender).Name == "verticleAdLeft" && ((AdControl)sender).Visibility == Windows.UI.Xaml.Visibility.Visible )
            {
                leftAdvisible = true;
            }
            else if (((AdControl)sender).Name == "verticleAdRight" && ((AdControl)sender).Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                rightAdvisible = true;
            }
            if ((leftAdvisible || rightAdvisible) && horizontalAd.Visibility == Windows.UI.Xaml.Visibility.Visible && ( verticleAdLeft.Visibility == Windows.UI.Xaml.Visibility.Visible || verticleAdRight.Visibility == Windows.UI.Xaml.Visibility.Visible  ))
            {
                horizontalAd.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //horizontalTrailer.Children.Remove(horizontalAd);
            }
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            trailerHolder.NavigateToString(@"<html><head></head><body></body></html>");
            for (int i = 0; i <= 1; i++)
            {
                base.GoBack(sender, e);
            }
        }

        
    }
}
