(function () {
    "use strict";

    var appView = Windows.UI.ViewManagement.ApplicationView;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    ui.Pages.define("/pages/about/about.html", {

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            element.querySelector(".win-label").textContent = WinJS.Resources.getString('About Us').value;
            document.getElementById("about11").textContent = WinJS.Resources.getString('The Newspapers app for Windows 8 offers the top newspapers in the world at your fingertips').value;
            document.getElementById("about12").textContent = WinJS.Resources.getString('You can view the top newspapers in any of the more than 150 countries and also the 50 states of the United States').value;
            document.getElementById("about13").textContent = WinJS.Resources.getString('The countries are organized by regions. Select the country and then select the newspaper').value;
            document.getElementById("about14").textContent = WinJS.Resources.getString('This will open the Newspapers website in a browser').value;
            document.getElementById("about21").textContent = WinJS.Resources.getString('The best way to view the newspapers is to put the app in Snapped view and then browse the different newspapers').value;
            document.getElementById("about22").textContent = WinJS.Resources.getString('Its like your virtual shelf').value;
            document.getElementById("about3").textContent = WinJS.Resources.getString('If you have any issues or have a suggestion for the app, please email us at').value;
            document.getElementById("about4").textContent = WinJS.Resources.getString('The app was developed by DaksaTech. We have many more apps for Windows 8 as well as Windows Phone.').value;
            document.getElementById("about5").textContent = WinJS.Resources.getString('Windows 8 Apps').value;
            document.getElementById("about6").textContent = WinJS.Resources.getString('Windows Phone Apps').value;
            
        }
        
    });
    
})();

WinJS.Utilities.markSupportedForProcessing(
window.errorLogger = function (sender, evt) {

});
