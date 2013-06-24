using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Diagnostics;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace QuotesOfWisdom.Common
{
    public class Flickr : IPagedSource<FlickrPhoto>
    {
        public Flickr()
        { }

        public async Task<IPagedResponse<FlickrPhoto>> GetPage(string query, int pageIndex, int pageSize)
        {
            try
            {
                if (pageIndex < 1)
                    throw new ArgumentOutOfRangeException("pageIndex");
                if (pageSize < 1)
                    throw new ArgumentOutOfRangeException("pageSize");

                query += "&page=" + pageIndex;

                HttpClient client = new HttpClient();
                var response = await client.GetAsync(query);

                var jsonbgImageresult = await response.Content.ReadAsStringAsync();

                jsonbgImageresult = jsonbgImageresult.Replace(@"\", "");

                string desppattern = @"(?<=""description"":"").*?(?="",""times_viewed"")";

                string despoutresult = Regex.Replace(jsonbgImageresult, desppattern, match => match.Value.Replace("\"", ""));

                string namepattern = @"(?<=""name"":"").*?(?="",""description"")";

                string nameoutresult = Regex.Replace(despoutresult, namepattern, match => match.Value.Replace("\"", ""));

                var bgItems = JsonConvert.DeserializeObject<BGImageListings>(nameoutresult);

                int virtualCount;

                if (!int.TryParse(bgItems.total_items.ToString(), out virtualCount))
                    virtualCount = 1000;

                return new FlickrResponse(from p in bgItems.photos
                                          select new FlickrPhoto
                                          {
                                              ImageURLsmall = p.images[0].url,
                                              ImageURLbig = p.images[1].url
                                          }, virtualCount);
                
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message.ToString());
                throw new Exception(ex.Message.ToString());
            }
        }

    }

    public class BGImageListings
    {
        public string total_items { get; set; }
        public photos[] photos = { };
    }

    public class photos
    {
        public images[] images = { };
    }

    public class images
    {
        public string size { get; set; }
        public string url { get; set; }
    }

    [DebuggerDisplay("ImageURLsmall = {ImageURLsmall}")]
    public class FlickrPhoto
    {
        public string ImageURLsmall { get; set; }
        public string ImageURLbig { get; set; }
    }    

    [DebuggerDisplay("PageIndex = {PageIndex} - PageSize = {PageSize} - VirtualCount = {VirtualCount}")]
    public class FlickrResponse : IPagedResponse<FlickrPhoto>
    {
        public FlickrResponse(IEnumerable<FlickrPhoto> items, int virtualCount)
        {
            this.Items = items;
            this.VirtualCount = virtualCount;
        }
        
        public int VirtualCount { get; private set; }
        public IEnumerable<FlickrPhoto> Items { get; private set; }
    }
}
