$("#txtCampusFS").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Fee/getClassJson"),
        data: { campusId: $("#txtCampusFS > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassFS").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassFS").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Fee/getSectionJson"),
        data: { campusId: $("#txtCampusFS > option:selected").attr("value"), classId: $("#txtClassFS > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionFS").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionFS").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Fee/getStudentJson"),
        data: { campusId: $("#txtCampusFS > option:selected").attr("value"), classId: $("#txtClassFS > option:selected").attr("value"), sectionId: $("#txtSectionFS > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option>Student</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtStudentFS").html(items.join(' '));
        }
    });
    return false;
});


// Examle data for jqGrid
var mydata = [
    { Sid: "1001", stdname: "Ahmed Ali", Date: "10may2017", status: "Paid" },
    { Sid: "1002", stdname: "Shayan", Date: "10may2017", status: "Paid" },
    { Sid: "1003", stdname: "Bilal khan", Date: "10may2017", status: "Unpaid" },
    { Sid: "1004", stdname: "Waleed abbasi", Date: "10may2017", status: "Paid" },
    { Sid: "1005", stdname: "Moiz", Date: "10may2017", status: "Unpaid" },

];

// Configuration for jqGrid Example 1


// Configuration for jqGrid Example 2
$("#feestatusgrid").jqGrid({
    data: mydata,
    datatype: "local",
    height: 450,
    autowidth: true,
    //rowNum: 20,
    //rowList: [10, 20, 30],
    colNames: ['Student Id', 'Name', 'Date', 'Status'],
    colModel: [
        //width: "350", resizable: false, editable: false, align: 'center'
        { name: 'Sid', index: 'id', editable: false, width: 106, align: 'left', resizable: false, sorttype: "int", search: true },
        { name: 'stdname', index: 'quizname', editable: false, width: 600, align: 'left', resizable: false, search: true },

        { name: 'Date', index: 'Date', editable: false, width: 170, align: 'left', resizable: false, search: true },

        { name: 'status', index: 'status', editable: false, width: 170, align: 'left', resizable: false, sortable: false },


    ],

    //gridComplete: function () {
    //    var ids = jQuery("#feestatusgrid").getDataIDs();
    //    for (var i = 0; i < ids.length; i++) {
    //        var cl = ids[i];
    //        //class='btn btn-xs btn-info'
    //        view = "<input style='height:18px;width:75px;' type='button' value='View' class='btn btn-xs btn-primary'>";
    //        download = "&nbsp;<input style='height:18px;width:75px;' type='button' class='btn btn-xs btn-primary' value='Download' />";

    //        jQuery("#feestatusgrid").setRowData(ids[i], { status: view + download })
    //    }
    //},

    pager: "#feestatus_pager",
    viewrecords: true,
    loadonce: true,
    hidegrid: false,
    shrinkToFit: false,
    caption: "Fee Status"
});

// Add selection
$("#feestatusgrid").setSelection(4, true);

// Add responsive to jqGrid
$(window).bind('resize', function () {
    var width = $('.feestatus_wrapper').width();

    $('#feestatusgrid').setGridWidth(width, false);
});

$("#feestatusdiv").hide();

$("#feestatusgrid").jqGrid('setGridState', 'hidden');//or 'hidden' 

$("#btnfeestatus").click(function (e) {

    if ($("#txtCampusFS > option:selected").attr("value") != "" && $("#txtClassFS > option:selected").attr("value") != "" &&
        $("#txtSectionFS > option:selected").attr("value") != "" && $("#txtDateFS").val().length != 0 &&
        $("#txtStudentFS > option:selected").attr("value") != "") {

        $("#feestatusdiv").show();
        $("#feestatusgrid").jqGrid('setGridState', 'visible');//or 'hidden'
    } else {
        alert("All fields must be selected!");
    }
    return false;

});






//$(document).ready(function()
//{
//    $("#btnviewresult").click(function () {

//        $("#resultgrid").show();

//        //$("#p1").hide();
//        //$("#div1").text($("#p1").css("display"));
//    });
//});

