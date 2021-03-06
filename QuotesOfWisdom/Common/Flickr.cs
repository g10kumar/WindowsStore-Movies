﻿using System;
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
                
                //if (query.Contains("term"))
                //{
                //    sessionData.searchKeyWord = "keyword";
                //}
                //else
                //{
                //    sessionData.searchKeyWord = "normal";
                //}
                //sessionData.totalImages = pageIndex;
                //HttpClient client = new HttpClient();
                var handler = new HttpClientHandler { AllowAutoRedirect = false };
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36");

                var randString = System.Guid.NewGuid();

                var response = await client.GetAsync(query + "&" + randString);

                var jsonbgImageresult = await response.Content.ReadAsStringAsync();

                jsonbgImageresult = jsonbgImageresult.Replace(@"\", "");

                string desppattern = @"(?<=""description"":"").*?(?="",""times_viewed"")";

                string despoutresult = Regex.Replace(jsonbgImageresult, desppattern, match => match.Value.Replace("\"", ""));

                string namepattern = @"(?<=""name"":"").*?(?="",""description"")";

                string nameoutresult = Regex.Replace(despoutresult, namepattern, match => match.Value.Replace("\"", ""));

                var bgItems = JsonConvert.DeserializeObject<BGImageListings>(nameoutresult);

                //sessionData.totalImages = Convert.ToInt32(bgItems.total_items.ToString());
                //Title = p.name.ToString(),
                //UserName = p.user.firstname.ToString() + " " + p.user.lastname.ToString()

                int virtualCount;

                if (!int.TryParse(bgItems.total_items.ToString(), out virtualCount))
                    virtualCount = 1000;

                List<BGImages> bglist = new List<BGImages>();

                for (int i = 0; i < bgItems.photos.Count(); i++)
                {
                    BGImages s = new BGImages();

                    s.ImageURLsmall = bgItems.photos[i].images[0].url.ToString().Replace("2.jpg", "3.jpg");
                    s.ImageURLbig = bgItems.photos[i].images[0].url.ToString().Replace("2.jpg", "4.jpg");
                    if (bgItems.photos[i].name.ToString() != "Untitled")
                    {
                        s.Title = bgItems.photos[i].name.ToString();
                    }
                    else
                    {
                        s.Title = "";
                    }

                    string userName = "";

                    if (bgItems.photos[i].user.firstname != null)
                    {
                        userName += bgItems.photos[i].user.firstname.ToString();
                    }

                    if (bgItems.photos[i].user.lastname != null)
                    {
                        userName += " " + bgItems.photos[i].user.lastname.ToString();
                    }

                    s.UserName = userName;

                    bglist.Add(s);
                    s = null;
                }

                return new FlickrResponse(from p in bglist
                                          select new FlickrPhoto
                                          {
                                              ImageURLsmall = p.ImageURLsmall.ToString(),
                                              ImageURLbig = p.ImageURLbig.ToString(),
                                              Title = p.Title.ToString(),
                                              UserName = p.UserName.ToString()
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
        public string name { get; set; }
        public images[] images = { }; 
        public user user = new user();
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
        public string UserName { get; set; }
        public string Title { get; set; }
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
