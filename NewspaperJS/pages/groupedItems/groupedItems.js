(function () {
    "use strict";

    var appView = Windows.UI.ViewManagement.ApplicationView;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;

    ui.Pages.define("/pages/groupedItems/groupedItems.html", {

        // This function updates the ListView with new layouts
        initializeLayout: function (listView, viewState, element) {
            /// <param name="listView" value="WinJS.UI.ListView.prototype" />

            if (viewState === appViewState.snapped) {
                listView.itemDataSource = Data.groups.dataSource;
                listView.groupDataSource = null;
                listView.layout = new ui.ListLayout();
                listView.itemTemplate = itemTemplateFunction;
                document.getElementById("myAd1").style.display = 'block';
                document.getElementById("myAd").style.display = 'none';
                
            } else {
                listView.itemDataSource = Data.items.dataSource;
                listView.groupDataSource = Data.groups.dataSource;
                listView.layout = new ui.GridLayout({ groupHeaderPosition: "top" });
                listView.itemTemplate = element.querySelector(".itemtemplate");
                document.getElementById("myAd1").style.display = 'none';
                document.getElementById("myAd").style.display = 'block';
            }
        },

        itemInvoked: function (args) {
            if (appView.value === appViewState.snapped) {
                // If the page is snapped, the user invoked a group.
                var group = Data.groups.getAt(args.detail.itemIndex);
                nav.navigate("/pages/groupDetail/groupDetail.html", { groupKey: group.key });
                //var item = Data.items.getAt(args.detail.itemIndex);
                ////nav.navigate("/pages/itemDetail/itemDetail.html", { item: Data.getItemReference(item) });
                //var region = item.group.title.toLowerCase();
                //region = region.replace(" ", "_");
                ////var tmpregion = region.tolower();
                //var country = item.title;
                //nav.navigate("/pages/newsPapers/newsPapersList.html", region + "#" + country);
            } else {
                // If the page is not snapped, the user invoked an item.
                var item = Data.items.getAt(args.detail.itemIndex);
                //nav.navigate("/pages/itemDetail/itemDetail.html", { item: Data.getItemReference(item) });
                var region = item.group.title.toLowerCase();
                region = region.replace(" ", "_");
                //var tmpregion = region.tolower();
                var country = item.title;

                if (region == "caribbean")
                {
                    region = "carib";
                }
                nav.navigate("/pages/newsPapers/newsPapersList.html", region + "#" + country);
            }
        },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var listView = element.querySelector(".groupeditemslist").winControl;
            listView.groupHeaderTemplate = element.querySelector(".headerTemplate");
            listView.itemTemplate = element.querySelector(".itemtemplate");
            listView.oniteminvoked = this.itemInvoked.bind(this);
            this.initializeLayout(listView, appView.value, element);
            listView.element.focus();

            document.getElementById("appbar").addEventListener("click", displayFav, false);
        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />
            /// <param name="viewState" value="Windows.UI.ViewManagement.ApplicationViewState" />
            /// <param name="lastViewState" value="Windows.UI.ViewManagement.ApplicationViewState" />

            var listView = element.querySelector(".groupeditemslist").winControl;
            if (lastViewState !== viewState) {
                if (lastViewState === appViewState.snapped || viewState === appViewState.snapped) {
                    var handler = function (e) {
                        listView.removeEventListener("contentanimating", handler, false);
                        e.preventDefault();
                    }
                    listView.addEventListener("contentanimating", handler, false);
                    this.initializeLayout(listView, viewState, element);
                }
            }
        }
    });

    function displayFav() {

        nav.navigate("/pages/favorites/favorites.html");
    }

    function itemTemplateFunction(itemPromise) {

        return itemPromise.then(function (item) {
            var div = document.createElement("div");
            div.className = "itemtemplate";
            
            var childDiv = document.createElement("div");
            childDiv.className = "item - overlay";
            childDiv.margin = "27px 0px 0px";

            var title = document.createElement("h4");
            title.className = "item-title";
            title.innerText = item.data.title;
            childDiv.appendChild(title);

            div.appendChild(childDiv);

            return div;
        });
    };
})();

WinJS.Utilities.markSupportedForProcessing(
window.errorLogger = function (sender, evt) {

});
