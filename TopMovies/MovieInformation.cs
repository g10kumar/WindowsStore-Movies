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
        bool downloadSucceeded= true;
        string Omdb = @"http://www.omdbapi.com/?plot=full&r=xml&tomatoes=true&i=tt";
        string myApi = @"http://mymovieapi.com/?type=json&plot=full&episode=0&lang=en-US&aka=simple&release=simple&business=0&tech=0&id=tt";
        HttpWebRequest request;
        Tuple<string, string, string,string> result = new Tuple<string, string, string,string>(null, null, null,null);
        Tuple<string, string, string,string> myApiresult = new Tuple<string, string, string,string>(null, null, null,null);
        string ratingInfo;
        string additionalInformation;

        //string mymovieApi=



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


                    var response = await request.GetResponseAsync();
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
                        result =  new Tuple<string, string, string,string>(source, content, ratingInfo,additionalInformation);
                    }
                    else
                    {   //If there was some error to getting information from omdb then get information using myapi.
                        downloadSucceeded = false;
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

        async public Task<Tuple<string, string, string,string>> myApiInformation(string movieID)
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
                var response = await request.GetResponseAsync();
            
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string tempContent = await streamReader.ReadToEndAsync();
                    if (!tempContent.Contains("error"))
                    {
                        source = "myApi";
                        JObject obj = JObject.Parse(tempContent);
                        //foreach (string actor in (JArray)obj["actors"])
                        //{
                        //    actors = actors + actor + ",";
                        //}
                        actors = Regex.Replace(obj["actors"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');
                        //foreach (string writer in (JArray)obj["writers"])
                        //{

                        //    writers = writers + writer + ",";
                        //}
                        writers = Regex.Replace(obj["writers"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');
                        //foreach (string director in (JArray)obj["directors"])
                        //{

                        //    directors = directors + director + ",";
                        //}
                        directors = Regex.Replace(obj["directors"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');
                        //foreach (string genre in (JArray)obj["genres"])
                        //{

                        //    generes = generes + genre + ",";
                        //}
                        generes = Regex.Replace(obj["genres"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');
                        //foreach (string time in (JArray)obj["runtime"])
                        //{

                        //    runtime = runtime + time + ",";
                        //}
                        runtime = Regex.Replace(obj["runtime"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');
                        //foreach (string lang in (JArray)obj["language"])
                        //{

                        //    language = language + lang;
                        //}
                        language = Regex.Replace(obj["language"].ToString(), @"\r|\n|[\[\]]", "").Replace("\"", "").TrimStart(' ');

                        content = directors + "|" + writers + "|" + actors + "|" + obj["plot"].ToString();
                        imdbRating = obj["rating"].ToString();
                        imdbVotesCount = obj["rating_count"].ToString();
                        ratingInfo = imdbRating + "|" + imdbVotesCount;
                        additionalInformation = obj["year"].ToString() + "|" + runtime + "|" + generes + "|" + language;
                        myApiresult= new Tuple<string, string, string,string>(source, content, ratingInfo,additionalInformation);
                    }
                    else
                    {
                        downloadSucceeded = false;
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
        
    }
}
