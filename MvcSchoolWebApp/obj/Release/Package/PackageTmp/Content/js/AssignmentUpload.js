//$(document).ready(function () {
//    $("#btn-submit").click(function (e) {
//        if ($("#uploadform").valid()) {
            
//        }
//        return false;
//    });
//    $("#uploadform").validate({
//        errorClass: "my-error-class",
//        validClass: "my-valid-class",
//        rules: {
//            txtSubject: {
//                required: true,
//            },
//            txtSection: {
//                required: true,
//            },
//            txtCategories: {
//                required: true,
//            },
//            txtClass: {
//                required: true,
//            },
//            txtCampus: {
//                required: true,
//            },
//            txtDate: {
//                required: true,
//            }
//        },
//        messages: {
//            txtSubject: {
//                required: "",
//            },
//            txtSection: {
//                required: ""
//            },
//            txtCategories: {
//                required: "",
//            },
//            txtClass: {
//                required: ""
//            },
//            txtCampus: {
//                required: ""
//            },
//            txtDate: {
//                required: ""
//            }
//        }
//    });

//});

$("#txtCampusUA").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Home/getClassJson"),
        data: { campusId: $("#txtCampusUA > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value = ''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassUA").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassUA").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Home/getSectionJson"),
        data: { campusId: $("#txtCampusUA > option:selected").attr("value"), classId: $("#txtClassUA > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value = ''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionUA").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionUA").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Home/getSubjectJson"),
        data: {
            campusId: $("#txtCampusUA > option:selected").attr("value"),
            classId: $("#txtClassUA > option:selected").attr("value"),
            sectionId: $("#txtSectionUA > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value = ''>Subject</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSubjectUA").html(items.join(' '));
        }
    });
    return false;
});

//$("#btnupload").click(function (e) {
//    debugger
//    var formData = new FormData();
//    var totalFiles = document.getElementById("postedFile").files.length;

//    var file = document.getElementById("postedFile").files[0];
//    formData.append("FileUpload", file);
//    alert(totalFiles);
//    alert(file);
//    $.ajax({
//        type: "POST",
//        url: encodeURI("../Home/uploadFile"),
//        data: {

//            campusId: $("#txtCampus > option:selected").attr("value"),
//            classId: $("#txtClass > option:selected").attr("value"),
//            sectionId: $("#txtSection > option:selected").attr("value"),
//            subjectId: $("#txtSubject > option:selected").attr("value"),
//            categoryId: $("#txtCategory > option:selected").attr("value"),
//            txtDate: $("#txtDate").val(),


//        },
//        success: function (data) {
//            alert("Success!")
//        }
//    })

//    return false;
//});