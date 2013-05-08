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
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace AdTest
{
    public sealed partial class MyDefaultAd : UserControl
    {
        public MyDefaultAd()
        {
            this.InitializeComponent();
        }

        private void UserControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var message = new MessageDialog("Thank you for clicking on my Ad");
            message.ShowAsync();
        }
    }
}
