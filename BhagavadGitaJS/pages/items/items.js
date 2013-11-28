(function () {
    "use strict";

    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var ui = WinJS.UI;
    var appdata = Windows.Storage.ApplicationData;
    ui.Pages.define("/pages/items/items.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var listView = element.querySelector(".itemslist").winControl;
            listView.itemDataSource = Data.groups.dataSource;
            listView.itemTemplate = element.querySelector(".itemtemplate");
            listView.oniteminvoked = this._itemInvoked.bind(this);

            document.getElementById("btnRecent").addEventListener("click", recentSloka, false);
            document.getElementById("btnBookmark").addEventListener("click", showBookmark, false);
            document.getElementById("btnVideo").addEventListener("click", showVideo, false);

            this._initializeLayout(listView, Windows.UI.ViewManagement.ApplicationView.value);
            listView.element.focus();
        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />

            var listView = element.querySelector(".itemslist").winControl;
            if (lastViewState !== viewState) {
                if (lastViewState === appViewState.snapped || viewState === appViewState.snapped) {
                    var handler = function (e) {
                        listView.removeEventListener("contentanimating", handler, false);
                        e.preventDefault();
                    }
                    listView.addEventListener("contentanimating", handler, false);
                    var firstVisible = listView.indexOfFirstVisible;
                    this._initializeLayout(listView, viewState);
                    if (firstVisible >= 0 && listView.itemDataSource.list.length > 0) {
                        listView.indexOfFirstVisible = firstVisible;
                    }
                }
            }
        },

        // This function updates the ListView with new layouts
        _initializeLayout: function (listView, viewState) {
            /// <param name="listView" value="WinJS.UI.ListView.prototype" />

            if (viewState === appViewState.snapped) {
                listView.layout = new ui.ListLayout();
                document.getElementById("footer").style.display = 'none';
            } else {
                listView.layout = new ui.GridLayout();
                document.getElementById("footer").style.display = 'block';
            }
        },

        _itemInvoked: function (args) {
            var groupKey = Data.groups.getAt(args.detail.itemIndex).key;
            //WinJS.Navigation.navigate("/pages/split/split.html", { groupKey: groupKey });
            //WinJS.Navigation.navigate("/pages/sloka/sloka.html", { groupKey: groupKey, selectedIndex: 9 });
            WinJS.Navigation.navigate("/pages/sloka/sloka.html", { groupKey: groupKey });
        }
    });

    function recentSloka() {
        //WinJS.Navigation.navigate("/pages/recent/recent.html");
        var recent = appdata.current.roamingSettings.values["recentSloka"];

        //var slokaItem = invokedItem.data.sloka.split(" ");
        WinJS.Navigation.navigate("/pages/sloka/sloka.html", { groupKey: recent.split("~")[0], selectedIndex: recent.split("~")[1] });
    }

    function showBookmark() {

        WinJS.Navigation.navigate("/pages/bookmarks/bookmarks.html");
    }

    function showVideo() {

        WinJS.Navigation.navigate("/pages/video/video.html");
    }
})();
