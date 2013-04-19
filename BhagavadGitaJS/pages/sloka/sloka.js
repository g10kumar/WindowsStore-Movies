(function () {
    //"use strict";

    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var appView = Windows.UI.ViewManagement.ApplicationView;
    var binding = WinJS.Binding;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    var dtm = Windows.ApplicationModel.DataTransfer.DataTransferManager;
    var item;
    var bookmarkitem;
    var chapterTitle;
    var chapterKey;
    var appdata = Windows.Storage.ApplicationData;

    var addedbookmarks = "";
    var existingBookmarkNodes = [];
    var addingBookmarkNodes = [];
    var exitsBookmarks = "";
    var db;
    var IDBTransaction = window.IDBTransaction;
    var indexDB = window.indexedDB;

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
            chapterKey = this._group.key;
            this._items = Data.getItemsFromGroup(this._group);
            this._itemSelectionIndex = (options && "selectedIndex" in options) ? options.selectedIndex : -1;


            element.querySelector("header[role=banner] .pagetitle").textContent = this._group.title;
            chapterTitle = this._group.title;

            var flipView = element.querySelector(".flipView").winControl;

            flipView.itemDataSource = this._items.dataSource;
            //flipView._selectedIndex = this._itemSelectionIndex;
            flipView.itemTemplate = document.getElementById("simple_ItemTemplate");
            flipView.addEventListener("pageselected", handlePageSelected);
            flipView.currentPage = this._itemSelectionIndex - 1;
            
            dtm.getForCurrentView().addEventListener("datarequested", this.onDataRequested);

            document.getElementById("btnCopy").addEventListener("click", textCopy, false);
            document.getElementById("btnCommentary").addEventListener("click", slokaMeaning, false);
            document.getElementById("btnBookmark").addEventListener("click", addBookmark, false);

            //window.indexedDB.deleteDatabase("BookmarksDB", 1);
            createDB();

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

    function createDB() {
        // Create the request to open the database, named FavoritesDB. If it doesn't exist, create it and immediately
        // upgrade to version 1.
        var dbRequest = window.indexedDB.open("BookmarksDB", 1);

        // Add asynchronous callback functions
        dbRequest.onerror = function () { };
        dbRequest.onsuccess = function (evt) { dbSuccess(evt); };
        dbRequest.onupgradeneeded = function (evt) { dbVersionUpgrade(evt); };
        dbRequest.onblocked = function () { };

    }

    function dbSuccess(evt) {
        addedbookmarks = "";

        var bookmarks = [];
        db = evt.target.result;

        // Create a transaction with which to query the IndexedDB.
        var txn = db.transaction("bookmarks", "readwrite");

        txn.onerror = function () { };
        txn.onabort = function () { };

        //// The oncomplete event handler is called asynchronously once reading is finished and the data arrays are fully populated. This
        //// completion event will occur later than the cursor iterations defined below, because the transaction will not complete until
        /// the cursors are finished.
        txn.oncomplete = function () {
            var txn = db.transaction("bookmarks", "readwrite");
            var bookmarksCursorRequest = txn.objectStore("bookmarks").openCursor();
            // As each record is returned (asynchronously), the cursor calls the onsuccess event; we store that data in our favorites array
            bookmarksCursorRequest.onsuccess = function (e) {
                var cursor = e.target.result;
                if (cursor) {
                    addedbookmarks += cursor.value.chaptersloka + ", ";
                    cursor.continue();
                }
            };
        };
    }

    // Whenever an IndexedDB is created, the version is set to "", but can be immediately upgraded by calling createDB. 
    function dbVersionUpgrade(evt) {

        // If the database was previously loaded, close it. 
        // Closing the database keeps it from becoming blocked for later delete operations.
        if (db) {
            db.close();
        }
        db = evt.target.result;

        // Get the version update transaction handle, since we want to create the schema as part of the same transaction.
        var txn = evt.target.transaction;

        // Create the favorites object store, with an index on the sloka title. Note that we set the returned object store to a variable
        // in order to make further calls (index creation) on that object store.
        var bookmarkStore = db.createObjectStore("bookmarks", { keyPath: "chaptersloka" });
        bookmarkStore.createIndex("chaptersloka", "chaptersloka", { unique: true });
        bookmarkStore.createIndex("chapter", "chapter", { unique: false });
        bookmarkStore.createIndex("sloka", "sloka", { unique: false });        
        bookmarkStore.createIndex("chapterKey", "chapterKey", { unique: false });

        // Once the creation of the object stores is finished (they are created asynchronously), log success.
        txn.oncomplete = function () { };
    }

    function addBookmark() {
        exitsBookmarks = "";
        addingBookmarkNodes.length = 0;
        try {

            var bookMarks = {
                chapter: bookmarkitem.split(";")[0].trim(),
                sloka: bookmarkitem.split(";")[1].trim(),
                chaptersloka: bookmarkitem.split(";")[0].trim() + ";" + bookmarkitem.split(";")[1].trim(),
                chapterKey: bookmarkitem.split(";")[2].trim()
            };
            addingBookmarkNodes.push(bookMarks);

            // Create a transaction with which to query the IndexedDB.
            var txn = db.transaction(["bookmarks"], "readwrite");

            // Set the event callbacks for the transaction.
            txn.onerror = function () { };
            txn.onabort = function () { };

            //// The oncomplete event handler is called asynchronously once reading is finished and the data arrays are fully populated. This
            //// completion event will occur later than the cursor iterations defined below, because the transaction will not complete until
            /// the cursors are finished.
            txn.oncomplete = function () {

                var txn = db.transaction(["bookmarks"], "readwrite");
                var bookmarksStore = txn.objectStore("bookmarks");


                var len = addedbookmarks.trim().length;
                addedbookmarks = addedbookmarks.slice(0, len - 1);

                //for (var i = 0; i < addingBookmarkNodes.length; i++) {

                    if (addedbookmarks.indexOf(addingBookmarkNodes[0].chaptersloka, 0) == -1)
                    {
                        //bookmarksStore.add(addingBookmarkNodes[0]);
                        var addResult = bookmarksStore.add({ chaptersloka: addingBookmarkNodes[0].chaptersloka, chapter: addingBookmarkNodes[0].chapter, sloka: addingBookmarkNodes[0].sloka, chapterKey: addingBookmarkNodes[0].chapterKey });
                        addResult.onerror = function (evt) {
                            msg += "Error while adding bookmarks";
                        };
                    }
                    else
                    {
                        exitsBookmarks += addingBookmarkNodes[0].chaptersloka + ",";
                    }
                    
               // }

                msg = "";
                if (exitsBookmarks != "") {
                    var len = exitsBookmarks.trim().length;
                    exitsBookmarks = exitsBookmarks.slice(0, len - 1);

                    msg += "The following Sloka already exist in your bookmarks list. " + "\n";
                    msg += exitsBookmarks + ".";

                }
                else {
                    msg += "Selected Sloka added to your bookmarks.";
                }

                var msgdialog = new Windows.UI.Popups.MessageDialog(msg);
                msgdialog.showAsync().done(function (command) {
                    if (command.id == 1) {

                    }
                    else {

                    }
                });

            };
        } catch (e) {
            var msg = new Windows.UI.Popups.MessageDialog("Unable to add Sloka to bookmarks!.");
            msg.showAsync().done(function (command) {
                if (command.id == 1) {

                }
                else {

                }
            });
        }
    }

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

    function slokaMeaning() {
        var dbPath = Windows.Storage.ApplicationData.current.localFolder.path + '\\gita.sqlite';
        SQLite3JS.openAsync(dbPath)
          .then(function (db) {
               return db.eachAsync('SELECT * FROM Item', function (row) {
                   console.log('Get a ' + row.name + ' for $' + row.price);
               });
           })
          .then(function (db) {
              db.close();
          });
    }

    function displayError(errorString) {
        var msg = new Windows.UI.Popups.MessageDialog(errorString);
        // Show the message dialog
        msg.showAsync()
    }

    function handlePageSelected(ev) {
        //item = ev.target.innerText.trim().replace(new RegExp("\r\n", "g"), "<p>").replace(new RegExp("Sloka", "g"), "");
        item = "Chapter: " + chapterTitle + "; " + ev.target.innerText.trim().replace(new RegExp("\r\n", "g"), "<p>");
        bookmarkitem = chapterTitle + ";" + ev.target.innerText.trim().replace(new RegExp("\r\n", "g"), "<p>").split("<p><p><p>")[0] + ";" + chapterKey;
        appdata.current.roamingSettings.values["recentSloka"] = item;
    }
})();


