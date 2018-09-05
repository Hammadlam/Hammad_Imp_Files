$("#txtCampusMA").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Attendance/getClassJson"),
        //url: '@Url.Action("getClassJson", "Attendance")',
        data: { campusId: $("#txtCampusMA > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassMA").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassMA").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Attendance/getSectionJson"),
        data: {
            campusId: $("#txtCampusMA > option:selected").attr("value"),
            classId: $("#txtClassMA > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionMA").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionMA").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Attendance/getSubjectJson"),
        data: {
            campusId: $("#txtCampusMA > option:selected").attr("value"),
            classId: $("#txtClassMA > option:selected").attr("value"),
            sectionId: $("#txtSectionMA > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Subject</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSubjectMA").html(items.join(' '));
        }
    });
    return false;
});


//$("#attendance_div").hide();
//$("#attendance_table").jqGrid('setGridState', 'hidden');

$("#attendanceForm").submit(function (e) {
   // show_progress();
    $.ajax({
        url: encodeURI("../Attendance/getJQGridJson"),
        data: {
            campus: $("#txtCampusMA > option:selected").attr("value"),
            classes: $("#txtClassMA > option:selected").attr("value"),
            section: $("#txtSectionMA > option:selected").attr("value"),
            subject: $("#txtSubjectMA > option:selected").attr("value"),
            date: $("#txtDateMA").val()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $('#attendance_table').jqGrid('clearGridData');
            $("#attendance_table").jqGrid("GridUnload");
            $('#attendance_table').jqGrid('setGridParam', { data: data });
            $("#attendance_div").show();
            $("#attendance_table").jqGrid('setGridState', 'visible');

            var $screensize = $("#attendance_div").width()-42;
            var $size = ($screensize / 100);

            $("#attendance_table").jqGrid({
                datatype: "local",
                colNames: ['Student Id', 'Student Name', 'Status'],
                colModel: [
                    { name: 'studentId', index: 'studentId', width: $size*30, resizable: false, align: 'left' },
                    { name: 'studentName', index: 'studentName', width: $size*40, resizable: false, align: 'left' },
                    {
                        name: 'status', index: 'status', editable: true, align: 'left', resizable: false, width: $size*30, edittype: "select",
                        formatter: 'select',
                        editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

                    }
                ],
                data:data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                loadonce: true,
                viewrecords: true,
                rowNum: data[0].count,
                iconSet: "fontAwesome",
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                cellEdit: true,
                editable: true,
                caption: "Attendance Sheet",
                add: true,
                edit: true,
                addtext: 'Add',
                edittext: 'Edit',
                hidegrid: false,
                shrinkToFit: false,
                cellsubmit: 'clientArray'

            }).trigger('reloadGrid', [{ current: true }]);
           // close_progress();
        },
        error: function (error) {
            show_alert_js();
            $("#attendance_div").css("display", "none");
        }
    });
        
        // Add responsive to jqGrid
        $(window).bind('resize', function () {
            var width = $('.attendance_jqGrid').width();
            $('#attendance_table').setGridWidth(width, false);
        });
        return false;
});


$("#submitattd_form").submit(function (e) {

    e.preventDefault();
    var gridRows = $("#attendance_table").jqGrid('getRowData');
    var rowData = new Array();

    var studentId = new Array();
    var studentName = new Array();
    var status = new Array();

    for (var i = 0; i < gridRows.length; i++) {

        studentId[i] = gridRows[i].studentId;
        studentName[i] = gridRows[i].studentName;
        status[i] = gridRows[i].status;
    }

        $.ajax({
            type: "POST",
            url: encodeURI("../Attendance/MarkAttendance"),
            data: {
                studentId: studentId,
                status: status,
                subjectId: $("#txtSubjectMA > option:selected").attr("value"),
                dateId: $("#txtDateMA").val()
            },
            success: function (data) {
                debugger
                if (data == "Success")
                {
                    show_suc_alert_js('Attendance Successfully Uploaded');
                    $("#attendance_div").css("display", "none");
                }
                else if (data == "Error")
                {
                    show_err_alert_js('Error Occured: Please press Enter Key after inserting Attendances');
                }
                else if (data == "Rights")
                {
                    show_err_alert_js('You Do Not Have Rights!');
                    $("#attendance_div").css("display", "none");
                }
            },
            error: function (data) {
                show_err_alert_js('Found Some Error! Please Try Again');
            },
            dataType: "json"
        });
});

//$("#datepickerAttendanceMA").datepicker();



//$(document).ready(function () {
//$("#attendance_table").jqGrid({

//    url: '@Url.Action("getJQGridJson", "Attendance")',
//    postData: {
//        campus: $("#txtCampus > option:selected").attr("value"),
//        classes: $("#txtClass > option:selected").attr("value"),
//        section: $("#txtSection > option:selected").attr("value")
//    },
//    datatype: "json",
//    colNames: ['Id No', 'Student Name', 'Status'],
//    colModel: [
//        { name: 'studentId', index: 'studentId', editable: true, width: 180, resizable: false },
//        { name: 'studentName', index: 'studentName', editable: true, width: 700, resizable: false },
//        {
//            name: 'status', index: 'status', editable: true, align: 'center', resizable: false, width: 190, edittype: "select",
//            editoptions: { value: "PE:Present;AB:Absent;LE:Leave", defaultValue: "PE" }
//        }
//    ],
//    styleUI: 'Bootstrap',
//    mtype: 'GET',
//    //loadonce: true,
//    height: 450,
//    autowidth: true,
//    //shrinkToFit: true,
//    rowNum: 10,
//    viewrecords: true,
//    iconSet: "fontAwesome",
//    sortorder: "desc",
//    threeStateSort: true,
//    sortIconsBeforeText: true,
//    headertitles: true,
//    cellEdit: true,
//    editable: true,
//    pager: "#pager_list",
//    caption: "Attendance Sheet",
//    add: true,
//    edit: true,
//    addtext: 'Add',
//    edittext: 'Edit',
//    hidegrid: false,
//    shrinkToFit: false

//});

////    // Add selection
////    //$("#attendance_table").setSelection(4, true);


////    // Setup buttons
////    $("#attendance_table").jqGrid('navGrid', '#pager_list',
////            { edit: true, add: true, del: true, search: true },
////            { height: 300, reloadAfterSubmit: true }
////    );

////    // Add responsive to jqGrid
////    $(window).bind('resize', function () {
////        var width = $('.attendance_jqGrid').width();
////        $('#attendance_table').setGridWidth(width,false);
////    });

////    $("#attendance_div").hide();

////    $("#attendance_table").jqGrid('setGridState', 'hidden');//or 'hidden' 

////    $("#showGrid").click(function (e) {
////        if ($("#attendanceForm").valid()) {
////            $("#attendance_div").show();
////            $("#attendance_table").jqGrid('setGridState', 'visible');//or 'hidden' 
////        }

////        return false;
////    });

//jQuery.validator.addMethod("lettersonly", function (value, element) {
//    return this.optional(element) || /^[a-zA-Z.\s]*$/.test(value);
//}, "Only alphabetical characters");

//$("#datepickerAttendance").datepicker();

//$("#attendanceForm").validate({
//    errorClass: "my-error-class",
//    validClass: "my-valid-class",
//    rules: {
//        txtSubject: {
//            required: true,
//        },
//        txtSection: {
//            required: true,
//        },
//        txtTeacherName: {
//            required: true,
//            lettersonly: true,
//        },
//        txtClass: {
//            required: true,
//        },
//        txtCampus: {
//            required: true,
//        },
//        txtDate: {
//            required: true,
//        }
//    },
//    messages: {
//        txtSubject: {
//            required: "",
//        },
//        txtSection: {

//            required: ""
//        },
//        txtTeacherName: {
//            required: "",
//            lettersonly: "Enter letters only",
//        },
//        txtClass: { 
//            required: ""
//        },
//        txtCampus: {
//            required: ""
//        },
//        txtDate: {
//            required: ""
//        }
//    }        
//});

//});

