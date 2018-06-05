$("#txtCampusWP").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Download/getClassJson"),
        data: { campusId: $("#txtCampusWP > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassWP").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassWP").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Download/getSectionJson"),
        data: {
            campusId: $("#txtCampusWP > option:selected").attr("value"),
            classId: $("#txtClassWP > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionWP").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionWP").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Download/getSubjectJson"),
        data: {
            campusId: $("#txtCampusWP > option:selected").attr("value"),
            classId: $("#txtClassWP > option:selected").attr("value"),
            sectionId: $("#txtSectionWP > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Subject</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSubjectWP").html(items.join(' '));
        }
    });

    return false;
});

$("#txtSubjectWP").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Download/getWeeks"),
        success: function (data) {
            var items = [];
            items.push("<option value=''>Week</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtWeekWP").html(items.join(' '));
        }
    });

    return false;
    var items = [];
    items.push("<option value=''>Week</option>");
    $("#txtWeekWP").html(items.join(' '));
});

$("#btn-editlesson").click(function () {

    var campusid = $("#txtCampusWP > option:selected").attr("value");
    var classid = $("#txtClassWP > option:selected").attr("value");
    var sectionid = $("#txtSectionWP > option:selected").attr("value");
    var subjectid = $("#txtSubjectWP > option:selected").attr("value");
    var begdate = $("#txtbegdateWP").val();
    var enddate = $("#txtenddateWP").val();

    if (campusid == "" || classid == "" || sectionid == "" || subjectid == "" || begdate == "") {
        show_err_alert_js('All header fields are Mandatory to Proceed');
    }
    else {
        $.ajax({
            type: "POST",
            url: encodeURI("../Download/getWeeklyLessonPlan"),
            data: {
                empid: $("#txtteacherWP").val(),
                begdate: $("#txtbegdateWP").val(),
                //enddate: $("#txtenddateWP").val(),
                campusid: $("#txtCampusWP > option:selected").attr("value"),
                classid: $("#txtClassWP > option:selected").attr("value"),
                sectionid: $("#txtSectionWP > option:selected").attr("value"),
                subjectid: $("#txtSubjectWP > option:selected").attr("value")
            },
            success: function (data) {
                debugger
                if (data[0] != undefined) {
                    var items = [];
                    $.each(data, function () {
                        items.push("<option value=" + data[0].teacherid + ">" + data[0].teachername + "</option>");
                    });
                    $("#txtteacherWP").html(items.join(' '));

                    //$("#txtteacherWP").val(data[0].teacherid);
                    $("#txtTopicWP").val(data[0].topic);
                    $("#txtObjectWP").val(data[0].objective);
                    $("#tb_objective").val(data[0].obj_time_break_id);
                    $("#txtResourceWP").val(data[0].resource);
                    $("#txtEvalutionWP").val(data[0].evaluation);
                    $("#tb_evaluation").val(data[0].eval_time_break_id);

                    $("#tb_teachmethod").val(data[0].tm_time_break_id);
                    $("#tb_readdisc").val(data[0].rd_time_break_id);
                    $("#txtWorkWP").val(data[0].writtenwork);
                    $("#tb_writtenwork").val(data[0].ww_time_break_id);
                    $("#txtWrapWP").val(data[0].wrapup);
                    $("#tb_wrapup").val(data[0].wu_time_break_id);
                    $("#txtStudentLearnWP").val(data[0].evaluationstdid);
                    $("#txtTeachingWP").val(data[0].evaluationteacherid);
                    $("#txtCommentsWP").val("");
                    if (data[0].princ_comments != "") {
                        for (var i = 0; i < data.length; i++) {
                            $("#txtCommentsWP").val($("#txtCommentsWP").val() +' > ' + data[i].princ_comments + "\n\n");
                        };
                    };
                    $("input[name=Methodology][value='" + data[0].teach_method + "']").prop("checked", true);
                    $("input[name=rd][value='" + data[0].read_disc + "']").prop("checked", true);
                }
                else {
                    //$("#txtteacherWP").val('');
                    $("#txtTopicWP").val('');
                    $("#txtObjectWP").val('');
                    $("#tb_objective").val('');
                    $("#txtResourceWP").val('');
                    $("#txtEvalutionWP").val('');
                    $("#tb_evaluation").val('');

                    $("#tb_teachmethod").val('');
                    $("#tb_readdisc").val('');
                    $("#txtWorkWP").val('');
                    $("#tb_writtenwork").val('');
                    $("#txtWrapWP").val('');
                    $("#tb_wrapup").val('');
                    $("#txtStudentLearnWP").val('');
                    $("#txtTeachingWP").val('');
                    $("#txtCommentsWP").val('');
                    $("input[name=Methodology][value='Recap']").prop("checked", true);
                    $("input[name=rd][value='Individual-Task']").prop("checked", true);
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
