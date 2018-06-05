$("#txtCampusFH").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Fee/getClassJson"),
        data: { campusId: $("#txtCampusFH > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassFH").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassFH").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Fee/getSectionJson"),
        data: { campusId: $("#txtCampusFH > option:selected").attr("value"), classId: $("#txtClassFH > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionFH").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionFH").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Fee/getStudentJson"),
        data: {
            campusId: $("#txtCampusFH > option:selected").attr("value"),
            classId: $("#txtClassFH > option:selected").attr("value"),
            sectionId: $("#txtSectionFH > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option>Student</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtStudentFH").html(items.join(' '));
        }
    });
    return false;
});

$('.date-own').datepicker({
    minViewMode: 2,
    format: 'yyyy'
});


// Examle data for jqGrid

// Configuration for jqGrid Example 1


// Configuration for jqGrid Example 2
//$("#feehistorygrid").jqGrid({
//    data: mydata,
//    datatype: "local",
//    autoheight: true,
//    autowidth: true,
//    rowNum: 20,
//    rowList: [10, 20, 30],
//    colNames: ['Student Id', 'Name', 'Date', 'Fee Amount'],
//    colModel: [
//        //width: "350", resizable: false, editable: false, align: 'center'
//        { name: 'Sid', index: 'id', editable: false, width: 106, align: 'left', resizable: false, sorttype: "int", search: true },
//        { name: 'stdname', index: 'stdname', editable: false, width: 600, align: 'left', resizable: false, search: true },

//        { name: 'Date', index: 'Date', editable: false, width: 170, align: 'left', resizable: false, search: true },

//        { name: 'Feeamount', index: 'status', editable: false, width: 170, align: 'left', resizable: false, sortable: false },


//    ],

//    //gridComplete: function () {
//    //    var ids = jQuery("#feehistorygrid").getDataIDs();
//    //    for (var i = 0; i < ids.length; i++) {
//    //        var cl = ids[i];

//    //        view = "<input style='height:18px;width:75px;' type='button' value='View' class='btn btn-xs btn-primary'>";
//    //        download = "&nbsp;<input style='height:18px;width:75px;' type='button' class='btn btn-xs btn-primary' value='Download' />";

//    //        jQuery("#feehistorygrid").setRowData(ids[i], { status: view + download })
//    //    }
//    //},

//    pager: "#feehistory_pager",
//    viewrecords: true,
//    loadonce: true,
//    hidegrid: false,
//    shrinkToFit: false,
//    caption: "Fee History"
//});

$('#feehistoryform').submit(function(e){

    var mydata = [
    { Sid: "1001", stdname: "Ahmed Ali", Date: "10 May 2017", Feeamount: "1000/-" },
    { Sid: "1002", stdname: "Shayan", Date: "10 May 2017", Feeamount: "1000/-" },
    { Sid: "1003", stdname: "Bilal khan", Date: "10 May 2017", Feeamount: "1000/-" },
    { Sid: "1004", stdname: "Waleed abbasi", Date: "10 May 2017", Feeamount: "1000/-" },
    { Sid: "1005", stdname: "Moiz", Date: "10 May 2017", Feeamount: "1000/-" }

    ];

    $.ajax({
        url: encodeURI("../Fee/getJQGridJson"),
        data: {
            campus: $("#txtCampusFH > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $('#feehistorydiv').show();
            //$('#campusStrength_table').jqGrid('setGridParam', { data: data });
            //$("#campusStrength_div").show();
            $("#feehistorygrid").jqGrid('setGridState', 'visible');//or 'hidden' 

            var $screensize = $("#feehistorydiv").width()-41;
            var $size = ($screensize / 100);
            $("#feehistorygrid").jqGrid({
                datatype: "local",
                colNames: ['Student Id.', 'Student Name', 'Date', 'Fee Amount'],
                colModel: [
                    { name: 'Sid', index: 'Sid', align: 'center', width: $size * 15, resizable: false },
                    { name: 'stdname', index: 'stdname', width: $size * 30, search: true, resizable: false },
                    { name: 'Date', index: 'Date', width: $size * 20, search: true, resizable: false },
                    { name: 'Feeamount', index: 'Feeamount', align: 'center', width: $size * 35, search: true, resizable: false }

                ],
                data: mydata,
                styleUI: 'Bootstrap',
                pager: "#campusStrength_pager_list",
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                //rowNum: 10,
                //rowList: [10, 20, 30],
                viewrecords: true,
                caption: "Fee History",
                shrinkToFit: false,
                gridview: true,
                hidegrid: false,
                loadonce: true
            }).trigger('reloadGrid', [{ current: true }]);
        },
        error: function (error) {
            alert('error');
            show_alert_js();
            $("#feehistorydiv").css("display", "none");
            //$(".camp-strength").css("display", "none");
        }
    });

    return false;

});

// Add selection
$("#feehistorygrid").setSelection(4, true);

// Add responsive to jqGrid
$(window).bind('resize', function () {
    var width = $('.feehistory_wrapper').width();

    $('#feehistorygrid').setGridWidth(width, false);
});

//$("#feehistorydiv").hide();

//$("#feehistorygrid").jqGrid('setGridState', 'hidden');//or 'hidden' 

//$("#btnfeehistory").click(function (e) {
//    if ($("#txtCampusFH > option:selected").attr("value") != "" && $("#txtClassFH > option:selected").attr("value") != "" &&
//        $("#txtSectionFH > option:selected").attr("value") != "" && $("#txtDateFH").val().length != 0 &&
//        $("#txtStudentFH > option:selected").attr("value") != "") {

//        $("#feehistorydiv").show();
//        $("#feehistorygrid").jqGrid('setGridState', 'visible');//or 'hidden' 
//    } else {
//        alert("All fields must be selected!");
//    }
//    return false;
//});

//$("#feehistoryform").validate({
//    errorClass: "my-error-class",
//    validClass: "my-valid-class",
//    rules: {
//        txtSubject: {
//            required: true,
//        },
//        txtStudentId: {
//            required: true,
//        },
//        txtSelectMonth: {
//            required: true,
//        },
//        txtClass: {
//            required: true,
//        },
//        txtCampus: {
//            required: true,
//        },
//        txtDate: {
//            required: true,
//        }
//    },
//    messages: {
//        txtSubject: {
//            required: "",
//        },
//        txtStudentId: {
//            required: ""
//        },
//        txtSelectMonth: {
//            required: ""
//        },
//        txtClass: {
//            required: ""
//        },
//        txtCampus: {
//            required: ""
//        },
//        txtDate: {
//            required: ""
//        }
//    }
//});




//$(document).ready(function()
//{
//    $("#btnviewresult").click(function () {

//        $("#resultgrid").show();

//        //$("#p1").hide();
//        //$("#div1").text($("#p1").css("display"));
//    });
//});

