using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace TopMovies
{
    /// <summary>
    /// This class is responsible for getting movie information from various IMDB Api avaliable over the internet .                                                                           
    /// </summary>
    public class MovieInformation
    {
        //Define variables and constants.
        string source;
        string content;
        string imdbRating;
        string imdbVotesCount;
        string tomatoRating;
        string tomatoCount;
        string requestUrl;
        string imdbID;
        string userLanguage;
        bool downloadSucceeded= true;
        string Omdb = @"http://www.omdbapi.com/?plot=full&r=xml&tomatoes=true&i=tt";
        string myApi = @"http://mymovieapi.com/?type=json&plot=full&episode=0&lang=en-US&aka=simple&release=simple&business=0&tech=0&id=tt";
        HttpWebRequest request;
        Tuple<string, string, string,string> result = new Tuple<string, string, string,string>(null, null, null,null);
        Tuple<string, string, string,string> myApiresult = new Tuple<string, string, string,string>(null, null, null,null);
        string ratingInfo;
        string tempArticle = null;
        string additionalInformation;
        WebResponse response;


        /// <summary>
        /// this function gets the movie infromation from omdb api .
        /// </summary>
        /// <param name="movieID">imdb id of the movie . </param>
        /// <returns></returns>
        async public Task<Tuple<string,string,string,string>> GetMovieInfo(string movieID)
        {

            if (movieID.Length < 7)
            {
                string modifiedID = movieID;
                for (int i = movieID.Length; i < 7; i++)
                {
                    modifiedID = "0" + modifiedID;
                }

                imdbID = modifiedID;
            }
            else
            {
                imdbID = movieID;
            }


            requestUrl = Omdb + imdbID;
            try
            {
                    
                    request = (HttpWebRequest)WebRequest.Create(requestUrl);
                
                    request.Method = "GET";

                    using (response = await request.GetResponseAsync())
                    {
                        XDocument omdbInfo = XDocument.Load(response.GetResponseStream());


                        if (omdbInfo.Root.Attribute("response").Value == "True")
                        {
                            source = "OMDB";
                            content = omdbInfo.Root.Element("movie").Attribute("director").Value + "|" + omdbInfo.Root.Element("movie").Attribute("writer").Value + "|" + omdbInfo.Root.Element("movie").Attribute("actors").Value + "|" + omdbInfo.Root.Element("movie").Attribute("plot").Value;
                            imdbRating = omdbInfo.Root.Element("movie").Attribute("imdbRating").Value;
                            imdbVotesCount = omdbInfo.Root.Element("movie").Attribute("imdbVotes").Value;
                            tomatoRating = omdbInfo.Root.Element("movie").Attribute("tomatoUserRating").Value;
                            tomatoCount = omdbInfo.Root.Element("movie").Attribute("tomatoUserReviews").Value;
                            ratingInfo = imdbRating + "|" + imdbVotesCount + "|" + tomatoRating + "|" + tomatoCount;
                            additionalInformation = omdbInfo.Root.Element("movie").Attribute("year").Value + "|" + omdbInfo.Root.Element("movie").Attribute("runtime").Value + "|" + omdbInfo.Root.Element("movie").Attribute("genre").Value;
                            result = new Tuple<string, string, string, string>(source, content, ratingInfo, additionalInformation);
                        }
                        else
                        {   //If there was some error to getting information from omdb then get information using myapi.
                            downloadSucceeded = false;
                        }
                    }
            }
            catch(Exception ex)
            {
                string error = ex.Message;
                downloadSucceeded = false;
            }

            if (!downloadSucceeded)
            {
                //If there was some error to getting information from omdb then get information using myapi.
                result = await myApiInformation(imdbID);
            }
            
            return result;
        
        }

        /// <summary>
        /// This function is to get the movie information from myapi . 
        /// </summary>
        /// <param name="movieID">the imdb id of the movie</param>
        /// <returns></returns>
        async private Task<Tuple<string, string, string,string>> myApiInformation(string movieID)
        {
            string generes = null;
            string runtime = null;
            string actors = null;
            string directors = null;
            string writers = null;
            string language = null;

            downloadSucceeded = true;
            try
            {
                requestUrl = myApi + movieID;
                request = (HttpWebRequest)WebRequest.Create(requestUrl);
                using (response = await request.GetResponseAsync())
                {
                    
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string tempContent = await streamReader.ReadToEndAsync();
                        if (!tempContent.Contains("error"))
                        {
                            source = "myApi";
                            JObject obj = JObject.Parse(tempContent);
                            actors = Regex.Replace(obj["actors"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');
                            writers = Regex.Replace(obj["writers"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');
                            directors = Regex.Replace(obj["directors"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');
                            generes = Regex.Replace(obj["genres"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');
                            runtime = Regex.Replace(obj["runtime"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');
                            language = Regex.Replace(obj["language"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');

                            content = directors + "|" + writers + "|" + actors + "|" + obj["plot"].ToString();
                            imdbRating = obj["rating"].ToString();
                            imdbVotesCount = obj["rating_count"].ToString();
                            ratingInfo = imdbRating + "|" + imdbVotesCount;
                            additionalInformation = obj["year"].ToString() + "|" + runtime + "|" + generes + "|" + language;
                            myApiresult = new Tuple<string, string, string, string>(source, content, ratingInfo, additionalInformation);
                        }
                        else
                        {
                            downloadSucceeded = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                //downloadSucceeded = false;
            }

            //if (!downloadSucceeded)
            //{
            //    //If there was some error to getting information from myapi then get information using tmdb.
            //    myApiresult = new Tuple<string, string, string>(null, null, null);
            //}

            return myApiresult;
        }

        //private Task<Tuple<string, string, string>> TmdbInformation(string imdbID)
        //{
        //    downloadSucceeded = true;
        //    try
        //    {
        //    }
        //    catch (Exception ex)
        //    {
 
        //    }
           
        //}
        
        /// <summary>
        /// This function is to get the wikipedia article for particular movie in the user navitve language if avaliable . 
        /// </summary>
        /// <param name="articleName">the movie name to be found</param>
        /// <param name="year">year of the movie release</param>
        /// <returns></returns>
        async public Task<string> WikiPediaArticleFinder(string articleName, int year)
        {
            Regex title = new Regex(articleName,RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);
            userLanguage = new Windows.ApplicationModel.Resources.Core.ResourceContext().Languages.FirstOrDefault();
            string firstQuery = @"https://en.wikipedia.org/w/api.php?action=query&list=search&format=json&srlimit=1&srprop=snippet&srsearch=" + articleName + "+incategory:" + year + "_films";
            request = WebRequest.Create(firstQuery) as HttpWebRequest;
            using (response = await request.GetResponseAsync())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string content = await reader.ReadToEndAsync();
                    JObject obj = JObject.Parse(content);
                    var firstResult = obj.Last.ElementAt(0)["search"];
                    foreach (JToken returnResult in firstResult)
                    {
                        //if (returnResult["title"].ToString().Contains(articleName) || returnResult["title"].ToString().Contains(articleName + " (film)") || articleName.Contains(returnResult["title"].ToString()))
                        //{
                        //    tempArticle = (string)returnResult["title"];
                        //    break;
                        //}
                        if (title.IsMatch(returnResult["title"].ToString())||Regex.IsMatch(returnResult["title"].ToString(),articleName))
                        {
                            tempArticle = (string)returnResult["title"];
                            break;
                        }
                    }

                    if (tempArticle == null)
                    {

                        string secondQuery = @"https://en.wikipedia.org/w/api.php?action=query&list=search&format=json&srlimit=5&srprop=snippet&srsearch=" + articleName;
                        request = WebRequest.Create(secondQuery) as HttpWebRequest;
                        using (response = await request.GetResponseAsync())
                        {
                            using (StreamReader secondReader = new StreamReader(response.GetResponseStream()))
                            {
                                obj = JObject.Parse(await secondReader.ReadToEndAsync());
                                var secondResult = obj.Last.ElementAt(0)["search"];
                                foreach (JToken returnResult in secondResult)
                                {
                                    if ((Regex.IsMatch(returnResult["title"].ToString(),articleName) || returnResult["title"].ToString().Contains(articleName + " (film)")) && !returnResult["snippet"].ToString().Contains("may refer to"))
                                    {
                                        tempArticle = (string)returnResult["title"];
                                        break;
                                    }
                                }
                            }
                        }
                    }


                    if (tempArticle == null)
                    {
                        foreach (JToken returnResult in firstResult)
                        {
                            if (title.IsMatch(returnResult["snippet"].ToString()) || Regex.IsMatch(returnResult["snippet"].ToString(),"<span class='searchmatch'>"))
                            {
                                tempArticle = (string)returnResult["title"];
                                break;
                            }
                        }
                    }
                }
            }

            if(!userLanguage.Contains("en"))
            {
                string langUrl = @"https://en.wikipedia.org/w/api.php?action=parse&format=xml&page="+tempArticle+"&prop=langlinks";
                request = WebRequest.Create(langUrl) as HttpWebRequest;
                using (response = await request.GetResponseAsync())
                {
                    XDocument doc = XDocument.Load(response.GetResponseStream());
                    XElement node1 = (XElement)doc.Root.LastNode;
                    if (userLanguage.Length > 2)
                    {
                        userLanguage = userLanguage.Remove(2);
                    }
                    try
                    {
                        var urlDictionary = (from ll in node1.Elements("langlinks").Elements()
                                             where ll.Attribute("lang").Value.Contains(userLanguage)
                                             select ll.Attribute("url").Value).FirstOrDefault();

                        return urlDictionary.ToString().Replace("wikipedia.org", "m.wikipedia.org");
                    }
                    catch (Exception ex)
                    {
                        string code = ex.Message;
                    }
                }
            }

            return ("https://en.m.wikipedia.org/wiki/" + tempArticle);

        }
    }
}
