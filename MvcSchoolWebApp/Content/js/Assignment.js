//$(document).ready(function () {

//    Dropzone.options.myAwesomeDropzone = {

//        autoProcessQueue: false,
//        uploadMultiple: true,
//        parallelUploads: 100,
//        maxFiles: 100,

//        // Dropzone settings
//        init: function () {
//            var myDropzone = this;

//            this.element.querySelector("button[type=submit]").addEventListener("click", function (e) {
//                e.preventDefault();
//                e.stopPropagation();
//                myDropzone.processQueue();
//            });
//            this.on("sendingmultiple", function () {
//            });
//            this.on("successmultiple", function (files, response) {
//            });
//            this.on("errormultiple", function (files, response) {
//            });
//        }

//    }

//    Dropzone.options.dropzoneForm = {
        
//        paramName: "file", // The name that will be used to transfer the file
//        maxFilesize: 2, // MB
//        dictDefaultMessage: "<strong>Drop files here or click to upload. </strong></br> (This is just a demo dropzone. Selected files are not actually uploaded.)"
//    };

//});