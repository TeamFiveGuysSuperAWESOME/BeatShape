
mergeInto(LibraryManager.library, {
    UploadFile: function () {
        var fileInput = document.createElement('input');
        fileInput.type = 'file';
        fileInput.accept = '.json';
        fileInput.onchange = function (event) {
            var file = event.target.files[0];
            if (file) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var jsonData = e.target.result;
                    SendMessage('Manager', 'OnFileUpload', jsonData);
                };
                reader.readAsText(file);
            }
        };
        fileInput.click();
    }
});