using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Networking.Connectivity;
using TopMovies.Common;
//using Common;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using DT.GoogleAnalytics.Metro;
using System.Diagnostics;

namespace TopMovies.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Movies 
    {        
        public DataTransferManager datatransferManager;
        Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
        //StorageFile movieFile = null;
        //string fileContent = "";
        Dictionary<string, string> countryList = new Dictionary<string, string>();                      // Initialize a dictionaty to store the country list . Values added in the 
        // constructor
        int countofMovies;
        string language;                                                                                // This is to get the language on the user system.
        CancellationTokenSource ts = new CancellationTokenSource();
        bool autoPlayOn = false;
        ConnectionProfile InternetconnectionProfile;
        ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();                // This is to get the resources defined in the resource.resw file . 
        Tuple<bool, ObservableCollection<Person>,int,int> returened;



        private ObservableCollection<Person> images;

        public ObservableCollection<Person> Images
        {
            get { return images; }
            set { images = value; }
        }

        public Movies() 
        {
            
            this.InitializeComponent();
            DataContext = new CoverFlowProperties();
            ShareSourceLoad();
            manageViewState();
            Window.Current.SizeChanged += Current_SizeChanged;
            countryList.Add("India", "IN");                             // Adding key & value to the countryList dictionary . 
            countryList.Add("United Kingdom", "GB");
            countryList.Add("United States", "US");
            //countryList.Add("Australia","AU");
            countryList.Add("Canada", "CA");
            //countryList.Add("China","CN");
            countryList.Add("Germany", "DE");
            countryList.Add("Italy", "IT");
            countryList.Add("Spain", "ES");
            countryList.Add("France", "FR");
            countryList.Add("Japan", "JP");
            genere.SelectedIndex = 0;
            sorter.SelectedIndex = 0;            
            sorter.SelectionChanged += sorter_SelectionChanged;
            genere.SelectionChanged += genere_SelectionChanged;
            if (sessionData.selectCategory == "TopAsian")
            {
                filterlang_asian.SelectedIndex = 0;
                filterlang_asian.SelectionChanged += filterlang_asian_SelectionChanged;
            }
            else if (sessionData.selectCategory == "TopForeign")
            {
                filterlang.SelectedIndex = 0;
                filterlang.SelectionChanged += filterlang_SelectionChanged;
            }
            sessionData.sortOrder = 0;
            sessionData.filterGenere = null;
            sessionData.filterLang = null;
           



        }

        async void genere_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ProgressRing.IsActive = true;

            if (genere.SelectedIndex == 0)
            {
                sessionData.filterGenere = null;
                CarouselImagePopulator obj1 = new CarouselImagePopulator();

                returened = await obj1.LoadMovieData();
            }
            else
            {
                sessionData.filterGenere = genere.SelectedItem.ToString();
                CarouselImagePopulator obj1 = new CarouselImagePopulator();

                returened = await obj1.LoadMovieData();
            }



            CoverFlowControl.ItemsSource = returened.Item2;                     //Getting the images from the class
            countofMovies = returened.Item3;                                    //Getting the count of the movies 
            CoverFlowControl.SelectedIndex = returened.Item4;                   // Gettig the carousel selcted item . 

            if (returened.Item1)
            {
                ProgressRing.IsActive = false;
            }

            
        }

        async void filterlang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProgressRing.IsActive = true;

            if (filterlang.SelectedIndex == 0)
            {
                sessionData.filterLang = null;
                CarouselImagePopulator obj1 = new CarouselImagePopulator();

                returened = await obj1.LoadMovieData();
            }
            else
            {
                sessionData.filterLang = filterlang.SelectedItem.ToString();
                CarouselImagePopulator obj1 = new CarouselImagePopulator();

                returened = await obj1.LoadMovieData();
            }



            CoverFlowControl.ItemsSource = returened.Item2;                     //Getting the images from the class
            countofMovies = returened.Item3;                                    //Getting the count of the movies 
            CoverFlowControl.SelectedIndex = returened.Item4;                   // Gettig the carousel selcted item . 

            if (returened.Item1)
            {
                ProgressRing.IsActive = false;
            }

        }

        async void filterlang_asian_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ProgressRing.IsActive = true;

            if (filterlang_asian.SelectedIndex == 0)
            {
                sessionData.filterLang = null;
                CarouselImagePopulator obj1 = new CarouselImagePopulator();

                returened = await obj1.LoadMovieData();
            }
            else
            {
                sessionData.filterLang = filterlang_asian.SelectedItem.ToString();
                CarouselImagePopulator obj1 = new CarouselImagePopulator();

                returened = await obj1.LoadMovieData();
            }
           
            

            CoverFlowControl.ItemsSource = returened.Item2;                     //Getting the images from the class
            countofMovies = returened.Item3;                                    //Getting the count of the movies 
            CoverFlowControl.SelectedIndex = returened.Item4;                   // Gettig the carousel selcted item . 

            if (returened.Item1)
            {
                ProgressRing.IsActive = false;
            }


        }

        async void sorter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProgressRing.IsActive = true;

            switch (((Windows.UI.Xaml.Controls.Primitives.Selector)(sender)).SelectedIndex)
            {
                case 0:
                    sessionData.sortOrder = 0;
                    break;
                case 1:
                    sessionData.sortOrder = 1;
                    break;
                case 2:
                    sessionData.sortOrder = 2;
                    break;
                case 3:
                    sessionData.sortOrder = 3;
                    break;
                case 4:
                    sessionData.sortOrder = 4;
                    break;

            }

            CarouselImagePopulator obj1 = new CarouselImagePopulator();

            returened = await obj1.LoadMovieData();

            CoverFlowControl.ItemsSource = returened.Item2;                     //Getting the images from the class
            countofMovies = returened.Item3;                                    //Getting the count of the movies 
            CoverFlowControl.SelectedIndex = returened.Item4;                   // Gettig the carousel selcted item . 

            if (returened.Item1)
            {
                ProgressRing.IsActive = false;
            }


        }

 

        private void manageViewState()
        {
            string visualState = DetermineVisualState(ApplicationView.Value);

            if (visualState == "Snapped")
            {
                mainStack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                snappedStack.Visibility = Windows.UI.Xaml.Visibility.Visible;
                txtName.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //txtName.FontSize = 12;
                backButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                pageTitle.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                stackPopup.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                bottonBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //backButton.Style = App.Current.Resources["SnappedBackButtonStyle"] as Style;
                //pageTitle.Style = App.Current.Resources["SnappedPageHeaderTextStyle"] as Style;
            }
            else
            {
                mainStack.Visibility = Windows.UI.Xaml.Visibility.Visible;
                snappedStack.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                txtName.Visibility = Windows.UI.Xaml.Visibility.Visible;
                backButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                pageTitle.Visibility = Windows.UI.Xaml.Visibility.Visible;
                bottonBar.Visibility = Windows.UI.Xaml.Visibility.Visible;

                //backButton.Style = App.Current.Resources["BackButtonStyle"] as Style;
                //pageTitle.Style = App.Current.Resources["PageHeaderTextStyle"] as Style;
            }
        }

        void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            manageViewState();
        }

        protected string DetermineVisualState(ApplicationViewState viewState)
        {
            if (viewState == ApplicationViewState.Filled || viewState == ApplicationViewState.FullScreenLandscape)
            {
                // Allow pages to request that the Filled state be used only for landscape layouts narrower
                // than 1366 virtual pixels
                var windowWidth = Window.Current.Bounds.Width;
                viewState = windowWidth >= 1366 ? ApplicationViewState.FullScreenLandscape : ApplicationViewState.Filled;
            }
            return viewState.ToString();
        }



        public void ShareSourceLoad()
        {
            datatransferManager = DataTransferManager.GetForCurrentView();
            datatransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }

        void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            string movieName = ((Person)(CoverFlowControl.Items[CoverFlowControl.SelectedIndex])).Name.Split('#')[0].ToString();
            string movieImage = ((Person)(CoverFlowControl.Items[CoverFlowControl.SelectedIndex])).Image.ToString();
            string categoryDesc = "One of the Best Movies Ever";


            if (sessionData.selectCategory == "TopBollywood")
            {
                categoryDesc = "One of the best Bollywood/ Indian movie ever";
            }
            else if (sessionData.selectCategory == "TopEnglish")
            {
                categoryDesc = "One of the best Hollywood movie ever";
            }
            else if (sessionData.selectCategory == "TopForeign")
            {
                categoryDesc = "One of the best international movies ever";
            }
            else if (sessionData.selectCategory == "TopAsian")
            {
                categoryDesc = "One of the best Asian movies ever";
            }



            if (movieName != null && this.Frame != null)
            {

                if (sessionData.selectCategory == "TopForeign" & movieName.IndexOf("/") > 5)
                {
                    movieName = movieName.Substring(0, movieName.IndexOf("/"));
                }


                string z = ((App)(App.Current)).countryCode;                                     // Variable x to store the value of country selcted by the user . 

                string shareCountryCode = "";

                if (countryList.ContainsKey(z))        // If the country selected is not present in the dictionary then the value of the countrycode will be null . 
                {
                    shareCountryCode = countryList[z];

                }


                string url = countryWiseUrl(shareCountryCode, movieName);

                e.Request.Data.Properties.Title = movieName;

                //e.Request.Data.Properties.Description = selectedItem.Title + "-" + selectedItem.Author + Environment.NewLine + selectedItem.Link.AbsoluteUri.ToString();

                e.Request.Data.SetText(movieName + ": " + categoryDesc + System.Environment.NewLine + "Buy from: " + url);
                e.Request.Data.SetUri(new Uri(url));
                e.Request.Data.SetHtmlFormat("Love  this movie: <strong>" + movieName + "</strong><br /><br />" + "Synopsis at: <a href=\"http://www.wikipedia.org\">Wikipedia<br />Check it out at: <a href=\"http://www.amazon.com\">Amazon</a>");

                //RandomAccessStreamReference imageStreamRef = RandomAccessStreamReference.CreateFromUri(new Uri(movieImage));
                //e.Request.Data.Properties.Thumbnail = imageStreamRef;
                //e.Request.Data.SetBitmap(imageStreamRef);



            }


        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //AnalyticsHelper.TrackPageView("/Movies");

            //<summary>Firstly we are getting the language information of the system , and then we are making changes accordingly.</summary>
            language = new Windows.ApplicationModel.Resources.Core.ResourceContext().Languages.FirstOrDefault();

            if(!language.Contains("en"))
            {
                TraslateDetails.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            if (sessionData.selectCategory == "TopBollywood")
            {
                pageTitle.Text = loader.GetString("BollywoodTitle");
            }
            else if (sessionData.selectCategory == "TopEnglish")
            {
                pageTitle.Text = loader.GetString("HollywoodTitle");
            }
            else if (sessionData.selectCategory == "TopForeign")
            {
                pageTitle.Text = loader.GetString("InternationalTitle");
            }
            else if (sessionData.selectCategory == "TopAsian")
            {
                pageTitle.Text = loader.GetString("AsianTitle");
            }
            else
                pageTitle.Text = "Movies";

            if (sessionData.selectCategory == "TopForeign")
            {
                filterlang.Visibility = Windows.UI.Xaml.Visibility.Visible;
                filterlang.SelectedIndex = 0;
            }
            else if (sessionData.selectCategory == "TopAsian")
            {
                filterlang_asian.Visibility = Windows.UI.Xaml.Visibility.Visible;
               
            }

            GetIntertCondition();
            //LoadMovieData();           


            CarouselImagePopulator obj1 = new CarouselImagePopulator();
            returened = await obj1.LoadMovieData();
            CoverFlowControl.ItemsSource = returened.Item2;                     //Getting the images from the class
            countofMovies = returened.Item3;                                    //Getting the count of the movies 
            CoverFlowControl.SelectedIndex = returened.Item4;                   // Gettig the carousel selcted item . 


            string testString = ((Person)CoverFlowControl.SelectedItem).Name;
            //#region Retrieving last viewing movie

            //switch (sessionData.selectCategory)
            //{
            //    case "TopEnglish":
            //        if (sessionData.lastEnglishMovie != null)
            //            CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastEnglishMovie);
            //        break;
            //    case "TopBollywood":
            //        if (sessionData.lastBollywoodMovie != null)
            //            CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastBollywoodMovie);
            //        break;
            //    case "TopForeign":
            //        if (sessionData.lastForeignMovie != null)
            //            CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastForeignMovie);
            //        break;
            //    case "TopAsian":
            //        if (sessionData.lastAsianMovie != null)
            //            CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastAsianMovie);
            //        break;
            //    default:
            //        CoverFlowControl.SelectedIndex = 20;
            //        break;

            //}

            //#endregion

            if (returened.Item1)
            {
                ProgressRing.IsActive = false;
            }

            bottonBar.IsOpen = true;

           

            AnalyticsHelper.Track(sessionData.selectCategory, "Movie_Cat_selection");
        }

        //<summary>This function gets the current device Internet Connection Status</summary>
        private void GetIntertCondition()
        {
            InternetconnectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
        }

        //<summary>This function loads the Movie Images and title to the corousel on the Movie Page</summary>
        //private async void LoadMovieData()
        //{
        //    //Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");
            
        //    //movieFile = await storageFolder.GetFileAsync(fileName + ".xml");
        //    //fileContent = await FileIO.ReadTextAsync(movieFile);
        //    //XElement xe = XElement.Parse(fileContent);          

        //    //Stopwatch stopWatch = new Stopwatch();
        //    //stopWatch.Start();
        //    //countofMovies = xe.Descendants("rank").Count();
        //    string fileName = sessionData.selectCategory.ToString();
        //    CarouselImagePopulator obj1 = new CarouselImagePopulator(fileName);
        //    returened = await obj1.LoadMovieData();
        //    CoverFlowControl.ItemsSource = returened.Item2;
           
        //    countofMovies = returened.Item3;

        //    #region Retrieving last viewing movie

        //    switch (sessionData.selectCategory)
        //    {
        //        case "TopEnglish":
        //            if (sessionData.lastEnglishMovieIndex != null)
        //                CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastEnglishMovieIndex);
        //            break;
        //        case "TopBollywood":
        //            if (sessionData.lastBollywoodMovieIndex != null)
        //                CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastBollywoodMovieIndex);
        //            break;
        //        case "TopForeign":
        //            if (sessionData.lastForeignMovieIndex != null)
        //                CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastForeignMovieIndex);
        //            break;
        //        case "TopAsian":
        //            if (sessionData.lastAsianMovieIndex != null)
        //                CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastAsianMovieIndex);
        //            break;
        //        default:
        //            CoverFlowControl.SelectedIndex = 20;
        //            break;

        //    }

        //    #endregion

        //    if (returened.Item1)
        //    {
        //        ProgressRing.IsActive = false;
        //    }
        //    //stopWatch.Stop();

        //    //DisplayInfo();
        //}

        //private async Task<Tuple<bool,ObservableCollection<Person>>> LoadMovie(string fileName, XElement xe)
        //{

        //    //language = new Windows.ApplicationModel.Resources.Core.ResourceContext().Languages.FirstOrDefault();
        //    var query = (from movies in xe.Elements("movie")
        //                 select new Person
        //                 {
        //                     Name = movies.Element("name").Value,
        //                     Desp = movies.Element("desc").Value,
        //                     //Image = "ms-appx:///Assets/" + movies.Element("rank").Value + " - " + movies.Element("name").Value + ".jpg"
        //                     Image = "ms-appx:///Assets/" + fileName + "/" + movies.Element("rank").Value + ".jpg"
        //                 }).AsParallel().AsSequential();


        //    Images = new ObservableCollection<Person>();
            
        //    foreach (Person p in query)
        //    {
        //        // await Task.Run(() => images.Add(new Person() { Image = p.Image, Name = p.Name + "#" + p.Desp }));
        //        images.Add(p);

        //    }
        //   Tuple<bool, ObservableCollection<Person>> _images = new Tuple<bool, ObservableCollection<Person>>(true, images);           

        //   return _images ;
        //}

        //<summary>This function downloads the information about the movie from WikiPedia</summary>
        private void DisplayInfo()
        {
            string movieName = ((Person)(CoverFlowControl.SelectedItem)).Name.Split('#')[0].ToString();
            txtName.Text = movieName;


            switch (sessionData.selectCategory)
            {
                case "TopEnglish":
                    sessionData.lastEnglishMovie = ((Person)CoverFlowControl.SelectedItem).Name;
                    roamingSettings.Values["lastEnglishMovie"] = sessionData.lastEnglishMovie;
                    break;
                case "TopBollywood":
                    sessionData.lastBollywoodMovie = ((Person)CoverFlowControl.SelectedItem).Name;
                    roamingSettings.Values["lastBollywoodMovie"] = sessionData.lastBollywoodMovie;
                    break;
                case "TopForeign":
                    sessionData.lastForeignMovie = ((Person)CoverFlowControl.SelectedItem).Name;
                    roamingSettings.Values["lastForeignMovie"] = sessionData.lastForeignMovie;
                    break;
                case "TopAsian":
                    sessionData.lastAsianMovie = ((Person)CoverFlowControl.SelectedItem).Name;
                    roamingSettings.Values["lastAsianMovie"] = sessionData.lastAsianMovie;
                    break;
            }



            try
            {
                if (sessionData.selectCategory == "TopForeign")
                    movieName = movieName.Substring(0, movieName.IndexOf("/"));
                if (sessionData.selectCategory == "TopAsian")
                    movieName = movieName.Substring(0, movieName.IndexOf("("));
            }
            catch
            { }
            //txtDesp.Text = ((Person)(CoverFlowControl.Items[CoverFlowControl.SelectedIndex])).Name.Split('#')[1].ToString();
            
            if (InternetconnectionProfile != null)          // Checking if Active internet connection is avaliable or not . 
            {
                movieWiki.Navigate(new Uri("http://en.m.wikipedia.org/wiki/" + movieName));
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if(autoPlayOn)
            {
            AutoStop();
            }

            //Below code was extra or still not determined why it was there on the first place.
            //sessionData.lastMovieIndex = sessionData.selectCategory.ToString() + "#" + CoverFlowControl.SelectedIndex.ToString();
            //switch (sessionData.selectCategory)
            //{
            //    case "TopEnglish":
            //        sessionData.lastEnglishMovieIndex = CoverFlowControl.SelectedIndex.ToString();
            //        break;
            //    case "TopBollywood":
            //        sessionData.lastBollywoodMovieIndex = CoverFlowControl.SelectedIndex.ToString();
            //        break;
            //    case "TopForeign":
            //        sessionData.lastForeignMovieIndex = CoverFlowControl.SelectedIndex.ToString();
            //        break;
            //    case "TopAsian":
            //        sessionData.lastAsianMovieIndex = CoverFlowControl.SelectedIndex.ToString();
            //        break;
            //}


            datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);

            // Use the navigation frame to return to the previous page
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();

        }

        //private void Options_Click(object sender, RoutedEventArgs e)
        //{
        //    //ToggleSettings();
        //    stackpop.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //}

        public void ToggleSettings()
        {
            //SampleLayout element = SampleContent.Content as SampleLayout;
            //if (element.SelectedItem.SettingsContent != null)
            //{

            //    element.SelectedItem.IsSettingsOpen = !element.SelectedItem.IsSettingsOpen;

            //}
        }

        private void previous_Click_1(object sender, RoutedEventArgs e)
        {
            CoverFlowControl.MovePrevious();
            //DisplayInfo();
        }

        private void next_Click_1(object sender, RoutedEventArgs e)
        {
            CoverFlowControl.MoveNext();
            //DisplayInfo();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            stackPopup.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Layout.Opacity = 1;
            //btnClose.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void CoverFlowControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {         
            convertedName.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            txtName.Visibility = Windows.UI.Xaml.Visibility.Visible;
            DisplayInfo();                           
            
        }

        private void btnMoreInfo_Click(object sender, RoutedEventArgs e)
        {
            GetIntertCondition();
            if (autoPlayOn)
            {
                AutoStop();
            }

            AnalyticsHelper.Track("Movie_Info", "Button_click", txtName.Text);

            if (InternetconnectionProfile == null)          // Functionality to check user internet connection & prompt is connection unavaliable . 
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("NoInternet"));
                var result = messageDialog.ShowAsync();
            }
            else
            {
                stackPopup.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Layout.Opacity = .5;
            }
            //btnWVClose.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        //private async void btnBuyOnAmazon_Click(object sender, RoutedEventArgs e)
        //{
        //    await Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.amazon.com/s/?_encoding=UTF8&field-keywords="+txtName.Text+"&linkCode=ur2&tag=artmaya-20&url=search-alias%3Dmovies-tv"));
        //}


        private async void btnBuyDVD_Click_1(object sender, RoutedEventArgs e)
        {
            GetIntertCondition();
            if (autoPlayOn)
            {
                AutoStop();
            }

            var movieName = "";
            if (sessionData.selectCategory == "TopForeign" & txtName.Text.IndexOf("/") > 5)
            {
                movieName = txtName.Text.Substring(0, txtName.Text.IndexOf("/"));
            }
            else
            {
                movieName = txtName.Text;
            }

            AnalyticsHelper.Track("Buy_Button","Button_click", movieName);

            string x = ((App)(App.Current)).countryCode;                                     // Variable x to store the value of country selcted by the user . 

            string buyMovieCountrycode = "";

            if (countryList.ContainsKey(x))        // If the country selected is not present in the dictionary then the value of the countrycode will be null . 
            {
                buyMovieCountrycode = countryList[x];

            }

            if (InternetconnectionProfile == null)          // Functionality to check user internet connection & prompt is connection unavaliable . 
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("NoInternet"));
                var result = messageDialog.ShowAsync();
            }
            else
            {
                string url = countryWiseUrl(buyMovieCountrycode, movieName);

                //await Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.amazon.com/s/?_encoding=UTF8&field-keywords=" + txtName.Text + "&linkCode=ur2&tag=artmaya-20&url=search-alias%3Dmovies-tv"));
                await Windows.System.Launcher.LaunchUriAsync(new Uri(url));

            }
        }

        private void btnMovieDetails_Click_1(object sender, RoutedEventArgs e)
        {
            GetIntertCondition();
            if (autoPlayOn)
            {
                AutoStop();
            }

            if (InternetconnectionProfile == null)          // Functionality to check user internet connection & prompt is connection unavaliable . 
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("NoInternet"));
                var result = messageDialog.ShowAsync();
            }
            else
            {
                stackPopup.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Layout.Opacity = .5;
            }
            AnalyticsHelper.Track("Movie_Info","Button_click",txtName.Text);
        }

        //private void btnMoreInfo_Click(object sender, DoubleTappedRoutedEventArgs e)
        //{

        //}


        private string countryWiseUrl(string country, string movieName)
        {
            string url;

            switch (country)
            {
                case "US":
                    url = "http://www.amazon.com/s/?_encoding=UTF8&field-keywords=" + movieName + "&linkCode=ur2&tag=artmaya-20&url=search-alias%3Dmovies-tv";
                    break;
                case "IN":
                    url = "http://www.flipkart.com/search/a/movies-music?fk-search=movies-music&query=" + movieName + "&vertical=movies-music&affid=dkdaksatec";
                    break;
                case "GB":
                    //url = "http://www.amazon.co.uk/s/?_encoding=UTF8&camp=1634&field-keywords=" + movieName + "&linkCode=ur2&tag=daksatech-21&url=search-alias=dvd";
                    url = "http://www.amazon.co.uk/s/?_encoding=UTF8&camp=1634&field-keywords=" + movieName + "&tag=daksatech-21&url=search-alias%3Ddvd";
                    break;
                case "CA":
                    url = "http://www.amazon.ca/s/?_encoding=UTF8&camp=15121&field-keywords=" + movieName + "&tag=daksatech-20&url=search-alias%3Ddvd";
                    break;
                //case "AU":
                //    url = "http://www.amazon.com";
                //    break;
                //case "CN":
                //    url = "http://www.amazon.com";
                //    break;
                case "DE":
                    url = "http://www.amazon.de/s/?url=search-alias%3Ddvd&field-keywords=" + movieName + "breathless&tag=daksatech02-21";
                    break;
                case "IT":
                    url = "http://www.amazon.it/s/?field-keywords=" + movieName + "&tag=daksatech086-21&url=search-alias%3Ddvd";
                    break;
                case "ES":
                    url = "http://www.amazon.es/s/?field-keywords=" + movieName + "&tag=daksatech0d-21&url=search-alias%3Ddvd";
                    break;
                case "FR":
                    url = "http://www.amazon.fr/s/?_encoding=UTF8&camp=1642&field-keywords=" + movieName + "&tag=daksatech09-21&url=search-alias%3Ddvd";
                    break;
                case "JP":
                    url = "http://www.amazon.co.jp/s/?_encoding=UTF8&field-keywords=" + movieName + "&tag=artmaya-22&url=search-alias%3Ddvd";
                    break;


                default:
                    url = "http://www.amazon.com/s/?_encoding=UTF8&field-keywords=" + movieName + "&linkCode=ur2&tag=artmaya-20&url=search-alias%3Dmovies-tv";
                    break;
            }

            return url;
        }

        private async void Translate(object sender, RoutedEventArgs e)
        {
            GetIntertCondition();
            Pause_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Play_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Forward_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Backward_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
            ts.Cancel();

            if (InternetconnectionProfile == null)          // Functionality to check user internet connection & prompt is connection unavaliable . 
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("NoInternet"));
                var result = messageDialog.ShowAsync();
            }
            else
            {

                GoogleTranslator result = new GoogleTranslator();

                string res = await result.Translator(txtName.Text, language);

                // string convertedText = res.ToString();

                convertedName.Text = res;

                txtName.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                convertedName.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            AnalyticsHelper.Track("Translate", "Button_click", language);

        }

        //private void Button_visible(object sender, PointerRoutedEventArgs e)
        //{
        //    Button_stackpanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        //}

        //private void Button_collapsed(object sender, PointerRoutedEventArgs e)
        //{
        //    Button_stackpanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        //}

        private void BackWard_move(object sender, RoutedEventArgs e)
        {
            if (!((CoverFlowControl.SelectedIndex - 10) <= 0))
            {
                CoverFlowControl.SelectedIndex = CoverFlowControl.SelectedIndex - 10;
            }
        }

        private void ForWard_move(object sender, RoutedEventArgs e)
        {
            if (!((CoverFlowControl.SelectedIndex + 10) >= countofMovies))
            {
                CoverFlowControl.SelectedIndex = CoverFlowControl.SelectedIndex + 10;
            }
        }

        private async void Auto_Play(object sender, RoutedEventArgs e)
        {
            autoPlayOn = true;
            CancellationTokenSource reset = new CancellationTokenSource();
            ts = reset;

            
            CancellationToken ct = ts.Token;
            if (!((CoverFlowControl.SelectedIndex + 1) >= countofMovies))
            {
                Play_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Pause_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Forward_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Backward_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                var pos = CoverFlowControl.SelectedIndex;

                 await Task.Factory.StartNew( async () =>
                    {

                        while (true)
                        {
                            
                            await Task.Delay(2000);
                            if (pos + 1 == countofMovies)
                            {
                                await Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate { Play_Button.Visibility = Windows.UI.Xaml.Visibility.Visible; Pause_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed; Backward_Button.Visibility = Windows.UI.Xaml.Visibility.Visible; Forward_Button.Visibility = Windows.UI.Xaml.Visibility.Visible; });
                                break;
                            }

                            if (ct.IsCancellationRequested)
                            {
                                
                                break;
                            }
                            else
                            {
                                await CoverFlowControl.Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate { CoverFlowControl.SelectedIndex = CoverFlowControl.SelectedIndex + 1; pos = CoverFlowControl.SelectedIndex; });
                            }


                        } 
                    },ct);

                 
            }

        }

      
        private void AutoStop(object sender, RoutedEventArgs e)
        {
            AutoStop();      
            
        }

        private void AutoStop()
        {
            Pause_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Play_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Forward_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
            Backward_Button.Visibility = Windows.UI.Xaml.Visibility.Visible;
            ts.Cancel();
            autoPlayOn = false;
        }




    }

}
