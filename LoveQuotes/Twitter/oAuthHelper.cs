//using System;
//using System.Net;
//using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Documents;
////using Windows.UI.Xaml.Ink;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Media.Animation;
//using Windows.UI.Xaml.Shapes;
//using Hammock.Authentication.OAuth;
//using Hammock.Web;

//namespace LoveQuotes
//{
//    public class oAuthHelper
//    {
//        /// <summary>
//        /// static method for Initializing the twitter objects from twitter settings
//        /// </summary>
//        /// <returns></returns>
//        internal static OAuthWebQuery GetRequestTokenQuery()
//        {
//            var oauth = new OAuthWorkflow
//            {
//                ConsumerKey = TwitterSettings.consumerKey,
//                ConsumerSecret = TwitterSettings.consumerKeySecret,
//                SignatureMethod = OAuthSignatureMethod.HmacSha1,
//                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
//                RequestTokenUrl = TwitterSettings.RequestTokenUri,
//                Version = TwitterSettings.oAuthVersion,
//                CallbackUrl = TwitterSettings.CallbackUri
//            };

//            var info = oauth.BuildRequestTokenInfo(WebMethod.Get);
//            var objOAuthWebQuery = new OAuthWebQuery(info);
//            objOAuthWebQuery.HasElevatedPermissions = true;
//            objOAuthWebQuery.SilverlightUserAgentHeader = "Hammock";
//            objOAuthWebQuery.SilverlightMethodHeader = "GET";
//            return objOAuthWebQuery;
//        }

//        /// <summary>
//        /// static method for Initializing the twitter objects from requestToken
//        /// </summary>
//        /// <param name="requestToken"></param>
//        /// <param name="RequestTokenSecret"></param>
//        /// <param name="oAuthVerificationPin"></param>
//        /// <returns></returns>
//        internal static OAuthWebQuery GetAccessTokenQuery(string requestToken, string RequestTokenSecret, string oAuthVerificationPin)
//        {
//            var oauth = new OAuthWorkflow
//            {
//                AccessTokenUrl = TwitterSettings.AccessTokenUri,
//                ConsumerKey = TwitterSettings.consumerKey,
//                ConsumerSecret = TwitterSettings.consumerKeySecret,
//                ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
//                SignatureMethod = OAuthSignatureMethod.HmacSha1,
//                Token = requestToken,
//                Verifier = oAuthVerificationPin,
//                Version = TwitterSettings.oAuthVersion
//            };

//            var info = oauth.BuildAccessTokenInfo(WebMethod.Post);
//            var objOAuthWebQuery = new OAuthWebQuery(info);
//            objOAuthWebQuery.HasElevatedPermissions = true;
//            objOAuthWebQuery.SilverlightUserAgentHeader = "Hammock";
//            objOAuthWebQuery.SilverlightMethodHeader = "GET";
//            return objOAuthWebQuery;
//        }
//    }
//}
