$("#txtCampusEI").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../personalinfo/getEmployeeJson"),
        data: { campusId: $("#txtCampusEI > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Employee</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtEmployees").html(items.join(' '));
        }
    });
    return false;
});

$("#txtCampusSWP").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../personalinfo/getClassJson"),
        data: { campusId: $("#txtCampusSWP > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtClassSWP").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassSWP").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../personalinfo/getSectionJson"),
        data: {
            campusId: $("#txtCampusSWP > option:selected").attr("value"),
            classId: $("#txtClassSWP > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtSectionSWP").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionSWP").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../personalinfo/getstudentname"),
        data: {
            campusId: $("#txtCampusSWP > option:selected").attr("value"),
            classId: $("#txtClassSWP > option:selected").attr("value"),
            sectionId: $("#txtSectionSWP > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Student</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtStudentSWP").html(items.join(' '));
        }
    });
    return false;
});

function getprofile() {
    $('#prodiv').show();
    $('#prodiv').removeAttr('hidden', true);
    $('#prodiv2').removeAttr('hidden', true);
    $('#prodiv3').removeAttr('hidden', true);
    $('#prodiv4').removeAttr('hidden', true);
    $('#prodiv5').removeAttr('hidden', true);
}

//    //Perosnal Info
//    $('#txtTitle').prop('readonly', true);
//    $('#txtInitials').prop('readonly', true);
//    $('#txtFirstName').prop('readonly', true);
//    $('#txtFatherName').prop('readonly', true);
//    $('#txtMiddleName').prop('readonly', true);
//    $('#txtSecondName').prop('readonly', true);
//    $('#txtLastName').prop('readonly', true);

//    //Additional Info
//    $('#txtIdNumber').prop('readonly', true);
//    $('#txtBirthDate').prop('readonly', true);
//    $('#txtGender').prop('readonly', true);
//    $('#txtBirthPlace').prop('readonly', true);
//    $('#txtNationality').prop('readonly', true);
//    $('#txtCountryofBirth').prop('readonly', true);

//    //Address Info
//    $('#txtAddressType').prop('disabled', true);
//    $('#txtCO').prop('readonly', true);
//    $('#txtStreet1').prop('readonly', true);
//    $('#txtStreet2').prop('readonly', true);
//    $('#txtCountry').prop('readonly', true);
//    $('#txtPhone').prop('readonly', true);
//    $('#txtZip').prop('readonly', true);
//    $('#txtCity').prop('readonly', true);
//    $('#txtDistrict').prop('readonly', true);


//    $('#divBtnupload').prop('hidden', true);
//    return false;
//});


//    $('#divBtnupload').prop('hidden', true);
//    //return false;
//});
//}
//$('#personalinfodiv').click(function () {
//       $('#prodiv').show();
//       $('#prodiv').removeAttr('hidden', true);
//       $('#prodiv2').removeAttr('hidden', true);
//       $('#prodiv3').removeAttr('hidden', true);
//       $('#prodiv4').removeAttr('hidden', true);
//       return false;
//   });

//$(document).ready(function () {
//    "use strict";

//    //$("#personalinfodiv").click(function (e) {
//    //    if ($("#profileinfoform").valid()) {
//    //        $('#prodiv').removeattr('hidden', true);
//    //        $('#prodiv2').removeattr('hidden', true);
//    //        $('#prodiv3').removeattr('hidden', true);
//    //        $('#prodiv4').removeattr('hidden', true);
//    //    }
//    //    return false;
//    //});




//        jquery.validator.addmethod("lettersonly", function(value, element) {
//            return this.optional(element) || /^[a-za-z.\s]*$/.test(value);
//        }, "only alphabetical characters"); 
//    $("#profileinfoform").validate({
//        errorclass: "my-error-class",
//        validclass: "my-valid-class",
//        rules: {
//            txtsection: {
//                required: true,
//            },
//            txtpersonid: {
//                required: true,
//                digits: true,
//            },
//            txtclass: {
//                required: true,
//            },
//            txtcampus: {
//                required: true,
//            },
//        },
//        messages: {
//            txtsection: {
//                required: ""
//            },
//            txtpersonid: {
//                required: "",
//                digits: "enter number only",
//            },
//            txtclass: {
//                required: ""
//            },
//            txtcampus: {
//                required: ""
//            },
//        }
//    });

//    $("#formids").validate({
//        errorclass: "my-error-class",
//        validclass: "my-valid-class",
//        rules: {

//            //basic info
//            txtid: {
//                required: true,
//                digits: true,
//            },
//            txtschool: {
//                required: true,
//                lettersonly: true,
//            },
//            txtcampus: {
//                required: true,
//            },
//            txtstatus: {
//                required: true,
//                lettersonly: true,
//            },
//            txtdepartment: {
//                required: true,
//                lettersonly: true,
//            },
//            txtfromdate: {
//                required: true,
//            },
//            txttodate: {
//                required: true,
//            },

//            //personal info
//            txttitle: {
//                required: true,
//                lettersonly: true,
//            },
//            txtinitials: {
//                required: true,
//                lettersonly: true,
//            },
//            txtfirstname: {
//                required: true,
//                lettersonly: true,
//            },
//            txtfathername: {
//                required: true,
//                lettersonly: true,
//            },
//            txtlastname: {
//                required: true,
//                lettersonly: true,
//            },
//            txtsecondname: {
//                required: true,
//                lettersonly: true,
//            },
//            txtmiddlename: {
//                required: true,
//                lettersonly: true,
//            },

//            //additional info
//            txtidnumber: {
//                required: true,
//                digits: true,
//            },
//            txtbirthdate: {
//                required: true,
//            },
//            txtgender: {
//                required: true,
//                lettersonly: true,
//            },
//            txtbirthplace: {
//                required: true,
//                lettersonly: true,
//            },
//            txtnationality: {
//                required: true,
//                lettersonly: true,
//            },
//            txtcountryofbirth: {
//                required: true,
//                lettersonly: true,
//            },

//            //address info
//            txtaddresstype: {
//                required: true,
//            },
//            txtco: {
//                required: true,
//                lettersonly: true,
//            },
//            txtstreet1: {
//                required: true,
//            },
//            txtstreet2: {
//                required: true,
//            },
//            txtcountry: {
//                required: true,
//                lettersonly: true,
//            },
//            txtphone: {
//                required: true,
//                digits: true,
//            },
//            txtzip: {
//                required: true,
//                digits: true,
//            },
//            txtcity: {
//                required: true,
//                lettersonly: true,
//            },
//            txtdistrict: {
//                required: true,
//                lettersonly: true,
//            },
//        },
//        messages: {

//            //basic info
//            txtid: {
//                required: "",
//                digits: "enter number only",
//            },
//            txtschool: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtcampus: {
//                required: "",
//            },
//            txtstatus: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtdepartment: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtfromdate: {
//                required: "",
//            },
//            txttodate: {
//                required: "",
//            },

//            //personal info
//            txttitle: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtinitials: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtfirstname: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtfathername: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtlastname: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtsecondname: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtmiddlename: {
//                required: "",
//                lettersonly: "enter letter only",
//            },

//            //additional info
//            txtidnumber: {
//                required: "",
//                digits: "enter number only",
//            },
//            txtbirthdate: {
//                required: "",
//            },
//            txtgender: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtbirthplace: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtnationality: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtcountryofbirth: {
//                required: "",
//                lettersonly: "enter letter only",
//            },

//            //address info
//            txtaddresstype: {
//                required: "",
//            },
//            txtco: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtstreet1: {
//                required: "",
//            },
//            txtstreet2: {
//                required: "",
//            },
//            txtcountry: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtphone: {
//                required: "",
//                digits: "enter number only",
//            },
//            txtzip: {
//                required: "",
//                digits: "enter number only",
//            },
//            txtcity: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//            txtdistrict: {
//                required: "",
//                lettersonly: "enter letter only",
//            },
//        }
//    });
//    $('#btn-edit').click(function () {

//        //basic info
//        $('#txtid').removeattr('readonly', true);
//        $('#txtschool').removeattr('readonly', true);
//        $('#txtcampus').removeattr('disabled', true);
//        $('#txtstatus').removeattr('readonly', true);
//        $('#txtdepartment').removeattr('readonly', true);
//        $('#txtfromdate').removeattr('readonly', true);
//        $('#txttodate').removeattr('readonly', true);
//        $('#uploadiv').removeattr('hidden', true);

//        //perosnal info
//        $('#txttitle').removeattr('readonly', true);
//        $('#txtinitials').removeattr('readonly', true);
//        $('#txtfirstname').removeattr('readonly', true);
//        $('#txtfathername').removeattr('readonly', true);
//        $('#txtmiddlename').removeattr('readonly', true);
//        $('#txtsecondname').removeattr('readonly', true);
//        $('#txtlastname').removeattr('readonly', true);

//        //additional info
//        $('#txtidnumber').removeattr('readonly', true);
//        $('#txtbirthdate').removeattr('readonly', true);
//        $('#txtgender').removeattr('readonly', true);
//        $('#txtbirthplace').removeattr('readonly', true);
//        $('#txtnationality').removeattr('readonly', true);
//        $('#txtcountryofbirth').removeattr('readonly', true);

//        //address info
//        $('#txtaddresstype').removeattr('disabled', true);
//        $('#txtco').removeattr('readonly', true);
//        $('#txtstreet1').removeattr('readonly', true);
//        $('#txtstreet2').removeattr('readonly', true);
//        $('#txtcountry').removeattr('readonly', true);
//        $('#txtphone').removeattr('readonly', true);
//        $('#txtzip').removeattr('readonly', true);
//        $('#txtcity').removeattr('readonly', true);
//        $('#txtdistrict').removeattr('readonly', true);

//        $('#divbtnupload').removeattr('hidden', true);
//    });

//    $('#btn-update').click(function () {
//        if ($("#formids").valid()) {
//            //basic info
//            $('#txtid').prop('readonly', true);
//            $('#txtschool').prop('readonly', true);
//            $('#txtcampus').prop('disabled', true);
//            $('#txtstatus').prop('readonly', true);
//            $('#txtdepartment').prop('readonly', true);
//            $('#txtfromdate').prop('readonly', true);
//            $('#txttodate').prop('readonly', true);
//            $('#uploadiv').prop('hidden', true);


//            //perosnal info
//            $('#txttitle').prop('readonly', true);
//            $('#txtinitials').prop('readonly', true);
//            $('#txtfirstname').prop('readonly', true);
//            $('#txtfathername').prop('readonly', true);
//            $('#txtmiddlename').prop('readonly', true);
//            $('#txtsecondname').prop('readonly', true);
//            $('#txtlastname').prop('readonly', true);

//            //additional info
//            $('#txtidnumber').prop('readonly', true);
//            $('#txtbirthdate').prop('readonly', true);
//            $('#txtgender').prop('readonly', true);
//            $('#txtbirthplace').prop('readonly', true);
//            $('#txtnationality').prop('readonly', true);
//            $('#txtcountryofbirth').prop('readonly', true);

//            //address info
//            $('#txtaddresstype').prop('disabled', true);
//            $('#txtco').prop('readonly', true);
//            $('#txtstreet1').prop('readonly', true);
//            $('#txtstreet2').prop('readonly', true);
//            $('#txtcountry').prop('readonly', true);
//            $('#txtphone').prop('readonly', true);
//            $('#txtzip').prop('readonly', true);
//            $('#txtcity').prop('readonly', true);
//            $('#txtdistrict').prop('readonly', true);


//            $('#divbtnupload').prop('hidden', true);
//        }
//    });

$('#btn-image').click(function () {
    $('#imageuploader').modal('show');
});
//});
