$("#txtCampusTE").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../home/getEmployeeJson"),
        data: { campusId: $("#txtCampusTE > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option>Employee</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtEmployeesTE").html(items.join(' '));
        }
    });
    return false;
});

$("#teacherAttendanceGrid").click(function (e) {
    $.ajax({
        url: encodeURI("../Home/getEmployeeAttendanceGrid"),
        data: {
            campus: $("#txtCampusTE > option:selected").attr("value"),
            employeeid: $("#txtEmployeesTE > option:selected").attr("value"),
            dateid: $("#ViewAttendaceDateTE").val()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            jQuery('#teacherattendanceView_table').jqGrid('clearGridData');

            $('#teacherattendanceView_table').jqGrid('setGridParam', { data: data });
            $("#teacherattendanceGrid_div").show();
            $("#teacherattendanceView_table").jqGrid('setGridState', 'visible');//or 'hidden' 

            var $screensize = $("#teacherattendanceGrid_div").width() - 42;
            var $size = ($screensize / 100);

            $("#teacherattendanceView_table").jqGrid({
                datatype: "local",
                colNames: ['Date', 'Day', 'Status'],
                colModel: [
                    { name: 'date', index: 'date', editable: false, width: $size * 34, search: true, resizable: false },
                    { name: 'day', index: 'day', editable: false, width: $size * 33, search: true, resizable: false },
                    {
                        name: 'status', index: 'status', align: 'left', editable: false, resizable: false, width: $size * 33
                    }],
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
                pager: "#pager_list_down",
                hidegrid: false,
                shrinkToFit: false,
                loadonce: true,
                caption: "Monthly Attendance"
            }).trigger('reloadGrid', [{ current: true }]);

        },
        error: function (error) {
            show_err_alert_js('No Record Found!');
            $("#teacherattendanceGrid_div").css("display", "none");
        }
    });
    // Add selection
    $("#teacherattendanceView_table").setSelection(4, true);

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.teacherattendanceView_jqGrid').width();

        $('#teacherattendanceView_table').setGridWidth(width, false);
    });

    return false;
});

