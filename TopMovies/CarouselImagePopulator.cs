using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TopMovies.Views;
using System.Collections.ObjectModel;

namespace TopMovies
{
    class CarouselImagePopulator
    {
        //Declaring fields and variables . 
        private int sort;
        private string filterLanguage;
        private string filterGenere;
        private string cate;
        int elementCount;
        string selectedMovie;
        private ObservableCollection<Person> images;
        int newIndex;

        public ObservableCollection<Person> Images
        {
            get { return images; }
            set { images = value; }
        }

        //This constructor takes in sortorder , language to filter, genere, catetory
        //public CarouselImagePopulator(string language,string category)
        //{
        //    filterLanguage = language;
        //    cate = category;
        //}

        ////This constructor takes in sort order, genere and category. This is to be used in the English and the bollywood section . 
        //public CarouselImagePopulator(string genere, string category)
        //{
        //    filterGenere = genere;
        //    cate = category;
        //}


        public CarouselImagePopulator()
        {
            filterLanguage = null;
            sort = 0;
            filterGenere = null;
        }

        //This constructor takes in sortorder , language to filter, genere, catetory
        public CarouselImagePopulator(int sortOrder, string category,string movieName)
        {
            sort = sortOrder;
            cate = category;
            selectedMovie = movieName;
        }

        public async Task<Tuple<bool, ObservableCollection<Person>,int>> LoadMovieData()
        {
            #region Retrieving last viewing movie

            switch (sessionData.selectCategory)
            {
                case "TopEnglish":
                    cate = "TopEnglish";
                    //if (sessionData.lastEnglishMovieIndex != null)
                    //    CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastEnglishMovieIndex);
                    break;
                case "TopBollywood":
                    cate = "TopBollywood";
                    //if (sessionData.lastBollywoodMovieIndex != null)
                    //    CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastBollywoodMovieIndex);
                    break;
                case "TopForeign":
                    cate = "TopForeign";
                    //if (sessionData.lastForeignMovieIndex != null)
                    //    CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastForeignMovieIndex);
                    break;
                case "TopAsian":
                    cate = "TopAsian";
                    //if (sessionData.lastAsianMovieIndex != null)
                    //    CoverFlowControl.SelectedIndex = Convert.ToInt32(sessionData.lastAsianMovieIndex);
                    break;
                //default:
                //    CoverFlowControl.SelectedIndex = 20;
                //    break;

            }

            #endregion


                Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");
                XDocument xmlfile = XDocument.Load(storageFolder.Path + @"\" + cate + ".xml");

                if (sort == 0 && filterLanguage == null && filterGenere == null)
                {
                    var query = (from movies in xmlfile.Root.Elements("movie")
                                 select new Person
                                {
                                    Name = movies.Element("name").Value,
                                    //Desp = movies.Element("desc").Value,
                                    Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg"
                                }).AsParallel().AsSequential();

                    elementCount = query.Count();

                    Images = new ObservableCollection<Person>();

                    foreach (Person p in query)
                    {
                        //Images.Add(p);
                        images.Add(p);
                        if (p.Name == selectedMovie)
                        {
                            newIndex = Convert.ToInt32(p.Image.Split(new char[] { '/', '.' })[5]);
                        }
                    }
                    
                }
                else if (sort == 1 && filterLanguage == null && filterGenere == null)
                {
                    var query = (from movies in xmlfile.Root.Elements("movie")
                                 orderby (int)movies.Element("release")
                                 select new Person
                                 {
                                     Name = movies.Element("name").Value,
                                     Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg"
                                 }).AsParallel().AsSequential();

                    elementCount = query.Count();

                    Images = new ObservableCollection<Person>();

                    foreach (Person p in query)
                    {
                        //Images.Add(p);
                        images.Add(p);
                        if (p.Name == selectedMovie)
                        {
                            newIndex = Convert.ToInt32(p.Image.Split(new char[] { '/', '.' })[5]);
                        }
                    }

                }

                Tuple<bool, ObservableCollection<Person>, int> _images = new Tuple<bool, ObservableCollection<Person>, int>(true, images, elementCount);

                return _images;



            //var query = (from movies in xmlfile.Root.Elements("movie")
            //             orderby (DateTime)movies.Element("release")
            //             where movies.Element("genere").Value.Contains("Action")
            //             select movies).AsParallel();


        }
    }
}
