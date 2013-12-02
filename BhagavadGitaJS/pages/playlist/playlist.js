(function () {
    "use strict";

    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var appView = Windows.UI.ViewManagement.ApplicationView;
    var binding = WinJS.Binding;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    var dtm = Windows.ApplicationModel.DataTransfer.DataTransferManager;
    var tmpplayListArray = new Array("Gita in Hindi#http://www.youtube.com/playlist?list=PLhtmKWc6vRTAbgzzXxDaaC5nLmGsSiybE", "Gita in Tamil#http://www.youtube.com/playlist?list=PLEE6703B259EF5E0F");
    var playListNodes = [];
    var videoListsNodes = [];
    ui.Pages.define("/pages/playlist/playlist.html", {


        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            // Store information about the group and selection that this page will
            // display.

            GetPlayLists();
            GetVideoLists(element, "PLhtmKWc6vRTAbgzzXxDaaC5nLmGsSiybE", "1");
        },

        unload: function () {

        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />            
        }

    });

    function GetPlayLists(){        
        playListNodes.length = 0;
        try {
            for (var i = 0; i < tmpplayListArray.length; i++) {
                var playLists = {
                    title: tmpplayListArray[i].split("#")[0],
                    playtitle: tmpplayListArray[i].split("#")[1]
                };
                playListNodes.push(playLists);
            }

            DisplayPlayList();

        } catch (e)
        {

        }
    }

    function DisplayPlayList() {
        try {
            if (playListNodes.length > 0) {
                var dataList = new WinJS.Binding.List(playListNodes);
                var listView = document.querySelector(".playlist").winControl;
                listView.itemDataSource = dataList.dataSource;
                listView.groupHeaderTemplate = document.querySelector(".headerTemplate");
                listView.addEventListener("iteminvoked", itemInvokedHandler, false);
                listView.itemTemplate = document.querySelector(".itemtemplate");
                initializeLayout(listView, appView.value);
                listView.element.focus();
            }
            else {
                var listView = document.querySelector(".playlist").winControl;
                if (listView != null) {
                    listView.itemDataSource = null;
                }
                var msg = new Windows.UI.Popups.MessageDialog("No play lists exits!");
                msg.showAsync().done(function (command) {
                    if (command.id == 1) {

                    }
                    else {

                    }
                });
            }
        }
        catch (e)
        {

        }
    }

    function itemInvokedHandler(eventObject) {
        eventObject.detail.itemPromise.done(function (invokedItem) {

            var playlist = invokedItem.data.playtitle.split("=");
            WinJS.Navigation.navigate("/pages/video/videolist.html", { playlist: playlist[1]});
        });
    }

    function GetVideoLists(element, objValue, startIndex) {

        try {

            var playlistURL = objValue;
            var URL = "";
            URL = "http://gdata.youtube.com/feeds/api/playlists/" + playlistURL + "?v=2&start-index=" + parseInt(startIndex) + "&next=25";
            WinJS.xhr({ url: URL }).then(function (result) {
                var videosListResponse = result.responseXML;
                var videoCount = videosListResponse.querySelector("totalResults").textContent;

                // Get the info for videos list 
                var videos = videosListResponse.querySelectorAll("entry");

                for (var videoIndex = 0; videoIndex < videos.length; videoIndex++) {
                    var videoLists = {
                        title: videos[videoIndex].querySelector("title").textContent,
                        videoId: videos[videoIndex].querySelector("videoid").textContent,
                        playtitle: playlistURL
                    };
                    videoListsNodes.push(videoLists);
                }

                if (parseInt(videoCount) == videoListsNodes.length) {
                    DisplayVideoList(element, objValue);
                }
                else {
                    var sindex = videoListsNodes.length + 1;
                    GetVideoLists(element, playlistURL, sindex);
                }

            },
            function (error) {
                // Create the message dialog and set its content
                var msg = new Windows.UI.Popups.MessageDialog(WinJS.Resources.getString('Unable to display the Videos list.').value);

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

    function DisplayVideoList(element, objValue) {

        if (videoListsNodes.length != 0) {
            var dataList = new WinJS.Binding.List(videoListsNodes);
            var listView = element.querySelector(".videolist").winControl;
            listView.itemDataSource = dataList.dataSource;
            listView.groupHeaderTemplate = element.querySelector(".headerTemplate");            
            listView.itemTemplate = element.querySelector(".videoitemtemplate");
            initializeLayout(listView, appView.value);
            listView.element.focus();
        }
        else {

            // Create the message dialog and set its content
            var msg = new Windows.UI.Popups.MessageDialog(WinJS.Resources.getString('Unable to display the Videos list.').value);

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