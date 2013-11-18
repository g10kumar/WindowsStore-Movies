using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace TopMovies
{
    class GoogleTranslator
    {

        string[] tokens;
        string resultUrl;
        string source;

        public GoogleTranslator(string source)
        {
            this.source = source;
        }


        public async Task<string> Translator(string text, string toCulture, string fromCulture = "en")
        {
            string Url;
            bool postRequest = false;
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

            if (text.Length < 1200)
            {
                Url = string.Format(@"http://translate.google.com/translate_a/t?client=j&ie=UTF-8&oe=UTF-8&text={0}&hl=en&sl={1}&tl={2}", Uri.EscapeUriString(text), fromCulture, toCulture);
            }
            else
            {
                postRequest = true;
                Url = string.Format(@"http://translate.google.com/translate_a/t?client=j&ie=UTF-8&oe=UTF-8&hl=en&sl={0}&tl={1}&uptl={1}&alttl={0}&ssel=0&tsel=0", fromCulture, toCulture);
            }

            // Retrieve Translation with HTTP GET call

            using (HttpClient web = new HttpClient())
            {
                try
                {
                    web.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0");
                    web.DefaultRequestHeaders.Add("AcceptCharset", "UTF-8");
                    web.DefaultRequestHeaders.Add("Accept", "*/*");

                    if (postRequest)
                    {
                        StringContent textToTranslate = new StringContent("q=" + text.Replace(" ", "%20"));
                        textToTranslate.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded;charset=UTF-8");
                        var html = await web.PostAsync(Url, textToTranslate);
                        resultUrl = await html.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        var html = await web.GetStringAsync(Url);
                        resultUrl = html.ToString();
                    }

                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            if (source == "infoPage")
            {
                StringBuilder translatedText = new StringBuilder();
                JObject obj = JObject.Parse(resultUrl);
                foreach (JToken data in obj["sentences"])
                {
                    translatedText.Append(((JProperty)data.First).Value.ToString());
                }

                return translatedText.ToString();
            }

            string result = Regex.Match(resultUrl, "trans\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value;


            if (string.IsNullOrEmpty(result))
            {
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
