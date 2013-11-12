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
using Windows.UI.Xaml.Media.Imaging;
using System.Xml.Linq;
using DT.GoogleAnalytics.Metro;
using Windows.Networking.Connectivity;
using Windows.ApplicationModel.Resources;
using System.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TopMovies
{
    /// <summary>
    /// This page displayes movie information from imdb . Also user has the controls to check information from Wikipedia , watch youtube trailer and buy the title. 
    /// </summary>
    public sealed partial class InfoPage : TopMovies.Common.LayoutAwarePage
    {
        string releaseYear;
        string title;
        Tuple<string, string, string,string> derivedContent;
        public List<MovieData> information;
        MovieInformation objMovie = new MovieInformation();
        bool wikiPediaContent = false;
        public List<MovieData> Information
        {
            get { return information; }
            set { information = value; }
        }

        ConnectionProfile InternetconnectionProfile;
        ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();                // This is to get the resources defined in the resource.resw file . 
        


        public InfoPage()
        {
            this.InitializeComponent();
        }



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            Tuple<int, string, string> contentInfo = e.Parameter as Tuple<int, string, string>;

            busyIndicator.IsBusy = true;
            
            derivedContent = await objMovie.GetMovieInfo(contentInfo.Item1.ToString());
            imageHolder2.Source = new BitmapImage(new Uri(contentInfo.Item2));
            infoHeader.Text = contentInfo.Item3;
            title = contentInfo.Item3;
            Information = new List<MovieData>();
            if (derivedContent.Item1 == "OMDB")
            {
                Information.Add(new MovieData(loader.GetString("Header1"), derivedContent.Item2.Split(new char[] { '|' })[3]));
                Information.Add(new MovieData(loader.GetString("Header2"), derivedContent.Item2.Split(new char[] { '|' })[2]));
                Information.Add(new MovieData(loader.GetString("Header3"), derivedContent.Item2.Split(new char[] { '|' })[0]));
                Information.Add(new MovieData(loader.GetString("Header4"), derivedContent.Item2.Split(new char[] { '|' })[1]));
                Information.Add(new MovieData(loader.GetString("Header5"), loader.GetString("Release") + ": " + derivedContent.Item4.Split(new char[] { '|' })[0] + Environment.NewLine + loader.GetString("Runtime") + ": " + derivedContent.Item4.Split(new char[] { '|' })[1] + Environment.NewLine + loader.GetString("Genre") + ": " + derivedContent.Item4.Split(new char[] { '|' })[2]));
                imdbRate.Value = Convert.ToDouble(derivedContent.Item3.Split(new char[] { '|' })[0], System.Globalization.NumberFormatInfo.InvariantInfo);
                imdbNumber.Text = derivedContent.Item3.Split(new char[] { '|' })[0] + "/10";
                imdbVotes.Text = derivedContent.Item3.Split(new char[] { '|' })[1] + loader.GetString("userVotes");
                if (derivedContent.Item3.Split(new char[] { '|' })[2] != "N/A")
                {
                    tomatoGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    tomatoRate.Value = Convert.ToDouble(derivedContent.Item3.Split(new char[] { '|' })[2], System.Globalization.NumberFormatInfo.InvariantInfo);
                    tomatoNumber.Text = derivedContent.Item3.Split(new char[] { '|' })[2] + "/5";
                    tomatoVotes.Text = derivedContent.Item3.Split(new char[] { '|' })[3] + " User Votes";
                }

                busyIndicator.IsBusy = false;
                accordion.ItemsSource = Information;
                accordion.SelectedIndex = 0;
                releaseYear = derivedContent.Item4.Split(new char[] { '|' })[0];
            }
            else if (derivedContent.Item1 == "myApi") 
            {
                Information.Add(new MovieData(loader.GetString("Header1"), derivedContent.Item2.Split(new char[] { '|' })[3]));
                Information.Add(new MovieData(loader.GetString("Header2"), derivedContent.Item2.Split(new char[] { '|' })[2]));
                Information.Add(new MovieData(loader.GetString("Header3"), derivedContent.Item2.Split(new char[] { '|' })[0]));
                Information.Add(new MovieData(loader.GetString("Header4"), derivedContent.Item2.Split(new char[] { '|' })[1]));
                Information.Add(new MovieData(loader.GetString("Header5"), loader.GetString("Release") + ": " + derivedContent.Item4.Split(new char[] { '|' })[0] + Environment.NewLine + loader.GetString("Runtime") + ": " + derivedContent.Item4.Split(new char[] { '|' })[1] + Environment.NewLine + loader.GetString("Genre") + ": " + derivedContent.Item4.Split(new char[] { '|' })[2] + Environment.NewLine + loader.GetString("Language") + ": " + derivedContent.Item4.Split(new char[] { '|' })[3]));
                imdbRate.Value = Convert.ToDouble(derivedContent.Item3.Split(new char[] { '|' })[0], System.Globalization.NumberFormatInfo.InvariantInfo);
                imdbNumber.Text = derivedContent.Item3.Split(new char[] { '|' })[0] + "/10";
                imdbVotes.Text = derivedContent.Item3.Split(new char[] { '|' })[1] + loader.GetString("userVotes");

                busyIndicator.IsBusy = false;
                accordion.ItemsSource = Information;
                accordion.SelectedIndex = 0;
                releaseYear = derivedContent.Item4.Split(new char[] { '|' })[0];
            }
            else if (derivedContent.Item1 == null)
            {
                accordion.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                WikiPediaArticle(null, null);
                wikiCloseButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }

        async private void WikiPediaArticle(object sender, RoutedEventArgs e)
        {
            if (!wikiPediaContent)
            {
                string url;
                tomatoGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                accordion.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imdbGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                busyIndicator.IsBusy = true;

                if (infoHeader.Text != null)
                {
                    if (releaseYear == null)
                    {
                        XDocument doc = XDocument.Load(@"D:\DaksaTech\Win8Apps\TopMovies\bin\Release\AppX\Xml\" + sessionData.selectCategory + ".xml");
                        var year = (from release in doc.Root.Elements("movie")
                                    where release.Element("name").Value.Equals(title)
                                    select release.Attribute("release").Value).AsParallel().FirstOrDefault();
                        url = await objMovie.WikiPediaArticleFinder(infoHeader.Text, Convert.ToInt32(year));
                    }
                    else
                    {
                        url = await objMovie.WikiPediaArticleFinder(infoHeader.Text, Convert.ToInt32(releaseYear));
                    }
                    if (url != null)
                    {
                        wikiView.LoadCompleted += wikiView_LoadCompleted;
                        wikiView.Navigate(new Uri(url));
                    }
                }
            }
            else
            {
                imdbGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                tomatoGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                accordion.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                selectionmenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                wikiGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
                 
        }

        void wikiView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            wikiGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            selectionmenu.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            busyIndicator.IsBusy = false;
            wikiPediaContent = true;
            wikiView.LoadCompleted -= wikiView_LoadCompleted;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            wikiGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            accordion.Visibility = Windows.UI.Xaml.Visibility.Visible;
            imdbGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            selectionmenu.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if(tomatoRate.Value != 0)
            tomatoGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            
        }

        async private void BuyMovie(object sender, RoutedEventArgs e)
        {


            //var movieName = "";
            //if (sessionData.selectCategory == "TopForeign")
            //{
            //    movieName = title(0, title.Text.IndexOf("/"));
            //}
            //else
            //{
            //movieName = title;
            //}

            AnalyticsHelper.Track("Buy_Button", "Button_click", title);

            //string x = ((App)(App.Current)).countryCode;                                     // Variable x to store the value of country selcted by the user . 

            //string buyMovieCountrycode = "";

            //if (countryList.ContainsKey(x))        // If the country selected is not present in the dictionary then the value of the countrycode will be null . 
            //{
            //    buyMovieCountrycode = countryList[x];

            //}

            if (!GetIntertCondition())          // Functionality to check user internet connection & prompt is connection unavaliable . 
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("NoInternet"));
                var result = messageDialog.ShowAsync();
            }
            else
            {
                string url = countryWiseUrl(((App)(App.Current)).countryCode, title);

                await Windows.System.Launcher.LaunchUriAsync(new Uri(url));

            }

        }

        private bool GetIntertCondition()
        {
            InternetconnectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            bool internetCondition = InternetconnectionProfile != null && InternetconnectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internetCondition;
        }


        private string countryWiseUrl(string country, string movieName)
        {
            string url;

            switch (country)
            {
                case "United States":
                    url = "http://www.amazon.com/s/?_encoding=UTF8&field-keywords=" + movieName + "&linkCode=ur2&tag=artmaya-20&url=search-alias%3Dmovies-tv";
                    break;
                case "India":
                    url = "http://www.flipkart.com/search/a/movies-music?fk-search=movies-music&query=" + movieName + "&vertical=movies-music&affid=dkdaksatec";
                    break;
                case "United Kingdom":
                    //url = "http://www.amazon.co.uk/s/?_encoding=UTF8&camp=1634&field-keywords=" + movieName + "&linkCode=ur2&tag=daksatech-21&url=search-alias=dvd";
                    url = "http://www.amazon.co.uk/s/?_encoding=UTF8&camp=1634&field-keywords=" + movieName + "&tag=daksatech-21&url=search-alias%3Ddvd";
                    break;
                case "Canada":
                    url = "http://www.amazon.ca/s/?_encoding=UTF8&camp=15121&field-keywords=" + movieName + "&tag=daksatech-20&url=search-alias%3Ddvd";
                    break;
                //case "AU":
                //    url = "http://www.amazon.com";
                //    break;
                //case "CN":
                //    url = "http://www.amazon.com";
                //    break;
                case "Germany":
                    url = "http://www.amazon.de/s/?url=search-alias%3Ddvd&field-keywords=" + movieName + "&tag=daksatech02-21";
                    break;
                case "Italy":
                    url = "http://www.amazon.it/s/?field-keywords=" + movieName + "&tag=daksatech086-21&url=search-alias%3Ddvd";
                    break;
                case "Spain":
                    url = "http://www.amazon.es/s/?field-keywords=" + movieName + "&tag=daksatech0d-21&url=search-alias%3Ddvd";
                    break;
                case "France":
                    url = "http://www.amazon.fr/s/?_encoding=UTF8&camp=1642&field-keywords=" + movieName + "&tag=daksatech09-21&url=search-alias%3Ddvd";
                    break;
                case "Japan":
                    url = "http://www.amazon.co.jp/s/?_encoding=UTF8&field-keywords=" + movieName + "&tag=artmaya-22&url=search-alias%3Ddvd";
                    break;
                default:
                    url = "http://www.amazon.com/s/?_encoding=UTF8&field-keywords=" + movieName + "&linkCode=ur2&tag=artmaya-20&url=search-alias%3Dmovies-tv";
                    break;
            }

            return url;

        }

        private async void ShowTrailer(object sender, RoutedEventArgs e)
        {
            if (!GetIntertCondition())
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("NoInternet"));
                var result = messageDialog.ShowAsync();
            }
            else
            {
                try
                {
                    if (!((App)(App.Current)).youtubeReachable)
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://gdata.youtube.com/demo/index.html");
                        request.ContinueTimeout = 10000;
                        request.Method = "HEAD";
                        using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                ((App)(App.Current)).youtubeReachable = true;
                            }
                        }
                    }
                    if (((App)(App.Current)).youtubeReachable)
                    {
                        int position;
                        string moviePosition = ((BitmapImage)(imageHolder2.Source)).UriSource.ToString();
                        position = Convert.ToInt32(moviePosition.Remove(moviePosition.IndexOf('.')).Replace("ms-appx:/Assets/" + sessionData.selectCategory + "/", ""));
                        if (sessionData.selectCategory == "TopEnglish")
                        {
                            if (position <= 13)
                            {
                                position = position + 20;
                            }
                            else if (position > 13 && position <= 33)
                            {
                                position = position - 13;
                            }
                        }

                        this.Frame.Navigate(typeof(TrailerPage), position);

                    }
                    else
                    {
                        var messageDialog = new Windows.UI.Popups.MessageDialog("Unable to reach Youtube from this Device.");
                        var result = messageDialog.ShowAsync();
                    }
                }
                catch (WebException)
                {
                    
                }

            }
        }
    }

    public class MovieData
    {
        public string Header { get; set; }

        public string MovieInfo { get; set; }

        public MovieData(string header, string info)
        {
            Header = header;
            MovieInfo = info;
        }

    }
}