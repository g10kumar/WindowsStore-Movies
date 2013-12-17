// Global array used to persist operations.
var downloadOperations = [];

(function () {
    "use strict";

    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var appView = Windows.UI.ViewManagement.ApplicationView;
    var binding = WinJS.Binding;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    var dtm = Windows.ApplicationModel.DataTransfer.DataTransferManager;
    var bookListsNodes = [];
    var authorListsNodes = [];
    var authorGroups;
    var dataList;
    var gitaBooks = new WinJS.Binding.List();
    var authorsGroupslist = [];
    var dataPromises = [];
    var pdfLib = Windows.Data.Pdf;
    var PDF_PAGE_INDEX = 0; // First Page
    var ZOOM_FACTOR = 2; // 200% Zoom
    var PDF_PORTION_RECT = { height: 400, width: 300, x: 100, y: 200 }; // Portion of a page
    
    

    ui.Pages.define("/pages/gitabooks/displaybooks.html", {

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            // Store information about the group and selection that this page will
            // display.
            bookListsNodes.length = 0;
            DisplayGitaBooksAuthors(options);
            DisplayGitaBooks("Paramahamsa Nithyananda");

        },

        unload: function () {

        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />            
        }

    });

    function DisplayGitaBooksAuthors(options) {
        authorListsNodes.length = 0;
        try {

            var URL = "";
            URL = "xml/books.xml";
            WinJS.xhr({ url: URL }).then(function (result) {
                var authorListResponse = result.responseXML;

                // Get the info for books list 
                var authorsList = authorListResponse.querySelectorAll("book");

                for (var authorIndex = 0; authorIndex < authorsList.length; authorIndex++) {
                    var authorLists = {
                        author: authorsList[authorIndex].querySelector("author").textContent
                    };
                    authorListsNodes.push(authorLists);
                }

                var listView = document.querySelector(".authorlist").winControl;
                var dataList = new WinJS.Binding.List(authorListsNodes);
                listView.itemDataSource = dataList.dataSource;
                listView.addEventListener("iteminvoked", itemInvokedHandler, false);
                listView.groupHeaderTemplate = document.querySelector(".headerTemplate");
                listView.itemTemplate = document.querySelector(".itemtemplate");
                initializeLayout(listView, appView.value);
                listView.element.focus();


            },
            function (error) {
                // Create the message dialog and set its content
                var msg = new Windows.UI.Popups.MessageDialog(WinJS.Resources.getString('Unable to display the Gita Books Authors.').value);

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

        } catch (e) {

        }
    }

    function itemInvokedHandler(eventObject) {
        eventObject.detail.itemPromise.done(function (invokedItem) {
            bookListsNodes.length = 0;
            var author = invokedItem.data.author;
            DisplayGitaBooks(author);
        });
    }    

    function DisplayGitaBooks(author) {
        try {

            var URL = "";
            URL = "xml/books.xml";
            WinJS.xhr({ url: URL }).then(function (result) {
                var bookListResponse = result.responseXML;

                // Get the info for books list 
                var booksList = bookListResponse.querySelectorAll("book");

                for (var bookIndex = 0; bookIndex < booksList.length; bookIndex++) {

                    if (booksList[bookIndex].querySelector("author").textContent == author) {

                        var volumelist = booksList[bookIndex].querySelectorAll("volume");

                        for (var volumeIndex = 0; volumeIndex < volumelist.length; volumeIndex++) {
                            var volumeLists = {
                                title: volumelist[volumeIndex].querySelector("title").textContent,
                                url: volumelist[volumeIndex].querySelector("url").textContent
                            };
                            bookListsNodes.push(volumeLists);
                        }
                    }
                }

                var listView = document.querySelector(".booklist").winControl;
                var dataList = new WinJS.Binding.List(bookListsNodes);
                listView.itemDataSource = dataList.dataSource;
                listView.groupHeaderTemplate = document.querySelector(".headertemplate");
                listView.itemTemplate = document.querySelector(".bookitemtemplate");
                initializeLayout(listView, appView.value);
                listView.element.focus();


            },
            function (error) {
                // Create the message dialog and set its content
                var msg = new Windows.UI.Popups.MessageDialog(WinJS.Resources.getString('Unable to display the Gita Books.').value);

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

        } catch (e) {

        }
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

function PDFDownload(pdfUrl, mouseEvent) {

    //var uri = new Windows.Foundation.Uri(pdfUrl);

    //var options = new Windows.System.LauncherOptions();
    //options.displayApplicationPicker = true;

    //// Launch the URI.
    //Windows.System.Launcher.launchUriAsync(uri, options).done(
    //    function (success) {
    //        if (success) {

    //        } else {

    //        }
    //    });

    // Instantiate downloads.
    var newDownload = new DownloadOperation();

    // Pass the uri and the file name to be stored on disk to start the download.
    var fileName = pdfUrl.split("/")[pdfUrl.split("/").length - 1];
    var uriString = pdfUrl;
    
    newDownload.start(uriString, fileName);

    // Persist the download operation in the global array.
    downloadOperations.push(newDownload);
}

// Class associated with each download.
function DownloadOperation() {
    var download = null;
    var promise = null;
    var imageStream = null;

    this.start = function (uriString, fileName) {
        try {
            // Asynchronously create the file in the pictures folder.
            Windows.ApplicationModel.Package.current.installedLocation.createFileAsync(fileName, Windows.Storage.CreationCollisionOption.generateUniqueName).done(function (newFile) {
                var uri = Windows.Foundation.Uri(uriString);
                var downloader = new Windows.Networking.BackgroundTransfer.BackgroundDownloader();
                // Create a new download operation.
                download = downloader.createDownload(uri, newFile);

                // Start the download and persist the promise to be able to cancel the download.
                promise = download.startAsync().then(complete, error, progress);
            }, error);
        } catch (err) {

        }
    };

    // On application activation, reassign callbacks for a download
    // operation persisted from previous application state.
    this.load = function (loadedDownload) {
        try {
            download = loadedDownload;
            promise = download.attachAsync().then(complete, error, progress);
        } catch (err) {
            displayException(err);
        }
    };

    // Removes download operation from global array.
    function removeDownload(guid) {
        downloadOperations.forEach(function (operation, index) {
            if (operation.hasGuid(guid)) {
                downloadOperations.splice(index, 1);
            }
        });
    }
    // Returns true if this is the download identified by the guid.
    this.hasGuid = function (guid) {
        return download.guid === guid;
    };

    // Progress callback.
    function progress() {
        try {
            // Output all attributes of the progress parameter.
            var currentProgress = download.progress;

            // Handle various pause status conditions.
            if (currentProgress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.pausedByApplication) {
                //printLog("Download " + download.guid + " paused by application <br\>");
            } else if (currentProgress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.pausedCostedNetwork) {
                //printLog("Download " + download.guid + " paused because of costed network <br\>");
            } else if (currentProgress.status === Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.pausedNoNetwork) {
                //printLog("Download " + download.guid + " paused because network is unavailable.<br\>");
            } else {
                // We need a response before assigning the result stream to the image: If we get a response from
                // the server (hasResponseChanged == true) and if we haven't assigned the stream yet
                // (imageStream == null), then assign the stream to the image.
                // There is a second scenario where we need to assign the stream to the image: If a download gets
                // interrupted and cannot be resumed, the request is restarted. In that case we need to re-assign
                // the stream to the image since the requested image may have changed.
                if ((currentProgress.hasResponseChanged && !imageStream) || (currentProgress.hasRestarted)) {
                    try {

                    } catch (err) {

                    }
                }
            }
        } catch (err) {
            
        }
    }

    // Completion callback.
    function complete() {
        removeDownload(download.guid);
        var msg = new Windows.UI.Popups.MessageDialog('download completes.');
        msg.showAsync();

    }

    // Error callback.
    function error(err) {
        removeDownload(download.guid);

        displayException(err);
    }
}

function displayException(err) {
    var message;
    if (err.stack) {
        message = err.stack;
    }
    else {
        message = err.message;
    }

    var errorStatus = Windows.Networking.BackgroundTransfer.BackgroundTransferError.getStatus(err.number);
    if (errorStatus === Windows.Web.WebErrorStatus.cannotConnect) {
        message = "App cannot connect. Network may be down, connection was refused or the host is unreachable.";
    }

    displayError(message);
}

function displayError(/*@type(String)*/message) {
    var msg = new Windows.UI.Popups.MessageDialog(message);
    msg.showAsync();
}

function PDFRead(pdfUrl) {

    var fileName = pdfUrl.split("/")[pdfUrl.split("/").length - 1];

    WinJS.Navigation.navigate("/pages/gitabooks/readpdf.html", { pdffile: fileName });
}
