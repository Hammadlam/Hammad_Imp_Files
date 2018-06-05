$("#txtCampusUT").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../TimeTable/getClassJson"),
        data: { campusId: $("#txtCampusUT > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassUT").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassUT").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../TimeTable/getSectionJson"),
        data: { campusId: $("#txtCampusUT > option:selected").attr("value"), classId: $("#txtClassUT > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionUT").html(items.join(' '));
        }
    });
    return false;
});