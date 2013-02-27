(function () {
    "use strict";

    var chaptersArray = [];
    var chaptersArray = new Array("1: Arjuna's Sorrow", "2: The Path of Knowledge", "3: Karma-Yoga", "4: Renunciation of Action with Knowledge", "5: Renunciation of Action", "6: Yoga of Self-Control", "7: Knowledge and Wisdom", "8: Yoga of Imperishable Brahman", "9: The Secret of Secrets", "10: The Manifestations of God", "11: The Universal Vision", "12: The Yoga of Devotion", "13: Purusha and Prakriti:", "14: The Triple Gunas", "15: The Yoga of the Supreme Being", "16: The Opposite Qualities", "17: The Division of Qualities", "18: Liberation by Renunciation");
    //var tmpchaptersArray = new Array("Arjuna's Sorrow#1", "The Path of Knowledge#2", "Karma-Yoga#3", "Renunciation of Action with Knowledge#4", "Renunciation of Action#5", "Yoga of Self-Control#6", "Knowledge and Wisdom#7", "Yoga of Imperishable Brahman#8", "The Secret of Secrets#9", "The Manifestations of God#10", "The Universal Vision#11", "The Yoga of Devotion#12", "Purusha and Prakriti:#13", "The Triple Gunas#14", "The Yoga of the Supreme Being#15", "The Opposite Qualities#16", "The Division of Qualities#17", "Liberation by Renunciation#18");
    var tmpchaptersArray = new Array("01: Arjuna's Sorrow#1", "02: The Path of Knowledge#2", "03: Karma-Yoga#3", "04: Renunciation of Action with Knowledge#4", "05: Renunciation of Action#5", "06: Yoga of Self-Control#6", "07: Knowledge and Wisdom#7", "08: Yoga of Imperishable Brahman#8", "09: The Secret of Secrets#9", "10: The Manifestations of God#10", "11: The Universal Vision#11", "12: The Yoga of Devotion#12", "13: Purusha and Prakriti:#13", "14: The Triple Gunas#14", "15: The Yoga of the Supreme Being#15", "16: The Opposite Qualities#16", "17: The Division of Qualities#17", "18: Liberation by Renunciation#18");
    //var tmpchaptersArray = new Array([1, "1: Arjuna's Sorrow#1"], [2, "2: The Path of Knowledge#2"], [03, "3: Karma-Yoga#3"], [4,"4: Renunciation of Action with Knowledge#4"], [5, "5: Renunciation of Action#5"], [6, "6: Yoga of Self-Control#6"], [7, "7: Knowledge and Wisdom#7"], [8, "8: Yoga of Imperishable Brahman#8"], [9,"9: The Secret of Secrets#9"], [10, "10: The Manifestations of God#10"], [11,"11: The Universal Vision#11"], [12, "12: The Yoga of Devotion#12"], [13, "13: Purusha and Prakriti:#13"], [14, "14: The Triple Gunas#14"], [15, "15: The Yoga of the Supreme Being#15"], [16, "16: The opposite Qualities#16"], [17, "17: The Division of Qualities#17"], [18, "18: Liberation by Renunciation#18"]);
    //var chaptersArray = new Array("Arjuna's Sorrow", "The Path of Knowledge", "Karma-Yoga", "Renunciation of Action with Knowledge", "Renunciation of Action", "Yoga of Self-Control", "Knowledge and Wisdom", "Yoga of Imperishable Brahman", "The Secret of Secrets", "The Manifestations of God", "The Universal Vision", "The Yoga of Devotion", "Purusha and Prakriti:", "The Triple Gunas", "The Yoga of the Supreme Being", "The Opposite Qualities", "The Division of Qualities", "Liberation by Renunciation");
    var sortArr = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17];


    function getSorted(tmpchaptersArray, sortArr) {
        for (var i = 0; i < tmpchaptersArray.length; i++) {
            chaptersArray[i] = tmpchaptersArray[sortArr[i]];
        }

        return chaptersArray;
    }
    //var lightGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY7h4+cp/AAhpA3h+ANDKAAAAAElFTkSuQmCC";
    //var darkGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY3B0cPoPAANMAcOba1BlAAAAAElFTkSuQmCC";
    var dataPromises = [];

    var chapterGroups;

    var chapterGroupItems = new WinJS.Binding.List();

    function compareNumbers(a, b) {
        return a - b;
    }
    function getChapterGroups() {
        //tmpchaptersArray = getSorted(tmpchaptersArray, sortArr);
        //tmpchaptersArray.sort();

        var chapterGroupslist = [];

        // tmpchaptersArray.sort(
        //        function (a, b) {
        //            if (isNaN(a) && isNaN(b)) return a < b ? -1 : a == b ? 0 : 1;//both are string
        //            else if (isNaN(a)) return 1;//only a is a string
        //            else if (isNaN(b)) return -1;//only b is a string
        //            else return a - b;//both are num
        //        }
        //);

        //tmpchaptersArray.sort(function (a, b) {
        //    var a1 = typeof a, b1 = typeof b;
        //    return a1 < b1 ? -1 : a1 > b1 ? 1 : a < b ? -1 : a > b ? 1 : 0;
        //});

        tmpchaptersArray.sort(function (a, b) {
            return (a[0] < b[0] ? -1 : (a[0] > b[0] ? 1 : 0));
        });

        // ------------------- using two dimensional array

        //var len = tmpchaptersArray.length;

        //for (var i = 0; i < len; i++) {
        //    var obj = {
        //        key: tmpchaptersArray[i][1],
        //        title: (tmpchaptersArray[i][1]).split("#")[0],
        //        subtitle: "Chapter " + (tmpchaptersArray[i][1]).split("#")[1],
        //        index: (tmpchaptersArray[i][1]).split("#")[1],
        //        backgroundImage: "../images/ch" + (tmpchaptersArray[i][1]).split("#")[1] + ".jpg",
        //        description: "",
        //        url: 'xml/GitaVersesWin8.xml',
        //        acquireSyndication: acquireSyndication, dataPromise: null
        //    };
        //    chapterGroupslist.push(obj);
        //}
        // --------------------------------------------

        //for (var i = 0; i < len; i++) {
        //    var obj = {
        //        key: tmpchaptersArray[i],
        //        title: tmpchaptersArray[i].split("#")[0],
        //        subtitle: "Chapter " + tmpchaptersArray[i].split("#")[1],
        //        index: tmpchaptersArray[i].split("#")[1],
        //        backgroundImage: "../images/ch" + tmpchaptersArray[i].split("#")[1] + ".jpg",
        //        description: "",
        //        url: 'xml/GitaVersesWin8.xml',
        //        acquireSyndication: acquireSyndication, dataPromise: null
        //    };
        //    chapterGroupslist.push(obj);
        //}

        // ----------------------------------------

        //sortArr.sort();
        //sortArr = sortArr;
        //tmpchaptersArray.sort(
        //        function (a, b) {
        //            if (isNaN(a) && isNaN(b)) return a < b ? -1 : a == b ? 0 : 1;//both are string
        //            else if (isNaN(a)) return 1;//only a is a string
        //            else if (isNaN(b)) return -1;//only b is a string
        //            else return a - b;//both are num
        //        }
        //);

        //sortArr.sort(function (a, b) { return a - b })
        ////var len = tmpchaptersArray.length;
        ////for (var i = 0; i < len; i++) {
        ////    var obj = {
        ////        key: tmpchaptersArray[sortArr[i]],
        ////        title: tmpchaptersArray[sortArr[i]].split("#")[0],
        ////        subtitle: "Chapter " + tmpchaptersArray[sortArr[i]].split("#")[1],
        ////        index: tmpchaptersArray[sortArr[i]].split("#")[1],
        ////        backgroundImage: "../images/ch" + tmpchaptersArray[sortArr[i]].split("#")[1] + ".jpg",
        ////        description: "",
        ////        url: 'xml/GitaVersesWin8.xml',
        ////        acquireSyndication: acquireSyndication, dataPromise: null
        ////    };
        ////    chapterGroupslist.push(obj);
        ////}

        // ------------------------------------------------------------

        //tmpchaptersArray.sort(compareNumbers);
        var len = tmpchaptersArray.length;
        for (var i = 0; i < len; i++) {
            var chaptertitle = tmpchaptersArray[i].split("#")[0].substring(3, tmpchaptersArray[i].split("#")[0].length).trim();
            var obj = {
                key: tmpchaptersArray[i],
                title: chaptertitle,
                subtitle: "Chapter " + tmpchaptersArray[i].split("#")[1],
                index: tmpchaptersArray[i].split("#")[1],
                backgroundImage: "../images/ch" + tmpchaptersArray[i].split("#")[1] + ".jpg",
                description: "",
                url: 'xml/GitaVersesWin8.xml',
                acquireSyndication: acquireSyndication, dataPromise: null
            };
            chapterGroupslist.push(obj);
        }

        //chapterGroupslist.sort(function (a, b) {
        //    return (a[0] < b[0] ? -1 : (a[0] > b[0] ? 1 : 0));
        //});

        //chapterGroupslist.sort();
        //chapterGroups = getSorted(chapterGroupslist,sortArr);
        chapterGroups = chapterGroupslist;
        // Get the content for each slokas in the chapters array
        chapterGroups.forEach(function (chapter) {
            chapter.dataPromise = chapter.acquireSyndication(chapter.url);
            dataPromises.push(chapter.dataPromise);
        });

        // Return when all asynchronous operations are complete
        return WinJS.Promise.join(dataPromises).then(function () {
            return chapterGroups;
        });
    }

    function acquireSyndication(url) {
        // Call xhr for the URL to get results asynchronously
        return WinJS.xhr({ url: url });
    }

    function getSlokas() {
        getChapterGroups().then(function () {
            // Process each chapter group
            chapterGroups.forEach(function (chapter) {
                chapter.dataPromise.then(function (chapterResponse) {
                    var chapterSyndication = chapterResponse.responseXML;

                    // Get the info for each chapter 
                    var chapters = chapterSyndication.querySelectorAll("chapter");

                    // Process each chapter
                    for (var chapterIndex = 0; chapterIndex < chapters.length; chapterIndex++) {

                        if (chapter.index == chapters[chapterIndex].attributes.getNamedItem("id").textContent) {
                            // Get the info for each chapter slokas
                            var slokas = chapters[chapterIndex].querySelectorAll("sloka");
                            var imageIndex = parseInt(chapterIndex + 1);
                            // process each sloka
                            for (var slokaIndex = 0; slokaIndex < slokas.length; slokaIndex++) {

                                //var slokaContent = "<p>" + slokas[slokaIndex].querySelector("sanskrit").textContent.replace(/\n/g, "<br />") + "</p><p>" + slokas[slokaIndex].querySelector("hindi").textContent.replace(/\n/g, "<br />") + "</p><p>" + slokas[slokaIndex].querySelector("english").textContent + "</p>"
                                var slokaContent = "<p>" + slokas[slokaIndex].querySelector("sanskrit").textContent.replace(/\n/g, "<br />") + "</p><p>" + slokas[slokaIndex].querySelector("hindi").textContent.replace(/\n/g, "<br />") + "</p><p>" + slokas[slokaIndex].querySelector("english").textContent + "</p>"
                                chapterGroupItems.push({
                                    group: chapter,
                                    title: "Sloka " + slokas[slokaIndex].attributes.getNamedItem("id").textContent,
                                    //subtitle: "Sloka",
                                    content: slokaContent,
                                    backgroundImage: "../images/ch" + imageIndex + ".jpg",
                                });
                            }
                        }
                    }
                });
            });
        });
        return chapterGroupItems;
    }

    //var list = new WinJS.Binding.List();
    var list = getSlokas();

    var groupedItems = list.createGrouped(
        function groupKeySelector(item) { return item.group.key; },
        function groupDataSelector(item) { return item.group; }
    );

    // TODO: Replace the data with your real data.
    // You can add data from asynchronous sources whenever it becomes available.
    //generateSampleData().forEach(function (item) {
    //    list.push(item);
    //});

    WinJS.Namespace.define("Data", {
        items: groupedItems,
        groups: groupedItems.groups,
        getItemReference: getItemReference,
        getItemsFromGroup: getItemsFromGroup,
        resolveGroupReference: resolveGroupReference,
        resolveItemReference: resolveItemReference
    });

    // Get a reference for an item, using the group key and item title as a
    // unique reference to the item that can be easily serialized.
    function getItemReference(item) {
        return [item.group.key, item.title];
    }

    // This function returns a WinJS.Binding.List containing only the items
    // that belong to the provided group.
    function getItemsFromGroup(group) {
        return list.createFiltered(function (item) { return item.group.key === group.key; });
    }

    // Get the unique group corresponding to the provided group key.
    function resolveGroupReference(key) {
        for (var i = 0; i < groupedItems.groups.length; i++) {
            if (groupedItems.groups.getAt(i).key === key) {
                return groupedItems.groups.getAt(i);
            }
        }
    }

    // Get a unique item from the provided string array, which should contain a
    // group key and an item title.
    function resolveItemReference(reference) {
        for (var i = 0; i < groupedItems.length; i++) {
            var item = groupedItems.getAt(i);
            if (item.group.key === reference[0] && item.title === reference[1]) {
                return item;
            }
        }
    }

    // Returns an array of sample data that can be added to the application's
    // data list. 
    //function generateSampleData() {
    //    var itemContent = "<p> Dhritarashtra Said: O Sanjaya, Tell me, what my sons and Pandu's sons assembled, on battle intent, did on the field of Kurukshethra, the field of duty. </p><p>dhruthara:shtra uva:cha<br>\
    //    dharma kshe:thre: kurukshe:thre:<br>\
    //    samave:tha yuyuthsavaha |<br>\
    //    ma:maka:h pa:ndava:s cha iva<br>\
    //    kim akurvatha sanjaya || 1<br>\
    //    </p>\
    //    <br>\
    //    धर्मक्षेत्रे कुरुक्षेत्रे समवेता युयुत्सवः |<br>\
    //    मामकाः पाण्डवाश्चैव किमकुर्वत सञ्जय ||१-१||<br>\
    //        ";
    //    var itemDescription = "Item Description: Pellentesque porta mauris quis interdum vehicula urna sapien ultrices velit nec venenatis dui odio in augue cras posuere enim a cursus convallis neque turpis malesuada erat ut adipiscing neque tortor ac erat";
    //    var groupDescription = "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante";

    //    // These three strings encode placeholder images. You will want to set the
    //    // backgroundImage property in your real data to be URLs to images.
    //    var darkGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY3B0cPoPAANMAcOba1BlAAAAAElFTkSuQmCC";
    //    var lightGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY7h4+cp/AAhpA3h+ANDKAAAAAElFTkSuQmCC";
    //    var mediumGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY5g8dcZ/AAY/AsAlWFQ+AAAAAElFTkSuQmCC";

    //    // Each of these sample groups must have a unique key to be displayed
    //    // separately.
    //    var sampleGroups = [
    //        { key: "group1", title: "Chapter: 1", subtitle: "Group Subtitle: 1", backgroundImage: darkGray, description: groupDescription },
    //        { key: "group2", title: "Chapter: 2", subtitle: "Group Subtitle: 2", backgroundImage: lightGray, description: groupDescription },
    //        { key: "group3", title: "Chapter: 3", subtitle: "Group Subtitle: 3", backgroundImage: mediumGray, description: groupDescription },
    //        { key: "group4", title: "Chapter: 4", subtitle: "Group Subtitle: 4", backgroundImage: lightGray, description: groupDescription },
    //        { key: "group5", title: "Chapter: 5", subtitle: "Group Subtitle: 5", backgroundImage: mediumGray, description: groupDescription },
    //        { key: "group6", title: "Chapter: 6", subtitle: "Group Subtitle: 6", backgroundImage: darkGray, description: groupDescription },
    //        { key: "group7", title: "Chapter: 7", subtitle: "Group Subtitle: 1", backgroundImage: darkGray, description: groupDescription },
    //        { key: "group8", title: "Chapter: 8", subtitle: "Group Subtitle: 2", backgroundImage: lightGray, description: groupDescription },
    //        { key: "group9", title: "Chapter: 9", subtitle: "Group Subtitle: 3", backgroundImage: mediumGray, description: groupDescription },
    //        { key: "group10", title: "Chapter: 10", subtitle: "Group Subtitle: 4", backgroundImage: lightGray, description: groupDescription },
    //        { key: "group11", title: "Chapter: 11", subtitle: "Group Subtitle: 5", backgroundImage: mediumGray, description: groupDescription },
    //        { key: "group12", title: "Chapter: 12", subtitle: "Group Subtitle: 6", backgroundImage: darkGray, description: groupDescription },
    //        { key: "group13", title: "Chapter: 13", subtitle: "Group Subtitle: 3", backgroundImage: mediumGray, description: groupDescription },
    //        { key: "group14", title: "Chapter: 14", subtitle: "Group Subtitle: 4", backgroundImage: lightGray, description: groupDescription },
    //        { key: "group15", title: "Chapter: 15", subtitle: "Group Subtitle: 5", backgroundImage: mediumGray, description: groupDescription },
    //        { key: "group16", title: "Chapter: 16", subtitle: "Group Subtitle: 6", backgroundImage: darkGray, description: groupDescription }


    //    ];

    //    // Each of these sample items should have a reference to a particular
    //    // group.
    //    var sampleItems = [
    //        { group: sampleGroups[0], title: "Sloka: 1", subtitle: "Item Subtitle: 1", description: itemDescription, content: itemContent, backgroundImage: lightGray },
    //        { group: sampleGroups[0], title: "Sloka: 2", subtitle: "Item Subtitle: 2", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[0], title: "Sloka: 3", subtitle: "Item Subtitle: 3", description: itemDescription, content: itemContent, backgroundImage: mediumGray },
    //        { group: sampleGroups[0], title: "Sloka: 4", subtitle: "Item Subtitle: 4", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[0], title: "Sloka: 5", subtitle: "Item Subtitle: 5", description: itemDescription, content: itemContent, backgroundImage: mediumGray },

    //        { group: sampleGroups[1], title: "Sloka: 1", subtitle: "Item Subtitle: 1", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[1], title: "Sloka: 2", subtitle: "Item Subtitle: 2", description: itemDescription, content: itemContent, backgroundImage: mediumGray },
    //        { group: sampleGroups[1], title: "Sloka: 3", subtitle: "Item Subtitle: 3", description: itemDescription, content: itemContent, backgroundImage: lightGray },

    //        { group: sampleGroups[2], title: "Sloka: 1", subtitle: "Item Subtitle: 1", description: itemDescription, content: itemContent, backgroundImage: mediumGray },
    //        { group: sampleGroups[2], title: "Sloka: 2", subtitle: "Item Subtitle: 2", description: itemDescription, content: itemContent, backgroundImage: lightGray },
    //        { group: sampleGroups[2], title: "Sloka: 3", subtitle: "Item Subtitle: 3", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[2], title: "Sloka: 4", subtitle: "Item Subtitle: 4", description: itemDescription, content: itemContent, backgroundImage: lightGray },
    //        { group: sampleGroups[2], title: "Sloka: 5", subtitle: "Item Subtitle: 5", description: itemDescription, content: itemContent, backgroundImage: mediumGray },
    //        { group: sampleGroups[2], title: "Sloka: 6", subtitle: "Item Subtitle: 6", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[2], title: "Sloka: 7", subtitle: "Item Subtitle: 7", description: itemDescription, content: itemContent, backgroundImage: mediumGray },

    //        { group: sampleGroups[3], title: "Sloka: 1", subtitle: "Item Subtitle: 1", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[3], title: "Sloka: 2", subtitle: "Item Subtitle: 2", description: itemDescription, content: itemContent, backgroundImage: lightGray },
    //        { group: sampleGroups[3], title: "Sloka: 3", subtitle: "Item Subtitle: 3", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[3], title: "Sloka: 4", subtitle: "Item Subtitle: 4", description: itemDescription, content: itemContent, backgroundImage: lightGray },
    //        { group: sampleGroups[3], title: "Sloka: 5", subtitle: "Item Subtitle: 5", description: itemDescription, content: itemContent, backgroundImage: mediumGray },
    //        { group: sampleGroups[3], title: "Sloka: 6", subtitle: "Item Subtitle: 6", description: itemDescription, content: itemContent, backgroundImage: lightGray },

    //        { group: sampleGroups[4], title: "Sloka: 1", subtitle: "Item Subtitle: 1", description: itemDescription, content: itemContent, backgroundImage: lightGray },
    //        { group: sampleGroups[4], title: "Sloka: 2", subtitle: "Item Subtitle: 2", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[4], title: "Sloka: 3", subtitle: "Item Subtitle: 3", description: itemDescription, content: itemContent, backgroundImage: lightGray },
    //        { group: sampleGroups[4], title: "Sloka: 4", subtitle: "Item Subtitle: 4", description: itemDescription, content: itemContent, backgroundImage: mediumGray },

    //        { group: sampleGroups[5], title: "Sloka: 1", subtitle: "Item Subtitle: 1", description: itemDescription, content: itemContent, backgroundImage: lightGray },
    //        { group: sampleGroups[5], title: "Sloka: 2", subtitle: "Item Subtitle: 2", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[5], title: "Sloka: 3", subtitle: "Item Subtitle: 3", description: itemDescription, content: itemContent, backgroundImage: mediumGray },
    //        { group: sampleGroups[5], title: "Sloka: 4", subtitle: "Item Subtitle: 4", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[5], title: "Sloka: 5", subtitle: "Item Subtitle: 5", description: itemDescription, content: itemContent, backgroundImage: lightGray },
    //        { group: sampleGroups[5], title: "Sloka: 6", subtitle: "Item Subtitle: 6", description: itemDescription, content: itemContent, backgroundImage: mediumGray },
    //        { group: sampleGroups[5], title: "Sloka: 7", subtitle: "Item Subtitle: 7", description: itemDescription, content: itemContent, backgroundImage: darkGray },
    //        { group: sampleGroups[5], title: "Sloka: 8", subtitle: "Item Subtitle: 8", description: itemDescription, content: itemContent, backgroundImage: lightGray }
    //    ];

    //    return sampleItems;
    //}
})();
