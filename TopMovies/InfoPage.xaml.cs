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
using Windows.Storage.Streams;
using System.Text.RegularExpressions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TopMovies
{
    /// <summary>
    /// This page displayes movie information from imdb . Also user has the controls to check information from Wikipedia , watch youtube trailer and buy the title. 
    /// </summary>
    public sealed partial class InfoPage : TopMovies.Common.LayoutAwarePage
    {
        string releaseYear;
        int moviePosition;
        string title;
        Tuple<string, string, string,string> derivedContent;
        public List<MovieData> information;
        MovieInformation objMovie = new MovieInformation();
        bool wikiPediaContent = false;
        string language;                                                                                // This is to get the language on the user system.
        public List<MovieData> Information
        {
            get { return information; }
            set { information = value; }
        }
        ConnectionProfile InternetconnectionProfile;
        ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();                // This is to get the resources defined in the resource.resw file . 

        private string _source;
        public string ImageSource
        {
            get { return _source; }
            set { _source = value; }
        }


        public InfoPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// Gets information from omdb api or the myapi in case if the omdb api is not reachable . And if both the api calls fail then the wikipedia article will be diaplayed. 
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AnalyticsHelper.TrackPageView("/InfoPage");

            string[] contentInfo = ((string)e.Parameter).Split(new char[]{'|'});
            moviePosition = Convert.ToInt32(contentInfo[1]);
            ImageSource = "Assets/" + sessionData.selectCategory + "/" + moviePosition + ".jpg";
            busyIndicator.IsBusy = true;
            derivedContent = await objMovie.GetMovieInfo(contentInfo[0]);
            infoHeader.Text = contentInfo[2];
            title = contentInfo[2];
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
                    tomatoVotes.Text = derivedContent.Item3.Split(new char[] { '|' })[3] + loader.GetString("userVotes");
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

            language = new Windows.ApplicationModel.Resources.Core.ResourceContext().Languages.FirstOrDefault();
            if (!language.Contains("en") && derivedContent.Item1 != null)
            {
                TraslateDetails.IsEnabled = true;
            }

            if (derivedContent.Item1 != null)
            {
                AnalyticsHelper.Track(title, "Movie_Info", derivedContent.Item1);
            }
            else
            {
                AnalyticsHelper.Track(title, "Movie_Info", "WikiPedia");
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if(!wikiPediaContent)
                wikiView.NavigateToString(@"<html><head></head><body></body></html>");
            //imageHolder2.Source = null;
            //imageHolder2.Background = null;
            base.GoBack(sender, e);
        }
        /// <summary>
        /// This application gets the article from wikipedia using the movieinformation class , and hides the other information grid. 
        /// If on navigating to the info page both the api don't return any information then the application will call this method and all other grid will be hidden. 
        /// This function is being called by the button in the radial menu as well as the onnavigation method if both the api calls fails to return any infromation. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void WikiPediaArticle(object sender, RoutedEventArgs e)
        {
            if (!wikiPediaContent)
            {                
                string url;
                if (translatedAccordion.Visibility == Windows.UI.Xaml.Visibility.Visible)
                {
                    translatedAccordion.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    accordion.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                tomatoGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                imdbGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                busyIndicator.IsBusy = true;

                if (infoHeader.Text != null)                        // We are checking the info header because the infoheader is assigned to title which is used to get the release year
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
                if (translatedAccordion.Visibility == Windows.UI.Xaml.Visibility.Visible)
                {
                    translatedAccordion.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    accordion.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
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
            if (translatedAccordion.ItemsSource == Information)
            {
                translatedAccordion.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                accordion.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            imdbGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            selectionmenu.Visibility = Windows.UI.Xaml.Visibility.Visible;
            if(tomatoRate.Value != 0)
            tomatoGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
            
        }

        /// <summary>
        /// This method is called when the buy movie button is pressed in the radial menu of the page. 
        /// It checks for the country setting of the user and then accordingly navigates to the respective amazon page. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void BuyMovie(object sender, RoutedEventArgs e)
        {


            if (!GetIntertCondition())          // Functionality to check user internet connection & prompt is connection unavaliable . 
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("NoInternet"));
                var result = messageDialog.ShowAsync();
            }
            else
            {
                AnalyticsHelper.Track("Buy_Button", "Button_click", title);

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
                case "China":
                    url = "http://www.amazon.cn/s/?_encoding=UTF8&field-keywords=" + movieName + "&linkCode=ur2&tag=daksatech-23&url=search-alias%3Dvideo";
                    break;
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
                    //Checking if youtube is reachable or not. Performed only once, then the value ((App)(App.Current)).youtubeReachable is updated to save the status.
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
                        //int position;
                        //position = Convert.ToInt32(Regex.Match(moviePosition, @"\d{1,3}").ToString());
                        if (sessionData.selectCategory == "TopEnglish")                     // This is done because the movies in the english section are not saved 
                                                                                            //according to their respective rank in the youtube playlist.
                        {
                            if (moviePosition <= 13)
                            {
                                moviePosition = moviePosition + 20;
                            }
                            else if (moviePosition > 13 && moviePosition <= 33)
                            {
                                moviePosition = moviePosition - 13;
                            }
                        }
                        this.Frame.Navigate(typeof(TrailerPage), moviePosition);

                    }
                    else
                    {
                        var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("Notrailer"));
                        var result = messageDialog.ShowAsync();
                        AnalyticsHelper.Track("YNR", "Movie_Trailer");                  //Here YNR means youtube not reachable . 
                    }
                }
                catch (WebException)
                {
                    
                }

            }
        }

        private async void Translate(object sender, RoutedEventArgs e)
        {
            if(language.Length >2)
            {
                language = language.Remove(2);
            }
            GoogleTranslator result = new GoogleTranslator("infoPage");
            List<string> translated = new List<string>();
            //translated.Add(information[accordion.SelectedIndex]);
            for (int i = 0; i <= 3; i++)
            {
                translated.Add(await result.Translator(information[i].MovieInfo, language));
            }
            translated.Add(information[4].MovieInfo);
            Information.RemoveRange(0, 5);
            Information.Add(new MovieData(loader.GetString("Header1"), translated[0]));
            Information.Add(new MovieData(loader.GetString("Header2"), translated[1]));
            Information.Add(new MovieData(loader.GetString("Header3"), translated[2]));
            Information.Add(new MovieData(loader.GetString("Header4"), translated[3]));
            Information.Add(new MovieData(loader.GetString("Header5"), translated[4]));
            accordion.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            translatedAccordion.ItemsSource = Information;
            translatedAccordion.SelectedIndex = accordion.SelectedIndex;
            translatedAccordion.Visibility = Windows.UI.Xaml.Visibility.Visible;
            AnalyticsHelper.Track("TranslateMovieInfo", "Button_click", language);

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