using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Xml.Linq;
using System.Security;
using System.Net;

//using Windows.Foundation;
//using Windows.Foundation.Collections;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;
//using Windows.Storage;


namespace QuotesOfWisdom
{
    public sealed partial class AdRotator : UserControl
    {
        public DispatcherTimer dtautoPlay = new DispatcherTimer();
        string uriToLaunch = @"ms-windows-store:PDP?PFN=DaksaTech.ShareAll_c7fyd19frge5m";
        string imageToDisplay = "ms-appx:Assets/Ads/ShareAllBanner.png";
        List<Ads> list = new List<Ads>();
        
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
            imageToDisplay = list[adIndex].Image;
            adImage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(imageToDisplay));

        }

        private void loadAds()
        {

            // Creating Http web request object for loading ads
            var request = HttpWebRequest.Create("http://ads.daksatech.com/quotesads.xml");

            // Creating ResponseCallBack method for above created web request object
            var result = (IAsyncResult)request.BeginGetResponse(adsListingsResponseCallback, request);
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
                a.Image = "ms-appx:Assets/Ads/amzn_assoc_gm-box-smile_728x90.png";
                a.Url = "http://www.amazon.com/?&tag=artmaya-20&camp=216797&creative=394537&linkCode=ur1&adid=1XH8Y4229YJY277EZ8ZW&&ref-refURL=http%3A%2F%2Frcm.amazon.com%2Fe%2Fcm%3Ft%3Dartmaya-20%26o%3D1%26p%3D48%26l%3Dur1%26category%3Damazonhomepage%26f%3Difr";

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
