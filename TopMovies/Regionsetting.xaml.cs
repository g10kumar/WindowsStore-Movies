﻿using System;
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

        public string cc;

        public Regionsetting()
        {

            cc = region.DisplayName;

            list.Add(cc);

            this.InitializeComponent();

            if (cc == "United States")
            {
                list.Add("India");
                list.Add("United Kingdom");
            }
            else if (cc == "United Kingdom")
            {   
                list.Add("India");
                list.Add("United States");
                
            }
            else if (cc == "India")
            {
                list.Add("Unites Kingdom");
                list.Add("United States");
            }
            else
            {
                list.Add("Unites Kingdom");
                list.Add("Uited States");
                list.Add("India");
            }

            list.Add("Afghanistan");
            list.Add("Akrotiri");
            list.Add("Albania");
            list.Add("Algeria");
            list.Add("American Samoa");
            list.Add("Andorra");
            list.Add("Angola");
            list.Add("Anguilla");
            list.Add("Antarctica");
            list.Add("Antigua and Barbuda");
            list.Add("Argentina");
            list.Add("Armenia");
            list.Add("Aruba");
            list.Add("Ashmore and Cartier Islands");
            list.Add("Australia");
            list.Add("Austria");
            list.Add("Azerbaijan");
            list.Add("Bahamas, The");
            list.Add("Bahrain");
            list.Add("Bangladesh");
            list.Add("Barbados");
            list.Add("Bassas da India");
            list.Add("Belarus");
            list.Add("Belgium");
            list.Add("Belize");
            list.Add("Benin");
            list.Add("Bermuda");
            list.Add("Bhutan");
            list.Add("Bolivia");
            list.Add("Bosnia and Herzegovina");
            list.Add("Botswana");
            list.Add("Bouvet Island");
            list.Add("Brazil");
            list.Add("British Indian Ocean Territory");
            list.Add("British Virgin Islands");
            list.Add("Brunei");
            list.Add("Bulgaria");
            list.Add("Burkina Faso");
            list.Add("Burma");
            list.Add("Burundi");
            list.Add("Cambodia");
            list.Add("Cameroon");
            list.Add("Canada");
            list.Add("Cape Verde");
            list.Add("Cayman Islands");
            list.Add("Central African Republic");
            list.Add("Chad");
            list.Add("Chile");
            list.Add("China");
            list.Add("Christmas Island");
            list.Add("Clipperton Island");
            list.Add("Cocos (Keeling) Islands");
            list.Add("Colombia");
            list.Add("Comoros");
            list.Add("Congo, Democratic Republic of the");
            list.Add("Congo, Republic of the");
            list.Add("Cook Islands");
            list.Add("Coral Sea Islands");
            list.Add("Costa Rica");
            list.Add("Cote d'Ivoire");
            list.Add("Croatia");
            list.Add("Cuba");
            list.Add("Cyprus");
            list.Add("Czech Republic");
            list.Add("Denmark");
            list.Add("Dhekelia");
            list.Add("Djibouti");
            list.Add("Dominica");
            list.Add("Dominican Republic");
            list.Add("Ecuador");
            list.Add("Egypt");
            list.Add("El Salvador");
            list.Add("Equatorial Guinea");
            list.Add("Eritrea");
            list.Add("Estonia");
            list.Add("Ethiopia");
            list.Add("Europa Island");
            list.Add("Falkland Islands (Islas Malvinas)");
            list.Add("Faroe Islands");
            list.Add("Fiji");
            list.Add("Finland");
            list.Add("France");
            list.Add("French Guiana");
            list.Add("French Polynesia");
            list.Add("French Southern and Antarctic Lands");
            list.Add("Gabon");
            list.Add("Gambia, The");
            list.Add("Gaza Strip");
            list.Add("Georgia");
            list.Add("Germany");
            list.Add("Ghana");
            list.Add("Gibraltar");
            list.Add("Glorioso Islands");
            list.Add("Greece");
            list.Add("Greenland");
            list.Add("Grenada");
            list.Add("Guadeloupe");
            list.Add("Guam");
            list.Add("Guatemala");
            list.Add("Guernsey");
            list.Add("Guinea");
            list.Add("Guinea-Bissau");
            list.Add("Guyana");
            list.Add("Haiti");
            list.Add("Heard Island and McDonald Islands");
            list.Add("Holy See (Vatican City)");
            list.Add("Honduras");
            list.Add("Hong Kong");
            list.Add("Hungary");
            list.Add("Iceland");
            list.Add("Indonesia");
            list.Add("Iran");
            list.Add("Iraq");
            list.Add("Ireland");
            list.Add("Isle of Man");
            list.Add("Israel");
            list.Add("Italy");
            list.Add("Jamaica");
            list.Add("Jan Mayen");
            list.Add("Japan");
            list.Add("Jersey");
            list.Add("Jordan");
            list.Add("Juan de Nova Island");
            list.Add("Kazakhstan");
            list.Add("Kenya");
            list.Add("Kiribati");
            list.Add("Korea, North");
            list.Add("Korea, South");
            list.Add("Kuwait");
            list.Add("Kyrgyzstan");
            list.Add("Laos");
            list.Add("Latvia");
            list.Add("Lebanon");
            list.Add("Lesotho");
            list.Add("Liberia");
            list.Add("Libya");
            list.Add("Liechtenstein");
            list.Add("Lithuania");
            list.Add("Luxembourg");
            list.Add("Macau");
            list.Add("Macedonia");
            list.Add("Madagascar");
            list.Add("Malawi");
            list.Add("Malaysia");
            list.Add("Maldives");
            list.Add("Mali");
            list.Add("Malta");
            list.Add("Marshall Islands");
            list.Add("Martinique");
            list.Add("Mauritania");
            list.Add("Mauritius");
            list.Add("Mayotte");
            list.Add("Mexico");
            list.Add("Micronesia, Federated States of");
            list.Add("Moldova");
            list.Add("Monaco");
            list.Add("Mongolia");
            list.Add("Montserrat");
            list.Add("Morocco");
            list.Add("Mozambique");
            list.Add("Namibia");
            list.Add("Nauru");
            list.Add("Navassa Island");
            list.Add("Nepal");
            list.Add("Netherlands");
            list.Add("Netherlands Antilles");
            list.Add("New Caledonia");
            list.Add("New Zealand");
            list.Add("Nicaragua");
            list.Add("Niger");
            list.Add("Nigeria");
            list.Add("Niue");
            list.Add("Norfolk Island");
            list.Add("Northern Mariana Islands");
            list.Add("Norway");
            list.Add("Oman");
            list.Add("Pakistan");
            list.Add("Palau");
            list.Add("Panama");
            list.Add("Papua New Guinea");
            list.Add("Paracel Islands");
            list.Add("Paraguay");
            list.Add("Peru");
            list.Add("Philippines");
            list.Add("Pitcairn Islands");
            list.Add("Poland");
            list.Add("Portugal");
            list.Add("Puerto Rico");
            list.Add("Qatar");
            list.Add("Reunion");
            list.Add("Romania");
            list.Add("Russia");
            list.Add("Rwanda");
            list.Add("Saint Helena");
            list.Add("Saint Kitts and Nevis");
            list.Add("Saint Lucia");
            list.Add("Saint Pierre and Miquelon");
            list.Add("Saint Vincent and the Grenadines");
            list.Add("Samoa");
            list.Add("San Marino");
            list.Add("Sao Tome and Principe");
            list.Add("Saudi Arabia");
            list.Add("Senegal");
            list.Add("Serbia and Montenegro");
            list.Add("Seychelles");
            list.Add("Sierra Leone");
            list.Add("Singapore");
            list.Add("Slovakia");
            list.Add("Slovenia");
            list.Add("Solomon Islands");
            list.Add("Somalia");
            list.Add("South Africa");
            list.Add("South Georgia and the South Sandwich Islands");
            list.Add("Spain");
            list.Add("Spratly Islands");
            list.Add("Sri Lanka");
            list.Add("Sudan");
            list.Add("Suriname");
            list.Add("Svalbard");
            list.Add("Swaziland");
            list.Add("Sweden");
            list.Add("Switzerland");
            list.Add("Syria");
            list.Add("Taiwan");
            list.Add("Tajikistan");
            list.Add("Tanzania");
            list.Add("Thailand");
            list.Add("Timor-Leste");
            list.Add("Togo");
            list.Add("Tokelau");
            list.Add("Tonga");
            list.Add("Trinidad and Tobago");
            list.Add("Tromelin Island");
            list.Add("Tunisia");
            list.Add("Turkey");
            list.Add("Turkmenistan");
            list.Add("Turks and Caicos Islands");
            list.Add("Tuvalu");
            list.Add("Uganda");
            list.Add("Ukraine");
            list.Add("United Arab Emirates");
            list.Add("Uruguay");
            list.Add("Uzbekistan");
            list.Add("Vanuatu");
            list.Add("Venezuela");
            list.Add("Vietnam");
            list.Add("Virgin Islands");
            list.Add("Wake Island");
            list.Add("Wallis and Futuna");
            list.Add("West Bank");
            list.Add("Western Sahara");
            list.Add("Yemen");
            list.Add("Zambia");
            list.Add("Zimbabwe");

            SelectCountry_Popup.ItemsSource = list;                                         // Populate the ListBox with countrylist . 

            SelectCountry_Popup.SelectedValue = ((App)(App.Current)).countryCode;           // This is to highlight the user previous selection . 

            
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
