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
    ui.Pages.define("/pages/sloka/sloka.html", {

        _items: null,
        _group: null,
        _itemSelectionIndex: -1,

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            // Store information about the group and selection that this page will
            // display.
            this._group = (options && options.groupKey) ? Data.resolveGroupReference(options.groupKey) : Data.groups.getAt(0);
            this._items = Data.getItemsFromGroup(this._group);
            this._itemSelectionIndex = (options && "selectedIndex" in options) ? options.selectedIndex : -1;
            element.querySelector("header[role=banner] .pagetitle").textContent = this._group.title;
            chapterTitle = this._group.title;

            var flipView = element.querySelector(".flipView").winControl;

            flipView.itemDataSource = this._items.dataSource;
            flipView.itemTemplate = document.getElementById("simple_ItemTemplate");
            flipView.addEventListener("pageselected", handlePageSelected);
            dtm.getForCurrentView().addEventListener("datarequested", this.onDataRequested);

            document.getElementById("btnCopy").addEventListener("click", textCopy, false);
            document.getElementById("btnCommentary").addEventListener("click", slokaMeaning, false);

            if (appView.value === appViewState.snapped) {
                document.getElementById("divMessage").style.visibility = 'visible';
                document.getElementById("basicFlipView").style.visibility = 'collapse';

            } else {
                document.getElementById("divMessage").style.visibility = 'collapse';
                document.getElementById("basicFlipView").style.visibility = 'visible';
            }
        },

        unload: function () {
            this._items.dispose();
            WinJS.Navigation.removeEventListener("datarequested", this.onDataRequested);
        },

        onDataRequested: function (e) {
            var request = e.request;
            request.data.properties.title = "Bhagavad Gita";
            var htmlExample = item;
            var htmlFormat = Windows.ApplicationModel.DataTransfer.HtmlFormatHelper.createHtmlFormat(htmlExample);
            request.data.setHtmlFormat(htmlFormat);
        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />

            if (viewState == 2) {
                document.getElementById("divMessage").style.visibility = 'visible';
                document.getElementById("basicFlipView").style.visibility = 'collapse';
                
            }
            else {
                document.getElementById("divMessage").style.visibility = 'collapse';
                document.getElementById("basicFlipView").style.visibility = 'visible';
            }
        }

    });    

    function textCopy() {

        // create the datapackage
        var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();

        // add the content of the "sloka" in HTML format
        // 1st step - get the HTML content of the element
        var htmlfragment = item;
        //var stripHTMLRE = /<.*?>/gi;
        // 2nd step - convert to html format
        var htmlformat = Windows.ApplicationModel.DataTransfer.HtmlFormatHelper.createHtmlFormat(htmlfragment);
        if (htmlformat !== "") {
            // 3rd step - add html format to datapackage
            dataPackage.setHtmlFormat(htmlformat);
            dataPackage.setText(htmlfragment.replace(/<p>/g, "\r\n").toString());
        }

       //dataPackage.setText(item.innerText);

        try {

            // copy the content to Clipboard
            Windows.ApplicationModel.DataTransfer.Clipboard.setContent(dataPackage);

            // Paste the copied content from Clipboard
           

            //var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.getContent();

            //// paste contents of HTML format (if present)
            //if (dataPackageView.contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.html)) {
            //    // 1st step - get the HTML Format (CF_HTML) from DataPackageView
            //    dataPackageView.getHtmlFormatAsync().done(function (htmlFormat) {
            //        // 2nd step - extract HTML fragment from HTML Format
            //        var htmlFragment = Windows.ApplicationModel.DataTransfer.HtmlFormatHelper.getStaticFragment(htmlFormat);

            //        // 3rd step - add the fragment to DOM
            //        var msg = new Windows.UI.Popups.MessageDialog(htmlFragment);
            //        // Show the message dialog
            //        msg.showAsync()
                    
            //    }, function (e) {
            //        displayError("Error retrieving HTML format from Clipboard: " + e);
            //    });
            //} else {
            //    displayError("No HTML format on Clipboard");
            //}

        } catch (e) {
            // error
            displayError("Error copying content to Clipboard: " + e + ". Try again.");
        }
    }
   
    function slokaMeaning()
    {
        //var dbPath = Windows.Storage.ApplicationData.current.localFolder.path + '\\gita.sqlite';
        //SQLite3JS.openAsync(dbPath)
        //  .then(function (db) {
        //       return db.eachAsync('SELECT * FROM Item', function (row) {
        //           console.log('Get a ' + row.name + ' for $' + row.price);
        //       });
        //   })
        //  .then(function (db) {
        //      db.close();
        //  });
    }

    function displayError(errorString) {
        var msg = new Windows.UI.Popups.MessageDialog(errorString);
        // Show the message dialog
        msg.showAsync()
    }

    function handlePageSelected(ev) {
        //item = ev.target.innerText.trim().replace(new RegExp("\r\n", "g"), "<p>").replace(new RegExp("Sloka", "g"), "");
        item = "Chapter: " + chapterTitle + "; " + ev.target.innerText.trim().replace(new RegExp("\r\n", "g"), "<p>");
        appdata.current.roamingSettings.values["recentSloka"] = item;
    }
})();


