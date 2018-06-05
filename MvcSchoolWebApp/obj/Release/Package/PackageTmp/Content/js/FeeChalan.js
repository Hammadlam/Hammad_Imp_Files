$("#txtCampusFC").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Fee/getClassJson"),
        data: { campusId: $("#txtCampusFC > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassFC").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassFC").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Fee/getSectionJson"),
        data: { campusId: $("#txtCampusFC > option:selected").attr("value"), classId: $("#txtClassFC > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionFC").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionFC").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Fee/getStudentJson"),
        data: {
            campusId: $("#txtCampusFC > option:selected").attr("value"),
            classId: $("#txtClassFC > option:selected").attr("value"),
            sectionId: $("#txtSectionFC > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option>Student</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtStudentFC").html(items.join(' '));
        }
    });
    return false;
});

$('#txtSelectMonth').datepicker({
    changeMonth: true,
    changeYear: true,
    showButtonPanel: true,
    dateFormat: 'MM yy',
    onClose: function (dateText, inst) {
        var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
        var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
        $(this).datepicker('setDate', new Date(year, month, 1));
    }
});