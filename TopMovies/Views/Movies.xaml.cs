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
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using DT.GoogleAnalytics.Metro;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Animation;
using System.Threading;
using System.Text.RegularExpressions;

namespace TopMovies.Views
{
    /// <summary>
    /// This page displays the carousel containing the movies poster. 
    /// </summary>
    public sealed partial class Movies : TopMovies.Common.LayoutAwarePage
    {        
        public DataTransferManager datatransferManager;
        Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
        int countofMovies;
        string language;                                                                                // This is to get the language on the user system.
        CancellationTokenSource ts = new CancellationTokenSource();
        bool autoPlayOn = false;
        ConnectionProfile InternetconnectionProfile;
        ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();                // This is to get the resources defined in the resource.resw file . 
        Tuple<ObservableCollection<Person>,int,int> returened;
        bool updateName = false;

        
        private ObservableCollection<Person> images;

        public ObservableCollection<Person> Images
        {
            get { return images; }
            set { images = value; }
        }

        public Movies() 
        {

            this.InitializeComponent();
            //Firstly populate the combo box then set it to the selected item
            for(int i=1;i<=5;i++)
            {
                sorter.Items.Add(loader.GetString("sorter"+i));
                genere.Items.Add(loader.GetString("genre"+i));
            }
            for (int i = 6; i <= 14; i++)
            {
                genere.Items.Add(loader.GetString("genre" + i));
            }
            genere.SelectedIndex = 0;
            sorter.SelectedIndex = 0;
            if (sessionData.selectCategory == "TopAsian")
            {
                for (int i = 1; i <= 7; i++)
                {
                    filterlang_asian.Items.Add(loader.GetString("asianLang" + i));
                }
                filterlang_asian.SelectedIndex = 0;
            }
            else if (sessionData.selectCategory == "TopForeign")
            {   
                for (int i = 1; i <= 11; i++)
                {
                    filterlang.Items.Add(loader.GetString("lang" + i));
                }
                filterlang.SelectedIndex = 0;
            }

        }

        /// <summary>
        /// This method is called when the user changes the genere selection of the movies dispalyed.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void genere_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (autoPlayOn)
                AutoStop();
            
            ProgressRing.IsBusy = true;

                switch (genere.SelectedIndex)
                {
                    case 0: 
                        sessionData.filterGenere = null;
                        break;
                    case 1:
                        sessionData.filterGenere = "Action";
                        break;
                    case 2:
                        sessionData.filterGenere = "Adventure";
                        break;
                    case 3:
                        sessionData.filterGenere = "Animation";
                        break;
                    case 4:
                        sessionData.filterGenere = "Comedy";
                        break;
                    case 5:
                        sessionData.filterGenere = "Crime";
                        break;
                    case 6:
                        sessionData.filterGenere = "Drama";
                        break;
                    case 7:
                        sessionData.filterGenere = "Family";
                        break;
                    case 8:
                        sessionData.filterGenere = "Fantasy";
                        break;
                    case 9:
                        sessionData.filterGenere = "Horror";
                        break;
                    case 10:
                        sessionData.filterGenere = "Mystery";
                        break;
                    case 11:
                        sessionData.filterGenere = "Romance";
                        break;
                    case 12:
                        sessionData.filterGenere = "Thriller";
                        break;
                    case 13:
                        sessionData.filterGenere = "War";
                        break;
                }

                CarouselImagePopulator obj1 = new CarouselImagePopulator();
                returened = await obj1.LoadMovieData();
            

            if (returened.Item2 != 0)
            {
                if (returened.Item3 == CoverFlowControl.SelectedIndex)
                {
                    updateName = true;
                }
                ProgressRing.IsBusy = false;
                CoverFlowControl.ItemsSource = returened.Item1;                     //Getting the images from the class
                countofMovies = returened.Item2;                                    //Getting the count of the movies 
                CoverFlowControl.SelectedIndex = returened.Item3;                   // Gettig the carousel selcted item . 
                if (updateName)
                {
                    CoverFlowControlSpecialCase();
                    updateName = false;
                }
                sessionData.genreIndex = genere.SelectedIndex;
            }
            else
            {
                //This will execute in case the user selection doesn't generates any movie.
                ProgressRing.IsBusy = false;
                if (sessionData.selectCategory != "TopBollywood" && sessionData.selectCategory != "TopEnglish")
                {
                    if (sessionData.selectCategory == "TopForeign")
                    {
                        txtName.Text = loader.GetString("NoMoviesMessage1") + genere.SelectedValue + loader.GetString("NoMoviesMessage2") + filterlang.SelectedValue + loader.GetString("NoMoviesMessage3");
                    }
                    else if (sessionData.selectCategory == "TopAsian")
                    {
                        txtName.Text = loader.GetString("NoMoviesMessage1") + genere.SelectedValue + loader.GetString("NoMoviesMessage2") + filterlang_asian.SelectedValue + loader.GetString("NoMoviesMessage3");
                    }
                }
                else
                {
                    txtName.Text = loader.GetString("NoMoviesMessage1") + genere.SelectedValue + loader.GetString("NoMoviesMessage2") + loader.GetString(sessionData.selectCategory);
                }
                updateName = true;

                //This will keep the genre filter and the selected index the same if the operation returns zero movies . Do not remove this portion. 
                switch (sessionData.genreIndex)
                {
                    case 0:
                        sessionData.filterGenere = null;
                        break;
                    case 1:
                        sessionData.filterGenere = "Action";
                        break;
                    case 2:
                        sessionData.filterGenere = "Adventure";
                        break;
                    case 3:
                        sessionData.filterGenere = "Animation";
                        break;
                    case 4:
                        sessionData.filterGenere = "Comedy";
                        break;
                    case 5:
                        sessionData.filterGenere = "Crime";
                        break;
                    case 6:
                        sessionData.filterGenere = "Drama";
                        break;
                    case 7:
                        sessionData.filterGenere = "Family";
                        break;
                    case 8:
                        sessionData.filterGenere = "Fantasy";
                        break;
                    case 9:
                        sessionData.filterGenere = "Horror";
                        break;
                    case 10:
                        sessionData.filterGenere = "Mystery";
                        break;
                    case 11:
                        sessionData.filterGenere = "Romance";
                        break;
                    case 12:
                        sessionData.filterGenere = "Thriller";
                        break;
                    case 13:
                        sessionData.filterGenere = "War";
                        break;
                }
                genere.SelectionChanged -= genere_SelectionChanged;
                genere.SelectedIndex = sessionData.genreIndex;
                genere.SelectionChanged += genere_SelectionChanged;


            }
        }

        /// <summary>
        /// This method is called when the language filter is applied in the international movie category.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void filterlang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (autoPlayOn)
                AutoStop();

            ProgressRing.IsBusy = true;

                switch (filterlang.SelectedIndex)
                {
                    case 0:
                        sessionData.filterLang = null;
                        break;
                    case 1:
                        sessionData.filterLang = "English";
                        break;
                    case 2:
                        sessionData.filterLang = "French";
                        break;
                    case 3:
                        sessionData.filterLang = "German";
                        break;
                    case 4:
                        sessionData.filterLang = "Italian";
                        break;
                    case 5:
                        sessionData.filterLang = "Japanese";
                        break;
                    case 6:
                        sessionData.filterLang = "Polish";
                        break;
                    case 7:
                        sessionData.filterLang = "Portuguese";
                        break;
                    case 8:
                        sessionData.filterLang = "Russian";
                        break;
                    case 9:
                        sessionData.filterLang = "Spanish";
                        break;
                    case 10:
                        sessionData.filterLang = "Swedish";
                        break;
                }

                CarouselImagePopulator obj1 = new CarouselImagePopulator();

                returened = await obj1.LoadMovieData();

            if (returened.Item2!=0)
            {
                if (returened.Item3 == CoverFlowControl.SelectedIndex)
                {
                    updateName = true;
                }
                ProgressRing.IsBusy = false;
                CoverFlowControl.ItemsSource = returened.Item1;                     //Getting the images from the class
                countofMovies = returened.Item2;                                    //Getting the count of the movies 
                CoverFlowControl.SelectedIndex = returened.Item3;                   // Gettig the carousel selcted item . 
                if (updateName)
                {
                    CoverFlowControlSpecialCase();
                    updateName = false;
                }
                sessionData.langIndex = filterlang.SelectedIndex;
            }
            else
            {
                ProgressRing.IsBusy = false;
                if (sessionData.selectCategory != "TopBollywood" && sessionData.selectCategory != "TopEnglish")
                {
                    if (sessionData.selectCategory == "TopForeign")
                    {
                        txtName.Text = loader.GetString("NoMoviesMessage1") + genere.SelectedValue + loader.GetString("NoMoviesMessage2") + filterlang.SelectedValue + loader.GetString("NoMoviesMessage3");
                    }
                    else if (sessionData.selectCategory == "TopAsian")
                    {
                        txtName.Text = loader.GetString("NoMoviesMessage1") + genere.SelectedValue + loader.GetString("NoMoviesMessage") + filterlang_asian.SelectedValue + loader.GetString("NoMoviesMessage3");
                    }
                }
                else
                {
                    txtName.Text = loader.GetString("NoMoviesMessage1") + genere.SelectedValue + loader.GetString("NoMoviesMessage") + loader.GetString(sessionData.selectCategory);
                }
                updateName = true;

                //This will keep the language filter and the selected index the same if the operation returns zero movies . Do not remove this portion. 
                switch (sessionData.langIndex)
                {
                    case 0:
                        sessionData.filterLang = null;
                        break;
                    case 1:
                        sessionData.filterLang = "English";
                        break;
                    case 2:
                        sessionData.filterLang = "French";
                        break;
                    case 3:
                        sessionData.filterLang = "German";
                        break;
                    case 4:
                        sessionData.filterLang = "Italian";
                        break;
                    case 5:
                        sessionData.filterLang = "Japanese";
                        break;
                    case 6:
                        sessionData.filterLang = "Polish";
                        break;
                    case 7:
                        sessionData.filterLang = "Portuguese";
                        break;
                    case 8:
                        sessionData.filterLang = "Russian";
                        break;
                    case 9:
                        sessionData.filterLang = "Spanish";
                        break;
                    case 10:
                        sessionData.filterLang = "Swedish";
                        break;
                }
                filterlang.SelectionChanged -= filterlang_SelectionChanged;
                filterlang.SelectedIndex = sessionData.langIndex;
                filterlang.SelectionChanged += filterlang_SelectionChanged;

            }

        }

        /// <summary>
        /// This method is called when the language filter is applied in the asian movie category. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void filterlang_asian_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (autoPlayOn)
                AutoStop();

            ProgressRing.IsBusy = true;
            switch (filterlang_asian.SelectedIndex)
            {
                case 0:
                    sessionData.filterLang = null;
                    break;
                case 1:
                    sessionData.filterLang = "Cantonese";
                    break;
                case 2:
                    sessionData.filterLang = "Chinese";
                    break;
                case 3:
                    sessionData.filterLang = "English";
                    break;
                case 4:
                    sessionData.filterLang = "Japanese";
                    break;
                case 5:
                    sessionData.filterLang = "Korean";
                    break;
                case 6:
                    sessionData.filterLang = "Thai";
                    break;
            }

                CarouselImagePopulator obj1 = new CarouselImagePopulator();

                returened = await obj1.LoadMovieData();

            if (returened.Item2!=0)
            {
                if (returened.Item3 == CoverFlowControl.SelectedIndex)
                {
                    updateName = true;
                }
                ProgressRing.IsBusy = false;
                CoverFlowControl.ItemsSource = returened.Item1;                     //Getting the images from the class
                countofMovies = returened.Item2;                                    //Getting the count of the movies 
                CoverFlowControl.SelectedIndex = returened.Item3;                   // Gettig the carousel selcted item . 
                if (updateName)
                {

                    CoverFlowControlSpecialCase();
                    updateName = false;
                }
                sessionData.asianLangIndex = filterlang_asian.SelectedIndex;
            }
            else
            {
                ProgressRing.IsBusy = false;
                if (sessionData.selectCategory != "TopBollywood" && sessionData.selectCategory != "TopEnglish")
                {
                    if (sessionData.selectCategory == "TopForeign")
                    {
                        txtName.Text = loader.GetString("NoMoviesMessage1") + genere.SelectedValue + loader.GetString("NoMoviesMessage2") + filterlang.SelectedValue + loader.GetString("NoMoviesMessage3");
                    }
                    else if (sessionData.selectCategory == "TopAsian")
                    {
                        txtName.Text = loader.GetString("NoMoviesMessage1") + genere.SelectedValue + loader.GetString("NoMoviesMessage") + filterlang_asian.SelectedValue + loader.GetString("NoMoviesMessage3");
                    }
                }
                else
                {
                    txtName.Text = loader.GetString("NoMoviesMessage1") + genere.SelectedValue + loader.GetString("NoMoviesMessage") + loader.GetString(sessionData.selectCategory) ;
                }
                updateName = true;

                //This will keep the language filter and the selected index the same if the operation returns zero movies . Do not remove this portion. 
                switch (sessionData.asianLangIndex)
                {
                    case 0:
                        sessionData.filterLang = null;
                        break;
                    case 1:
                        sessionData.filterLang = "Cantonese";
                        break;
                    case 2:
                        sessionData.filterLang = "Chinese";
                        break;
                    case 3:
                        sessionData.filterLang = "English";
                        break;
                    case 4:
                        sessionData.filterLang = "Japanese";
                        break;
                    case 5:
                        sessionData.filterLang = "Korean";
                        break;
                    case 6:
                        sessionData.filterLang = "Thai";
                        break;
                }
                filterlang_asian.SelectionChanged -= filterlang_asian_SelectionChanged;
                filterlang_asian.SelectedIndex = sessionData.asianLangIndex;
                filterlang_asian.SelectionChanged += filterlang_asian_SelectionChanged;



            }


        }

        async void sorter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (autoPlayOn)
                AutoStop();

            ProgressRing.IsBusy = true;

            sessionData.sortOrder = sorter.SelectedIndex;

            CarouselImagePopulator obj1 = new CarouselImagePopulator();

            returened = await obj1.LoadMovieData();

            if (returened.Item2!=0)
            {
                ProgressRing.IsBusy = false;
                CoverFlowControl.ItemsSource = returened.Item1;                     //Getting the images from the class
                countofMovies = returened.Item2;                                    //Getting the count of the movies 
                CoverFlowControl.SelectedIndex = returened.Item3;                   // Gettig the carousel selcted item . 

            }

        }

        public void ShareSourceLoad()
        {
            datatransferManager = DataTransferManager.GetForCurrentView();
            datatransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        }

        void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            string movieName = ((Person)(CoverFlowControl.Items[CoverFlowControl.SelectedIndex])).Name.ToString();
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

                //if (sessionData.selectCategory == "TopForeign" && movieName.IndexOf("/") > 5)
                //{
                //    movieName = movieName.Substring(0, movieName.IndexOf("/"));
                //}


                string url = countryWiseUrl(((App)(App.Current)).countryCode, movieName);

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
        /// Firstly we are getting the language information of the system , and then we are making changes accordingly.
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            AnalyticsHelper.TrackPageView("/Movies");
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
            }
            else if (sessionData.selectCategory == "TopAsian")
            {
                filterlang_asian.Visibility = Windows.UI.Xaml.Visibility.Visible;               
            }

            CarouselImagePopulator obj1 = new CarouselImagePopulator();
            returened = await obj1.LoadMovieData();
            if (returened.Item2 != 0)
            {
                CoverFlowControl.ItemsSource = returened.Item1;                     //Getting the images from the class
                CoverFlowControl.SelectedIndex = returened.Item3;                   // Gettig the carousel selcted item . 
                countofMovies = returened.Item2;                                    //Getting the count of the movies 
                ProgressRing.IsBusy = false;
            }

            if (sessionData.filterGenere != null || sessionData.filterLang != null || sessionData.sortOrder != 0)
            {
                if (sessionData.filterGenere != null)
                    genere.SelectedIndex = sessionData.genreIndex;
                if (sessionData.sortOrder != 0)
                    sorter.SelectedIndex = sessionData.sortOrder;
                if (sessionData.filterLang != null)
                {
                    if (sessionData.selectCategory == "TopAsian")
                    {
                        filterlang_asian.SelectedIndex = sessionData.asianLangIndex;
                    }
                    else if (sessionData.selectCategory == "TopForeign")
                    {
                        filterlang.SelectedIndex = sessionData.langIndex;
                    }
                }
            }

            sorter.SelectionChanged += sorter_SelectionChanged;
            genere.SelectionChanged += genere_SelectionChanged;
            if (sessionData.selectCategory == "TopAsian")
            {
                filterlang_asian.SelectionChanged += filterlang_asian_SelectionChanged; 
            }
            else if (sessionData.selectCategory == "TopForeign")
            {
                filterlang.SelectionChanged += filterlang_SelectionChanged;
            }
            language = new Windows.ApplicationModel.Resources.Core.ResourceContext().Languages.FirstOrDefault();

            if (!language.Contains("en"))
            {
                TraslateDetails.IsEnabled = true;
            }


            //if (this.Frame.CanGoForward)
                //CoverFlowControl.InvalidateArrange();


            ShareSourceLoad();
            AnalyticsHelper.Track(sessionData.selectCategory, "Movie_Cat_selection");
        }

        /// <summary>
        /// This function gets the current device Internet Connection Status
        /// </summary>
        /// <returns></returns>
        private bool GetIntertCondition()
        {
            InternetconnectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            bool internetCondition = InternetconnectionProfile != null && InternetconnectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internetCondition;
        }


        /// <summary>
        /// We cannot use ((Person)coverflowcontrol.selecteditem).Name for saving to sessiondata name since when the index is not changes in case of filter and genere , the
        /// selection changed event is not fired and also the selected item is not updated. Using below way confirms that the sessiondata is always stored correctly.
        /// </summary>
        private void DisplayInfo()
        {

            string movieName = ((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).Name.Split('#')[0].ToString();
            txtName.Text = movieName;

            switch (sessionData.selectCategory)
            {
                case "TopEnglish":
                    sessionData.lastEnglishMovie = ((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).Name;
                    roamingSettings.Values["lastEnglishMovie"] = sessionData.lastEnglishMovie;
                    break;
                case "TopBollywood":
                    sessionData.lastBollywoodMovie = ((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).Name;
                    roamingSettings.Values["lastBollywoodMovie"] = sessionData.lastBollywoodMovie;
                    break;
                case "TopForeign":
                    sessionData.lastForeignMovie = ((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).Name;
                    roamingSettings.Values["lastForeignMovie"] = sessionData.lastForeignMovie;
                    break;
                case "TopAsian":
                    sessionData.lastAsianMovie = ((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).Name;
                    roamingSettings.Values["lastAsianMovie"] = sessionData.lastAsianMovie;
                    break;
            }

         }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if(autoPlayOn)
            AutoStop();

            datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            ts.Dispose();
            base.GoBack(sender, e);

        }


        private void previous_Click_1(object sender, RoutedEventArgs e)
        {
            CoverFlowControl.MovePrevious();
        }

        private void next_Click_1(object sender, RoutedEventArgs e)
        {
            CoverFlowControl.MoveNext();
        }

        private void CoverFlowControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CoverFlowControlSpecialCase();         
        }

        /// <summary>
        /// It was noticed that when the coverflow index remains same , i.e 0 then the selection changed event was not fired , thus this function was created, and is being
        /// called when filtered via language and rating. 
        /// </summary>
        private void CoverFlowControlSpecialCase()
        {
            convertedName.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            txtName.Visibility = Windows.UI.Xaml.Visibility.Visible;
            DisplayInfo(); 
        }

        private void btnMoreInfo_Click(object sender, RoutedEventArgs e)
        {
            if (autoPlayOn)
                AutoStop();


            if (!GetIntertCondition())          // Functionality to check user internet connection & prompt is connection unavaliable . 
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("NoInternet"));
                var result = messageDialog.ShowAsync();
            }
            else
            {
                //Deregestring all the events that are attached to different combobox and the button on the page. 
                datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
                sorter.SelectionChanged -= sorter_SelectionChanged;
                genere.SelectionChanged -= genere_SelectionChanged;
                if (sessionData.selectCategory == "TopAsian")
                {
                    filterlang_asian.SelectionChanged -= filterlang_asian_SelectionChanged;
                }
                else if (sessionData.selectCategory == "TopForeign")
                {
                    filterlang.SelectionChanged -= filterlang_SelectionChanged;
                }

                //Person selectedItem = (Person)CoverFlowControl.SelectedItem;
                //this.Frame.Navigate(typeof(InfoPage), (selectedItem.imdbID + "|" + selectedItem.Image + "|" + selectedItem.Name) as string);

                ts.Dispose();
                //Below way of sending data is working properly. 
                this.Frame.Navigate(typeof(InfoPage), ((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).imdbID + "|" + Regex.Match(((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).Image, @"\d{1,3}") + "|" + ((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).Name);
            }


        }


        //private void btnMovieDetails_Click_1(object sender, RoutedEventArgs e)
        //{
        //    if (autoPlayOn)
        //        AutoStop();


        //    if (!GetIntertCondition())          // Functionality to check user internet connection & prompt is connection unavaliable . 
        //    {
        //        var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("NoInternet"));
        //        var result = messageDialog.ShowAsync();
        //    }
        //    else
        //    {
        //        //Deregestring all the events that are attached to different combobox and the button on the page. 
        //        datatransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
        //        sorter.SelectionChanged -= sorter_SelectionChanged;
        //        genere.SelectionChanged -= genere_SelectionChanged;
        //        if (sessionData.selectCategory == "TopAsian")
        //        {
        //            filterlang_asian.SelectionChanged -= filterlang_asian_SelectionChanged;
        //        }
        //        else if (sessionData.selectCategory == "TopForeign")
        //        {
        //            filterlang.SelectionChanged -= filterlang_SelectionChanged;
        //        }


        //        this.Frame.Navigate(typeof(InfoPage), new Tuple<int, string, string>(((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).imdbID, ((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).Image, ((Person)CoverFlowControl.Items[CoverFlowControl.SelectedIndex]).Name));
        //    }

        //}




        private async void Translate(object sender, RoutedEventArgs e)
        {
            if (autoPlayOn)
                AutoStop();

            if (!GetIntertCondition())          // Functionality to check user internet connection & prompt is connection unavaliable . 
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(loader.GetString("NoInternet"));
                var result = messageDialog.ShowAsync();
            }
            else
            {

                GoogleTranslator result = new GoogleTranslator("Movies");

                string res = await result.Translator(txtName.Text, language);

                if (res != null)
                {
                    convertedName.Text = res;

                    txtName.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    convertedName.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }

            AnalyticsHelper.Track("Translate", "Button_click", language);

        }

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
                ts.Dispose();
                ts = new CancellationTokenSource();
                CancellationToken ct = ts.Token;
                if (!((CoverFlowControl.SelectedIndex + 1) >= countofMovies))
                {
                    Play_Button.IsEnabled = false;
                    Play_Button1.IsEnabled = false;
                    Pause_Button.IsEnabled = true;
                    Pause_Button1.IsEnabled = true;
                    Forward_Button.IsEnabled = false;
                    Backward_Button.IsEnabled = false;
                    var pos = CoverFlowControl.SelectedIndex;

                    await Task.Factory.StartNew(async () =>
                       {

                           while (true)
                           {

                               await Task.Delay(2000);
                               if (pos + 1 == countofMovies)
                               {
                                   //await Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate { Play_Button.Visibility = Windows.UI.Xaml.Visibility.Visible; Pause_Button.Visibility = Windows.UI.Xaml.Visibility.Collapsed; Backward_Button.Visibility = Windows.UI.Xaml.Visibility.Visible; Forward_Button.Visibility = Windows.UI.Xaml.Visibility.Visible; });
                                   await Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate
                                   {
                                       Play_Button.IsEnabled = true; Play_Button1.IsEnabled = true;
                                       Pause_Button.IsEnabled = false; Pause_Button1.IsEnabled = false; Backward_Button.IsEnabled = true; Forward_Button.IsEnabled = true;
                                   });
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
                       }, ct);


                }

        }

      
        private void AutoStop(object sender, RoutedEventArgs e)
        {
            AutoStop();      
            
        }

        private void AutoStop()
        {
            Pause_Button.IsEnabled = false;
            Pause_Button1.IsEnabled = false;
            Play_Button.IsEnabled = true;
            Play_Button1.IsEnabled = true;
            Forward_Button.IsEnabled = true;
            Backward_Button.IsEnabled = true;
            ts.Cancel();
            autoPlayOn = false;
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

        private void ShowFilters(object sender, RoutedEventArgs e)
        {
            filterPopUp.IsLightDismissEnabled = true;
            filterPopUp.ChildTransitions = new Windows.UI.Xaml.Media.Animation.TransitionCollection();
            filterPopUp.ChildTransitions.Add(new PaneThemeTransition()
            {
                Edge = Windows.UI.Xaml.Controls.Primitives.EdgeTransitionLocation.Top
            });
            filterPopUp.IsOpen = true;
        }

        private void carousel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            carousel_ImageCorrector();
        }

        private void carousel_ImageCorrector()
        {
            CoverFlowControl.Refresh();
        }








    }

}
