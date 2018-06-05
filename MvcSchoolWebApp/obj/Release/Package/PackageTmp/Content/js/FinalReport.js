$("#txtCampusRF").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Result/getClassJson"),
        data: { campusId: $("#txtCampusRF > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtClassRF").html(items.join(' '));
            $("#txtModuleRF")[0].selectedIndex = 0;
        }
    });
    return false;
});

$("#txtClassRF").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Result/getSectionJson"),
        data: {
            campusId: $("#txtCampusRF > option:selected").attr("value"),
            classId: $("#txtClassRF > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtSectionRF").html(items.join(' '));
            $("#txtModuleRF")[0].selectedIndex = 0;
        }
    });
    return false;
});

$("#txtSectionRF").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Result/getstudentname"),
        data: {
            campusId: $("#txtCampusRF > option:selected").attr("value"),
            classId: $("#txtClassRF > option:selected").attr("value"),
            sectionId: $("#txtSectionRF > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            var itemm = [];
            items.push("<option value=''>Student</option>");
            itemm.push("<option value=''>Report</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtnameRF").html(items.join(' '));
            $("#txtRptId").html(itemm.join(' '));
            $("#txtModuleRF")[0].selectedIndex = 0;
        }
    });
    return false;

});

$("#txtModuleRF").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Result/getfinalreports"),
        data: {
            campusId: $("#txtCampusRF > option:selected").attr("value"),
            classId: $("#txtClassRF > option:selected").attr("value"),
            sectionId: $("#txtSectionRF > option:selected").attr("value"),
            moduleId: $("#txtModuleRF > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Report</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtRptId").html(items.join(' '));
        }
    });
    return false;

});

//$("#txtnameRF").change(function () {
//    $.ajax({
//        type: "POST",
//        url: encodeURI("../Result/getsubjectJSON"),
//        data: {
//            campusId: $("#txtCampusRF > option:selected").attr("value"),
//            classId: $("#txtClassRF > option:selected").attr("value"),
//            sectionId: $("#txtSectionRF > option:selected").attr("value"),
//            nameId: $("#txtnameRF > option:selected").attr("value")
//        },
//        success: function (data) {
//            var items = [];
//            items.push("<option value=''>Subject</option>");
//            $.each(data,
//                function () {
//                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
//                });
//            $("#txtSubjectRF").html(items.join(' '));
//        }
//    });
//    return false;
//});


$("#FinalReportForm").submit(function () {
    var $campus = $("#txtCampusRF > option:selected").attr("value");
    var $classes = $("#txtClassRF > option:selected").attr("value");
    var $section = $("#txtSectionRF > option:selected").attr("value");
    var $nameId = $("#txtnameRF > option:selected").attr("value");
    var $session = $("#txtSessionRF").val();
    var $moduleid = $("#txtModuleRF > option:selected").attr("value");
    var $reportid = $("#txtRptId > option:selected").attr("value");

    //var ReportParam = [];

    //  ReportParam.push({
    //    Campus: campus,
    //    Class: classes,
    //    Section: section,
    //    StudentName: nameId,
    //    Session: subjectid,
    //    ReportId: reportid,
    //    ModuleID: moduleid
    //})

    //$.ajax({


    //    type: "POST",
    //    //url: '/FinalReport/GetReportParam',
    //    url: encodeURI("../FinalReport/GetReportParam"),

    //    //data: JSON.stringify(ReportParam),
    //    data: {
    //        campusId: $campus,
    //        classId: $classes,
    //        sectionId: $section,
    //        name: $nameId,
    //        moduleid: $moduleid,
    //        reportid: $reportid
    //    },        
    //    success: function (result) {
    //        //var url = result;
    //        // window.location.href = url;
    //    },
    //    error: function (result) {
    //        show_err_alert_js('Error Occured Due to late Response from Server');
    //    }

    //});
    waitingDialog.show('Please Wait: This May Take a While');
    $.ajax({
        type: "POST",
        url: encodeURI("../FinalReport/Getsquery"),
        data: {
            campusId: $campus,
            classId: $classes,
            sectionId: $section,
            name: $nameId,
            session: $session,
            moduleid: $moduleid,
            reportid: $reportid
        },
        success: function (data) {
            waitingDialog.show('Opening Report');
            waitingDialog.hide();
            window.location.href = "/FinalReport/ViewReport";
        },
        error: function (result) {
            waitingDialog.hide();
            show_err_alert_js('Error Occured');
        }
    });

    return false;
});
