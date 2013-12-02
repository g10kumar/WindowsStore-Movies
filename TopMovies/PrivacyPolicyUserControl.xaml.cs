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
using Windows.UI.ApplicationSettings;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TopMovies
{
    public sealed partial class PrivacyPolicyUserControl : UserControl
    {
        public PrivacyPolicyUserControl()
        {
            this.InitializeComponent();
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

        async private void ExamplesLink_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://choice.microsoft.com/AdvertisementChoice/"));
        }
    }
}
