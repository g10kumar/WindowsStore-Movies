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
using Windows.UI.ApplicationSettings;
using LoveQuotes.Common;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using System.Text.RegularExpressions;
namespace LoveQuotes
{
    public sealed partial class SettingsUserControl : UserControl
    {
        string size = string.Empty;
        private AppSettings settings = new AppSettings();
        public SettingsUserControl()
        {
            this.InitializeComponent();

            // Initialize the Radio button from roaming settings
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            {
                //if (ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "layoutBlockStyle0")
                //{
                //    radioButton0.IsChecked = true;
                //}
                //else if (ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "layoutBlockStyle1")
                //{
                //    radioButton1.IsChecked = true;
                //}
                //else if (ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "layoutBlockStyle2")
                //{
                //    radioButton2.IsChecked = true;
                //}
                //else if (ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "layoutBlockStyle3")
                //{
                //    radioButton3.IsChecked = true;
                //}
                //else if (ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "layoutBlockStyle4")
                //{
                //    radioButton4.IsChecked = true;
                //}
                //else if (ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "layoutBlockStyle5")
                //{
                //    radioButton5.IsChecked = true;
                //}
                //else if (ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "layoutBlockStyle6")
                //{
                //    radioButton6.IsChecked = true;
                //}

            }

            //if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Auto-play"))
            //{
            //    txtAutoPlay.Text = ApplicationData.Current.RoamingSettings.Values["Auto-play"].ToString();
            //}

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("FontSize"))
            {
                sizeSlider.Value = Convert.ToInt32(ApplicationData.Current.RoamingSettings.Values["FontSize"].ToString());                
            }


            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Font") && (string)ApplicationData.Current.RoamingSettings.Values["Font"].ToString() != "")
            {
                txtSample.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Font"].ToString()] as Style;
            }
            else
            {
                txtSample.Style = App.Current.Resources["Style20"] as Style;
            }
            sizeSlider.ValueChanged += sizeSlider_ValueChanged;

            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Auto-play"))
            {
                autoPlaySlider.Value = Convert.ToInt32(ApplicationData.Current.RoamingSettings.Values["Auto-play"].ToString());                
            }

            autoPlaySlider.ValueChanged +=autoPlaySlider_ValueChanged;
            // Calls the Background change method
            ChangeBackground();
            
        }

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
            // Initialize the Radio button from roaming settings
            if (ApplicationData.Current.RoamingSettings.Values["Settings"] != null)
            {
                if ((string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString() == "Generic")
                {
                    SolidColorBrush s = new SolidColorBrush();
                    s.Color = Utilities.GetColor(sessionQuotes.colorValue);
                    LayoutRoot.Background = s;
                }
                else if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
                {
                    LayoutRoot.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                }
                else
                {
                    LayoutRoot.Style = App.Current.Resources["layoutBlockStyle_f2b100"] as Style;

                }
            }
            else
            {
                LayoutRoot.Style = App.Current.Resources["layoutBlockStyle_f2b100"] as Style;

            }
        }

        #region Checked events for Radio Buttons

        //private void radioButton0_Checked(object sender, RoutedEventArgs e)
        //{
        //    ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle0";

        //    ChangeBackground();
        //}

        //private void radioButton1_Checked(object sender, RoutedEventArgs e)
        //{
        //    ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle1";
        //    ChangeBackground();
        //}
        //private void radioButton2_Checked(object sender, RoutedEventArgs e)
        //{
        //    ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle2";
        //    ChangeBackground();
        //}
        //private void radioButton3_Checked(object sender, RoutedEventArgs e)
        //{
        //    ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle3";
        //    ChangeBackground();
        //}
        //private void radioButton4_Checked(object sender, RoutedEventArgs e)
        //{
        //    ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle4";
        //    ChangeBackground();
        //}
        //private void radioButton5_Checked(object sender, RoutedEventArgs e)
        //{
        //    ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle5";
        //    ChangeBackground();
        //}

        //private void radioButton6_Checked(object sender, RoutedEventArgs e)
        //{
        //    ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle6";
        //    ChangeBackground();
        //}
        #endregion

        #region Click events for Buttons

        //private void btnBlue_Click(object sender, RoutedEventArgs e)
        //{
        //    ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle0";
        //    ChangeBackground();
        //}

        //private void btnOlive_Click(object sender, RoutedEventArgs e)
        //{
        //    ApplicationData.Current.RoamingSettings.Values["Settings"] = "layoutBlockStyle1";
        //    ChangeBackground();
        //}

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
            //SolidColorBrush s = new SolidColorBrush();
            //s.Color = Windows.UI.Colors.Transparent;
            //LayoutRoot.Background.ClearValue;
            ChangeBackground();            
        }
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

        public void colorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem cbi = (ComboBoxItem)colorPicker.SelectedItem;
                SolidColorBrush s = new SolidColorBrush();
                s = (SolidColorBrush)cbi.Background;
                
                //Windows.UI.Xaml.Controls.ComboBoxItem item = (Windows.UI.Xaml.Controls.ComboBoxItem)colorPicker.SelectedValue;
                string colorValue = s.Color.ToString();
                
                sessionQuotes.colorValue = colorValue;
                
                string layout = Utilities.GetLayout(sessionQuotes.colorValue);

                ApplicationData.Current.RoamingSettings.Values["Settings"] = layout;
                ChangeBackground();
            }
            catch
            {
                //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                //dialog.ShowAsync();

                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Oops! There was a problem. Please try again.");
                dialog.ShowAsync();
            }
        }

        //private async void txtAutoPlay_KeyUp(object sender, KeyRoutedEventArgs e)
        //{
        //    if (txtAutoPlay.Text.Length == 1)
        //    {
        //        if (Regex.Match(txtAutoPlay.Text, @"^[5-9]$").Success)
        //        {
        //            ApplicationData.Current.RoamingSettings.Values["Auto-play"] = txtAutoPlay.Text;
        //        }
        //        else
        //        {
        //            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Please enter the number between 5 to 600.");
        //            await dialog.ShowAsync();
        //        }
        //    }
        //    else if(txtAutoPlay.Text.Length == 2)
        //    {
        //        if (Regex.Match(txtAutoPlay.Text, @"^[1-9][0-9]$").Success)
        //        {
        //            ApplicationData.Current.RoamingSettings.Values["Auto-play"] = txtAutoPlay.Text;
        //        }
        //        else
        //        {
        //            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Please enter the number between 5 to 600.");
        //            await dialog.ShowAsync();
        //        }
        //    }
        //    else if (txtAutoPlay.Text.Length == 3)
        //    {
        //        if (Regex.Match(txtAutoPlay.Text, @"^[1-6]00$").Success)
        //        {
        //            ApplicationData.Current.RoamingSettings.Values["Auto-play"] = txtAutoPlay.Text;
        //        }
        //        else
        //        {
        //            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Please enter the number between 5 to 600.");
        //            await dialog.ShowAsync();
        //        }
        //    }
        //    //if (Regex.Match(txtAutoPlay.Text, @"^[5-9]$|^[1-9][0-9]$|^[1-6]00$").Success)
        //    //{
        //    //    ApplicationData.Current.RoamingSettings.Values["Auto-play"] = txtAutoPlay.Text;
        //    //}
        //    //else
        //    //{
        //    //    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Please enter the number between 5 to 600.");
        //    //    await dialog.ShowAsync();
        //    //}
        //}

        //private void txtAutoPlay_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    //"^\d{1,3}$"
            
        //    //if (!Regex.Match(txtAutoPlay.Text, @"^\([1-9]\d?|5\d{1,2}|600$").Success)
        //    //{
        //    //    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Success.");
        //    //    dialog.ShowAsync();
        //    //}
        //    //else
        //    //{
        //    //    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Oops!");
        //    //    dialog.ShowAsync();
        //    //}
        //}

        //private void txtAutoPlay_KeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    //if (!Regex.Match(txtAutoPlay.Text, @"^\([1-9]\d?|5\d{1,3}|600$").Success)
        //    //{
        //    //    ApplicationData.Current.RoamingSettings.Values["Auto-play"] = txtAutoPlay.Text;
        //    //}
        //    //else
        //    //{
        //    //    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Please enter the number between 5 to 600.");
        //    //    dialog.ShowAsync();
        //    //}
        //}

        private void textColorPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem cbi = (ComboBoxItem)textColorPicker.SelectedItem;
                SolidColorBrush s = new SolidColorBrush();
                s = (SolidColorBrush)cbi.Background;

                //Windows.UI.Xaml.Controls.ComboBoxItem item = (Windows.UI.Xaml.Controls.ComboBoxItem)colorPicker.SelectedValue;
                string colorValue = s.Color.ToString();

                ApplicationData.Current.RoamingSettings.Values["FontColor"] = colorValue;
                ApplicationData.Current.RoamingSettings.Values["Font"] = "Test1";
                txtSample.Foreground = cbi.Background;

                if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
                {
                    Style style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                    LayoutRoot.Style = style;
                }

                ChangeBackground();
            }
            catch
            {
                //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
                //dialog.ShowAsync();

                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Oops! There was a problem. Please try again.");
                dialog.ShowAsync();
            }
        }

        //private void fontSizePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        ComboBoxItem cbi = (ComboBoxItem)fontSizePicker.SelectedItem;
        //        string styleValue = "";
        //        if (cbi.Content.ToString() == "20")
        //        {
        //            styleValue = "Style20";
        //        }
        //        else if (cbi.Content.ToString() == "30")
        //        {
        //            styleValue = "Style30";
        //        }
        //        else if (cbi.Content.ToString() == "40")
        //        {
        //            styleValue = "Style40";
        //        }
        //        else if (cbi.Content.ToString() == "50")
        //        {
        //            styleValue = "Style50";
        //        }
        //        else if (cbi.Content.ToString() == "60")
        //        {
        //            styleValue = "Style60";
        //        }
        //        else if (cbi.Content.ToString() == "70")
        //        {
        //            styleValue = "Style70";
        //        }
        //        ApplicationData.Current.RoamingSettings.Values["Font"] = styleValue;


        //        if ((string)ApplicationData.Current.RoamingSettings.Values["Font"].ToString() != "")
        //        {
        //            txtSample.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Font"].ToString()] as Style;
        //        }
        //        else
        //        {
        //            txtSample.Style = App.Current.Resources["Style20"] as Style;
        //        }

        //        if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
        //        {
        //            Style style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
        //            LayoutRoot.Style = style;
        //        }

        //        ChangeBackground();
        //    }
        //    catch
        //    {
        //        //Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(ex.Message.ToString());
        //        //dialog.ShowAsync();

        //        Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Oops! There was a problem. Please try again.");
        //        dialog.ShowAsync();
        //    }
        //}

        private void sizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["FontSize"] = Convert.ToInt32(sizeSlider.Value);

            string styleValue = "";
            if (sizeSlider.Value == 20)
            {
                styleValue = "Style20";
            }
            else if (sizeSlider.Value == 30)
            {
                styleValue = "Style30";
            }
            else if (sizeSlider.Value == 40)
            {
                styleValue = "Style40";
            }
            else if (sizeSlider.Value == 50)
            {
                styleValue = "Style50";
            }
            else if (sizeSlider.Value == 60)
            {
                styleValue = "Style60";
            }
            else if (sizeSlider.Value == 70)
            {
                styleValue = "Style70";
            }
            ApplicationData.Current.RoamingSettings.Values["Font"] = styleValue;


            if ((string)ApplicationData.Current.RoamingSettings.Values["Font"].ToString() != "")
            {
                txtSample.Style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Font"].ToString()] as Style;
            }
            else
            {
                txtSample.Style = App.Current.Resources["Style20"] as Style;
            }
        //    //ApplicationData.Current.RoamingSettings.Values["FontSize"] = (int)sizeSlider.Value;
        //    //txtSample.FontSize = Convert.ToInt32(sizeSlider.Value);
        //    txtSample.FontSize = Convert.ToInt32(ApplicationData.Current.RoamingSettings.Values["FontSize"].ToString());
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Settings"))
            {
                Style style = App.Current.Resources[(string)ApplicationData.Current.RoamingSettings.Values["Settings"].ToString()] as Style;
                LayoutRoot.Style = style;
            }

            ChangeBackground();
        }   

        private void autoPlaySlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["Auto-play"] = Convert.ToInt32(autoPlaySlider.Value);        
        }        
        
    }
}
