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
    var tmptext = "";
    var addedFavorites = "";
    var existingFavoriteNodes = [];
    var addingFavoriteNodes = [];
    var exitsNewsPapers = "";
    var msg;
    var db;
    var IDBTransaction = window.IDBTransaction;
    var indexDB = window.indexedDB;

    var page = WinJS.UI.Pages.define("/pages/newsPapers/newsPapersList.html", {

        ready: function (element, options) {

            element.querySelector(".titlearea .pagetitle").textContent = options.split("#")[1];

            document.getElementById("btnAddtoFav").winControl.label = WinJS.Resources.getString('Command2Label').value;

            // Initialize the license info for use in the app that is uploaded to the Store.
            // uncomment for release
            // currentApp = Windows.ApplicationModel.Store.CurrentApp;

            // Initialize the license info for testing.
            // comment the next line for release
            currentApp = Windows.ApplicationModel.Store.CurrentAppSimulator;

            // get the license info
            //licenseInformation = currentApp.licenseInformation;

            //if (licenseInformation.isActive) {
            if (1 == 1) {
                //if (licenseInformation.isTrial) {
                if (1 == 0) {
                    // Show the features that are available during trial only.
                    var tmpDate = new Date();

                    tmpDate.setDate(tmpDate.getDate() + 10);

                    var expiryDate = new Date(tmpDate);
                    var currentDate = new Date();

                    expiryDate = expiryDate.getTime();
                    currentDate = currentDate.getTime();
                    var diff_ms = Math.abs(expiryDate - currentDate);

                    var daysRemaining = Math.round(diff_ms / 86400000);
                    //var daysRemaining = (licenseInformation.expirationDate - new Date()) / 86400000;

                    if (parseInt(Math.round(daysRemaining)) > 0) {

                        // Create the message dialog and set its content
                        var msg = new Windows.UI.Popups.MessageDialog("Your trial ends in " + daysRemaining + ".  Do you want to buy the app now");


                        // Add commands and set their command handlers
                        msg.commands.append(new Windows.UI.Popups.UICommand("Yes", commandYesInvokedHandler));
                        msg.commands.append(new Windows.UI.Popups.UICommand("No", commandNoInvokedHandler));

                        // Set the command that will be invoked by default
                        msg.defaultCommandIndex = 0;

                        // Set the command to be invoked when escape is pressed
                        msg.cancelCommandIndex = 1;

                        // Show the message dialog
                        msg.showAsync();

                        getNewsPapersList(element, options);

                    }
                    else {
                        // Create the message dialog and set its content
                        var msg = new Windows.UI.Popups.MessageDialog("Your trail has ended.  Do you want to buy the app?");

                        // Add commands and set their command handlers
                        msg.commands.append(new Windows.UI.Popups.UICommand("Yes", commandYesInvokedHandler));
                        msg.commands.append(new Windows.UI.Popups.UICommand("No", commandNoInvokedHandler));

                        // Set the command that will be invoked by default
                        msg.defaultCommandIndex = 0;

                        // Set the command to be invoked when escape is pressed
                        msg.cancelCommandIndex = 1;

                        // Show the message dialog
                        msg.showAsync();

                        getNewsPapersList(element, options);
                    }
                }
                else {
                    // Show the features that are available only with a full license.
                    getNewsPapersList(element, options);

                }
            }
            else {
                // A license is inactive only when there's an error.
            }
            //window.indexedDB.deleteDatabase("FavoritesDB", 1);
            createDB();

            document.getElementById("appbar").addEventListener("click", AddtoFav, false);
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
        dbRequest.onupgradeneeded = function (evt) { dbVersionUpgrade(evt); };
        dbRequest.onblocked = function () { };

    }

    function dbSuccess(evt) {
        addedFavorites = "";

        var favorites = [];
        db = evt.target.result;

        // Create a transaction with which to query the IndexedDB.
        var txn = db.transaction("favorites", "readwrite");

        txn.onerror = function () { };
        txn.onabort = function () { };

        //// The oncomplete event handler is called asynchronously once reading is finished and the data arrays are fully populated. This
        //// completion event will occur later than the cursor iterations defined below, because the transaction will not complete until
        /// the cursors are finished.
        txn.oncomplete = function () {
            var txn = db.transaction("favorites", "readwrite");
            var favoriteCursorRequest = txn.objectStore("favorites").openCursor();
            // As each record is returned (asynchronously), the cursor calls the onsuccess event; we store that data in our favorites array
            favoriteCursorRequest.onsuccess = function (e) {
                var cursor = e.target.result;
                if (cursor) {
                    addedFavorites += cursor.value.orgnewsTitle + ",";
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

        // Create the favorites object store, with an index on the news paper title. Note that we set the returned object store to a variable
        // in order to make further calls (index creation) on that object store.
        var favoriteStore = db.createObjectStore("favorites", { keyPath: "orgnewsTitle" });
        favoriteStore.createIndex("newsTitle", "newsTitle", { unique: true });
        favoriteStore.createIndex("title", "title", { unique: false });
        favoriteStore.createIndex("webSite", "webSite", { unique: false });
        favoriteStore.createIndex("orgnewsTitle", "orgnewsTitle", { unique: false });
        favoriteStore.createIndex("orgtitle", "orgtitle", { unique: false });

        // Once the creation of the object stores is finished (they are created asynchronously), log success.
        txn.oncomplete = function () { };
    }
    

    function AddtoFav() {
        exitsNewsPapers = "";
        addingFavoriteNodes.length = 0;
        try {

            var listView = document.querySelector(".newsPaperslist").winControl;
            // Get the actual selected items
            var currentSelectionItems = listView.selection.getItems().then(function (items) {
                return items;
            });

            currentSelectionItems._value.forEach(function (selectedItem) {
                var newsPapers = {
                    title: selectedItem.data.title.trim(),
                    newsTitle: selectedItem.data.newsTitle.trim(),
                    webSite: selectedItem.data.webSite.trim(),
                    orgtitle: selectedItem.data.orgtitle.trim(),
                    orgnewsTitle: selectedItem.data.orgnewsTitle.trim()
                };
                addingFavoriteNodes.push(newsPapers);
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


                var len = addedFavorites.trim().length;
                addedFavorites = addedFavorites.slice(0, len - 1);

                for (var i = 0; i < addingFavoriteNodes.length; i++) {

                    if (addedFavorites.indexOf(addingFavoriteNodes[i].orgnewsTitle, 0) == -1) {
                        favoritesStore.add(addingFavoriteNodes[i]);
                    }
                    else {
                        exitsNewsPapers += addingFavoriteNodes[i].newsTitle + ",";
                    }
                }

                msg = "";
                if (addingFavoriteNodes.length != 0) {
                    if (exitsNewsPapers != "") {
                        var len = exitsNewsPapers.trim().length;
                        exitsNewsPapers = exitsNewsPapers.slice(0, len - 1);

                        if (currentSelectionItems._value.length > exitsNewsPapers.split(",").length) {
                            msg += WinJS.Resources.getString('The following Newspaper(s) already exist in your favorites list.').value + " " + "\n";
                            msg += exitsNewsPapers + "\n\n";
                            msg += WinJS.Resources.getString('Remaining Newspaper(s) added to the favorites list.').value + " "
                        }
                        else if (currentSelectionItems._value.length == exitsNewsPapers.split(",").length) {
                            msg += WinJS.Resources.getString('The following News paper(s) already added to the favorite list.').value + " " + "\n";
                            msg += exitsNewsPapers + "\n";
                        }
                    }
                    else {
                        msg += WinJS.Resources.getString('Selected Newspaper(s) added to your favorites.').value;
                    }
                }
                else {
                    msg += WinJS.Resources.getString('Please select the Newspaper(s) to add into your favorites.').value;
                }

                //var msgdialog = new Windows.UI.Popups.MessageDialog(msg);
                //msgdialog.showAsync().done(function (command) {
                //    if (command.id == 1) {

                //    }
                //    else {

                //    }
                //});

                // Create the message dialog and set its content
                var msgdialog = new Windows.UI.Popups.MessageDialog(msg);

                // Add commands and set their command handlers
                msgdialog.commands.append(new Windows.UI.Popups.UICommand(WinJS.Resources.getString('Close').value));

                // Set the command that will be invoked by default
                msgdialog.defaultCommandIndex = 0;

                // Show the message dialog
                msgdialog.showAsync().done(function (command) {
                    if (command.id == 1) {

                    }
                    else {

                    }
                });
                
            };
        } catch (e) {

           
            //var msg = new Windows.UI.Popups.MessageDialog(WinJS.Resources.getString('Unable to add Newpaper to favorites!.').value);
            //msg.showAsync().done(function (command) {
            //    if (command.id == 1) {

            //    }
            //    else {

            //    }
            //});
            

            var msg = new Windows.UI.Popups.MessageDialog(WinJS.Resources.getString('Unable to add Newpaper to favorites!.').value);

            // Add commands and set their command handlers
            msg.commands.append(new Windows.UI.Popups.UICommand(WinJS.Resources.getString('Close').value));

            // Set the command that will be invoked by default
            msg.defaultCommandIndex = 0;

            // Show the message dialog
            msg.showAsync().done(function (command) {
                if (command.id == 1) {

                }
                else {

                }
            });
        }
    }

    function commandYesInvokedHandler(command) {

        currentApp.requestAppPurchaseAsync(true).done(function (result) {
            var msg = new Windows.UI.Popups.MessageDialog("The full version of the app was enabled");
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

    function getNewsPapersList(element, objValue) {
        var mySplitResult = objValue.split("#");
        var URL = "";
        URL = "xml/NewsPapers.xml";
        WinJS.xhr({ url: URL }).then(function (result) {
            var newsPapersResponse = result.responseXML;

            // Get the info for each news papers 
            var newsPaper = newsPapersResponse.querySelectorAll("c");

            var newsPapersNodes = [];

            for (var newsIndex = 0; newsIndex < newsPaper.length; newsIndex++) {

                if (mySplitResult[1] == WinJS.Resources.getString(newsPaper[newsIndex].attributes.getNamedItem("name").textContent).value && mySplitResult[0] == newsPaper[newsIndex].attributes.getNamedItem("value").textContent) {

                    var newsPaperList = newsPaper[newsIndex].querySelectorAll("paper");

                    for (var newsPaperIndex = 0; newsPaperIndex < newsPaperList.length; newsPaperIndex++) {

                        var newsPapers = {
                            title: WinJS.Resources.getString(newsPaper[newsIndex].attributes.getNamedItem("name").textContent).value,
                            newsTitle: WinJS.Resources.getString(newsPaperList[newsPaperIndex].querySelector("Name").textContent.replace("&", "and")).value,
                            webSite: newsPaperList[newsPaperIndex].querySelector("WebSite").textContent,
                            orgtitle: newsPaper[newsIndex].attributes.getNamedItem("name").textContent,
                            orgnewsTitle: newsPaperList[newsPaperIndex].querySelector("Name").textContent,
                            backgroundImage: "../../../images/Country/" + newsPaper[newsIndex].attributes.getNamedItem("name").textContent + "_" + newsPaperList[newsPaperIndex].querySelector("Name").textContent + ".jpg"
                        };
                        newsPapersNodes.push(newsPapers);
                    }
                }
            }

            if (newsPapersNodes.length != 0) {
                var dataList = new WinJS.Binding.List(newsPapersNodes);
                var listView = element.querySelector(".newsPaperslist").winControl;
                listView.itemDataSource = dataList.dataSource;
                listView.groupHeaderTemplate = element.querySelector(".headerTemplate");
                listView.addEventListener("iteminvoked", itemInvokedHandler, false);
                listView.itemTemplate = element.querySelector(".itemtemplate");
                initializeLayout(listView, appView.value);
                listView.element.focus();
            }
            else {

                //var msg = new Windows.UI.Popups.MessageDialog(WinJS.Resources.getString('Unable to display the newspapers.').value);
                //msg.showAsync().done(function (command) {
                //    if (command.id == 1) {

                //    }
                //    else {

                //    }
                //});

                // Create the message dialog and set its content
                var msg = new Windows.UI.Popups.MessageDialog(WinJS.Resources.getString('Unable to display the newspapers.').value);

                // Add commands and set their command handlers
                msg.commands.append(new Windows.UI.Popups.UICommand(WinJS.Resources.getString('Close').value));

                // Set the command that will be invoked by default
                msg.defaultCommandIndex = 0;

                // Show the message dialog
                msg.showAsync().done(function (command) {
                    if (command.id == 1) {

                    }
                    else {

                    }
                });
            }
        },
        function (error) {
            //document.getElementById("outputDiv").style.display = 'block';
            //document.getElementById("outputDiv").innerHTML = "Unable to display the newspapers.";

            //var msg = new Windows.UI.Popups.MessageDialog(WinJS.Resources.getString('Unable to display the newspapers.').value);
            //msg.showAsync().done(function (command) {
            //    if (command.id == 1) {

            //    }
            //    else {

            //    }
            //});

            // Create the message dialog and set its content
            var msg = new Windows.UI.Popups.MessageDialog(WinJS.Resources.getString('Unable to display the newspapers.').value);

            // Add commands and set their command handlers
            msg.commands.append(new Windows.UI.Popups.UICommand(WinJS.Resources.getString('Close').value));

            // Set the command that will be invoked by default
            msg.defaultCommandIndex = 0;

            // Show the message dialog
            msg.showAsync().done(function (command) {
                if (command.id == 1) {

                }
                else {

                }
            });
        });
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

    // This function updates the page layout in response to viewState changes.
    //function updateLayout(element,viewState,lastViewState) {
    //    /// <param name="element" domElement="true" />
    //    /// <param name="viewState" value="Windows.UI.ViewManagement.ApplicationViewState" />
    //    /// <param name="lastViewState" value="Windows.UI.ViewManagement.ApplicationViewState" />

    //    var listView = element.querySelector(".newsPaperslist").winControl;
    //    if (lastViewState !== viewState) {
    //        if (lastViewState === appViewState.snapped || viewState === appViewState.snapped) {
    //            var handler = function (e) {
    //                listView.removeEventListener("contentanimating", handler, false);
    //                e.preventDefault();
    //            }
    //            listView.addEventListener("contentanimating", handler, false);
    //            initializeLayout(listView, viewState);
    //        }
    //    }
    //}
})();
WinJS.Utilities.markSupportedForProcessing(
window.errorLogger = function (sender, evt) {

});



/*

//var listView = document.querySelector(".newsPaperslist").winControl;
        //// Get the actual selected items
        //listView.selection.getItems().done(function (currentSelection) {
        //    currentSelection.forEach(function (selectedItem) {
        //        var cc = selectedItem;
        //    });
        //}); 

        //    currentSelection.forEach(function (selectedItem) {
        //        var cc = selectedItem;
        //    });
        //});

        //var currentSelectionItems = listView.selection.getItems().done(function (items)
        //{ return items; });


        //for (var i = 0; i < currentSelectionItems.length; i++) {
        //    //		currentSelectionItems._value[0].data.title	"Algeria"	String

        //}

        //roamingFolder.getFileAsync(filename)
        //    .then(function (file) {
        //        //return Windows.Storage.FileIO.readTextAsync(file);
        //    }).done(function (text) {
        //        //counter = parseInt(text);
        //        //document.getElementById("filesOutput").innerText = "file: " + counter;
        //    }, function () {
        //            roamingFolder.createFileAsync(filename, Windows.Storage.CreationCollisionOption.replaceExisting)
        //            .then(function (file) {

        //    }).done(function () {

        //            // ----------------- Commented ----------------------------------------
        //            //IRandomAccessStream readStream = file.OpenAsync(FileAccessMode.ReadWrite);
        //            var xw = Windows.Storage.Streams.DataWriter(new Windows.Storage.Streams.InMemoryRandomAccessStream());

        //            //var xw = new Windows.Storage.Streams.DataWriter('UTF-8');
        //            //xw.formatting = 'indented';//add indentation and newlines
        //            //xw.indentChar = ' ';//indent with spaces

        //            //xw.writeStartDocument();
        //            //xw.writeStartElement('root');

        //            //xw.writeStartElement('c');
        //            //xw.writeAttributeString('name', 'name');
        //            //xw.writeAttributeString('value', 'value');
        //            //xw.writeStartElement('paper');
        //            //xw.writeElementString('Name', 'Name');
        //            //xw.writeElementString('City', 'City');
        //            //xw.writeElementString('WebSite', 'WebSite');
        //            //xw.writeEndElement();
        //            //xw.writeEndElement();
        //            //xw.writeEndElement();
        //            //xw.writeEndDocument();
        //            //Windows.Storage.FileIO.writeTextAsync(filename, xw);

        //           // filename.openTransactedWriteAsync().then(function (transaction) {
        //           //     var dataWriter = new Windows.Storage.Streams.DataWriter(transaction.stream);
        //           //     dataWriter.writeString("<?xml version='1.0' encoding='UTF-8'?><root><c name='Algeria' value='africa'><paper><Name>Algeria Daily</Name><City/><WebSite>http://algeriadaily.com</WebSite></paper>");
        //           //     dataWriter.storeAsync().then(function (size) {
        //           //         transaction.stream.size = size; // reset stream size to override the file
        //           //         transaction.commitAsync().done(function () {
        //           //             transaction.close();
        //           //         });
        //           //     });
        //           // },
        //           //function (error) {
        //           //    WinJS.log && WinJS.log(error, "sample", "error");
        //            //});

        //            //var doc = new Windows.Data.Xml.Dom.XmlDocument;
        //            //doc.toLocaleString("<?xml version='1.0' encoding='UTF-8'?><root><c name='Algeria' value='africa'><paper><Name>Algeria Daily</Name><City/><WebSite>http://algeriadaily.com</WebSite></paper></c></root>");
        //            //doc.saveToFileAsync(filename).done(function () {

        //            //    var originalData = new Windows.Storage.Streams.InMemoryRandomAccessStream();

        //            //    //Populate the new memory stream
        //            //    var outputStream = originalData.getOutputStreamAt(0);
        //            //    var writer = new Windows.Storage.Streams.DataWriter(outputStream);
        //            //    writer.writeBuffer("<?xml version='1.0' encoding='UTF-8'?><root><c name='Algeria' value='africa'><paper><Name>Algeria Daily</Name><City/><WebSite>http://algeriadaily.com</WebSite></paper></c></root>");
        //            //    writer.storeAsync().done (function () {
        //            //        outputStream.flushAsync().done(function () {
        //            //        });
        //            //    });

        //            //}, function (error) {

        //            //});

        //    });            

        //roamingFolder.getFileAsync(filename)
        //    .then(function (file) {
        //        Windows.Storage.FileIO.readTextAsync(file);
        //        return file
        //        //AddFav(Windows.Storage.FileIO.readTextAsync(file), currentSelectionItems, file);
        //    }).done(function (text) {
        //        AddFav(text, currentSelectionItems, file);
        //    }, function () {
        //        roamingFolder.createFileAsync(filename, Windows.Storage.CreationCollisionOption.replaceExisting)
        //        .then(function (file) {
        //            Windows.Storage.FileIO.readTextAsync(file);
        //            return file
        //            //AddFav(Windows.Storage.FileIO.readTextAsync(file), currentSelectionItems, file);
        //        }).done(function (text) {
        //            AddFav(text, currentSelectionItems, file);
        //        });

        //    });


        // ============================================================================================================
        //var dataFile = roamingFolder.getFileAsync(filename)
        //    .then(function (file) {
        //        return Windows.Storage.FileIO.readTextAsync(file);
        //        //return file
        //        //AddFav(Windows.Storage.FileIO.readTextAsync(file), currentSelectionItems, file);
        //    }).done(function (text) {
        //        //AddFav(text, currentSelectionItems);
        //    }, function () {
        //        roamingFolder.createFileAsync(filename, Windows.Storage.CreationCollisionOption.replaceExisting)
        //        .then(function (file) {
        //            return Windows.Storage.FileIO.readTextAsync(file);
        //            //return file
        //            //AddFav(Windows.Storage.FileIO.readTextAsync(file), currentSelectionItems, file);
        //        }).done(function (text) {
        //            //AddFav(text, currentSelectionItems);
        //        });

        //    });
        // ===============================================================================================================

        //var text = Windows.Storage.FileIO.readTextAsync(dataFile);
        //AddFav(text, currentSelectionItems, file);
        //if (favContent == "") {
        //    storageFile = roamingFolder.createFileAsync(filename, Windows.Storage.CreationCollisionOption.openIfExists).then(function (dataFile) {
        //        return dataFile;
        //    });
        //}

        //for (var i = 0; i < currentSelectionItems._value.length; i++) {
        //currentSelectionItems._value.forEach(function (selectedItem) {
        //    roamingFolder.createFileAsync(filename, Windows.Storage.CreationCollisionOption.openIfExists).then(function (dataFile) {
        //        dataFile.openAsync(Windows.Storage.FileAccessMode.read).then(function (stream) {
        //            var size = stream.size;
        //            if (size == 0) {
        //                // Data not found
        //                Windows.Storage.FileIO.writeTextAsync(dataFile, selectedItem.data.newsTitle + "," + selectedItem.data.webSite + " #").then(function () {
        //                });
        //            }
        //            else {
        //                Windows.Storage.FileIO.appendTextAsync(dataFile, selectedItem.data.newsTitle + "," + selectedItem.data.webSite + " #").then(function () {
        //                });
        //            }

        //        })
        //    });
        //});
        //}

        //var dataFile = roamingFolder.createFileAsync(filename, Windows.Storage.CreationCollisionOption.openIfExists);
        //var stream = dataFile.openAsync(Windows.Storage.FileAccessMode.read);

        //var size = stream.size;
        //for (var index = 0; index < mySplitResult.length; index++) {

                                //    if (mySplitResult[index].split(",")[0] != selectedItem.data.newsTitle) {
                                //        // Windows.Storage.FileIO.appendTextAsync(dataFile, selectedItem.data.newsTitle + "," + selectedItem.data.webSite + " # ").then(function () {
                                //        //  });
                                //    }
                                //    else {

                                //        //window.external.notify("News paper already exits!.");
                                //        var msg = new Windows.UI.Popups.MessageDialog("News paper already exits!.");
                                //        //msg.showAsync();
                                //        //// Show the message dialog                                    
                                //        msg.showAsync().done(function (command) {
                                //            if (command.id == 1) {

                                //            }
                                //            else {

                                //            }
                                //        });
                                //    }
                                //}
    //function AddFav(stext, currentItems) {
    //    var file = roamingFolder.getFileAsync(filename)
    //        .then(function (file) {
    //            return file
    //        });
    //    for (var i = 0; i < currentItems._value.length; i++) {
    //        if (stext == "") {

    //            if (i == 0) {
    //                Windows.Storage.FileIO.writeTextAsync(file, currentItems._value[i].data.title + "," + currentItems._value[i].data.webSite + "#").then(function () {
    //                });
    //            }
    //            else {
    //                Windows.Storage.FileIO.appendTextAsync(file, currentItems._value[i].data.title + "," + currentItems._value[i].data.webSite + "#").then(function () {
    //                });
    //            }
    //        }
    //        else {

    //        }

    //    }
    //}
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
    */