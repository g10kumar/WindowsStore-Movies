(function () {
    //"use strict";
    var appView = Windows.UI.ViewManagement.ApplicationView;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;

    var ui = WinJS.UI;
    var utils = WinJS.Utilities;

    ui.Pages.define("/pages/itemDetail/itemDetail.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
            //element.querySelector(".titlearea .pagetitle").textContent = item.group.title;


            getSpeeches(element, options);

            // Initialize the license info for use in the app that is uploaded to the Store.
            // uncomment for release
            //currentApp = Windows.ApplicationModel.Store.CurrentApp;

            // Initialize the license info for testing.
            // comment the next line for release
            //currentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;

            //// get the license info
            //licenseInformation = currentApp.licenseInformation;

            //if (licenseInformation.isActive) {
            //    if (licenseInformation.isTrial) {
            //        // Show the features that are available during trial only.
            //        //var tmpDate = new Date();

            //        //tmpDate.setDate(tmpDate.getDate() + 10);

            //        var expiryDate = licenseInformation.expirationDate;
            //        var currentDate = new Date();

            //        expiryDate = expiryDate.getTime();
            //        currentDate = currentDate.getTime();
            //        var diff_ms = Math.abs(expiryDate - currentDate);

            //        var daysRemaining = Math.round(diff_ms / 86400000);
            //        //var daysRemaining = (licenseInformation.expirationDate - new Date()) / 86400000;

            //        if (parseInt(Math.round(daysRemaining)) > 0) {

            //            if (parseInt(Math.round(daysRemaining)) < 5) {
            //                // Create the message dialog and set its content
            //                var msg = new Windows.UI.Popups.MessageDialog("Your trial ends in " + daysRemaining + " days. Do you want to buy the app now?");


            //                // Add commands and set their command handlers
            //                msg.commands.append(new Windows.UI.Popups.UICommand("Yes", commandYesInvokedHandler));
            //                msg.commands.append(new Windows.UI.Popups.UICommand("No", commandNoInvokedHandler));

            //                // Set the command that will be invoked by default
            //                msg.defaultCommandIndex = 0;

            //                // Set the command to be invoked when escape is pressed
            //                msg.cancelCommandIndex = 1;

            //                // Show the message dialog
            //                msg.showAsync();
            //            }
            //            getSpeeches(element, options);

            //            //element.querySelector("article .item-title").textContent = item.title;
            //            //element.querySelector("article .item-subtitle").textContent = item.subtitle;
            //            ////element.querySelector("article .item-image").src = item.backgroundImage;
            //            ////element.querySelector("article .item-image").alt = item.subtitle;            
            //            //WinJS.xhr({ url: "xml/" + item.rank + ".xml" }).then(function (result) {
            //            //    //WinJS.xhr({ url: "xml/79.xml" }).then(function (result) {
            //            //    var speeches = result.responseXML;
            //            //    var items = speeches.querySelectorAll("root > Speeches");
            //            //    if (items.length == 0) {
            //            //        items = speeches.querySelectorAll("ArrayOfSpeeches > Speeches");
            //            //    }
            //            //    var itemContent = "";
            //            //    itemContent = items[0].querySelector("Speech").textContent.toString();
            //            //    itemContent = itemContent.replace(new RegExp("\\n", "g"), "<p>");
            //            //    element.querySelector("article .item-content").innerHTML = itemContent;
            //            //    element.querySelector("article .item-image").src = items[0].querySelector("Photo").textContent.toString();
            //            //    //element.querySelector("article .item-image").src = "../images/speaker/" + item.subtitle + "_" + item.rank + ".jpg";
            //            //});
            //            ////element.querySelector("article .item-content").innerHTML = item.content;
            //            //element.querySelector(".content").focus();
            //        }
            //        else {
            //            // Create the message dialog and set its content
            //            var msg = new Windows.UI.Popups.MessageDialog("Your trial has ended. Do you want to buy the app?");

            //            // Add commands and set their command handlers
            //            msg.commands.append(new Windows.UI.Popups.UICommand("Yes", commandYesInvokedHandler));
            //            msg.commands.append(new Windows.UI.Popups.UICommand("No", commandNoInvokedHandler));

            //            // Set the command that will be invoked by default
            //            msg.defaultCommandIndex = 0;

            //            // Set the command to be invoked when escape is pressed
            //            msg.cancelCommandIndex = 1;

            //            // Show the message dialog
            //            msg.showAsync();

            //            getSpeeches(element, options);
            //        }
            //    }
            //    else {
            //        // Show the features that are available only with a full license.
            //        getSpeeches(element, options);

            //        //element.querySelector("article .item-title").textContent = item.title;
            //        //element.querySelector("article .item-subtitle").textContent = item.subtitle;
            //        ////element.querySelector("article .item-image").src = item.backgroundImage;
            //        ////element.querySelector("article .item-image").alt = item.subtitle;            
            //        //WinJS.xhr({ url: "xml/" + item.rank + ".xml" }).then(function (result) {
            //        //    //WinJS.xhr({ url: "xml/79.xml" }).then(function (result) {
            //        //    var speeches = result.responseXML;
            //        //    var items = speeches.querySelectorAll("root > Speeches");
            //        //    if (items.length == 0) {
            //        //        items = speeches.querySelectorAll("ArrayOfSpeeches > Speeches");
            //        //    }
            //        //    var itemContent = "";
            //        //    itemContent = items[0].querySelector("Speech").textContent.toString();
            //        //    itemContent = itemContent.replace(new RegExp("\\n", "g"), "<p>");
            //        //    element.querySelector("article .item-content").innerHTML = itemContent;
            //        //    element.querySelector("article .item-image").src = items[0].querySelector("Photo").textContent.toString();
            //        //    //element.querySelector("article .item-image").src = "../images/speaker/" + item.subtitle + "_" + item.rank + ".jpg";
            //        //});
            //        ////element.querySelector("article .item-content").innerHTML = item.content;
            //        //element.querySelector(".content").focus();
            //    }
            //}
            //else {
            //    // A license is inactive only when there's an error.
            //}
        },
        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />
            /// <param name="viewState" value="Windows.UI.ViewManagement.ApplicationViewState" />
            /// <param name="lastViewState" value="Windows.UI.ViewManagement.ApplicationViewState" />

            if (lastViewState !== viewState) {
                if (lastViewState === appViewState.snapped || viewState === appViewState.snapped) {
                    
                    initializeLayout(viewState);
                }
            }
        }
    });

    function getSpeeches(element, options) {
        var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
        element.querySelector("article .item-title").textContent = item.title;
        element.querySelector("article .item-subtitle").textContent = item.subtitle;
        document.getElementById("hrefWiki").href = "http://www.wikipedia.org/wiki/" + item.subtitle.replace(" ", "_");
        document.getElementById("hrefBooks").href = "http://www.amazon.com/mn/search/?field-keywords=" + item.subtitle + "&tag=artmaya-20";
        WinJS.xhr({ url: "xml/" + item.rank + ".xml" }).then(function (result) {
            var speeches = result.responseXML;
            var items = speeches.querySelectorAll("root > Speeches");
            if (items.length == 0) {
                items = speeches.querySelectorAll("ArrayOfSpeeches > Speeches");
            }
            var itemContent = "";
            itemContent = items[0].querySelector("Speech").textContent.toString();
            itemContent = itemContent.replace(new RegExp("\\n", "g"), "<p>");

            element.querySelector("article .item-content").innerHTML = itemContent;
            element.querySelector("article .item-image").src = items[0].querySelector("Photo").textContent.toString();

            var itemVideo = items[0].querySelector("Youtube").textContent.toString();
            element.querySelector("article .item-content").innerHTML = itemContent;

        },
        function (error) {
            document.getElementById("outputDiv").style.display = 'block';
            document.getElementById("outputDiv").innerHTML = "Unable to display the Speech Details.";            
        });

        element.querySelector(".content").focus();
        initializeLayout(appView.value);
    }

    function commandYesInvokedHandler(command) {
        
        currentApp.requestAppPurchaseAsync(true).done(function (result) {
            var msg = new Windows.UI.Popups.MessageDialog("The app was upgraded to the full version.");
            msg.showAsync();
            
        },
        function () {
            var msg = new Windows.UI.Popups.MessageDialog("The upgrade transaction failed. You still have a trial license for this app.");
            msg.showAsync();
        }
        );
    }


    function commandNoInvokedHandler(command) {
        // nothing
    }
    
    // This function updates the ListView with new layouts
    function initializeLayout(viewState) {
        if (viewState === appViewState.snapped) {
            document.getElementById("myAd").style.display = 'none';

        } else {
            document.getElementById("myAd").style.display = 'block';
        }
    }

    //function callback(xml, element) {
    //    var speeches = xml.responseXML;
    //    var items = speeches.querySelectorAll("root > Speeches > Speech");
    //    if (items.length == 0) {
    //        items = speeches.querySelectorAll("ArrayOfSpeeches > Speeches > Speech");
    //    }
    //    element.querySelector("article .item-content").innerHTM = items[0].textContent
    //}
})();

WinJS.Utilities.markSupportedForProcessing(
window.errorLogger = function (sender, evt) {
    
});