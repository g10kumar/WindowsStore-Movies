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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TopMovies
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrailerPage : TopMovies.Common.LayoutAwarePage
    {
        public TrailerPage()
        {
            this.InitializeComponent();
        }

        void trailerHolder_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ProgressRing.IsBusy = false;
            trailerHolder.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AnalyticsHelper.Track(trailerHeader.Text, "Movie_Trailer");
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
                trailerHeader.Text = "You are watching : " + sessionData.lastEnglishMovie;
            }
            else if (sessionData.selectCategory == "TopForeign")
            {
                trailerHeader.Text = "You are watching : " + sessionData.lastForeignMovie;
            }
            else if (sessionData.selectCategory == "TopBollywood")
            {
                trailerHeader.Text = "You are watching : " + sessionData.lastBollywoodMovie;
            }
            else if (sessionData.selectCategory == "TopAsian")
            {
                trailerHeader.Text = "You are watching : " + sessionData.lastAsianMovie;
            }

            YouTubeVideoId obj = new YouTubeVideoId();
            int position = (int)e.Parameter;
            string videoWidth = "853";
            string videoHeight = "450";
            string videoID = await obj.getVideoId(position);

            string videoSrc = "https://www.youtube.com/embed/" + videoID + @"?autoplay=1&vq=720p&controls=1&rel=0&autohide=1&showinfo=0&theme=light&enablejsapi=1";
            StringBuilder html = new StringBuilder();
            html.Append(@"<html><head></head><body style=""padding:0; margin:0"" >");
            //html.Append(@"<iframe frameborder=""0"" scrolling=""no"" style=""padding:auto; margin:auto"" width=""" + videoWidth + @""" height=""" + videoHeight + @""" src=" + videoSrc + " allowfullscreen=" + @"""1" + @"""></iframe>");
            html.Append(@"<iframe type=""text/html"" frameborder=""0"" scrolling=""no"" style=""padding:0; margin:0"" width=""" + videoWidth + @""" height=""" + videoHeight + @""" src=" + videoSrc + "></iframe>");
            html.Append(@"</body></html>");
            trailerHolder.NavigateToString(html.ToString());
        }

       


        private void GoBack(object sender, RoutedEventArgs e)
        {
            trailerHolder.LoadCompleted -= trailerHolder_LoadCompleted;
            trailerHolder.NavigateToString(@"<html><head></head><body></body></html>");
            this.Frame.GoBack();
        }

        private void OnAdError(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
            //Always add an adcontroler inside a grid or stackpanel.

            string code = e.ErrorCode.ToString();
            //adGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ((AdControl)sender).Suspend();
            ((AdControl)sender).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        
    }
}
