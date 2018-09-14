$("#viewAttendanceTMRForm").submit(function (e) {
    debugger
    $.ajax({
        url: encodeURI("../TM/getAttendanceFillJQGrid"),
        data: {
            empid: $("#txtEmpNameTMR > option:selected").attr("value"),
            dateid: $("#ViewAttendaceDateTMR").val()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $('#attendanceViewTMRA_table').jqGrid('clearGridData');
            //var role = data[data.length - 1].user_role;
            //var caption = data[data.length - 1].Caption;
            $('#attendanceViewTMRA_table').jqGrid('setGridParam', { data: data });
            $("#attendanceViewTMRA_div").show();
            $("#attendanceViewTMRA_table").jqGrid('setGridState', 'visible');//or 'hidden' 

            var $screensize = $("#attendanceViewTMRA_div").width() - 42;
            var $size = ($screensize / 100);

            $("#attendanceViewTMRA_table").jqGrid({
                datatype: "local",
                loadonce: true,
                colNames: ['Date', 'Day', 'Check In', 'Check Out','Client Id', 'Client', 'Remarks (Time in)', 'Remarks (Time out)'],
                colModel: [
                    //{ name: 'action', index: 'action', editable: false, width: $size * 15, align: 'left', resizable: false },
                    //{ name: 'serial', index: 'serial', editable: false, width: $size * 15, align: 'left', resizable: false },
                    { name: 'date', index: 'date', editable: false, width: $size * 15, resizable: false },
                    { name: 'day', index: 'day', editable: false, width: $size * 10, resizable: false },
                    { name: 'checkintime', index: 'checkintime', editable: false, width: $size * 10, resizable: false, align: 'center' },
                    { name: 'checkouttime', index: 'checkouttime', editable: false, width: $size * 10, resizable: false, align: 'center' },
                    { name: 'clientid', index: 'clientid', editable: false, width: $size * 05, resizable: false, hidden: true },
                    { name: 'client', index: 'client', editable: false, width: $size * 30, resizable: false },
                    { name: 'remarks', index: 'remarks', editable: false, width: $size * 30,  resizable: false },
                    { name: 'remarkstout', index: 'remarkstout', editable: false, width: $size * 30,  resizable: false }
                ],
                data: data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                viewrecords: true,
                sortorder: "desc",
                sortIconsBeforeText: true,
                headertitles: true,
                caption: "Monthly Attendance",
                pager: "#pager_list_TMR",
                viewrecords: true,
                hidegrid: false,
                shrinkToFit: false
                
            }).trigger('reloadGrid', [{ current: true }]);
        },
        error: function (error) {
            show_err_alert_js('No Record Found');
            $("#attendanceViewTMR_div").css("display", "none");
        }
    });
    return false;
    $("#attendanceViewTMRA_table").setSelection(4, true);
    $(window).bind('resize', function () {
        var width = $('.attendanceViewTMR_jqGrid').width();

        $('#attendanceViewTMRA_table').setGridWidth(width, false);
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

function deletedtime() {
    var selRowId = $('#attendanceViewTMR_table').jqGrid('getGridParam', 'selrow');
    var date = $('#attendanceViewTMR_table').jqGrid('getCell', selRowId, 'date');
    var clientid = $('#attendanceViewTMR_table').jqGrid('getCell', selRowId, 'date');
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
                $('#attendanceViewTMR_table').delRowData(selRowId[0]);
            else
                show_err_alert_js('You Do Not Have Rights');
        }
    });

}
