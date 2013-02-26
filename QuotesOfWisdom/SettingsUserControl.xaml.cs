using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using QuotesOfWisdom.Common;

//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Windows.Foundation;
//using Windows.Foundation.Collections;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Navigation;
//using Windows.Graphics.Imaging;
//using Windows.Storage.FileProperties;
//using Windows.Storage.Pickers;
//using System.Threading.Tasks;
//using Windows.UI.Xaml.Media.Imaging;

namespace QuotesOfWisdom
{
    public sealed partial class SettingsUserControl : UserControl
    {
        #region Objects

        //string fileToken = "";

        #endregion

        public SettingsUserControl()
        {
            this.InitializeComponent();

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Auto-play"))
            {
                autoPlaySlider.Value = Convert.ToInt32(ApplicationData.Current.RoamingSettings.Values["Auto-play"].ToString());
            }

            autoPlaySlider.ValueChanged += autoPlaySlider_ValueChanged;

            // Calls the Background change method
            ChangeBackground();
        }

        private void autoPlaySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["Auto-play"] = Convert.ToInt32(autoPlaySlider.Value);
        } 

        /// <summary>
        /// Click event of the back button
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

        /// <summary>
        /// Method of changing background
        /// </summary>
        private void ChangeBackground()
        {
            //// Initialize the Radio button from roaming settings
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            {
                LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle6"] as Style;
            }

            #region Custom Image selection
            // Initialize the Radio button from roaming settings
            //if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "Generic")
            //{
            //    SolidColorBrush s = new SolidColorBrush();
            //    s.Color = Utilities.GetColor(sessionData.colorValue);
            //    LayoutRoot.Background = s;
            //}
            //else 
            //if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            //{
            //    if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() != "CustomImage")                
            //    {
            //        LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;

            //        #region Directly assigning the background property of the Main Grid

            //        /*
            //        Windows.Storage.StorageFolder storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            //        Windows.Storage.StorageFolder storageFolder1 = await storageFolder.GetFolderAsync("background");
            //        StorageFile sampleFile = await storageFolder1.GetFileAsync("drop1.jpg");

            //        ImageBrush ib = new ImageBrush();

            //        BitmapImage src = new BitmapImage();
            //        src.SetSource(await sampleFile.OpenAsync(FileAccessMode.Read));


            //        ib.ImageSource = src;

            //        LayoutRoot.Background = ib;
            //        */
            //        #endregion
            //    }
            //    else
            //    {
            //        StorageFile file;


            //        file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(ApplicationData.Current.RoamingSettings.Values["fileToken"].ToString());

            //        ImageBrush ib = new ImageBrush();

            //        BitmapImage src = new BitmapImage();
            //        src.SetSource(await file.OpenAsync(FileAccessMode.Read));


            //        ib.ImageSource = src;

            //        LayoutRoot.Background = ib;
            //    }
            //}
            //else
            //{
            //    LayoutRoot.Style = App.Current.Resources["layoutBlockStyle4"] as Style;

            //}
            #endregion
        }        

        #region Click events for Buttons

        private void btnRosePetals_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle2";
            ChangeBackground();

        }

        private void btnBeerMug_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle3";
            ChangeBackground();
        }

        private void btnLeaf_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle4";
            ChangeBackground();
        }

        private void btnTulipPetal_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle5";
            ChangeBackground();
        }

        private void btnRedPaper_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle6";
            ChangeBackground();
        }

        //private async void btnCustomImage_Click(object sender, RoutedEventArgs e)
        //{
        //    FileOpenPicker picker = new FileOpenPicker();
        //    picker.FileTypeFilter.Add("*");
        //    StorageFile file = await picker.PickSingleFileAsync();

        //    if (file == null)
        //    {
        //        Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("User did not select a file.");
        //        await dialog.ShowAsync();
        //    }

        //    fileToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);

        //    ApplicationData.Current.RoamingSettings.Values["Settings"] = "CustomImage";
        //    ApplicationData.Current.RoamingSettings.Values["fileToken"] = fileToken;

        //    ChangeBackground();
        //}
        #endregion

        /// <summary>
        /// LayoutUpdted Event of the page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_LayoutUpdated(object sender, object e)
        {
            // Calls the Background change method
            ChangeBackground();
        }

        /// <summary>
        /// SelectionChanged event of Color dropdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void colorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem cbi = (ComboBoxItem)colorPicker.SelectedItem;

                SolidColorBrush s = new SolidColorBrush();
                s = (SolidColorBrush)cbi.Background;
                string colorValue = s.Color.ToString();

                sessionData.colorValue = colorValue;

                string layout = Utilities.GetLayout(sessionData.colorValue);

                ApplicationData.Current.RoamingSettings.Values["Settings"] = layout;
                ChangeBackground();
            }
            catch
            {
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Oops! There was a problem. Please try again.");
                dialog.ShowAsync();
            }
        }
    }
}
