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
using Windows.Globalization;
using Windows.UI.ApplicationSettings;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TopMovies
{
    public sealed partial class Regionsetting : UserControl
    {
        List<string> list = new List<string>();
       

        Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
        Windows.Globalization.GeographicRegion region = new Windows.Globalization.GeographicRegion();

       // public string cc;

        public Regionsetting()
        {

          // cc = region.DisplayName;

            //list.Add(cc);

            

            this.InitializeComponent();

            list.Add("Australia");
            list.Add("Canada");
            list.Add("China");
            list.Add("Germany");
            list.Add("Spain");
            list.Add("France");
            list.Add("India");
            list.Add("Italy");
            list.Add("Japan");
            list.Add("United Kingdom");
            list.Add("United States");
            list.Add("Others(Rest of the World)");

            
            

            SelectCountry_Popup.ItemsSource = list;                                         // Populate the ListBox with countrylist . 
            

            if (list.Contains(((App)(App.Current)).countryCode))                                                          // This is to highlight the user previous selection .
            {
                SelectCountry_Popup.SelectedValue = ((App)(App.Current)).countryCode;            
            }
            else
            { 
                SelectCountry_Popup.SelectedIndex = list.IndexOf("Others(Rest of the World)"); 
            }
            
        }




        private async void country_selection(object sender, TappedRoutedEventArgs e)
        {

            try
            {
                if (SelectCountry_Popup.SelectedIndex != -1)
                {
                    ((App)(App.Current)).countryCode = SelectCountry_Popup.SelectedValue.ToString();                    // Assigning the user selection to the global countryCode

                    roamingSettings.Values["userCountrySetting"] = SelectCountry_Popup.SelectedValue.ToString();        // Storing the user selection in the roamingSetting .
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message.ToString());
                //throw new Exception("This exception is created in the selcetion procedure");
            }

        }

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
