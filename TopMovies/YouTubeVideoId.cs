using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace TopMovies
{


    class YouTubeVideoId
    {
        //Define variable & constants
        string hollywoodPlaylist = "PLrnMXztgruUBql8IQRVWqWqzCt-WaYwvF";
        string asianPlaylist = "PLrnMXztgruUBYbibpjfKyC0lLU_-nAiCA";
        string bollyWoodPlaylist = "PLrnMXztgruUCSqbHBZH_gpCNdWuF9dgsB";
        string foreignPlaylistOne = "PLrnMXztgruUAo4dAHBWcfFLtdsvM8lksV";
        string foreignPlaylistTwo = "PLrnMXztgruUBnatsaq9j5XmblNyKD6YSw";
        HttpWebRequest request;
        WebResponse response;

       

        public async Task<string> getVideoId(int position)
        {
            try
            {               
                string youtubeResponse = null;
                string youtubeRequest = CreateRequest(sessionData.selectCategory, position);
                if (youtubeRequest != null)
                {
                    youtubeResponse = await MakeRequest(youtubeRequest);
                }

                return youtubeResponse;
                
            }
            catch(Exception ex)
            {
                string ecd =ex.StackTrace;
                return null;
            }
            

        }

        private async Task<string> MakeRequest(string youtubeRequest)
        {
            string videoId;
            //HttpClient client = new HttpClient();
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, youtubeRequest);
            //request.Headers.UserAgent.ParseAdd("Mozilla/5.0");
            //request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            //request.Headers.AcceptLanguage.ParseAdd("en-US,en;q=0.8");

            //HttpResponseMessage response = await client.SendAsync(request);
            request = (HttpWebRequest)WebRequest.Create(youtubeRequest);
            using (response = await request.GetResponseAsync())
            {
                XDocument doc = XDocument.Load(response.GetResponseStream());
                string url = (string)doc.Descendants("{http://search.yahoo.com/mrss/}player").Attributes().First();
                videoId = url.Substring(url.IndexOf("=") + 1, (url.IndexOf("&") - url.IndexOf("=") - 1));
            }
          
            //XDocument xmlDoc = new XDocument(response.Content);
            //string videoid = Regex.Match(url,
            return videoId;
        }

        private string CreateRequest(string category, int youtubePosition)
        {
            string urlRequest = null;

            if (category.Contains("English"))
            {
                urlRequest = "https://gdata.youtube.com/feeds/api/playlists/" + hollywoodPlaylist + "?max-results=1&&prettyprint=true&fields=entry(media:group(media:player))&start-index=" + youtubePosition;
            }
            else if (category.Contains("Bollywood"))
            {
                urlRequest = "https://gdata.youtube.com/feeds/api/playlists/" + bollyWoodPlaylist + "?max-results=1&&prettyprint=true&fields=entry(media:group(media:player))&start-index=" + youtubePosition;
            }
            else if (category.Contains("Asian"))
            {
                urlRequest = "https://gdata.youtube.com/feeds/api/playlists/" + asianPlaylist + "?max-results=1&&prettyprint=true&fields=entry(media:group(media:player))&start-index=" + youtubePosition;
            }
            else if (category.Contains("Foreign") && youtubePosition <= 150)
            {
                urlRequest = "https://gdata.youtube.com/feeds/api/playlists/" + foreignPlaylistOne + "?max-results=1&&prettyprint=true&fields=entry(media:group(media:player))&start-index=" + youtubePosition;
            }
            else if (category.Contains("Foreign") && youtubePosition > 150)
            {
                youtubePosition = youtubePosition - 150;
                urlRequest = "https://gdata.youtube.com/feeds/api/playlists/" + foreignPlaylistTwo + "?max-results=1&&prettyprint=true&fields=entry(media:group(media:player))&start-index=" + youtubePosition;
            }

            return urlRequest;
        }
    }
}
