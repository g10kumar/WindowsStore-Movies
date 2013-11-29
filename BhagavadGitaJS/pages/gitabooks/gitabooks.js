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
    ui.Pages.define("/pages/gitabooks/gitabooks.html", {


        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            // Store information about the group and selection that this page will
            // display.
            //DisplayGitaBooks(element, options);

            //makeXhrCall(GetAuthorList);
            DisplayGitaBooks1(element, options);
        },

        unload: function () {

        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />            
        }

    });

    function DisplayGitaBooks(element, options) {
        bookListsNodes.length = 0;
        try {

            var URL = "";
            URL = "xml/books.xml";
            WinJS.xhr({ url: URL }).then(function (result) {
                var bookListResponse = result.responseXML;

                // Get the info for books list 
                var booksList = bookListResponse.querySelectorAll("book");

                for (var bookIndex = 0; bookIndex < booksList.length; bookIndex++) {
                    var bookLists = {
                        author: booksList[bookIndex].querySelector("author").textContent
                    };
                    bookListsNodes.push(bookLists);
                }

                var listView = element.querySelector(".playlist").winControl;
                var dataList = new WinJS.Binding.List(bookListsNodes);
                listView.itemDataSource = dataList.dataSource;
                listView.groupHeaderTemplate = element.querySelector(".headerTemplate");
                listView.itemTemplate = element.querySelector(".itemtemplate");
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