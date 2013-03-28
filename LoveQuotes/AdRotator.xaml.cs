using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Xml.Linq;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236


//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Windows.Foundation;
//using Windows.Foundation.Collections;
//using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;
//using Windows.Storage;
//using Windows.Storage.AccessCache;
//using Windows.Storage.FileProperties;
//using Windows.Storage.Streams;
//using System.Xml;
//using System.Xml.Linq;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using System.Security;
using System.Net;
//using System.ComponentModel;

namespace LoveQuotes
{
    public sealed partial class AdRotator : UserControl
    {
        public DispatcherTimer dtautoPlay = new DispatcherTimer();
        string uriToLaunch = @"ms-windows-store:PDP?PFN=DaksaTech.QuotesofWisdom_c7fyd19frge5m";
        string imageToDisplay = "ms-appx:Assets/Ads/Quotes_BannerAd.jpg";
    //    StorageFile sampleFile = null;
        List<Ads> list = new List<Ads>();

    //    string fileContent = "";

        public AdRotator()
        {
            this.InitializeComponent();

            loadAds();

            dtautoPlay.Interval = TimeSpan.FromSeconds(10);
            dtautoPlay.Tick += dtautoPlay_Tick;
            dtautoPlay.Start();
        }

        private void dtautoPlay_Tick(object sender, object e)
        {

            System.Random ra = new Random();

            int adIndex = ra.Next(0, list.Count - 1);
            uriToLaunch = list[adIndex].Url;
            //imageToDisplay = "ms-appx:Assets/Ads/" + list[adIndex].Image;
            imageToDisplay = list[adIndex].Image;
            adImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(imageToDisplay));

        }

        private async void loadAds()
        {

            // Creating Http web request object for loading ads
            var request = HttpWebRequest.Create("http://ads.daksatech.com/LoveQuotesAds.xml");

            // Creating ResponseCallBack method for above created web request object
            var result = (IAsyncResult)request.BeginGetResponse(adsListingsResponseCallback, request);

            #region Commented
            //Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Xml");
            //sampleFile = await storageFolder.GetFileAsync("Ads.xml");
            //fileContent = await FileIO.ReadTextAsync(sampleFile);

            //XElement xe = XElement.Parse(fileContent);

            //var query = (from quotes in xe.Elements("ads")
            //             //orderby System.Guid.NewGuid()
            //             select new Ads
            //             {
            //                 Image = quotes.Element("image").Value,
            //                 Url = quotes.Element("url").Value
            //             });



            //foreach (Ads a in query)
            //{
            //    list.Add(a);
            //}
            #endregion
        }


        /// <summary>
        /// Completed Method for HttpWebRequest
        /// </summary>
        /// <param name="result"></param>
        public void adsListingsResponseCallback(IAsyncResult result)
        {
            #region XML Parsing
            HttpWebRequest request = result.AsyncState as HttpWebRequest;

            WebResponse response = null;
            try
            {
                response = request.EndGetResponse(result);
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string xmlData = reader.ReadToEnd();


                        ParseResult(xmlData);
                    }
                }
                //return;
            }
            catch (WebException we)
            {
                this.ShowMessage(we.InnerException.Message.ToString());
            }
            catch (SecurityException se)
            {
                string statusString = se.Message;
                if (statusString == "")
                    statusString = se.InnerException.Message;

                this.ShowMessage(statusString);
            }
            #endregion
        }

        /// <summary>
        /// Method for parsing xml data
        /// </summary>
        /// <param name="data"></param>
        void ParseResult(string data)
        {
            if (data != "")
            {
                XElement xe = XElement.Parse(data);

                var query = (from quotes in xe.Elements("ads")
                             select new Ads
                             {
                                 Image = quotes.Element("image").Value,
                                 Url = quotes.Element("url").Value
                             });



                foreach (Ads a in query)
                {
                    list.Add(a);
                }
            }
            else
            {
                Ads a = new Ads();
                a.Image = "ms-appx:Assets/Ads/Quotes_BannerAd.jpg";
                a.Url = "ms-windows-store:PDP?PFN=DaksaTech.QuotesofWisdom_c7fyd19frge5m";

                list.Add(a);
            }

        }
        async void ShowMessage(string msg)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(msg);
            await dialog.ShowAsync();
        }

        private async void adRotatorButton_Click_1(object sender, RoutedEventArgs e)
        {
            // The URI to launch


            // Create a Uri object from a URI string 
            var uri = new Uri(uriToLaunch);

            await Windows.System.Launcher.LaunchUriAsync(uri);

        }
    }

    public class Ads
    {
        public string Image { get; set; }
        public string Url { get; set; }

    }
}
