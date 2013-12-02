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
        private string cate;
        int elementCount;
        string selectedMovie;
        private ObservableCollection<Person> images;
        int newIndex;
        int movieCounter=0;

        public ObservableCollection<Person> Images
        {
            get { return images; }
            set { images = value; }
        }

        public async Task<Tuple<ObservableCollection<Person>,int,int>> LoadMovieData()
        {   
            Images = new ObservableCollection<Person>();

            IEnumerable<Person> query = null;
            #region Retrieving last viewing movie

            switch (sessionData.selectCategory)
            {
                case "TopEnglish":
                    cate = "TopEnglish";
                    if (sessionData.lastEnglishMovie != null)
                        selectedMovie= sessionData.lastEnglishMovie;
                    break;
                case "TopBollywood":
                    cate = "TopBollywood";
                    if (sessionData.lastBollywoodMovie != null)
                        selectedMovie = sessionData.lastBollywoodMovie;
                    break;
                case "TopForeign":
                    cate = "TopForeign";
                    if (sessionData.lastForeignMovie != null)
                        selectedMovie = sessionData.lastForeignMovie;
                    break;
                case "TopAsian":
                    cate = "TopAsian";
                    if (sessionData.lastAsianMovie != null)
                        selectedMovie = sessionData.lastAsianMovie;
                    break;
                //default:
                //    CoverFlowControl.SelectedIndex = 20;
                //    break;

            }

            #endregion


                Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");
                XDocument xmlfile = XDocument.Load(storageFolder.Path + @"\" + cate + ".xml");

                if (sessionData.sortOrder == 0 && sessionData.filterGenere == null && sessionData.filterLang == null)
				{
					    query = (from movies in xmlfile.Root.Elements("movie")
                                select new Person
                                {
                                    Name = movies.Element("name").Value,                                   
                                    Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg",
                                    imdbID = Convert.ToInt32(movies.Element("imdbid").Value)
                                }).AsParallel().AsSequential();    
								
				}
                else if (sessionData.sortOrder == 0 && (sessionData.filterGenere != null || sessionData.filterLang != null))
                {
                         query = (from movies in xmlfile.Root.Elements("movie")
                                     select new Person
                                    {
                                		Genere = movies.Attribute("genere").Value,
                                        Language = movies.Attribute("language").Value,
                                        Name = movies.Element("name").Value,
                                        Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg",
                                        imdbID = Convert.ToInt32(movies.Element("imdbid").Value)
                                    }).AsParallel().AsSequential();                  
                    
                }
                if (sessionData.sortOrder != 0 && sessionData.filterGenere == null && sessionData.filterLang == null)
                {

                    switch (sessionData.sortOrder)
                        {
                            case 1:
                                query = (from movies in xmlfile.Root.Elements("movie")
                                         orderby movies.Attribute("release").Value
                                         select new Person
                                         {
                                             Name = movies.Element("name").Value,
                                             Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg",
                                             imdbID = Convert.ToInt32(movies.Element("imdbid").Value)
                                         }).AsParallel().AsSequential();
                                break;
                            case 2:
                                 query = (from movies in xmlfile.Root.Elements("movie")
                                         orderby movies.Attribute("release").Value descending
                                         select new Person
                                         {
                                             Name = movies.Element("name").Value,
                                             Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg",
                                             imdbID = Convert.ToInt32(movies.Element("imdbid").Value)
                                         }).AsParallel().AsSequential();
                                break;
                            case 3:
                                query = (from movies in xmlfile.Root.Elements("movie")
                                         orderby movies.Attribute("rating").Value
                                         select new Person
                                         {
                                             Name = movies.Element("name").Value,
                                             Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg",
                                             imdbID = Convert.ToInt32(movies.Element("imdbid").Value)
                                         }).AsParallel().AsSequential();
                                break;
                            case 4:
                                query = (from movies in xmlfile.Root.Elements("movie")
                                         orderby movies.Attribute("rating").Value descending
                                         select new Person
                                         {
                                             Name = movies.Element("name").Value,
                                             Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg",
                                             imdbID = Convert.ToInt32(movies.Element("imdbid").Value)
                                         }).AsParallel().AsSequential();
                                break;

                        }                

                }
                else if (sessionData.sortOrder != 0 && (sessionData.filterGenere != null || sessionData.filterLang != null))
                {
                    switch (sessionData.sortOrder)
                    {
                        case 1:
                            query = (from movies in xmlfile.Root.Elements("movie")
                                     orderby movies.Attribute("release").Value
                                     select new Person
                                     {
                                         Genere = movies.Attribute("genere").Value,
                                         Language = movies.Attribute("language").Value,
                                         Name = movies.Element("name").Value,
                                         Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg",
                                         imdbID = Convert.ToInt32(movies.Element("imdbid").Value)
                                     }).AsParallel().AsSequential();
                            break;
                        case 2:
                            query = (from movies in xmlfile.Root.Elements("movie")
                                     orderby movies.Attribute("release").Value descending
                                     select new Person
                                     {
                                         Genere = movies.Attribute("genere").Value,
                                         Language = movies.Attribute("language").Value,
                                         Name = movies.Element("name").Value,
                                         Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg",
                                         imdbID = Convert.ToInt32(movies.Element("imdbid").Value)
                                     }).AsParallel().AsSequential();
                            break;
                        case 3:
                            query = (from movies in xmlfile.Root.Elements("movie")
                                     orderby movies.Attribute("rating").Value
                                     select new Person
                                     {
                                         Genere = movies.Attribute("genere").Value,
                                         Language = movies.Attribute("language").Value,
                                         Name = movies.Element("name").Value,
                                         Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg",
                                         imdbID = Convert.ToInt32(movies.Element("imdbid").Value)
                                     }).AsParallel().AsSequential();
                            break;
                        case 4:
                            query = (from movies in xmlfile.Root.Elements("movie")
                                     orderby movies.Attribute("rating").Value descending
                                     select new Person
                                     {
                                         Genere = movies.Attribute("genere").Value,
                                         Language = movies.Attribute("language").Value,
                                         Name = movies.Element("name").Value,
                                         Image = "ms-appx:///Assets/" + cate + "/" + movies.Element("rank").Value + ".jpg",
                                         imdbID = Convert.ToInt32(movies.Element("imdbid").Value)
                                     }).AsParallel().AsSequential();
                            break;
                    }
                
                }

                if ((sessionData.filterLang != null || sessionData.filterGenere != null) && query != null)
                {
                		IEnumerable<Person> filteredQuery = null;
                        if (sessionData.filterLang != null && sessionData.filterGenere == null)
                        {
                        	     filteredQuery =  (from movies in query
                                                   where movies.Language.Contains(sessionData.filterLang)
 												   select new Person 
 													{
 													  Name = movies.Name,
                                     				  Image = movies.Image,
                                                      imdbID = movies.imdbID
 													}).AsParallel().AsSequential(); 	
                        }
                        else if (sessionData.filterLang == null && sessionData.filterGenere != null)
                        {
                    			  filteredQuery =  (from movies in query
                                                    where movies.Genere.Contains(sessionData.filterGenere)
 												   select new Person 
 													{
 													 Name = movies.Name,
                                        		     Image = movies.Image,
                                                     imdbID = movies.imdbID
 													}).AsParallel().AsSequential(); 	
                    
                        }
                        else if (sessionData.filterLang != null && sessionData.filterGenere != null)
                        {
                     			  filteredQuery =  (from movies in query
                                                    where movies.Genere.Contains(sessionData.filterGenere) && movies.Language.Contains(sessionData.filterLang)
 									               select new Person 
 													  {
 													    Name = movies.Name,
                                     				    Image = movies.Image,
                                                        imdbID = movies.imdbID
 													  }).AsParallel().AsSequential(); 	
                    
                        }
                    
                        elementCount = filteredQuery.Count();
                    
                        foreach (Person p in filteredQuery)
                        {

                            //Images.Add(p);
                            images.Add(p);
                            if (p.Name == selectedMovie)
                            {
                                newIndex = movieCounter;
                            }
                            movieCounter++;
                        }
                Tuple<ObservableCollection<Person>, int, int> _filteredImages = new Tuple< ObservableCollection<Person>, int, int>( images, elementCount, newIndex);



                return _filteredImages;
                    
                }
               

                    elementCount = query.Count();


                    foreach (Person p in query)
                    {

                        //Images.Add(p);
                        images.Add(p);
                        if (p.Name == selectedMovie)
                        {
                            newIndex = movieCounter;
                        }
                        movieCounter++;
                    }

                    Tuple< ObservableCollection<Person>, int, int> _images = new Tuple<ObservableCollection<Person>, int, int>(images, elementCount, newIndex);

                    return _images;
                

        }
    }
}