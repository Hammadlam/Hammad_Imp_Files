$("#txtCampusWLPP").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Download/getClassJson"),
        data: { campusId: $("#txtCampusWLPP > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassWLPP").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassWLPP").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Download/getSectionJson"),
        data: {
            campusId: $("#txtCampusWLPP > option:selected").attr("value"),
            classId: $("#txtClassWLPP > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionWLPP").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionWLPP").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Download/getSubjectJson"),
        data: {
            campusId: $("#txtCampusWLPP > option:selected").attr("value"),
            classId: $("#txtClassWLPP > option:selected").attr("value"),
            sectionId: $("#txtSectionWLPP > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Subject</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSubjectWLPP").html(items.join(' '));
        }
    });

    return false;
});


$(document).ready(function (e) {
    $("#weeklyDatePicker").datetimepicker({
        format: 'DD-MMMM-YY'
    });
    var firstDate, lastDate;
    //Get the value of Start and End of Week
    $('#weeklyDatePicker').on('dp.change', function (e) {
        var value = $("#weeklyDatePicker").val();
        firstDate = moment(value, "DD-MMMM-YY").day(0).format("DD-MMMM-YY");
        lastDate = moment(value, "DD-MMMM-YY").day(6).format("DD-MMMM-YY");
        $("#weeklyDatePicker").val(firstDate + " - " + lastDate);
        $("#txtbegdateWLPP").val(firstDate);
        $("#txtenddateWLPP").val(lastDate);
    });
});

$("#btn-editWLPP").click(function (e) {
    debugger
    var campusid = $("#txtCampusWLPP > option:selected").attr("value");
    var classid = $("#txtClassWLPP > option:selected").attr("value");
    var sectionid = $("#txtSectionWLPP > option:selected").attr("value");
    var subjectid = $("#txtSubjectWLPP > option:selected").attr("value");
    var begdate = $("#txtbegdateWLPP").val();
    var enddate = $("#txtenddateWLPP").val();

    if (campusid == "" || classid == "" || sectionid == "" || subjectid == "" || begdate == "" || enddate == "") {
        show_err_alert_js('All header fields are Mandatory to Proceed');
    }
    else {
        $.ajax({
            type: "POST",
            url: encodeURI("../Download/getWeeklyLessonPlanPP"),
            data: {
                empid: $("#txtteacherWLPP").val(),
                begdate: $("#txtbegdateWLPP").val(),
                enddate: $("#txtenddateWLPP").val(),
                campusid: $("#txtCampusWLPP > option:selected").attr("value"),
                classid: $("#txtClassWLPP > option:selected").attr("value"),
                sectionid: $("#txtSectionWLPP > option:selected").attr("value"),
                subjectid: $("#txtSubjectWLPP > option:selected").attr("value")
            },
            success: function (data) {
                debugger
                if (data[0] != undefined) {
                    var items = [];
                    $.each(data, function () {
                        items.push("<option value=" + data[0].studentid + ">" + data[0].teachername + "</option>");
                    });
                    $("#txtteacherWLPP").html(items.join(' '));
                    $("#txtTopicWLPP").val(data[0].topic);
                    $("#txtObjectWLPP").val(data[0].objective);
                    $("#txtMaterialWLPP").val(data[0].resource);
                    $("#txtIniActWLPP").val(data[0].evaluation);
                    $("#txtDevproWLPP").val(data[0].writtenwork);
                    $("#txtAssessWLPP").val(data[0].wrapup);
                    $("#txthomeWLPP").val(data[0].evaluationstdid);
                    $("#txtCommentsWLPP").val("");
                    if (data[0].princ_comments != "") {
                        for (var i = 0; i < data.length; i++) {
                            $("#txtCommentsWLPP").val($("#txtCommentsWLPP").val() + ' > ' + data[i].princ_comments + "\n\n");
                        };
                    };
                }
                else {
                    $("#txtteacherWLPP").val('');
                    $("#txtTopicWLPP").val('');
                    $("#txtObjectWLPP").val('');
                    $("#txtMaterialWLPP").val('');
                    $("#txtIniActWLPP").val('');
                    $("#txtDevproWLPP").val('');
                    $("#txtAssessWLPP").val('');
                    $("#txthomeWLPP").val('');
                    $("#txtCommentsWLPP").val('');

                    show_err_alert_js('No Record Found');
                }
            },
            error: function (e) {
                show_err_alert_js('No Record Found');
            }
        });
    }

    return false;
});


