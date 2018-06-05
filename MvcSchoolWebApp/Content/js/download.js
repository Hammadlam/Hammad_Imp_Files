$("#txtCampusDI").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Download/getClassJson"),
        data: { campusId: $("#txtCampusDI > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassDI").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassDI").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Download/getSectionJson"),
        data: { campusId: $("#txtCampusDI > option:selected").attr("value"), classId: $("#txtClassDI > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionDI").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionDI").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Download/getSubjectJson"),
        data: {
            campusId: $("#txtCampusDI > option:selected").attr("value"),
            classId: $("#txtClassDI > option:selected").attr("value"),
            sectionId: $("#txtSectionDI > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Subject</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSubjectDI").html(items.join(' '));
        }
    });
    return false;
});

$("#downloadform").submit(function (e) {
    $.ajax({
        url: encodeURI("../Download/getFillLessonPlan"),
        data: {
            campus: $("#txtCampusDI > option:selected").attr("value"),
            schclass: $("#txtClassDI > option:selected").attr("value"),
            section: $("#txtSectionDI > option:selected").attr("value"),
            subject: $("#txtSubjectDI > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var role = data[data.length - 1].user_role;
            var caption = data[data.length - 1].Caption;
            jQuery('#table_list_down').jqGrid('clearGridData');
            $('#table_list_down').jqGrid('setGridParam', { data: data });
            $("#download").show();
            $("#table_list_down").jqGrid('setGridState', 'visible');//or 'hidden' 

            var $screensize = $("#download").width() - 42;
            var $size = ($screensize / 100);

            $("#table_list_down").jqGrid({
                datatype: "local",
                colNames: ['Action', 'S. No.', 'File Name', 'Date', ],
                colModel: [
                    { name: 'status', index: 'status', editable: false, width: $size * 15, align: 'left', resizable: false },
                    { name: 'serialNo', index: 'serialNo', width: $size * 10, resizable: false },
                    { name: 'fileName', index: 'fileName', width: $size * 40, resizable: false },
                    { name: 'date', index: 'date', width: $size * 35, resizable: false }
                ],
                gridComplete: function () {
                    var ids = jQuery("#table_list_down").getDataIDs();
                    for (var i = 0; i < ids.length; i++) {
                        var cl = ids[i];
                        //Download = "&nbsp;<input style='height:18px;width:75px;' type='button' onclick='getSelectedRows()' class='btn btn-xs btn-danger' value='Download' />";
                        Download = "<i style='margin-left:30%; color:#e60000;' title='Download' class='fa fa-download fa-lg' onclick='getSelectedRows()'></i>";
                        Delete = "<i style='margin-left:15%; color:#e60000' title='Delete' class='fa fa-trash-o fa-lg' onclick='getDeletedRows()'></i>";
                        jQuery("#table_list_down").setRowData(ids[i], { status: Download + Delete });

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
                iconSet: "fontAwesome",
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                caption: caption,
                pager: "#pager_list_down",
                hidegrid: false,
                shrinkToFit: false,
                loadonce: true
            }).trigger('reloadGrid', [{ current: true }]);
        },
        error: function (error) {
            show_err_alert_js('No record Found');
            $("#download").css("display", "none");
        }
    });
    return false;
});


function getSelectedRows() {

    var selRowId = $('#table_list_down').jqGrid('getGridParam', 'selrow');

    var ReportsId = $('#table_list_down').jqGrid('getCell', selRowId, 'fileName');

    if (ReportsId == '' || ReportsId == null) {

    }
    else {
        window.location.href = "/Download/DownloadLessons?reportid=" + ReportsId;
    }
}

function getDeletedRows() {
    debugger
    var selRowId = $('#table_list_down').jqGrid('getGridParam', 'selrow');
    var filename = $('#table_list_down').jqGrid('getCell', selRowId, 'fileName');

   

    $.ajax({
        type: "POST",
        url: encodeURI("../Download/DeleteAssignment"),
        data: { filename: filename },
        success: function (data) {
            if (data)
                $('#table_list_down').delRowData(selRowId[0]);
            else
                show_err_alert_js('You Do Not Have Rights');
        }
    });

}

// Configuration for jqGrid Example 1


// Configuration for jqGrid Example 2
//    $("#table_list_down").jqGrid({
//        data: mydata,
//        datatype: "local",
//        autoheight: true,
//        autowidth: true,

//        rowNum: 20,
//        rowList: [10, 20, 30],
//        colNames: ['S No', 'File Name', 'Date', 'Action'],
//        colModel: [
//            //width: "350", resizable: false, editable: false, align: 'center'
//            { name: 'id', index: 'id', editable: false, width: 100, align: 'left', resizable: false, sorttype: "int", search: true },
//            { name: 'filename', index: 'filename', editable: false, width: 700, align: 'left', resizable: false, search: true },
//            //{ name: 'status', index: 'status', editable: false, width: 50, align: 'left', resizable: false}
//            { name: 'date', index: 'Date', editable: false, width: 80, align: 'left', resizable: false, sorttype: "int", search: true },

//            { name: 'status', index: 'status', editable: false, width: 180, align: 'left', resizable: false, sortable: false },


//        ],

//        gridComplete: function () {
//            var ids = jQuery("#table_list_down").getDataIDs();
//            for (var i = 0; i < ids.length; i++) {
//                var cl = ids[i];
//                //class='btn btn-xs btn-info'
//                view = "<input style='height:18px;width:75px;' type='button' value='View' class='btn btn-xs btn-primary'>";
//                download = "&nbsp;<input style='height:18px;width:75px;' type='button' class='btn btn-xs btn-primary' value='Download' />";

//                jQuery("#table_list_down").setRowData(ids[i], { status: view + download })
//            }
//        },

//        pager: "#pager_list_down",
//        viewrecords: true,
//        //caption: "Quiz",
//        hidegrid: false,
//        shrinkToFit: false
//    });

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.jqGrid_wrapper_down').width();

//        $('#table_list_down').setGridWidth(width, false);
//    });

//    return false;
//});

//$(document).ready(function () {


//    // Examle data for jqGrid
//    var mydata = [
//        { id: "1", filename: "Filename1", date:'11-07-2016', status: "view / download" },
//        { id: "2", filename: "Filename2", date:'11-07-2016', status: "view / download" },
//        { id: "3", filename: "Filename3", date:'11-07-2016', status: "view / download" },
//        { id: "4", filename: "Filename4", date:'11-07-2016', status: "view / download" },
//        { id: "5", filename: "Filename5", date:'11-07-2016', status: "view / download" },
//        { id: "6", filename: "Filename6", date:'11-07-2016', status: "view / download" },

//    ];

//    // Configuration for jqGrid Example 1


//    // Configuration for jqGrid Example 2
//    $("#table_list_down").jqGrid({
//        data: mydata,
//        datatype: "local",
//        height: 450,
//        autowidth: true,

//        rowNum: 20,
//        rowList: [10, 20, 30],
//        colNames: ['S No', 'File Name', 'Date' , 'Action'],
//        colModel: [
//            //width: "350", resizable: false, editable: false, align: 'center'
//            { name: 'id', index: 'id', editable: false, width: 100, align: 'left', resizable: false, sorttype: "int", search: true },
//            { name: 'filename', index: 'filename', editable: false, width: 700, align: 'left', resizable: false, search: true },
//            //{ name: 'status', index: 'status', editable: false, width: 50, align: 'left', resizable: false}
//            { name: 'date', index: 'Date', editable: false, width: 80, align: 'left', resizable: false, sorttype: "int", search: true },

//            { name: 'status', index: 'status', editable: false, width: 180, align: 'left', resizable: false, sortable: false },


//        ],

//        gridComplete: function () {
//            var ids = jQuery("#table_list_down").getDataIDs();
//            for (var i = 0; i < ids.length; i++) {
//                var cl = ids[i];
//                //class='btn btn-xs btn-info'
//                view = "<input style='height:18px;width:75px;' type='button' value='View' class='btn btn-xs btn-primary'>";
//                download = "&nbsp;<input style='height:18px;width:75px;' type='button' class='btn btn-xs btn-primary' value='Download' />";

//                jQuery("#table_list_down").setRowData(ids[i], { status: view + download })
//            }
//        },

//        pager: "#pager_list_down",
//        viewrecords: true,
//        //caption: "Quiz",
//        add: true,
//        edit: true,
//        addtext: 'Add',
//        edittext: 'Edit',
//        hidegrid: false,
//    shrinkToFit:false
//    });

//    // Add selection
//    //$("#table_list_down").setSelection(4, true);


//    // Setup buttons
//    $("#table_list_down").jqGrid('navGrid', '#pager_list_down',
//            { edit: true, add: true, del: true, search: true },
//            { height: 200, reloadAfterSubmit: true }
//    );

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.jqGrid_wrapper_down').width();

//        $('#table_list_down').setGridWidth(width,false);
//    });

//    $("#download").hide();

//    $("#table_list_down").jqGrid('setGridState', 'hidden');//or 'hidden' 

//    $("#btnviewdown").click(function (e) {
//        if ($("#downloadform").valid()) {
//            $("#download").show();
//            $("#table_list_down").jqGrid('setGridState', 'visible');//or 'hidden' 
//        }
//        return false;
//    });

//    $("#downloadform").validate({
//        errorClass: "my-error-class",


//        rules: {
//            txtSubject: {
//                required: true,
//            },
//            txtSection: {
//                required: true,
//            },

//            txtClass: {
//                required: true,
//            },
//            txtCampus: {
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
//            txtName: {
//                required: ""
//            },
//            txtClass: {
//                required: ""
//            },
//            txtCampus: {
//                required: ""
//            }
//        }



//    });

//});



