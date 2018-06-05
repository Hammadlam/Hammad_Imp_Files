$("#txtCampusAG").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../home/getClassJson"),
        data: { campusId: $("#txtCampusAG > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassAG").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassAG").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../home/getSectionJson"),
        data: { campusId: $("#txtCampusAG > option:selected").attr("value"), classId: $("#txtClassAG > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionAG").html(items.join(' '));
        }
    });
    return false;
});

$("#activityform").submit(function (e) {
    $.ajax({
        url: encodeURI("../home/getActivityGrid"),
        data: {
            campus: $("#txtCampusAG > option:selected").attr("value"),
            classes: $("#txtClassAG > option:selected").attr("value"),
            section: $("#txtSectionAG > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $('#table_list_activity').jqGrid('clearGridData');
            $("#table_list_activity").jqGrid("GridUnload");
            $('#table_list_activity').jqGrid('setGridParam', { data: data });
            $("#activity_div").show();
            $("#table_list_activity").jqGrid('setGridState', 'visible');//or 'hidden' 

            $("#table_list_activity").jqGrid({
                datatype: "local",
                colNames: ['Student Id', 'Student Name', 'Sports', 'Assembly Presentation', 'General Knowledge', 'Behaviour', 'Discipline', 'Cleanliness', 'Compliance to School Rules', 'Meeting Task Deadlines', 'Teacher Comments'],
                colModel: [
                    { name: 'studentId', index: 'studentId', editable: false, width: 100, align: 'left', resizable: false },
                    { name: 'studentName', index: 'studentName', editable: false, width: 180, align: 'left', resizable: false },
                    {
                        name: 'status', index: 'status', editable: true, width: 150, align: 'left', resizable: false, edittype: "select",
                        formatter: 'select', editoptions: {
                            value: "08:Excellent;07:Very Good;06:Good;05:Work Hard;04:Fair;03:Work Extremely Hard;02:Satisfactory;01:Needs Improvement", defaultValue: data[0].status
                        },
                    },
                    {
                        name: 'assignppt', index: 'assignppt', editable: true, width: 180, align: 'left', resizable: false, edittype: "select",
                        formatter: 'select', editoptions: {
                            value: "08:Excellent;07:Very Good;06:Good;05:Work Hard;04:Fair;03:Work Extremely Hard;02:Satisfactory;01:Needs Improvement", defaultValue: data[0].assignppt
                        },
                    },
                    {
                        name: 'gk', index: 'gk', editable: true, width: 150, align: 'left', resizable: false, edittype: "select",
                        formatter: 'select', editoptions: {
                            value: "08:Excellent;07:Very Good;06:Good;05:Work Hard;04:Fair;03:Work Extremely Hard;02:Satisfactory;01:Needs Improvement", defaultValue: data[0].gk
                        },
                    },
                    {
                        name: 'behave', index: 'behave', editable: true, width: 150, align: 'left', resizable: false, edittype: "select",
                        formatter: 'select', editoptions: {
                            value: "08:Excellent;07:Very Good;06:Good;05:Work Hard;04:Fair;03:Work Extremely Hard;02:Satisfactory;01:Needs Improvement", defaultValue: data[0].behave
                        },
                    },
                    {
                        name: 'discp', index: 'discp', editable: true, width: 150, align: 'left', resizable: false, edittype: "select",
                        formatter: 'select', editoptions: {
                            value: "08:Excellent;07:Very Good;06:Good;05:Work Hard;04:Fair;03:Work Extremely Hard;02:Satisfactory;01:Needs Improvement", defaultValue: data[0].discp
                        },
                    },
                    {
                        name: 'clean', index: 'clean', editable: true, width: 150, align: 'left', resizable: false, edittype: "select",
                        formatter: 'select', editoptions: {
                            value: "08:Excellent;07:Very Good;06:Good;05:Work Hard;04:Fair;03:Work Extremely Hard;02:Satisfactory;01:Needs Improvement", defaultValue: data[0].clean
                        },
                    },
                    {
                        name: 'compliance', index: 'compliance', editable: true, width: 155, align: 'left', resizable: false, edittype: "select",
                        formatter: 'select', editoptions: {
                            value: "08:Excellent;07:Very Good;06:Good;05:Work Hard;04:Fair;03:Work Extremely Hard;02:Satisfactory;01:Needs Improvement", defaultValue: data[0].compliance
                        },
                    },
                    {
                        name: 'task', index: 'task', editable: true, width: 152, align: 'left', resizable: false, edittype: "select",
                        formatter: 'select', editoptions: {
                            value: "08:Excellent;07:Very Good;06:Good;05:Work Hard;04:Fair;03:Work Extremely Hard;02:Satisfactory;01:Needs Improvement", defaultValue: data[0].task
                        },
                    },
                    {
                        name: 'teachercom', index: 'teachercom', editable: true, width: 152, align: 'left', resizable: false, edittype: "text"
                    }
                ],

                data: data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                rowNum: data[0].count,
                viewrecords: true,
                iconSet: "fontAwesome",
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                hidegrid: false,
                shrinkToFit: false,
                loadonce: true,
                cellEdit: true,
                editable: true,
                caption: "Grade Activities",
                cellsubmit: 'clientArray'
            }).trigger('reloadGrid', [{ current: true }]);
        },
        error: function (error) {
            show_err_alert_js('Found Some Error! Please Try Again');
            $("#activity_div").css("display", "none");
        }
    });
    return false;
});

$("#submitActivity").click(function (e) {
    e.preventDefault();
    var gridRows = $("#table_list_activity").jqGrid('getRowData');
    var rowData = new Array();

    var studentid = new Array();
    var status = new Array();
    var assignppt = new Array();
    var gk = new Array();
    var behave = new Array();
    var discp = new Array();
    var clean = new Array();
    var compliance = new Array();
    var task = new Array();
    var teachercom = new Array();
    for (var i = 0; i < gridRows.length; i++) {

        studentid[i] = gridRows[i].studentId;
        status[i] = gridRows[i].status;
        assignppt[i] = gridRows[i].assignppt;
        gk[i] = gridRows[i].gk;
        behave[i] = gridRows[i].behave;
        discp[i] = gridRows[i].discp;
        clean[i] = gridRows[i].clean;
        compliance[i] = gridRows[i].compliance;
        task[i] = gridRows[i].task;
        teachercom[i] = gridRows[i].teachercom;
    }
    $.ajax({
        type: "POST",
        url: encodeURI("../Home/GradeActivitySubmit"),
        data: {
            studentId: studentid,
            status: status,
            assignppt: assignppt,
            gk: gk,
            behave: behave,
            discp: discp,
            clean: clean,
            compliance: compliance,
            task: task,
            teachercom: teachercom,
            campusId: $("#txtCampusAG > option:selected").attr("value"),
            classId: $("#txtClassAG > option:selected").attr("value"),
            sectionId: $("#txtSectionAG > option:selected").attr("value")
        },
        success: function (data) {
            if (data == "Success") {
                show_suc_alert_js('Grade Activities Successfully Uploaded');
                $("#activity_div").css("display", "none");
            }
            else if (data == "Error") {
                show_err_alert_js('Error Occured: Please press Enter Key after inserting Attendances');
            }
            else if (data == "Rights") {
                show_err_alert_js('You Do Not Have Rights!');
                $("#activity_div").css("display", "none");
            }
            data.preventDefault();
        },
        error: function (data) {
            show_err_alert_js('Found Some Error! Please Try Again');
            data.preventDefault();
        },
        dataType: "json"
    });

});

$(window).bind('resize', function () {
    var width = $('.jqGrid_wrapper').width();
    $('#table_list_activity').setGridWidth(width, false);
});