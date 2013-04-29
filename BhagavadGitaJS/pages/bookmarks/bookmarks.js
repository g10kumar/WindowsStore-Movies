(function () {
    //"use strict";
    var appView = Windows.UI.ViewManagement.ApplicationView;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var binding = WinJS.Binding;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    var currentApp;
    var favText = "";
    var bookMarksNodes = [];
    var db;
    var IDBTransaction = window.IDBTransaction;
    var indexDB = window.indexedDB;
    var deletingBookmarkNodes = [];
    var page = WinJS.UI.Pages.define("/pages/bookmarks/bookmarks.html", {

        ready: function (element, options) {

            element.querySelector(".titlearea .pagetitle").textContent = "Favorites";

            createDB();

            document.getElementById("btnDeleteBookmarks").addEventListener("click", deleteBookmark, false);

        },
        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />
            /// <param name="viewState" value="Windows.UI.ViewManagement.ApplicationViewState" />
            /// <param name="lastViewState" value="Windows.UI.ViewManagement.ApplicationViewState" />

            var listView = element.querySelector(".bookmarkslist").winControl;

            if (lastViewState !== viewState) {
                if (lastViewState === appViewState.snapped || viewState === appViewState.snapped) {
                    var handler = function (e) {
                        listView.removeEventListener("contentanimating", handler, false);
                        e.preventDefault();
                    }
                    listView.addEventListener("contentanimating", handler, false);
                    initializeLayout(listView, viewState);
                }
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
        dbRequest.onupgradeneeded = function (evt) { if (db) { db.close(); } };
        dbRequest.onblocked = function () { };

    }

    function dbSuccess(evt) {
        if (db) {
            db.close();
        }
        db = evt.target.result;
        if (db.objectStoreNames.length === 0) {
            db.close();
            db = null;
            window.indexedDB.deleteDatabase("BookmarksDB", 1);

            if (bookMarksNodes.length == 0) {
                var msg = new Windows.UI.Popups.MessageDialog("No slokas in your Favorites!");
                msg.showAsync().done(function (command) {
                    if (command.id == 1) {

                    }
                    else {

                    }
                });
            }
        } else {
            readData(evt);
        }
    }

    function readData(evt) {

        var bookmarks = [];

        // Create a transaction with which to query the IndexedDB.
        var txn = db.transaction("bookmarks", "readwrite");

        txn.onerror = function () { };
        txn.onabort = function () { };

        //// The oncomplete event handler is called asynchronously once reading is finished and the data arrays are fully populated. This
        //// completion event will occur later than the cursor iterations defined below, because the transaction will not complete until
        /// the cursors are finished.
        txn.oncomplete = function () {
            bookMarksNodes.length = 0;
            var txn = db.transaction("bookmarks", "readwrite");

            var bookmarksCursorRequest = txn.objectStore("bookmarks").openCursor();
            // As each record is returned (asynchronously), the cursor calls the onsuccess event; we store that data in our favorites array
            bookmarksCursorRequest.onsuccess = function (e) {
                var cursor = e.target.result;
                if (cursor) {
                    var bookMarks = {
                        chapter: cursor.value.chapter.trim(),
                        sloka: "Chapter " + cursor.value.chapterKey.trim().split("#")[1].trim() +"/"+cursor.value.sloka.trim(),
                        chaptersloka: cursor.value.chaptersloka.trim(),
                        chapterKey: cursor.value.chapterKey.trim()
                    };
                    bookMarksNodes.push(bookMarks);
                    cursor.continue();
                }
                displayBookmarksList();
            };
            
        };
    }

    function displayBookmarksList() {

        if (bookMarksNodes.length > 0) {
            var dataList = new WinJS.Binding.List(bookMarksNodes);
            var listView = document.querySelector(".bookmarkslist").winControl;
            listView.itemDataSource = dataList.dataSource;
            listView.groupHeaderTemplate = document.querySelector(".headerTemplate");
            listView.addEventListener("iteminvoked", itemInvokedHandler, false);
            listView.itemTemplate = document.querySelector(".itemtemplate");
            initializeLayout(listView, appView.value);
            listView.element.focus();
        }
        else {
            var listView = document.querySelector(".bookmarkslist").winControl;
            if (listView != null) {
                listView.itemDataSource = null;
            }
            var msg = new Windows.UI.Popups.MessageDialog("No Slokas in your Favorites!");
            msg.showAsync().done(function (command) {
                if (command.id == 1) {

                }
                else {

                }
            });
        }
    }

    function deleteBookmark() {
        try {

            var notdeletingbookmarks = "";

            var listView = document.querySelector(".bookmarkslist").winControl;
            // Get the actual selected items
            var currentSelectionItems = listView.selection.getItems().then(function (items) {
                return items;
            });

            currentSelectionItems._value.forEach(function (selectedItem) {
                var bookMarks = {
                    chapter: selectedItem.data.chapter.trim(),
                    sloka: selectedItem.data.sloka.trim().split("/")[1].trim(),
                    chaptersloka: selectedItem.data.chaptersloka.trim(),
                    chapterKey: selectedItem.data.chapterKey.trim()
                };
                deletingBookmarkNodes.push(bookMarks);
            });

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

                for (var i = 0; i < deletingBookmarkNodes.length; i++) {

                    try {
                        bookmarksStore.delete(deletingBookmarkNodes[i].chaptersloka);
                    }
                    catch (e) {
                        notdeletingbookmarks += deletingBookmarkNodes[i].chaptersloka + ",";
                    }
                }

                msg = "";
                if (notdeletingbookmarks != "") {
                    var len = notdeletingbookmarks.trim().length;
                    notdeletingbookmarks = notdeletingbookmarks.slice(0, len - 1);

                    msg += "The following Sloka(s) were not deleted from Favorites list." + "\n";
                    msg += notdeletingpapers + "\n\n";
                }
                else {
                    msg += "Selected Sloka(s) deleted from Favorites.";
                }

                var msgdialog = new Windows.UI.Popups.MessageDialog(msg);
                msgdialog.showAsync().done(function (command) {
                    if (command.id == 1) {

                    }
                    else {

                    }

                    createDB();
                });

            };
        } catch (e) {
            var msg = new Windows.UI.Popups.MessageDialog("Unable to delete Slokas from Favorites.");
            msg.showAsync().done(function (command) {
                if (command.id == 1) {

                }
                else {

                }
            });
        }
    }


    function itemInvokedHandler(eventObject) {
        eventObject.detail.itemPromise.done(function (invokedItem) {

            var slokaItem = invokedItem.data.sloka.split(" ");            
            WinJS.Navigation.navigate("/pages/sloka/sloka.html", { groupKey: invokedItem.data.chapterKey, selectedIndex: slokaItem[1] });
        });
    }

    // This function updates the ListView with new layouts
    function initializeLayout(listView, viewState) {
        if (viewState === appViewState.snapped) {
            listView.layout = new ui.ListLayout();

        } else {
            listView.layout = new ui.GridLayout({ groupHeaderPosition: "top" });
            listView.layout = new ui.GridLayout();
        }
    }

})();
WinJS.Utilities.markSupportedForProcessing(
window.errorLogger = function (sender, evt) {

});