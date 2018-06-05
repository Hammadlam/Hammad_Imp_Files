$("#teachertimetableform").submit(function (e) {

    $.ajax({
        url: encodeURI("../TimeTable/getJQGridJsonTT"),
        data: {
            campusId: $("#txtCampusTT > option:selected").attr("value"),
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            jQuery('#Teachertimetable_grid').jqGrid('clearGridData');
            var role = data[data.length - 1].user_role;
            $('#Teachertimetable_grid').jqGrid('setGridParam', { data: data });
            $("#Teachertimetable_div").show();
            $("#Teachertimetable_grid").jqGrid('setGridState', 'visible');//or 'hidden' 

            var $screensize = $("#Teachertimetable_div").width() - 42;
            var $size = ($screensize / 100);

            $("#Teachertimetable_grid").jqGrid({
                datatype: "local",
                loadonce: true,
                colNames: ['Action', 'S No', 'Name', 'Date'],
                colModel: [
                    { name: 'status', index: 'status', editable: false, width: $size * 15, align: 'left', resizable: false },
                    { name: 'serialNo', index: 'serialNo', editable: false, width: $size * 10, align: 'left', resizable: false },
                    { name: 'fileName', index: 'fileName', editable: false, width: $size * 40, align: 'left', resizable: false },
                    { name: 'date', index: 'date', editable: false, width: $size * 35, align: 'left', resizable: false }
                ],
                gridComplete: function () {
                    var ids = jQuery("#Teachertimetable_grid").getDataIDs();
                    for (var i = 0; i < ids.length; i++) {
                        var cl = ids[i];

                        Download = "<i style='margin-left:30%; color:#e60000;' title='Download' class='fa fa-download fa-lg' onclick='getSelected_T_Timetable()'></i>";
                        Delete = "<i style='margin-left:15%; color:#e60000;' title='Delete' class='fa fa-trash-o fa-lg' onclick='getDeleted_T_Timetable()'></i>";
                        jQuery("#Teachertimetable_grid").setRowData(ids[i], { status: Download + Delete });
                    }
                },
                data: data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                rowNum: 10,
                //rowList: [10, 20, 30],
                viewrecords: true,
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                caption: "Teacher Timetable",
                pager: "#Teachertimetable_pager",
                viewrecords: true,
                hidegrid: false,
                shrinkToFit: false
            }).trigger('reloadGrid', [{ current: true }]);
        },
        error: function (error) {
            show_err_alert_js('No Record Found');
            $("#Teachertimetable_div").css("display", "none");
        }
    });
    return false
    // Add selection
    $("#Teachertimetable_grid").setSelection(4, true);

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.Teachertimetable_wrapper').width();

        $('#Teachertimetable_grid').setGridWidth(width, false);
    });

});

function getSelected_T_Timetable() {

    var selRowId = $('#Teachertimetable_grid').jqGrid('getGridParam', 'selrow');

    var ReportsId = $('#Teachertimetable_grid').jqGrid('getCell', selRowId, 'fileName');

    window.location.href = "/TimeTable/DownloadTimetable?reportid=" + ReportsId;

}


function getDeleted_T_Timetable() {
    var selRowId = $('#Teachertimetable_grid').jqGrid('getGridParam', 'selrow');
    var serialId = $('#Teachertimetable_grid').jqGrid('getCell', selRowId, 'fileName');


    $.ajax({
        type: "POST",
        url: encodeURI("../TimeTable/DeleteTimetable"),
        data: { filename: serialId },
        success: function (data) {
            if (data)
                $('#Teachertimetable_grid').delRowData(selRowId[0]);
            else
                show_err_alert_js("You Do Not Have Rights");
        }
    });

}
//$(document).ready(function () {


//    // Examle data for jqGrid
//    var mydata = [
//    { id: "1", filename: "file1.text", Date: "10may2017", status: "view / download" },
//        { id: "2", filename: "file2.text", Date: "20may2017", status: "view / download" },
//        { id: "3", filename: "file3.text", Date: "25may2017", status: "view / download" },
//        { id: "4", filename: "file4.text", Date: "8jun2017", status: "view / download" },
//        { id: "5", filename: "file5.text", Date: "17jun2017", status: "view / download" },
//        { id: "6", filename: "file6.text", Date: "3jul2017", status: "view / download" },

//    ];

//    // Configuration for jqGrid Example 1


//    // Configuration for jqGrid Example 2
//    $("#Teachertimetable_grid").jqGrid({
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
//            var ids = jQuery("#Teachertimetable_grid").getDataIDs();
//            for (var i = 0; i < ids.length; i++) {
//                var cl = ids[i];
//                //class='btn btn-xs btn-info'
//                view = "<input style='height:18px;width:75px;' type='button' value='View' class='btn btn-xs btn-primary'>";
//                download = "&nbsp;<input style='height:18px;width:75px;' type='button' class='btn btn-xs btn-primary' value='Download' />";

//                jQuery("#Teachertimetable_grid").setRowData(ids[i], { status: view + download })
//            }
//        },

//        pager: "#Teachertimetable_pager",
//        viewrecords: true,
//        //caption: "Quiz",
//        add: true,
//        edit: true,
//        addtext: 'Add',
//        edittext: 'Edit',
//        hidegrid: false,
//        shrinkToFit: false,
//        caption: "Teacher TIMETABLE"
//    });

//    // Add selection
//    $("#Teachertimetable_grid").setSelection(4, true);


//    // Setup buttons
//    $("#Teachertimetable_grid").jqGrid('navGrid', '#Teachertimetable_pager',
//            { edit: true, add: true, del: true, search: true },
//            { height: 200, reloadAfterSubmit: true }
//    );

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.Teachertimetable_wrapper').width();

//        $('#Teachertimetable_grid').setGridWidth(width, false);
//    });

//    $("#Teachertimetable_div").hide();

//    $("#Teachertimetable_grid").jqGrid('setGridState', 'hidden');//or 'hidden' 

//    $("#btnTeachertimetable").click(function (e) {
//        if ($("#teachertimetableform").valid()) {

//            $("#Teachertimetable_div").show();
//            $("#Teachertimetable_grid").jqGrid('setGridState', 'visible');//or 'hidden' 
//        }
//        return false;
//    });

//    $("#teachertimetableform").validate({
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