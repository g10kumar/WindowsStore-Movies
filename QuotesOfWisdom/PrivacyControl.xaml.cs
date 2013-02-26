using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.ApplicationSettings;
using Windows.Storage;

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Windows.Foundation;
//using Windows.Foundation.Collections;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;
//using QuotesOfWisdom.Common;
//using Windows.UI.Xaml.Media.Imaging;

namespace QuotesOfWisdom
{
    public sealed partial class PrivacyControl : UserControl
    {
        public PrivacyControl()
        {
            this.InitializeComponent();

            ChangeBackground();
        }
        
        /// <summary>
        /// LayoutUpdated event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_LayoutUpdated(object sender, object e)
        {
            // Calls the Background change method
            ChangeBackground();
        }

        /// <summary>
        /// Method of changing background
        /// </summary>
        private void ChangeBackground()
        {
            if (ApplicationData.Current.RoamingSettings.Values["Settings"] != null)
            {
                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
                {
                    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                }
                else
                {
                    LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
                }

                #region Custom Image Selection
                /*
                if (ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() != "CustomImage")
                {
                    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                }
                else
                {
                    StorageFile file;

                    file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(ApplicationData.Current.RoamingSettings.Values["fileToken"].ToString());

                    ImageBrush ib = new ImageBrush();

                    BitmapImage src = new BitmapImage();
                    src.SetSource(await file.OpenAsync(FileAccessMode.Read));


                    ib.ImageSource = src;

                    LayoutRoot.Background = ib;

                }
                */
                #endregion
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;
            }
        }

        /// <summary>
        /// Click event of back button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBackButtonClicked(object sender, RoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }
            SettingsPane.Show();
        }
    }
}
