$("#txtCampusVA").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../attendance/getClassJson"),
        data: { campusId: $("#txtCampusVA > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtClassVA").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassVA").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../attendance/getSectionJson"),
        data: {
            campusId: $("#txtCampusVA > option:selected").attr("value"),
            classId: $("#txtClassVA > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtSectionVA").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionVA").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../attendance/getstudentname"),
        data: {
            campusId: $("#txtCampusVA > option:selected").attr("value"),
            classId: $("#txtClassVA > option:selected").attr("value"),
            sectionId: $("#txtSectionVA > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Student</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtnameVA").html(items.join(' '));
        }
    });
    return false;

});

$("#txtnameVA").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../attendance/getsubjectJSON"),
        data: {
            campusId: $("#txtCampusVA > option:selected").attr("value"),
            classId: $("#txtClassVA > option:selected").attr("value"),
            sectionId: $("#txtSectionVA > option:selected").attr("value"),
            nameId: $("#txtnameVA > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Subject</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtSubjectVA").html(items.join(' '));
        }
    });

    return false;
});

$("#viewAttendanceForm").submit(function (e) {
    $.ajax({
        url: encodeURI("../Attendance/getStudentFillJQGrid"),
        data: {
            campusid: $("#txtCampusVA > option:selected").attr("value"),
            classid: $("#txtClassVA > option:selected").attr("value"),
            sectionid: $("#txtSectionVA > option:selected").attr("value"),
            studentid: $("#txtnameVA > option:selected").attr("value"),
            dateid: $("#ViewAttendaceDateVA").val(),
            subjectid: $("#txtSubjectVA > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            jQuery('#attendanceView_table').jqGrid('clearGridData');
            var role = data[data.length - 1].user_role;
            var caption = data[data.length - 1].Caption;
            $('#attendanceView_table').jqGrid('setGridParam', { data: data });
            $("#attendanceView_div").show();
            $("#attendanceView_table").jqGrid('setGridState', 'visible');//or 'hidden' 

            var $screensize = $("#attendanceView_div").width() - 42;
            var $size = ($screensize / 100);

            $("#attendanceView_table").jqGrid({
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
                rowList: [10, 20, 30],
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
                loadonce: true,
                caption: "Monthly Attendance"
            }).trigger('reloadGrid', [{ current: true }]);

            $("#txtPresentVA").val(data[data.length-1].total_present);
            $("#txtAbsentVA").val(data[data.length-1].total_absent);
        },
        error: function (error) {
            show_err_alert_js('No Record Found');
            $("#txtPresentVA").val("");
            $("#txtAbsentVA").val("");
            $("#attendanceView_div").css("display", "none");
        }
    });
    return false;
});

