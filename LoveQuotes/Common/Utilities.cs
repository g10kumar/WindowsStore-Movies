using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LoveQuotes
{
    class Utilities
    {
        
        async void ShowMessage(string msg)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }

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
        }

        public static string GetLayout(string colorValue)
        {

            switch (colorValue.ToLower())
            {
                case "#fff2b100":
                    return "layoutBlockStyle_f2b100";
                case "#ffebebeb":
                    return "layoutBlockStyle_ebebeb";
                case "#ff2471ea":
                    return "layoutBlockStyle_2471ea";
                case "#ff000000":
                    return "layoutBlockStyle_000000";
                case "#ffff00ff":
                    return "layoutBlockStyle_ff00ff";
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
        }


        ///// <summary>
        ///// Method for returning XElement based on matching nodes
        ///// </summary>
        ///// <param name="inputUrl"></param>
        ///// <param name="matchName"></param>
        ///// <returns></returns>
        //public static IEnumerable<XElement> SimpleStreamAxis(string inputUrl, string matchName)
        //{
        //    //StringReader stringReader = new StringReader(c);
        //    byte[] byteArray = Encoding.Unicode.GetBytes(inputUrl);
        //    MemoryStream stream = new MemoryStream(byteArray);

        //    using (XmlReader reader = XmlReader.Create(stream))
        //    {
        //        reader.MoveToContent();
        //        while (reader.Read())
        //        {
        //            switch (reader.NodeType)
        //            {
        //                case XmlNodeType.Element:
        //                    if (reader.Name == matchName)
        //                    {
        //                        XElement el = XElement.ReadFrom(reader)
        //                                              as XElement;
        //                        if (el != null)
        //                            yield return el;
        //                    }
        //                    break;
        //            }
        //        }
        //        //reader.Dispose();
        //    }
        //}

        ///// <summary>
        ///// Method for adding Quotes to Favorites
        ///// </summary>
        ///// <param name="cat"></param>
        ///// <returns></returns>
        //async static void addQuoteToFavs(StorageFolder roamingFolder, Quotation q)
        //{
        //    string returnValue = "Added";
        //    const string filename = "favQuotes.xml";
        //    roamingFolder = ApplicationData.Current.RoamingFolder;
        //    StorageFile file = await roamingFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);


        //    //using (IsolatedStorageFile myStore = IsolatedStorageFile.GetUserStoreForApplication())
        //    //{
        //    //    // Checking for Directory exists in Isolated storage
        //    //    if (!myStore.DirectoryExists("Favorites"))
        //    //    {
        //    //        myStore.CreateDirectory("Favorites");
        //    //    }
        //    //    // Checking for File exists in Isolated storage and
        //    //    // reads into XDocument object
        //    //    if (!myStore.FileExists("Favorites\\favQuotes.xml"))
        //    //    {
        //    //        using (IsolatedStorageFileStream stream = myStore.CreateFile("Favorites\\favQuotes.xml"))
        //    //        {
        //    //            XDocument xdoc = new XDocument(new XElement("root"));
        //    //            xdoc.Save(stream);
        //    //        }

        //    //    }

        //    //    // Open the Favorite file from Isolated storage
        //    //    XDocument doc;
        //    //    using (IsolatedStorageFileStream qstream = myStore.OpenFile("Favorites\\favQuotes.xml", FileMode.Open, FileAccess.ReadWrite))
        //    //    {
        //    //        doc = XDocument.Load(qstream);
        //    //        qstream.Seek(0, SeekOrigin.Begin);

        //    //        // execute the query
        //    //        IEnumerable<XElement> qteExists =
        //    //        from el in doc.Descendants("q")
        //    //        where el.Attribute("id").Value == q.QuoteId
        //    //        select el;

        //    //        // Checks the Quote already exists based on the quote id
        //    //        if (qteExists.Count() == 0)
        //    //        {
        //    //            XElement qElement = new XElement("q",
        //    //                new XAttribute("id", q.QuoteId),
        //    //                new XElement("d", q.Quote),
        //    //                new XElement("a", q.Author)
        //    //                );

        //    //            doc.Element("root").Add(qElement);
        //    //            doc.Save(qstream);
        //    //            returnValue = "Added";
        //    //        }
        //    //        else
        //    //        {
        //    //            returnValue = "Exists";
        //    //        }
        //    //    }


        //    //}

        //    //return returnValue;

        //}

        /// <summary>
        /// Method for setting background image of the page
        /// </summary>
        /// <returns></returns>
        public static string getBkgImageSetting()
        {
            string styleName = "layoutBlockStyle0";
            //AppSettings ap = new AppSettings();
            //if (ap.RadioBkgImage0)
            //    styleName = "layoutBlockStyle0";
            //else if (ap.RadioBkgImage1)
            //    styleName = "layoutBlockStyle1";
            //else if (ap.RadioBkgImage2)
            //    styleName = "layoutBlockStyle2";
            //else if (ap.RadioBkgImage3)
            //    styleName = "layoutBlockStyle3";
            //else if (ap.RadioBkgImage4)
            //    styleName = "layoutBlockStyle4";
            //else if (ap.RadioBkgImage5)
            //    styleName = "layoutBlockStyle5";
            //else if (ap.RadioBkgImage6)
            //    styleName = "layoutBlockStyle6";
            return styleName;
        }
    }
}
