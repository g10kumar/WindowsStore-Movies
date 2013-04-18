(function () {
    "use strict";

    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var appView = Windows.UI.ViewManagement.ApplicationView;
    var binding = WinJS.Binding;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    var dtm = Windows.ApplicationModel.DataTransfer.DataTransferManager;
    var item;
    var chapterTitle;
    var appdata = Windows.Storage.ApplicationData;

    ui.Pages.define("/pages/recent/recent.html", {

        
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            // Store information about the group and selection that this page will
            // display.

            var recent = appdata.current.roamingSettings.values["recentSloka"];
            recent = recent.trim().replace(new RegExp("<p>", "g"), "<br/>");
            var recentSplit = recent.split(";");
            element.querySelector(".titlearea .pagetitle").textContent = recentSplit[0].replace("Chapter:", "");
            element.querySelector("article .item-content").innerHTML = recentSplit[1];
        },

        unload: function () {
            
        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />            
        }

    });    
})();
