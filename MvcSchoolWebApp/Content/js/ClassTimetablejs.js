$("#txtCampusCT").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../TimeTable/getClassJson"),
        data: { campusId: $("#txtCampusCT > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value =''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassCT").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassCT").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../TimeTable/getSectionJson"),
        data: { campusId: $("#txtCampusCT > option:selected").attr("value"), classId: $("#txtClassCT > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value =''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionCT").html(items.join(' '));
        }
    });
    return false;
});

$("#classtimetableform").submit(function (e) {

    $.ajax({
        url: encodeURI("../TimeTable/getJQGridJsonCT"),
        data: {
            campusId: $("#txtCampusCT > option:selected").attr("value"),
            classId: $("#txtClassCT > option:selected").attr("value"),
            sectionId: $("#txtSectionCT > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            jQuery('#classtimetable_grid').jqGrid('clearGridData');
            var role = data[data.length - 1].user_role;
            $('#classtimetable_grid').jqGrid('setGridParam', { data: data });
            $("#classtimetable_div").show();
            $("#classtimetable_grid").jqGrid('setGridState', 'visible');//or 'hidden' 

            var $screensize = $("#classtimetable_div").width() - 42;
            var $size = ($screensize / 100);

            $("#classtimetable_grid").jqGrid({
                datatype: "local",
                loadonce: true,
                colNames: ['Action', 'S No', 'Name', 'Date'],
                colModel: [
                    { name: 'status', index: 'status', editable: false, width: $size*15, align: 'left', resizable: false },
                    { name: 'serialNo', index: 'serialNo', editable: false, width: $size*10, align: 'left', resizable: false },
                    { name: 'fileName', index: 'fileName', editable: false, width: $size*40, align: 'left', resizable: false },

                    { name: 'date', index: 'date', editable: false, width: $size*35, align: 'left', resizable: false }

                ],

                gridComplete: function () {
                    var ids = jQuery("#classtimetable_grid").getDataIDs();
                    for (var i = 0; i < ids.length; i++) {
                        var cl = ids[i];

                        //view = "<input style='height:18px;width:75px;' type='button' value='View' class='btn btn-xs btn-primary'>";
                        Download = "<i style='margin-left:30%; color:#e60000;' title='Download' class='fa fa-download fa-lg' onclick='getSelected_C_Timetable()'></i>";
                            Delete = "<i style='margin-left:15%; color:#e60000;' title='Delete' class='fa fa-trash-o fa-lg' onclick='getDeleted_C_Timetable()'></i>";
                            jQuery("#classtimetable_grid").setRowData(ids[i], { status: Download + Delete });

                    }
                },
                data:data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                rowNum: 10,
                viewrecords: true,
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                caption: "Class Timetable",
                pager: "#classtimetable_pager",
                viewrecords: true,
                hidegrid: false,
                shrinkToFit: false
            }).trigger('reloadGrid', [{ current: true }]);

        },
        error: function (error)
        {
            show_err_alert_js('No Record Found');
            $("#classtimetable_div").css("display", "none");
        }
    });

        $("#classtimetable_grid").setSelection(4, true);

        // Add responsive to jqGrid
        $(window).bind('resize', function () {
            var width = $('.classtimetable_wrapper').width();

            $('#classtimetable_grid').setGridWidth(width, false);
        });

    return false;
});
function getSelected_C_Timetable() {

    var selRowId = $('#classtimetable_grid').jqGrid('getGridParam', 'selrow');

    var ReportsId = $('#classtimetable_grid').jqGrid('getCell', selRowId, 'fileName');

    window.location.href = "/TimeTable/DownloadTimetable?reportid=" + ReportsId;

}

function getDeleted_C_Timetable() {

    var selRowId = $('#classtimetable_grid').jqGrid('getGridParam', 'selrow');
    var fileName = $('#classtimetable_grid').jqGrid('getCell', selRowId, 'fileName');

    
    $.ajax({
        type: "POST",
        url: encodeURI("../TimeTable/DeleteTimetable"),
        data: { filename: fileName },
        success: function (data) {
            if (data)
                $('#classtimetable_grid').delRowData(selRowId[0]);
            else
                show_err_alert_js("You Do Not Have Rights");
        }
    });

}
//$(document).ready(function () {


//    // Examle data for jqGrid
//    var mydata = [
//        { id: "1", filename: "file1.text", Date: "10may2017", status: "view / download" },
//        { id: "2", filename: "file2.text", Date: "20may2017", status: "view / download" },
//        { id: "3", filename: "file3.text", Date: "25may2017", status: "view / download" },
//        { id: "4", filename: "file4.text", Date: "8jun2017", status: "view / download" },
//        { id: "5", filename: "file5.text", Date: "17jun2017", status: "view / download" },
//        { id: "6", filename: "file6.text", Date: "3jul2017", status: "view / download" },


//    ];

//    // Configuration for jqGrid Example 1


//    // Configuration for jqGrid Example 2
//    $("#classtimetable_grid").jqGrid({
//        data: mydata,
//        datatype: "local",
//        height: 450,
//        autowidth: true,
//        rowNum: 20,
//        rowList: [10, 20, 30],
//        colNames: ['S No', 'Name', 'Date', 'Action'],
//        colModel: [
//            //width: "350", resizable: false, editable: false, align: 'center'
//           { name: 'id', index: 'id', editable: false, width: 100, align: 'left', resizable: false, sorttype: "int", search: true },
//            { name: 'filename', index: 'quizname', editable: false, width: 600, align: 'left', resizable: false, search: true },

//            { name: 'Date', index: 'Date', editable: false, width: 165, align: 'left', resizable: false, search: true },

//            { name: 'status', index: 'status', editable: false, width: 180, align: 'left', resizable: false, sortable: false },


//        ],

//        gridComplete: function () {
//            var ids = jQuery("#classtimetable_grid").getDataIDs();
//            for (var i = 0; i < ids.length; i++) {
//                var cl = ids[i];
//                //class='btn btn-xs btn-info'
//                view = "<input style='height:18px;width:75px;' type='button' value='View' class='btn btn-xs btn-primary'>";
//                download = "<input style='height:18px;width:75px;' type='button' class='btn btn-xs btn-primary' value='Download' />";

//                jQuery("#classtimetable_grid").setRowData(ids[i], { status: view + download })
//            }
//        },

//        pager: "#classtimetable_pager",
//        viewrecords: true,
//        //caption: "Quiz",
//        add: true,
//        edit: true,
//        addtext: 'Add',
//        edittext: 'Edit',
//        hidegrid: false,
//        shrinkToFit: false,
//        caption: "Class Timetable"
//    });

//    // Add selection
//    $("#classtimetable_grid").setSelection(4, true);


//    // Setup buttons
//    $("#classtimetable_grid").jqGrid('navGrid', '#classtimetable_pager',
//            { edit: true, add: true, del: true, search: true },
//            { height: 200, reloadAfterSubmit: true }
//    );

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.classtimetable_wrapper').width();

//        $('#classtimetable_grid').setGridWidth(width, false);
//    });

//    $("#classtimetable_div").hide();

//    $("#classtimetable_grid").jqGrid('setGridState', 'hidden');//or 'hidden' 

//    $("#btnclasstimetable").click(function (e) {
//        if ($("#classtimetableform").valid()) {

//            $("#classtimetable_div").show();
//            $("#classtimetable_grid").jqGrid('setGridState', 'visible');//or 'hidden' 
//        }
//        return false;
//    });

//    $("#classtimetableform").validate({
//        errorClass: "my-error-class",
//        validClass: "my-valid-class",
//        rules: {
//            txtSubject: {
//                required: true,
//            },
//            txtSection: {
//                required: true,
//            },
//            txtTeacherName: {
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
//            txtTeacherName: {
//                required: ""
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


////$(document).ready(function()
////{
////    $("#btnviewresult").click(function () {

////        $("#resultgrid").show();

////        //$("#p1").hide();
////        //$("#div1").text($("#p1").css("display"));
////    });
////});

