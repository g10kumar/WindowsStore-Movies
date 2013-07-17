using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using QuotesOfWisdom.Data;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;

namespace QuotesOfWisdom.Common
{
    public static class Utilities
    {


        /// <summary>
        /// Method for returning Quotation for trimming the quotation for passing input length
        /// </summary>
        /// <param name="strParam"></param>
        /// <param name="iLen"></param>
        /// <returns></returns>
        public static String GetTeaser(String strParam, int iLen)
        {
            if (iLen > 0 && iLen < strParam.Length)
                return strParam.Substring(0, iLen) + "...";
            else
                return strParam;
        }

        /// <summary>
        /// Deserializing the JSON string method for Quotes
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<Quotations> DeserializeToListOfQuotations(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<Quotations>));
                return (List<Quotations>)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// Hex Color method
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>returns the RGB value of hex string</returns>
        public static Windows.UI.Color HexColor(String hex)
        {
            //remove the # at the front
            hex = hex.Replace("#", "");

            byte a = 255;
            byte r = 255;
            byte g = 255;
            byte b = 255;

            int start = 0;

            //handle ARGB strings (8 characters long)
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                start = 2;
            }

            //convert RGB characters to bytes
            r = byte.Parse(hex.Substring(start, 2), System.Globalization.NumberStyles.HexNumber);
            g = byte.Parse(hex.Substring(start + 2, 2), System.Globalization.NumberStyles.HexNumber);
            b = byte.Parse(hex.Substring(start + 4, 2), System.Globalization.NumberStyles.HexNumber);

            return Windows.UI.Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// GetColor method
        /// </summary>
        /// <param name="colorValue"></param>
        /// <returns>returns the color code based on the color value</returns>
        public static Windows.UI.Color GetColor(string colorValue)
        {
            if (colorValue != null)
            {
                return HexColor(colorValue);
            }
            else
            {
                return Windows.UI.Color.FromArgb(255, 255, 0, 0);
            }

            //switch (colorValue)
            //{
            //    case "Orange":
            //        return Windows.UI.Color.FromArgb(255, 39, 115, 230);
            //    case "Green":
            //        return Windows.UI.Colors.AliceBlue;
            //    case "Blue":
            //        return Windows.UI.Color.FromArgb(255,39,115,230);
            //    case "Maroon":
            //        return Windows.UI.Colors.Aqua;
            //    case "Brown":
            //        return Windows.UI.Colors.Aquamarine;
            //    case "Red":
            //        return Windows.UI.Colors.Azure;
            //    case "Beige":
            //        return Windows.UI.Colors.Beige;
            //    case "Bisque":
            //        return Windows.UI.Colors.Bisque;
            //    case "Black":
            //        return Windows.UI.Colors.Black;
            //    case "BlanchedAlmond":
            //        return Windows.UI.Colors.BlanchedAlmond;
            //    //case "Blue":
            //    //    return Windows.UI.Colors.Blue;
            //    case "Olive":
            //        return Windows.UI.Colors.Olive;
            //    default:
            //        return Windows.UI.Colors.Blue;
            //}
        }

        /// <summary>
        /// GetLayout method
        /// </summary>
        /// <param name="colorValue"></param>
        /// <returns>returns the layout based on the color value</returns>
        public static string GetLayout(string colorValue)
        {

            switch (colorValue.ToLower())
            {
                case "#fff2b100":
                    return "layoutBlockStyle_f2b100";
                case "#ff76b800":
                    return "layoutBlockStyle_76b800";
                case "#ff2471ea":
                    return "layoutBlockStyle_2471ea";
                case "#ffac0f3b":
                    return "layoutBlockStyle_ac0f3b";
                case "#ff632f00":
                    return "layoutBlockStyle_632f00";
                case "#ffae1c00":
                    return "layoutBlockStyle_ae1c00";
                case "#ffc0004e":
                    return "layoutBlockStyle_c0004e";
                case "#ff7100ab":
                    return "layoutBlockStyle_7100ab";
                case "#ff4516b3":
                    return "layoutBlockStyle_4516b3";
                case "#ff0069c0":
                    return "layoutBlockStyle_0069c0";
                case "#ff004d60":
                    return "layoutBlockStyle_004d60";
                case "#ff179700":
                    return "layoutBlockStyle_179700";
                case "#ff00c13f":
                    return "layoutBlockStyle_00c13f";
                case "#fffd961b":
                    return "layoutBlockStyle_fd961b";
                case "#fffe2d11":
                    return "layoutBlockStyle_fe2d11";
                case "#fffe1c76":
                    return "layoutBlockStyle_fe1c76";
                case "#ffa83efd":
                    return "layoutBlockStyle_a83efd";
                case "#ff1dacfd":
                    return "layoutBlockStyle_1dacfd";
                case "#ff55c4fe":
                    return "layoutBlockStyle_55c4fe";
                case "#ff00d7cb":
                    return "layoutBlockStyle_00d7cb";
                case "#ff90d000":
                    return "layoutBlockStyle_90d000";
                case "#ffdfb500":
                    return "layoutBlockStyle_dfb500";
                case "#ffe064b7":
                    return "layoutBlockStyle_e064b7";
                case "#ff00a4a4":
                    return "layoutBlockStyle_00a4a4";
                case "#fffd7b21":
                    return "layoutBlockStyle_fd7b21";
                default:
                    return "layoutBlockStyle_fd7b21";
            }

            //switch (colorValue)
            //{
            //    case "#FF00C13F":
            //        return "layoutBlockStyle7";
            //    case "AntiqueWhite":
            //        return "layoutBlockStyle8";
            //    case "Aqua":
            //        return "layoutBlockStyle9";
            //    case "Aquamarine":
            //        return "layoutBlockStyle10";
            //    case "Azure":
            //        return "layoutBlockStyle11";
            //    case "Beige":
            //        return "layoutBlockStyle12";
            //    case "Bisque":
            //        return "layoutBlockStyle13";
            //    case "Black":
            //        return "layoutBlockStyle14";
            //    case "BlanchedAlmond":
            //        return "layoutBlockStyle15";
            //    case "Blue":
            //        return "layoutBlockStyle0";
            //    case "Olive":
            //        return "layoutBlockStyle1";
            //    default:
            //        return "layoutBlockStyle7";
            //}
        }

        /// <summary>
        /// Determine the Window State
        /// </summary>
        /// <param name="viewState"></param>
        /// <returns>returns the widnow state</returns>
        public static string DetermineVisualState(ApplicationViewState viewState)
        {
            if (viewState == ApplicationViewState.Filled || viewState == ApplicationViewState.FullScreenLandscape)
            {
                // Allow pages to request that the Filled state be used only for landscape layouts narrower
                // than 1366 virtual pixels
                var windowWidth = Window.Current.Bounds.Width;
                viewState = windowWidth >= 1366 ? ApplicationViewState.FullScreenLandscape : ApplicationViewState.Filled;
            }
            return viewState.ToString();
        }

        public static async void dynamicBackgroundChange(Windows.UI.Xaml.Controls.Grid gd)
        {
            StorageFolder localFolder = null;
            StorageFile file;
            localFolder = ApplicationData.Current.LocalFolder;
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            {
                if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "dynamicStyle")
                {
                    #region Commented on 16.07.2013

                    //ImageBrush ib = new ImageBrush();

                    //BitmapImage bi = new BitmapImage();
                    //bi.UriSource = new Uri(ApplicationData.Current.RoamingSettings.Values["ImageURLForDynamicStyle"].ToString());
                    //ib.ImageSource = bi;

                    //gd.Background = ib;

                    #endregion

                    #region Commented
                    /*
                    file = await localFolder.GetFileAsync("backgroundImage.jpg");
                    using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                    {

                        ////file = await localFolder.GetFileAsync("backgroundImage.jpg");
                        ////if (file != null)
                        ////{
                        //    //var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                        ImageBrush ib = new ImageBrush();

                        //    // Set the image source to the selected bitmap
                        BitmapImage bitmapImage = new BitmapImage();
                        //    ib.ImageSource = new BitmapImage(new Uri("ms-appdata:///local/backgroundImage.jpg", UriKind.Absolute));
                        bitmapImage.SetSource(fileStream);
                        ib.ImageSource = bitmapImage;
                        gd.Background = ib;
                        ////}
                    }
                    */
                    #endregion

                    try
                    {

                        //gd.Background = new ImageBrush
                        //{
                        //    Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill,
                        //    ImageSource =
                        //        new BitmapImage { UriSource = new Uri("ms-appx:///local/backgroundImage.jpg") }
                        //};

                        file = await localFolder.GetFileAsync("backgroundImage.jpg");

                        if (file != null)
                        {
                            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                            {
                                ImageBrush imageBrush = new ImageBrush();

                                // Set the image source to the selected bitmap
                                BitmapImage bitmapImage = new BitmapImage();
                                bitmapImage.SetSource(fileStream);
                                imageBrush.ImageSource = bitmapImage;
                                gd.Background = imageBrush;
                            }
                        }
                    }
                    catch
                    {
                        ApplicationData.Current.RoamingSettings.Values["bgColor"] = "#f2b100";
                        SolidColorBrush sbColorBrush = new SolidColorBrush(Utilities.HexColor("#f2b100"));
                        gd.Background = sbColorBrush;
                    }                    
                }
            }
        }

        /// <summary>
        /// Method for showing pop up messages
        /// </summary>
        /// <param name="msg"></param>
        public static async void ShowMessage(string msg)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }


    }
}
