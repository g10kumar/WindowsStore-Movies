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
using System.Text;
using System.Collections.Generic;
//using System.IO.IsolatedStorage;

namespace LoveQuotes
{
    public class HelperMethods
    {

        /// <summary>
        /// Method for split the response and adds to Dictionary variable and 
        /// returns that Dictionary variable
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetQueryParameters(string response)
        {
            // Declaring the dictionary variable
            Dictionary<string, string> nameValueCollection = new Dictionary<string, string>();
                       
            // split the response with "&"
            string[] items = response.Split('&');

            // loop through each item in the Items array
            foreach (string item in items)
            {
                // checks whether item contains "="
                if (item.Contains("="))
                {
                    // split element in the item with "=" and binds to dictionary varible
                    string[] nameValue = item.Split('=');
                    if (nameValue[0].Contains("?"))
                        nameValue[0] = nameValue[0].Replace("?", "");
                    nameValueCollection.Add(nameValue[0], System.Net.WebUtility.UrlDecode(nameValue[1]));
                }
            }
            return nameValueCollection;
        }

        /// <summary>
        /// UrlEncode method of given value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(string value)
        {
            // checks whether value is null
            if (value == null)
            {
                return null;
            }

            // creating StringBuilder object
            StringBuilder result = new StringBuilder();

            // loop through each symbol in the value
            foreach (char symbol in value)
            {
                if ("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~".IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + string.Format("{0:X2}", (int)symbol));
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Generic Method of getting the global elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static T GetKeyValue<T>(string key)
        {
            //if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            //    return (T)IsolatedStorageSettings.ApplicationSettings[key];
            //else
                return default(T);
        }

        /// <summary>
        /// Generic Method of setting the global elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static void SetKeyValue<T>(string key, T value)
        {
            //if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
            //    IsolatedStorageSettings.ApplicationSettings[key] = value;
            //else
            //    IsolatedStorageSettings.ApplicationSettings.Add(key, value);
            //IsolatedStorageSettings.ApplicationSettings.Save();
        }
    }

    /// <summary>
    /// Class definition for TwitterItem
    /// </summary>
    public class TwitterItem
    {
        public string UserName { get; set; }
        public string ImageSource { get; set; }
        public string Tweet { get; set; }
    }
}
