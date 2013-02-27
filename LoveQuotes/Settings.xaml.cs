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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LoveQuotes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        //AppSettings ap = new AppSettings();

        public Settings()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #region Page Events

        /// <summary>
        /// Page Load event for setting background image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Style = App.Current.Resources[Utilities.getBkgImageSetting()] as Style;
        }

        #region Checked events for Radio Buttons
        private void radioButton0_Checked(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Style = App.Current.Resources[Utilities.getBkgImageSetting()] as Style;
        }
        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Style = App.Current.Resources[Utilities.getBkgImageSetting()] as Style;
        }
        private void radioButton2_Checked(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Style = App.Current.Resources[Utilities.getBkgImageSetting()] as Style;
        }
        private void radioButton3_Checked(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Style = App.Current.Resources[Utilities.getBkgImageSetting()] as Style;
        }
        private void radioButton4_Checked(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Style = App.Current.Resources[Utilities.getBkgImageSetting()] as Style;
        }
        private void radioButton5_Checked(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Style = App.Current.Resources[Utilities.getBkgImageSetting()] as Style;
        }
        private void radioButton6_Checked(object sender, RoutedEventArgs e)
        {
            //SettingsStorage.SetLocalSetting("RadioBkgImage6", 1);
            //LayoutRoot.Style = App.Current.Resources[Utilities.getBkgImageSetting()] as Style;
            LayoutRoot.Style = App.Current.Resources[Utilities.getBkgImageSetting()] as Style;
        }
        #endregion

        #endregion

    }
}
