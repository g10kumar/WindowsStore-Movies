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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TopMovies
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InfoPage : Page
    {
        string releaseYear;
        Tuple<string, string, string,string> derivedContent;
        public List<MovieData> information;

        public List<MovieData> Information
        {
            get { return information; }
            set { information = value; }
        }

        //public InfoPage(int movieId,string posterLocation,string movieTitle)
        //{

        //    imdbID = movieId;
        //    imageLocation = posterLocation;
        //    title = movieTitle;
        //    this.InitializeComponent();
        //}

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
            MovieInformation objMovie = new MovieInformation();
            derivedContent = await objMovie.GetMovieInfo(contentInfo.Item1.ToString());
            imageHolder2.Source = new BitmapImage(new Uri(contentInfo.Item2));
            infoHeader.Text = contentInfo.Item3;
            Information = new List<MovieData>();
            if (derivedContent.Item1 == "OMDB")
            {
                Information.Add(new MovieData ("Plot",derivedContent.Item2.Split(new char[] { '|' })[3] ));
                Information.Add(new MovieData( "Actors",derivedContent.Item2.Split(new char[] { '|' })[2] ));
                Information.Add(new MovieData("Director",derivedContent.Item2.Split(new char[] { '|' })[0] ));
                Information.Add(new MovieData("Writer",derivedContent.Item2.Split(new char[] { '|' })[1] ));
                imdbRate.Value = Convert.ToDouble(derivedContent.Item3.Split(new char[] { '|' })[0]);
                imdbNumber.Text = derivedContent.Item3.Split(new char[] { '|' })[0] + "/10";
                imdbVotes.Text = derivedContent.Item3.Split(new char[] { '|' })[1] + " User Votes";
                if (derivedContent.Item3.Split(new char[] { '|' })[2] != "N/A")
                {
                    tomatoGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    tomatoRate.Value = Convert.ToDouble(derivedContent.Item3.Split(new char[] { '|' })[2]);
                    tomatoNumber.Text = derivedContent.Item3.Split(new char[] { '|' })[2] + "/5";
                    tomatoVotes.Text = derivedContent.Item3.Split(new char[] { '|' })[3] + " User Votes";
                }
                Information.Add(new MovieData("Additional Info.","Release Year: "+derivedContent.Item4.Split(new char[]{'|'})[0]+Environment.NewLine+"Runtime: "+derivedContent.Item4.Split(new char[]{'|'})[1]+Environment.NewLine+"Genre: "+derivedContent.Item4.Split(new char[]{'|'})[2]));

                busyIndicator.IsBusy = false;

                accordion.ItemsSource = Information;
                accordion.SelectedIndex = 0;
            }
            else if (derivedContent.Item1 == "myApi") 
            {
                Information.Add(new MovieData("Plot", derivedContent.Item2.Split(new char[] { '|' })[3]));
                Information.Add(new MovieData("Actors", derivedContent.Item2.Split(new char[] { '|' })[2]));
                Information.Add(new MovieData("Director", derivedContent.Item2.Split(new char[] { '|' })[0]));
                Information.Add(new MovieData("Writer", derivedContent.Item2.Split(new char[] { '|' })[1]));
                Information.Add(new MovieData("Additional Info.", "Release Year: " + derivedContent.Item4.Split(new char[] { '|' })[0] + Environment.NewLine + "Runtime: " + derivedContent.Item4.Split(new char[] { '|' })[1] + Environment.NewLine + "Genre: " + derivedContent.Item4.Split(new char[] { '|' })[2] + Environment.NewLine + "Language: " + derivedContent.Item4.Split(new char[] { '|' })[3]));
                imdbRate.Value = Convert.ToDouble(derivedContent.Item3.Split(new char[] { '|' })[0]);
                imdbNumber.Text = derivedContent.Item3.Split(new char[] { '|' })[0] + "/10";
                imdbVotes.Text = derivedContent.Item3.Split(new char[] { '|' })[1] + " User Votes";

                busyIndicator.IsBusy = false;
                accordion.ItemsSource = Information;
                accordion.SelectedIndex = 0;

            }

        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
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