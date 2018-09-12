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
            jQuery('#attendanceViewTMR_table').jqGrid('clearGridData');
            var role = data[data.length - 1].user_role;
            var caption = data[data.length - 1].Caption;
            $('#attendanceViewTMR_table').jqGrid('setGridParam', { data: data });
            $("#attendanceViewTMR_div").show();
            $("#attendanceViewTMR_table").jqGrid('setGridState', 'visible');//or 'hidden' 

            var $screensize = $("#attendanceViewTMR_div").width() - 42;
            var $size = ($screensize / 100);

            $("#attendanceViewTMR_table").jqGrid({
                datatype: "local",
                colNames: ['Date', 'Day', 'Check In', 'Check Out', 'Client', 'Remarks (Time in)', 'Remarks (Time out)'],
                colModel: [
                    { name: 'date', index: 'date', editable: false, width: $size * 15, search: true, resizable: false },
                    { name: 'day', index: 'day', editable: false, width: $size * 10, search: true, resizable: false },
                    { name: 'checkintime', index: 'checkintime', editable: false, width: $size * 10, search: true, resizable: false, align: 'center' },
                    { name: 'checkouttime', index: 'checkouttime', editable: false, width: $size * 10, search: true, resizable: false, align: 'center' },
                    { name: 'client', index: 'client', editable: false, width: $size * 30, search: true, resizable: false },
                    { name: 'remarks', index: 'remarks', editable: false, width: $size * 30, search: true, resizable: false },
                    { name: 'remarkstout', index: 'remarkstout', editable: false, width: $size * 30, search: true, resizable: false }
                ],
                data: data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                rowNum: 10,
                rowList: [10, 20, 30],
                viewrecords: true,
                iconSet: "fontAwesome",
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                caption: caption,
                pager: "#pager_list_TMR",
                hidegrid: false,
                shrinkToFit: false,
                loadonce: true,
                caption: "Monthly Attendance"
            }).trigger('reloadGrid', [{ current: true }]);
        },
        error: function (error) {
            show_err_alert_js('No Record Found');
            $("#attendanceViewTMR_div").css("display", "none");
        }
    });
    return false;
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

