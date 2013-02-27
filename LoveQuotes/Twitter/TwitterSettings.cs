using System;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
//using Windows.UI.Xaml.Ink;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace LoveQuotes
{
    public class TwitterSettings
    {
        #region Old Values
        //public static string RequestTokenUri = "https://api.twitter.com/oauth/request_token";
        //public static string AuthorizeUri = "https://api.twitter.com/oauth/authorize";
        //public static string AccessTokenUri = "https://api.twitter.com/oauth/access_token";
        //public static string CallbackUri = "http://www.google.com";
        ////public static string CallbackUri = "oob";
        //public static string StatusUpdateUrl { get { return "https://api.twitter.com"; } }

        ////TODO REGISTER YOUR APP WITH TWITTER TO GET YOUR KEYS AND FILL THEM IN HERE

        ////public static string consumerKey = "x1xzUqPAxq2T8NDfVvyv0w";
        ////public static string consumerKeySecret = "en2679ibYehXsKexTHnkSmJfRS0knvYRCqQ2Pl6kLo";

        //public static string consumerKey = "I0sAuRSpfqhpLTD2yvltQ";
        //public static string consumerKeySecret = "VoAfQiKgwx8SD5iflDxmxRCzFD7yX1qtEWiYP5gRo";

        //public static string oAuthVersion = "1.0a";
        #endregion

        #region Initializing the Twitter object values
        public static string RequestTokenUri = "https://api.twitter.com/oauth/request_token";
        public static string AuthorizeUri = "https://api.twitter.com/oauth/authorize";
        public static string AccessTokenUri = "https://api.twitter.com/oauth/access_token";
        public static string CallbackUri = "http://www.daksatech.com";
        //public static string CallbackUri = "oob";
        public static string StatusUpdateUrl { get { return "https://api.twitter.com"; } }

        //TODO REGISTER YOUR APP WITH TWITTER TO GET YOUR KEYS AND FILL THEM IN HERE

        public static string consumerKey = "EhY3grAWGzIxF0YXPr70Yw";
        public static string consumerKeySecret = "uwXl6wNaLowV9No68bNHtQNbMhSpSSgXCXEk4P0g";

        public static string oAuthVersion = "1.0a";
        #endregion
    }
}
