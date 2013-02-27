(function () {
    //"use strict";

    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    
    var binding = WinJS.Binding;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;
    var utils = WinJS.Utilities;
    var dtm = Windows.ApplicationModel.DataTransfer.DataTransferManager;
    var item;
    var fvindex = 0;
    ui.Pages.define("/pages/split/split.html", {

        /// <field type="WinJS.Binding.List" />
        _items: null,
        _group: null,
        _itemSelectionIndex: -1,
        
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var listView = element.querySelector(".itemlist").winControl;
            var flipView = element.querySelector(".flipView").winControl;
            //var listViewFV = document.getElementById("listViewFV").winControl;

            //flipView.style.height.set = 800;
            //document.getElementById("simple_FlipView").style.height = 700;
            // Store information about the group and selection that this page will
            // display.
            this._group = (options && options.groupKey) ? Data.resolveGroupReference(options.groupKey) : Data.groups.getAt(0);
            this._items = Data.getItemsFromGroup(this._group);
            this._itemSelectionIndex = (options && "selectedIndex" in options) ? options.selectedIndex : -1;
            element.querySelector("header[role=banner] .pagetitle").textContent = this._group.title;

            // Set up the ListView.
            listView.itemDataSource = this._items.dataSource;
            listView.itemTemplate = element.querySelector(".itemtemplate");
            listView.onselectionchanged = this._selectionChanged.bind(this);
            listView.layout = new ui.ListLayout();

            this._updateVisibility();
            if (this._isSingleColumn()) {
                if (this._itemSelectionIndex >= 0) {
                    // For single-column detail view, load the article.
                    binding.processAll(element.querySelector(".articlesection"), this._items.getAt(this._itemSelectionIndex));
                }
            } else {
                if (nav.canGoBack && nav.history.backStack[nav.history.backStack.length - 1].location === "/pages/split/split.html") {
                    // Clean up the backstack to handle a user snapping, navigating
                    // away, unsnapping, and then returning to this page.
                    nav.history.backStack.pop(); 
                }
                // If this page has a selectionIndex, make that selection
                // appear in the ListView.
                listView.selection.set(Math.max(this._itemSelectionIndex, 0));
            }


            flipView.itemDataSource = this._items.dataSource;;
            flipView.itemTemplate = document.getElementById("simple_ItemTemplate");            
            //flipView.addEventListener("next", nexthandler, false);
            //flipView.addEventListener("previous", previoushandler, false);
            //flipView.onselectionchanged = this._selectionChanged1.bind(this);
            //flipView.onselectionchanged = selecthandler(this);
            flipView.addEventListener("click", clickHandler, false);
            flipView.addEventListener("pageselected", handlePageSelected);            
            // Register for datarequested events for sharing

            

            //listViewFV.itemDataSource = this._items.dataSource;;
            //listViewFV.itemTemplate = document.getElementById("simple_ItemTemplate");
            //listViewFV.addEventListener("pageselected", handlePageSelected);

            dtm.getForCurrentView().addEventListener("datarequested", this.onDataRequested);
        },
        
        unload: function () {
            this._items.dispose();
            WinJS.Navigation.removeEventListener("datarequested", this.onDataRequested);
        },

        // This function updates the page layout in response to viewState changes.
        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />

            var listView = element.querySelector(".itemlist").winControl;
            //var flipView = element.querySelector(".flipView").winControl;
            //flipView.itemDataSource = listView.itemDataSource;
            //flipView.itemTemplate = document.getElementById("simple_ItemTemplate");
            //flipView.addEventListener("pageselected", handlePageSelected);


            var firstVisible = listView.indexOfFirstVisible;
            this._updateVisibility();

            var handler = function (e) {
                listView.removeEventListener("contentanimating", handler, false);
                e.preventDefault();
            }

            if (this._isSingleColumn()) {
                
                listView.selection.clear();
                this._itemSelectionIndex = 0;
                if (this._itemSelectionIndex >= 0) {
                    // If the app has snapped into a single-column detail view,
                    // add the single-column list view to the backstack.
                    document.getElementById("divMessage").style.visibility = 'visible';
                    nav.history.current.state = {
                        groupKey: this._group.key,
                        selectedIndex: this._itemSelectionIndex
                    };
                    nav.history.backStack.push({
                        location: "/pages/split/split.html",
                        state: { groupKey: this._group.key }
                    }); 
                    //document.getElementById("simple_FlipView").style.width = 200;
                    //document.getElementById("itemList1111").style.visibility = 'collapse';
                    //element.querySelector(".articlesection").style.marginLeft = 106;
                    //element.querySelector(".articlesection").style.width = 200;
                    //utils.addClass(document.querySelector(".articlesection"), "primarycolumn");
                    element.querySelector(".articlesection").focus();
                    var flipView = document.getElementById("simple_FlipView").winControl;
                    //flipView.style.height = 600;
                    //flipView.itemDataSource = this._items.dataSource;;
                    //flipView.itemTemplate = document.getElementById("simple_ItemTemplate");
                    //flipView.addEventListener("pageselected", handlePageSelected);
                    
                } else {
                    listView.addEventListener("contentanimating", handler, false);
                    if (firstVisible >= 0 && listView.itemDataSource.list.length > 0) {
                        listView.indexOfFirstVisible = firstVisible;
                    }
                    listView.forceLayout();
                    //element.querySelector(".articlesection").focus();
                    //flipView.itemDataSource = this._items.dataSource;;
                    //flipView.itemTemplate = document.getElementById("simple_ItemTemplate");
                    //flipView.addEventListener("pageselected", handlePageSelected);
                }
            } else {
                document.getElementById("divMessage").style.visibility = 'collapse';
                // If the app has unsnapped into the two-column view, remove any
                // splitPage instances that got added to the backstack.
                if (nav.canGoBack && nav.history.backStack[nav.history.backStack.length - 1].location === "/pages/split/split.html") {
                    nav.history.backStack.pop();
                }
                if (viewState !== lastViewState) {
                    listView.addEventListener("contentanimating", handler, false);
                    if (firstVisible >= 0 && listView.itemDataSource.list.length > 0) {
                        listView.indexOfFirstVisible = firstVisible;
                    }
                    listView.forceLayout();
                }

                listView.selection.set(this._itemSelectionIndex >= 0 ? this._itemSelectionIndex : Math.max(firstVisible, 0));
            }
        },

        // This function checks if the list and details columns should be displayed
        // on separate pages instead of side-by-side.
        _isSingleColumn: function () {
            var viewState = Windows.UI.ViewManagement.ApplicationView.value;
            return (viewState === appViewState.snapped || viewState === appViewState.fullScreenPortrait);
        },

        _selectionChanged: function (args) {
            var listView = document.body.querySelector(".itemlist").winControl;
            var flipView = document.body.querySelector(".flipView").winControl;
            //var listViewFV = document.body.querySelector(".listViewFV").winControl;
            
            var details;
            // By default, the selection is restriced to a single item.
            listView.selection.getItems().done(function updateDetails(items) {
                if (items.length > 0) {
                    this._itemSelectionIndex = items[0].index;
                    
                    if (this._isSingleColumn()) {
                        // If snapped or portrait, navigate to a new page containing the
                        // selected item's details.
                        nav.navigate("/pages/split/split.html", { groupKey: this._group.key, selectedIndex: this._itemSelectionIndex });
                    } else {


                        flipView.selectedIndex = items[0].index;
                        //flipView.next();
                        fvindex = flipView.selectedIndex;
                        // If fullscreen or filled, update the details column with new data.
                        details = document.querySelector(".articlesection");
                        binding.processAll(details, items[0].data);
                        details.scrollTop = 0;
                        //item = items[0].data.content.replace("<br><br>", "");
                        //item = "<html><table><tr><td>" + items[0].data.content + "</td></tr></table></html>";
                        item = items[0].data.content.replace(new RegExp("<br><br>", "g"), "<p>");
                        //item = details;
                    }
                }
            }.bind(this));

            
        },

        //_selectionChanged1: function (args) {
        //    var flipView = document.body.querySelector(".flipView").winControl;
        //},
        //_selectionChanged: function (args) {
        //    var flipView = document.body.querySelector(".flipView").winControl;
        //    flipView.selectedIndex = flipView.selectedIndex + 1;
        //},

        //_selectionChanged: function (args, fv) {
        //    var flipView = document.body.querySelector(".flipView").winControl;
        //    flipView.selectedIndex = flipView.selectedIndex + 1;
        //},

        onDataRequested: function (e) {
            var request = e.request;
            request.data.properties.title = "Bhagavad Gita";
            //request.data.properties.description = "This is the description text.";

            // sharing html content
            //var range = document.createRange();
            //range.selectNode(document.getElementById("scenarioOutput"));
            //request.data = MSApp.createDataPackage(range);

            //var range = document.createRange();
            //range.selectNode(document.getElementById("div1"));
            //request.data = MSApp.createDataPackage(item.innerText);

            //request.data.properties.set

            // Share text
            //var shareText = item.replace(/<br\s\/>/g, "\n").replace(/<p>/g, "\r\n").replace(/<\/p>/g, "\r\n");
            //request.data.setText(shareText);
            //request.data.setHtmlFormat(item);
            //request.data.setRtf(item);
            //request.data.setText(item);

            var htmlExample = item;
            var htmlFormat = Windows.ApplicationModel.DataTransfer.HtmlFormatHelper.createHtmlFormat(htmlExample);
            request.data.setHtmlFormat(htmlFormat);
            //request.data.setText(shareText);

            //var range = document.createRange();
            //range.selectNode(document.getElementById("htmlFragment"));
            //request.data = MSApp.createDataPackage(range);

        },
        // This function toggles visibility of the two columns based on the current
        // view state and item selection.
        _updateVisibility: function () {
            var oldPrimary = document.querySelector(".primarycolumn");
            if (oldPrimary) {
                utils.removeClass(oldPrimary, "primarycolumn");
            }
            if (this._isSingleColumn()) {
                if (this._itemSelectionIndex >= 0) {
                    //utils.addClass(document.querySelector(".articlesection"), "primarycolumn");
                    //document.querySelector(".articlesection").focus();
                    utils.addClass(document.querySelector(".itemlistsection"), "primarycolumn");
                    document.querySelector(".itemlist").focus();
                } else {
                    //utils.addClass(document.querySelector(".itemlistsection"), "primarycolumn");
                    //document.querySelector(".itemlist").focus();
                    utils.addClass(document.querySelector(".articlesection"), "primarycolumn");
                    document.querySelector(".articlesection").focus();
                }
            } else {
                document.querySelector(".itemlist").focus();
            }
        }

    });    

    function nexthandler() {
        var flipView = document.getElementById("simple_FlipView").winControl;
        var boolean = flipView.next();
    }

    function previoushandler() {
        var flipView = document.getElementById("simple_FlipView").winControl;
        var boolean = flipView.previous();
    }

    function clickHandler(evt) {
        //var flipView = document.body.querySelector(".flipView").winControl;
        var details;
        var flipView = document.getElementById("simple_FlipView").winControl;
        var listView = document.body.querySelector(".itemlist").winControl;
        //fvindex = 
        //flipView.selectedIndex++;
        document.querySelector(".articlesection").focus();
        //flipView.next();
        //var boolean = flipView.next();
        //if (!boolean) {
        //    flipView.previous();
        //    //    flipView.selectedIndex = flipView.selectedIndex;
        //    //flipView.selectedIndex += 1;
        //}
        //else {
        ////    //flipView.next();
        //    //    flipView.selectedIndex = flipView.selectedIndex;
        //    flipView.selectedIndex -= 1;
        //}
    }

    function selecthandler(eventinfo) {
        var flipView = document.getElementById("simple_FlipView").winControl;
    }

    function handlePageSelected(ev) {
        var flipView = document.getElementById("simple_FlipView");
        //flipView.selection.getItems().done(function (items) {
        //    var cc = items.content;
        //})
        //var cc = ev.target.innerText.trim();
        var cc = flipView.currentPage;

        item = ev.target.innerText.trim().replace(new RegExp("\r\n", "g"), "<p>").replace(new RegExp("Sloka", "g"), "");
    }
})();



