using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace TopMovies
{
    class GoogleTranslator
    {

        string[] tokens;
        string resultUrl;


        public async Task<string> Translator(string text, string toCulture, string fromCulture ="en")
        {
            fromCulture = fromCulture.ToLower();
            toCulture = toCulture.ToLower();

            // normalize the culture in case something like en-us was passed 
            // retrieve only en since Google doesn't support sub-locales
            tokens = fromCulture.Split('-');
            if (tokens.Length > 1)
                fromCulture = tokens[0];

            // normalize ToCulture
            tokens = toCulture.Split('-');
            if (tokens.Length > 1)
                toCulture = tokens[0];

            string Url = string.Format(@"http://translate.google.com/translate_a/t?client=j&text={0}&hl=en&sl={1}&tl={2}",Uri.EscapeUriString(text),fromCulture,toCulture);

            // Retrieve Translation with HTTP GET call


    try
    {
        HttpClient web = new HttpClient();

        //   System.Net.HttpRequestHeader.UserAgent

        // web.D

        web.DefaultRequestHeaders.Add("UserAgent","Mozilla/5.0");

        web.DefaultRequestHeaders.Add("AcceptCharset","UTF-8");
        

        var  html = await web.GetStringAsync(Url);

        resultUrl = html;



     }
      catch (Exception ex)
        {
           ex.Message.ToString();
        }  


            string result = Regex.Match(resultUrl, "trans\":(\".*?\"),\"",RegexOptions.IgnoreCase).Groups[1].Value;

            if (string.IsNullOrEmpty(result))
            {
               var  ErrorMessage = "Invalid search result. Couldn't find marker.";
                return null;
            }

            var _Bytes = Encoding.Unicode.GetBytes(result);

            using (MemoryStream _Stream = new MemoryStream(_Bytes))
               
            {
                var _Serializer = new DataContractJsonSerializer(typeof(string));

                    return (string)_Serializer.ReadObject(_Stream);
             }


          
                

    }

        //   public static T Deserialize <T>(string json)
        //{
        //    var _Bytes = Encoding.Unicode.GetBytes(json);
        //    using (MemoryStream _Stream = new MemoryStream(_Bytes))
        //    {
        //        var _Serializer = new DataContractJsonSerializer(typeof(T));
        //        return (T)_Serializer.ReadObject(_Stream);
        //    }
        //}


    }

}
