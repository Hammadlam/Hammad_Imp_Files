$("#viewAttendanceTMRForm").submit(function (e) {
    $.ajax({
        url: encodeURI("../TM/getAttendanceFillJQGrid"),
        data: {
            empid: $("#txtEmpNameTMR > option:selected").attr("value"),
            dateid: $("#ViewAttendaceDateTMR").val()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $('#attendanceViewTMRA_table_c').jqGrid('clearGridData');
            //var role = data[data.length - 1].user_role;
            //var caption = data[data.length - 1].Caption;
            $('#attendanceViewTMRA_table_c').jqGrid('setGridParam', { data: data });
            $("#attendanceViewTMRA_div_c").show();
            $("#attendanceViewTMRA_table_c").jqGrid('setGridState', 'visible');//or 'hidden' 

            var $screensize = $("#attendanceViewTMRA_div_c").width() - 42;
            var $size = ($screensize / 100);

            $("#attendanceViewTMRA_table_c").jqGrid({
                datatype: "local",
                colNames: ['Action', 'Employee Name', 'Date', 'Day', 'Check In', 'Check Out', 'Client Id', 'Client', 'Remarks (Time in)', 'Remarks (Time out)'],
                colModel: [
                    { name: 'status', index: 'status', editable: false, width: $size * 8, align: 'left', resizable: false },
                    { name: 'employeename', index: 'employeename', width: $size * 22, resizable: false, align: 'left' },
                    { name: 'date', index: 'date', editable: false, width: $size * 15, resizable: false },
                    { name: 'day', index: 'day', editable: false, width: $size * 10, resizable: false },
                    { name: 'checkintime', index: 'checkintime', editable: false, width: $size * 10, resizable: false, align: 'center' },
                    { name: 'checkouttime', index: 'checkouttime', editable: false, width: $size * 10, resizable: false, align: 'center' },
                    { name: 'clientid', index: 'clientid', editable: false, width: $size * 05, resizable: false, hidden: true },
                    { name: 'client', index: 'client', editable: false, width: $size * 30, resizable: false },
                    { name: 'remarks', index: 'remarks', editable: false, width: $size * 30, resizable: false, classes: 'wrap' },
                    { name: 'remarkstout', index: 'remarkstout', editable: false, width: $size * 30, resizable: false, classes: 'wrap' }
                ],
                gridComplete: function () {
                    var ids = jQuery("#attendanceViewTMRA_table_c").getDataIDs();
                    for (var i = 0; i < ids.length; i++) {
                        var cl = ids[i];
                        //Download = "&nbsp;<input style='height:18px;width:75px;' type='button' onclick='getSelectedRows()' class='btn btn-xs btn-danger' value='Download' />";
                        Delete = "<i style='margin-left:15%; color:#e60000' title='Delete' class='fa fa-trash-o fa-lg' onclick='confirm_dialogue_TMRA()'></i>";
                        jQuery("#attendanceViewTMRA_table_c").setRowData(ids[i], { status: Delete });

                    }
                },
                data: data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                rowNum: 300,
                //rowList: [10, 20, 30],
                viewrecords: true,
                iconSet: "fontAwesome",
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                caption: "Monthly Time Sheet",
                hidegrid: false,
                shrinkToFit: false,
                loadonce: true                
            }).trigger('reloadGrid', [{ current: true }]);
        },
        error: function (error) {
            show_err_alert_js('No Record Found');
            $("#attendanceViewTMRA_div_c").css("display", "none");
        }
    });
    return false;
    $("#attendanceViewTMRA_table_c").setSelection(4, true);
    $(window).bind('resize', function () {
        var width = $('.attendanceViewTMR_jqGrid_c').width();

        $('#attendanceViewTMRA_table_c').setGridWidth(width, false);
    });
});



$("#btn_createreport").click(function (e) {
    $.ajax({
        url: encodeURI("../TM/CreatePdf"),
        data: {
            empid: $("#txtEmpNameTMR > option:selected").attr("value"),
            date: $("#ViewAttendaceDateTMR").val()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            window.location.href = '../TM/launch_report';
        },
        error: function (error) {
            show_err_alert_js('No Record Found');
            $("#attendanceViewTMR_div").css("display", "none");
        }
    });
});

$("#ViewReportECRForm").submit(function (e) {
    e.preventDefault();
    $.ajax({
        url: encodeURI("../TM/CreateConveyanceReport"),
        data: {
            Month: $("#ViewReportMonthECR").val()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            window.location.href = '../TM/launch_Conveyance_report';
        },
        error: function (error) {
            show_err_alert_js('Found Some Error, Please Try again');
        }
    });
});

function confirm_dialogue_TMRA() {
    $('.summernote').summernote();
    $('#empTMRA_dialogue').modal('show');
}

$("#empTMRA_dialogue_frm").submit(function (e) {
    e.preventDefault();
    $('#empTMRA_dialogue').modal('hide');
    getDeletedTMRA();
});

function getDeletedTMRA() {

    var selRowId = $('#attendanceViewTMRA_table_c').jqGrid('getGridParam', 'selrow');
    var date = $('#attendanceViewTMRA_table_c').jqGrid('getCell', selRowId, 'date');
    var clientid = $('#attendanceViewTMRA_table_c').jqGrid('getCell', selRowId, 'clientid');
    var empid = $("#txtEmpNameTMR > option:selected").attr("value");


    $.ajax({
        type: "POST",
        url: encodeURI("../TM/DeleteTime"),
        data: {
            date: date,
            clientid: clientid,
            empid: empid
        },
        success: function (data) {
            if (data)
            {
                show_suc_alert_js('Successfully Deleted');
                $('#attendanceViewTMRA_table_c').delRowData(selRowId[0]);
            }
            else
                show_err_alert_js('You Do Not Have Rights');
        }
    });


}
