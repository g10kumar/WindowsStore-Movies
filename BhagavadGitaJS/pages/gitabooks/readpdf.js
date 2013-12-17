(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/gitabooks/readpdf.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var that = this;
            this.openPDF(options.pdffile).then(function (pdfDocument) {
                var array = [];
                for (var i = 0; i < pdfDocument.pageCount; i++) {
                    array.push(i);
                }
                var dataList = new WinJS.Binding.List(array);
                var flipViewDOM = document.getElementById("flipView");
                var flipView = flipViewDOM.winControl;
                var width = flipViewDOM.clientWidth;
                var height = flipViewDOM.clientHeight;

                flipView.itemDataSource = dataList.dataSource;
                flipView.itemTemplate = (function (itemPromise) {
                    return itemPromise.then(function (item) {
                        // root element for the item
                        var canvas = document.createElement("canvas");

                        canvas.style.width = width;
                        canvas.style.height = height;

                        var canvasContext = canvas.getContext("2d");

                        that.drawPage(pdfDocument, canvasContext, item.index);
                        return canvas;
                    });
                });
            });

        },
        openPDF: function (filename) {
            return Windows.ApplicationModel.Package.current.installedLocation.getFileAsync(filename).then(function (file) {
                return Windows.Storage.FileIO.readBufferAsync(file).then(function (fileBuffer) {
                    return MuPDFWinRT.Document.create(fileBuffer, MuPDFWinRT.DocumentType.pdf, Windows.Graphics.Display.DisplayProperties.logicalDpi);
                });
            });
        },
        drawPage: function (pdfDocument, canvasContext, index) {
            var size = pdfDocument.getPageSize(index);

            canvasContext.canvas.width = size.x;
            canvasContext.canvas.height = size.y;

            var imageData = canvasContext.createImageData(size.x, size.y);

            var current = new Int32Array(size.x * size.y);

            pdfDocument.drawPage(index, current, 0, 0, size.x, size.y, false);

            // from  ARGB to ABGR
            for (var i = 0; i < current.length; i++) {

                var val = current[i];

                var cursor = i * 4;

                imageData.data[cursor] = (val >> 16) & 0xFF; //r
                imageData.data[cursor + 1] = (val >> 8) & 0xFF; //g
                imageData.data[cursor + 2] = val & 0xFF; //b
                imageData.data[cursor + 3] = 255; // a
            }
            canvasContext.putImageData(imageData, 0, 0);
        }
    });
})();
