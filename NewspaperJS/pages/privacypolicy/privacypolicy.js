(function () {
    "use strict";

    var appView = Windows.UI.ViewManagement.ApplicationView;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    ui.Pages.define("/pages/privacypolicy/privacypolicy.html", {

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            element.querySelector(".win-label").textContent = WinJS.Resources.getString('Privacy Policy').value;
            document.getElementById("pol11").textContent = WinJS.Resources.getString('This Privacy Policy covers your use of this application').value;
            document.getElementById("pol12").textContent = WinJS.Resources.getString('Newspapers app does not collect, store, or share any personal information, or anything related to your device').value;
            document.getElementById("pol13").textContent = WinJS.Resources.getString('We do not collect any statistics, trends, neither do we track user movements').value;
            document.getElementById("pol21").textContent = WinJS.Resources.getString('The application allows you to open website URLs from the application').value;
            document.getElementById("pol22").textContent = WinJS.Resources.getString('The URLs might be saved by internet browsers on your device').value;
            document.getElementById("pol23").textContent = WinJS.Resources.getString('For information regarding the information stored by these application, please read the privacy policy of the internet browsers on your device').value;
            document.getElementById("pol24").textContent = WinJS.Resources.getString('The app links to newspaper websites from all over the world').value;
            document.getElementById("pol25").textContent = WinJS.Resources.getString('These websites may collect information regarding your visit to those sites').value;

        }

    });

})();

WinJS.Utilities.markSupportedForProcessing(
window.errorLogger = function (sender, evt) {

});
