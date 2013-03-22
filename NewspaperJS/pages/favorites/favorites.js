(function () {
    //"use strict";
    var appView = Windows.UI.ViewManagement.ApplicationView;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var binding = WinJS.Binding;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    var roamingFolder = Windows.Storage.ApplicationData.current.roamingFolder;
    var filename = "FavQuotes.txt";
    var currentApp;
    var favText = "";
    var newsPapersNodes = [];
    var db;
    var IDBTransaction = window.IDBTransaction;
    var indexDB = window.indexedDB;
    var deletingFavoriteNodes = [];
    var page = WinJS.UI.Pages.define("/pages/favorites/favorites.html", {

        ready: function (element, options) {

            element.querySelector(".titlearea .pagetitle").textContent = "Favorites";

            // Initialize the license info for use in the app that is uploaded to the Store.
            // uncomment for release
            // currentApp = Windows.ApplicationModel.Store.CurrentApp;

            // Initialize the license info for testing.
            // comment the next line for release
            currentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;

            createDB();
            //displayFavoriteNewsPapersList();            

            document.getElementById("appbar").addEventListener("click", deleteFav, false);
        },
        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />
            /// <param name="viewState" value="Windows.UI.ViewManagement.ApplicationViewState" />
            /// <param name="lastViewState" value="Windows.UI.ViewManagement.ApplicationViewState" />

            var listView = element.querySelector(".newsPaperslist").winControl;
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
        var dbRequest = window.indexedDB.open("FavoritesDB", 1);

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
            window.indexedDB.deleteDatabase("FavoritesDB", 1);

            if (newsPapersNodes.length == 0) {
                var msg = new Windows.UI.Popups.MessageDialog("No Newspapers in your favorites!");
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

        var favorites = [];

        // Create a transaction with which to query the IndexedDB.
        var txn = db.transaction("favorites", "readwrite");

        txn.onerror = function () { };
        txn.onabort = function () { };

        //// The oncomplete event handler is called asynchronously once reading is finished and the data arrays are fully populated. This
        //// completion event will occur later than the cursor iterations defined below, because the transaction will not complete until
        /// the cursors are finished.
        txn.oncomplete = function () {
            newsPapersNodes.length = 0;
            var txn = db.transaction("favorites", "readwrite");
                       
            var favoriteCursorRequest = txn.objectStore("favorites").openCursor();
            // As each record is returned (asynchronously), the cursor calls the onsuccess event; we store that data in our favorites array
            favoriteCursorRequest.onsuccess = function (e) {
                var cursor = e.target.result;
                if (cursor) {
                    var newsPapers = {
                        title: cursor.value.title.trim(),
                        newsTitle: cursor.value.newsTitle.trim(),
                        webSite: cursor.value.webSite.trim(),
                        backgroundImage: "../../../images/Country/" + cursor.value.title.trim() + "_" + cursor.value.newsTitle.trim() + ".jpg"
                    };
                    newsPapersNodes.push(newsPapers);
                    cursor.continue();
                }
                displayFavoriteNewsPapersList();
            };           
        };
    }

    function displayFavoriteNewsPapersList() {

        if (newsPapersNodes.length > 0)
        {
            var dataList = new WinJS.Binding.List(newsPapersNodes);
            var listView = document.querySelector(".newsPaperslist").winControl;
            listView.itemDataSource = dataList.dataSource;
            listView.groupHeaderTemplate = document.querySelector(".headerTemplate");
            listView.addEventListener("iteminvoked", itemInvokedHandler, false);
            listView.itemTemplate = document.querySelector(".itemtemplate");
            initializeLayout(listView, appView.value);
            listView.element.focus();
        }
        else
        {
            var listView = document.querySelector(".newsPaperslist").winControl;
            if (listView != null) {
                listView.itemDataSource = null;
            }
            var msg = new Windows.UI.Popups.MessageDialog("No Newspapers in your favorites!");
            msg.showAsync().done(function (command) {
                if (command.id == 1) {

                }
                else {

                }
            });
        }
    }

    function deleteFav() {
        try {

            var notdeletingpapers = "";

            var listView = document.querySelector(".newsPaperslist").winControl;
            // Get the actual selected items
            var currentSelectionItems = listView.selection.getItems().then(function (items) {
                return items;
            });

            currentSelectionItems._value.forEach(function (selectedItem) {
                var newsPapers = {
                    title: selectedItem.data.title.trim(),
                    newsTitle: selectedItem.data.newsTitle.trim(),
                    webSite: selectedItem.data.webSite.trim()
                };
                deletingFavoriteNodes.push(newsPapers);
            });

            // Create a transaction with which to query the IndexedDB.
            var txn = db.transaction(["favorites"], "readwrite");

            // Set the event callbacks for the transaction.
            txn.onerror = function () { };
            txn.onabort = function () { };

            //// The oncomplete event handler is called asynchronously once reading is finished and the data arrays are fully populated. This
            //// completion event will occur later than the cursor iterations defined below, because the transaction will not complete until
            /// the cursors are finished.
            txn.oncomplete = function () {

                var txn = db.transaction(["favorites"], "readwrite");
                var favoritesStore = txn.objectStore("favorites");

                for (var i = 0; i < deletingFavoriteNodes.length; i++) {

                    try{
                        favoritesStore.delete(deletingFavoriteNodes[i].newsTitle);
                        //delete newsPapersNodes[i]
                    }
                    catch(e) {
                        notdeletingpapers += deletingFavoriteNodes[i].newsTitle + ",";
                    }
                }

                msg = "";
                if (notdeletingpapers != "") {
                    var len = notdeletingpapers.trim().length;
                    notdeletingpapers = notdeletingpapers.slice(0, len - 1);

                    if (currentSelectionItems._value.length > notdeletingpapers.split(",").length) {
                        msg += "The following Newspaper(s) were not deleted from favorite list." + "\n";
                        msg += notdeletingpapers + "\n\n";
                        msg += "Remaining Newspaper(s) were deleted from favorite list."
                    }
                    else if (currentSelectionItems._value.length == notdeletingpapers.split(",").length) {
                        msg += "The following News paper(s) are deleted from favorite list." + "\n";
                        msg += notdeletingpapers + "\n";
                    }
                }
                else {
                    msg += "Selected Newspaper(s) deleted from favorites.";
                }

                var msgdialog = new Windows.UI.Popups.MessageDialog(msg);
                msgdialog.showAsync().done(function (command) {
                    if (command.id == 1) {

                    }
                    else {

                    }

                    createDB();
                });
                
                //displayFavoriteNewsPapersList(newsPapersNodes);

            };
        } catch (e) {
            var msg = new Windows.UI.Popups.MessageDialog("Unable to delete Newpapers from Favorites.");
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

            window.open(invokedItem.data.webSite);
        });
    }

    // This function updates the ListView with new layouts
    function initializeLayout(listView, viewState) {
        if (viewState === appViewState.snapped) {
            listView.layout = new ui.ListLayout();
            //document.getElementById("myAd2").style.display = 'block';
            document.getElementById("myAd").style.display = 'none';
            document.getElementById("myAd1").style.display = 'none';
            document.getElementById("myAd2").style.display = 'block';
            document.getElementById("myAd2").style.visibility = 'visible';

        } else {
            listView.layout = new ui.GridLayout({ groupHeaderPosition: "top" });
            listView.layout = new ui.GridLayout();
            document.getElementById("myAd2").style.display = 'none';
            document.getElementById("myAd2").style.visibility = 'collapse';
            document.getElementById("myAd").style.display = 'block';
            document.getElementById("myAd1").style.display = 'block';
        }
    }

})();
WinJS.Utilities.markSupportedForProcessing(
window.errorLogger = function (sender, evt) {

});

/*
var getFavString = function () {
    var sText;
    roamingFolder.getFileAsync(filename)
            .then(function (file) {
                return Windows.Storage.FileIO.readTextAsync(file);
            }).done(function (text) {
                //Printing Correct String
                sText = text;
            });
    return sText;
}

function displayFavoriteNewsPapersList() {

    favText = getFavString();

    roamingFolder.getFileAsync(filename)
        .then(function (file) {
            return Windows.Storage.FileIO.readTextAsync(file);
        }).done(function (content) {
            //Printing Correct String
            text = content;
            favText = text;
            if (favText != 'undefined' && favText != null) {

                if (favText != "") {
                    var len = favText.trim().length;
                    favText = favText.slice(0, len - 1);
                    var mySplitResult = favText.split("#");
                    newsPapersNodes.length = 0;
                    for (var newsPaperIndex = 0; newsPaperIndex < mySplitResult.length; newsPaperIndex++) {

                        if (mySplitResult[newsPaperIndex].split(",")[0] != null) {

                            var newsPapers = {
                                title: mySplitResult[newsPaperIndex].split("~")[0].trim(),
                                newsTitle: mySplitResult[newsPaperIndex].split("~")[1].trim(),
                                webSite: mySplitResult[newsPaperIndex].split("~")[2].trim(),
                                backgroundImage: "../../../images/Country/" + mySplitResult[newsPaperIndex].split("~")[0].trim() + "_" + mySplitResult[newsPaperIndex].split("~")[1].trim() + ".jpg"
                            };
                            newsPapersNodes.push(newsPapers);
                        }
                    }

                    var dataList = new WinJS.Binding.List(newsPapersNodes);
                    var listView = document.querySelector(".newsPaperslist").winControl;
                    listView.itemDataSource = dataList.dataSource;
                    listView.groupHeaderTemplate = document.querySelector(".headerTemplate");
                    listView.addEventListener("iteminvoked", itemInvokedHandler, false);
                    listView.itemTemplate = document.querySelector(".itemtemplate");
                    initializeLayout(listView, appView.value);
                    listView.element.focus();
                }
                else {
                    var msg = new Windows.UI.Popups.MessageDialog("You have not added any News papers to your favorites as yet!");
                    msg.showAsync().done(function (command) {
                        if (command.id == 1) {

                        }
                        else {

                        }
                    });
                }
            }
            else {
                var msg = new Windows.UI.Popups.MessageDialog("Unable to load Favorite new papers now!.");
                msg.showAsync().done(function (command) {
                    if (command.id == 1) {

                    }
                    else {

                    }
                });
            }
        }, function (error) {
            var msg = new Windows.UI.Popups.MessageDialog("Unable to load Favorite new papers now!.");
            msg.showAsync().done(function (command) {
                if (command.id == 1) {

                }
                else {

                }
            });
        });

}

function deleteFavNewspapers() {
    var favoritesText;
    var tmptext;

    try {
        var listView = document.querySelector(".newsPaperslist").winControl;
        // Get the actual selected items
        var currentSelectionItems = listView.selection.getItems().then(function (items) {
            return items;
        });


        currentSelectionItems._value.forEach(function (selectedItem) {
            //favoritesText.replace(selectedItem.data.title + "," + selectedItem.data.newsTitle + "," + selectedItem.data.webSite + " # ", "");
            //tmptext.concat(selectedItem.data.title + "," + selectedItem.data.newsTitle + "," + selectedItem.data.webSite + " #");                    
            //favoritesText.replace(new RegExp(selectedItem.data.title + "," + selectedItem.data.newsTitle + "," + selectedItem.data.webSite + " #"), "");                    
            for (var i = 0; i < newsPapersNodes.length; i++) {
                if (newsPapersNodes[i] != null) {
                    if (newsPapersNodes[i].newsTitle == selectedItem.data.newsTitle) {
                        delete newsPapersNodes[i];
                    }
                }
            }
        });
        tmptext = "";
        for (var j = 0; j < newsPapersNodes.length; j++) {
            if (newsPapersNodes[j] != null) {
                tmptext += newsPapersNodes[j].title + "~" + newsPapersNodes[j].newsTitle + "~" + newsPapersNodes[j].webSite + " # ";
            }
        }

        roamingFolder.createFileAsync(filename, Windows.Storage.CreationCollisionOption.openIfExists).then(function (dataFile) {
            dataFile.openAsync(Windows.Storage.FileAccessMode.read).then(function (stream) {
                return stream.size;
            }).done(function (size) {
                if (size == 0) {
                    // Data not found

                }
                else {
                    Windows.Storage.FileIO.writeTextAsync(dataFile, tmptext).then(function () {
                    });
                }
                newsPapersNodes.length = 0;
                //nav.navigate("/pages/favorites/favorites.html");
                displayFavoriteNewsPapersList();
            })
        });

        //roamingFolder.getFileAsync(filename)
        //.then(function (file) {
        //    return Windows.Storage.FileIO.readTextAsync(file);
        //}).done(function (content) {
        //    //Printing Correct String
        //    text = content;
        //    favoritesText = text;
        //    currentSelectionItems._value.forEach(function (selectedItem) {
        //        favoritesText.replace(selectedItem.data.title + "," + selectedItem.data.newsTitle + "," + selectedItem.data.webSite + " # ", "");
        //        //tmptext.concat(selectedItem.data.title + "," + selectedItem.data.newsTitle + "," + selectedItem.data.webSite + " #");                    
        //        //favoritesText.replace(new RegExp(selectedItem.data.title + "," + selectedItem.data.newsTitle + "," + selectedItem.data.webSite + " #"), "");                    
        //    });

        //    //Windows.Storage.FileIO.writeTextAsync(file, favText).then(function () {
        //    //});

        //    //favoritesText.replace(new RegExp(tmptext), "");

        //    roamingFolder.createFileAsync(filename, Windows.Storage.CreationCollisionOption.openIfExists).then(function (dataFile) {
        //        dataFile.openAsync(Windows.Storage.FileAccessMode.read).then(function (stream) {
        //            return stream.size;
        //        }).done(function (size) {
        //            if (size == 0) {
        //                // Data not found

        //            }
        //            else {
        //                Windows.Storage.FileIO.writeTextAsync(dataFile, favoritesText).then(function () {
        //                });
        //            }
        //        })
        //    });

        //});


    }
    catch (e) {

    }
}
*/