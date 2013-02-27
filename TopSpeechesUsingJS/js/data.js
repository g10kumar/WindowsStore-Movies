(function () {
    "use strict";

    // These three strings encode placeholder images. You will want to set the
    // backgroundImage property in your real data to be URLs to images.
    var lightGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY7h4+cp/AAhpA3h+ANDKAAAAAElFTkSuQmCC";
    var mediumGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY5g8dcZ/AAY/AsAlWFQ+AAAAAElFTkSuQmCC";
    var darkGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY3B0cPoPAANMAcOba1BlAAAAAElFTkSuQmCC";

    var dataPromises = [];
    
    var alphabetGroups;

    var alphabetSpeeches = new WinJS.Binding.List();

    function getAlphabets() {

        var alphabetGroupslist = [];
        var Alphabet = "#ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var len = Alphabet.length;
        for (var i = 0; i < len; i++) {
            var obj = {
                key: Alphabet.charAt(i),
                title: Alphabet.charAt(i),
                subtitle: Alphabet.charAt(i),
                backgroundImage: darkGray,
                description: "",
                acquireSyndication: acquireSyndication, dataPromise: null
            };
            alphabetGroupslist.push(obj);
        }
        alphabetGroups = alphabetGroupslist

        // Get the content for each feed in the blogs array
        alphabetGroups.forEach(function (alphabet) {
            alphabet.dataPromise = alphabet.acquireSyndication("xml/TopSpeeches.xml");
            dataPromises.push(alphabet.dataPromise);            
        });

        // Return when all asynchronous operations are complete
        return WinJS.Promise.join(dataPromises).then(function () {
            return alphabetGroups;
        });
    }

    function acquireSyndication(url) {
        // Call xhr for the URL to get results asynchronously
        return WinJS.xhr({ url: url });
    }

    function getTopSpeeches() {
        getAlphabets().then(function () {
            // Process each blog
            alphabetGroups.forEach(function (alphabet) {
                alphabet.dataPromise.then(function (articlesResponse) {
                    var articleSyndication = articlesResponse.responseXML;

                    // Get the info for each blog post
                    var posts = articleSyndication.querySelectorAll("TopSpeeches");
                    var numberPattern=new RegExp("[0-9]");

                    // Process each blog post
                    for (var postIndex = 0; postIndex < posts.length; postIndex++) {
                        var post = posts[postIndex];
                        var datac = "";
                        var speechTitle = post.querySelector("Title").textContent.replace("&amp;", "&").replace("&quot;", "\"").replace("&nbsp;", "").replace("\"", "").substring(0, 1);

                        if ((alphabet.title == "#" & numberPattern.test(speechTitle)) || (alphabet.title == speechTitle)) {
                            // Get the title, author
                            var postTitle = post.querySelector("Title").textContent.replace("&amp;", "&").replace("&quot;", "\"").replace("&nbsp;", "");
                            var postAuthor = post.querySelector("Speaker").textContent.replace("&amp;", "&").replace("&quot;", "\"").replace("&nbsp;", "");
                            
                            var postRank = post.querySelector("Rank").textContent;                            

                            // Store the post info we care about in the array
                            alphabetSpeeches.push({
                                group: alphabet,
                                key: alphabet.title,
                                title: postTitle.replace("&amp;", "&").replace("&quot;", "\""),
                                author: postAuthor,
                                subtitle: postAuthor,
                                content: "",
                                //backgroundImage: "../images/speaker/"+postAuthor+"_"+postRank+".jpg",
                                backgroundImage: "../images/speaker/" + postAuthor + ".jpg",
                                rank: postRank,
                                index: postIndex
                            });
                        }
                    }
                    
                });
            });

        });
        
        return alphabetSpeeches
    }
    
    // Get a reference for an item, using the group key and item title as a
    // unique reference to the item that can be easily serialized.
    function getItemReference(item) {
        return [item.group.key, item.title, item.subtitle];
    }

    function resolveGroupReference(key) {
        for (var i = 0; i < groupedItems.groups.length; i++) {
            if (groupedItems.groups.getAt(i).key === key) {
                return groupedItems.groups.getAt(i);
            }
        }
    }

    function resolveItemReference(reference) {
        for (var i = 0; i < groupedItems.length; i++) {
            var item = groupedItems.getAt(i);
            if (item.group.key === reference[0] && (item.title === reference[1] && item.subtitle === reference[2])) {
                return item;
            }
        }
    }

    // This function returns a WinJS.Binding.List containing only the items
    // that belong to the provided group.
    function getItemsFromGroup(group) {
        return list.createFiltered(function (item) { return item.group.key === group.key; });
    }

    //var list = new WinJS.Binding.List();
    var list = getTopSpeeches();

    var groupedItems = list.createGrouped(
        function groupKeySelector(item) { return item.group.key; },
        function groupDataSelector(item) { return item.group; }
    );

    // TODO: Replace the data with your real data.
    // You can add data from asynchronous sources whenever it becomes available.
    //sampleItems.forEach(function (item) {
    //    list.push(item);
    //});

    WinJS.Namespace.define("Data", {
        items: groupedItems,
        groups: groupedItems.groups,
        getItemsFromGroup: getItemsFromGroup,
        getItemReference: getItemReference,
        resolveGroupReference: resolveGroupReference,
        resolveItemReference: resolveItemReference
    });
})();
