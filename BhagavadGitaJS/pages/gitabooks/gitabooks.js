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
    var PDFFILENAME = "Assets\\Windows_7_Product_Guide.pdf";

    var RENDEROPTIONS = {
        NORMAL: 0,
        ZOOM: 1,
        PORTION: 2
    };
    ui.Pages.define("/pages/gitabooks/gitabooks.html", {


        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            // Store information about the group and selection that this page will
            // display.
            bookListsNodes.length = 0;
            DisplayGitaBooksAuthors(options);
            DisplayGitaBooks("Paramahamsa Nithyananda");
            //makeXhrCall(GetAuthorList);
            //DisplayGitaBooks1(element, options);
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
    
    function itemInvokedHandler(eventObject) {
        eventObject.detail.itemPromise.done(function (invokedItem) {
            bookListsNodes.length = 0;
            var author = invokedItem.data.author;
            DisplayGitaBooks(author);
        });
    }

    function itembookInvokedHandler(eventObject) {
        eventObject.detail.itemPromise.done(function (invokedItem) {
            var uri = new Windows.Foundation.Uri(invokedItem.data.url);
            //renderpage("https://archive.org/download/BhagavadGitaVolume13rdEdition/Bhagavad%20Gita%20Volume%201%203rd%20Edition.pdf");
            //Windows.System.Launcher.LaunchFileAsync(uri);
                        
            // Launch the URI.
            Windows.System.Launcher.launchUriAsync(uri).done(
                function (success) {
                    if (success) {
                        
                    } else {
                        
                    }
                });
        });
    }
    
    function DisplayGitaBooks(author)
    {
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

                        for (var volumeIndex = 0; volumeIndex < volumelist.length; volumeIndex++)
                        {
                        var volumeLists = {
                            title: volumelist[volumeIndex].querySelector("title").textContent,
                            url: volumelist[volumeIndex].querySelector("url").textContent,
                            download: "Download",
                            read: "Read"

                        };
                        bookListsNodes.push(volumeLists);
                        }
                    }
                }

                var listView = document.querySelector(".booklist").winControl;
                var dataList = new WinJS.Binding.List(bookListsNodes);
                listView.itemDataSource = dataList.dataSource;
                listView.addEventListener("iteminvoked", itembookInvokedHandler, false);
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

    function renderpage(url)
    {
        renderPDFPage("Bhagavad%20Gita%20Volume%201%203rd%20Edition.pdf", PDF_PAGE_INDEX, RENDEROPTIONS.NORMAL);
    }


    function renderPDFPage(pdfFileName, pageIndex, renderOptions) {
        "use strict";
        Windows.ApplicationModel.Package.current.installedLocation.getFileAsync(pdfFileName).then(function loadDocument(file) {
            // Call pdfDocument.'loadfromFileAsync' to load pdf file
            return pdfLib.PdfDocument.loadFromFileAsync(file);
        }).then(function setPDFDoc(doc) {
            renderPage(doc, pageIndex, renderOptions);
        });
    };

    function renderPage(pdfDocument, pageIndex, renderOptions) {
        "use strict";
        var pageRenderOutputStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();


        // Get PDF Page
        var pdfPage = pdfDocument.getPage(pageIndex);

        var pdfPageRenderOptions = new Windows.Data.Pdf.PdfPageRenderOptions();
        var renderToStreamPromise;
        var pagesize = pdfPage.size;

        // Call pdfPage.renderToStreamAsync
        switch (renderOptions) {
            case RENDEROPTIONS.NORMAL:
                renderToStreamPromise = pdfPage.renderToStreamAsync(pageRenderOutputStream);
                break;
            case RENDEROPTIONS.ZOOM:
                // Set pdfPageRenderOptions.'destinationwidth' or 'destinationHeight' to take into effect zoom factor
                pdfPageRenderOptions.destinationHeight = pagesize.height * ZOOM_FACTOR;
                renderToStreamPromise = pdfPage.renderToStreamAsync(pageRenderOutputStream, pdfPageRenderOptions);
                break;
            case RENDEROPTIONS.PORTION:
                // Set pdfPageRenderOptions.'sourceRect' to the rectangle containing portion to show
                pdfPageRenderOptions.sourceRect = PDF_PORTION_RECT;
                renderToStreamPromise = pdfPage.renderToStreamAsync(pageRenderOutputStream, pdfPageRenderOptions);
                break;
        };

        renderToStreamPromise.then(function Flush() {
            return pageRenderOutputStream.flushAsync();
        }).then(function DisplayImage() {
            if (pageRenderOutputStream !== null) {
                // Get Stream pointer
                var blob = MSApp.createBlobFromRandomAccessStream("image/png", pageRenderOutputStream);
                var picURL = URL.createObjectURL(blob, { oneTimeOnly: true });
                scenario1ImageHolder1.src = picURL;
                pageRenderOutputStream.close();
                blob.msClose();
            };
        },
           function error() {
               if (pageRenderOutputStream !== null) {
                   pageRenderOutputStream.close();

               }
           });
    }

    function DisplayGitaBooks1(element, options) {
        bookListsNodes.length = 0;
        try {
            var list = GetGitaBooks();            

        } catch (e) {
            // Create the message dialog and set its content
            var msg = new Windows.UI.Popups.MessageDialog(e.message);

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

    function GetGitaBooks() {
       // GetAuthors().then(function () {
            // Process each author
            authorGroups.forEach(function (author) {
                author.dataPromise.then(function (authorBooksResponse) {
                    var authorSyndication = authorBooksResponse.responseXML;

                    // Get the info for each book of author
                    var authorbookList = authorSyndication.querySelectorAll("book");

                    for (var authorIndex = 0; authorIndex < authorbookList.length; authorIndex++) {
                        var authorbooks = authorbookList[authorIndex];

                        if (authorbooks.querySelector("author").textContent == author.title) {
                            var booksList = authorbooks.querySelectorAll("volume");
                            for (var bookIndex = 0; bookIndex < booksList.length; bookIndex++) {

                                gitaBooks.push({
                                    group: author,
                                    key: author.title,
                                    title: booksList[bookIndex].querySelector("title").content,
                                    url: booksList[bookIndex].querySelector("url").content
                                });
                            }

                        }
                    }
                });
            });
        //});

        return gitaBooks;
    }

    function GetAuthors() {

        //authorListsNodes = GetAuthorList();
        makeXhrCall(GetAuthorList);

        //for (var i = 0; i < authorListsNodes.length; i++) {
        //    var obj = {
        //        key: authorListsNodes[i].author,
        //        title: authorListsNodes[i].author,
        //        subtitle: authorListsNodes[i].author,
        //        backgroundImage: "",
        //        description: "",
        //        acquireSyndication: acquireSyndication, dataPromise: null
        //    };
        //    authorsGroupslist.push(obj);
        //}

        //authorGroups = authorsGroupslist;
        //// Get the content for each feed in the blogs array
        //authorGroups.forEach(function (author) {
        //    author.dataPromise = alphabet.acquireSyndication("xml/books.xml");
        //    dataPromises.push(author.dataPromise);
        //});

        //// Return when all asynchronous operations are complete
        //return WinJS.Promise.join(dataPromises).then(function () {
        //    return authorGroups;
        //});

        //var URL = "";
        //URL = "xml/books.xml";
        //WinJS.xhr({ url: URL }).done(function (result) {
        //    var authorListResponse = result.responseXML;
            // Get the info for authors list 
            //var authorsList = authorListResponse.querySelectorAll("book");

            //for (var authorIndex = 0; authorIndex < authorsList.length; authorIndex++) {
            //    var authorLists = {
            //        author: authorsList[authorIndex].querySelector("author").textContent
            //    };
            //    authorListsNodes.push(authorLists);
            //}

            //for (var i = 0; i < authorListsNodes.length; i++) {
            //    var obj = {
            //        key: authorListsNodes[i].author,
            //        title: authorListsNodes[i].author,
            //        subtitle: authorListsNodes[i].author,
            //        backgroundImage: "",
            //        description: "",
            //        acquireSyndication: acquireSyndication, dataPromise: null
            //    };
            //    authorsGroupslist.push(obj);
            //}

            //authorGroups = authorsGroupslist;
            //// Get the content for each feed in the blogs array
            //authorGroups.forEach(function (author) {
            //    author.dataPromise = author.acquireSyndication("xml/books.xml");
            //    dataPromises.push(author.dataPromise);
            //});

            //// Return when all asynchronous operations are complete
            //return WinJS.Promise.join(dataPromises).then(function () {
            //    return authorGroups;
            //});
        //});

    }

    function GetAuthorList(authorListResponse)
    {
        //var authorListResponse = result.responseXML;
        // Get the info for authors list 
        var authorsList = authorListResponse.querySelectorAll("book");

        for (var authorIndex = 0; authorIndex < authorsList.length; authorIndex++) {
            var authorLists = {
                author: authorsList[authorIndex].querySelector("author").textContent
            };
            authorListsNodes.push(authorLists);
        }

        for (var i = 0; i < authorListsNodes.length; i++) {
            var obj = {
                key: authorListsNodes[i].author,
                title: authorListsNodes[i].author,
                subtitle: authorListsNodes[i].author,
                backgroundImage: "",
                description: "",
                acquireSyndication: acquireSyndication, dataPromise: null
            };
            authorsGroupslist.push(obj);
        }

        authorGroups = authorsGroupslist;
        // Get the content for each feed in the blogs array
        authorGroups.forEach(function (author) {
            author.dataPromise = author.acquireSyndication("xml/books.xml");
            dataPromises.push(author.dataPromise);
        });

        // Return when all asynchronous operations are complete
        return WinJS.Promise.join(dataPromises).then(function () {
            return authorGroups;
        });
    }

    function makeXhrCall(callback) {
        
        var URL = "";
        URL = "xml/books.xml";
        WinJS.xhr({ url: URL }).done(function (result) {
            callback(result.responseXML);          
        });      
        
    }
    function acquireSyndication(url) {
        // Call xhr for the URL to get results asynchronously
        return WinJS.xhr({ url: url });
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

///function check(mouseEvent) {
//    
//    return false;
//}