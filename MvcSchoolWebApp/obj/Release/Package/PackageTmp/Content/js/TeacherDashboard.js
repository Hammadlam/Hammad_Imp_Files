$(function () {
    //$.ajax({
    //    url: encodeURI("../dashboard/getClasswiseTestJson/"),
    //    dataType: 'json',
    //    async: false
    //}).done(function (data) {
    //    GetClassTestChart(data);
    //});

    //$.ajax({
    //    url: encodeURI("../dashboard/getOverallTestJson/"),
    //    dataType: 'json',
    //    async: false
    //}).done(function (data) {
    //    GetOverAllTestChart(data);
    //});
});

function GetClassTestChart(data) {
    var ctx = document.getElementById('classwise-chart');
    if (ctx != null) {
        var stdname = [];
        var marks = [];
        data.forEach(function (obj) {
            stdname.push(obj.teachername);
            marks.push(obj.marks);
        });
        var chart = ctx.getContext('2d')
        var myChart = new Chart(chart, {
            type: 'line',
            data: {
                labels: stdname,
                datasets: [
                {
                    label: 'Result',
                    data: marks,
                    borderColor: "#0f5b78",
                    pointBackgroundColor: "#b40e1a",
                    pointBorderColor: "#b40e1a",
                    //backgroundColor:"black",
                    fill: false
                },
                ]
            },

        });
        $("#classResultBox").html(data[data.length - 1].highname + ": " + data[data.length - 1].maxmarks);

    }
}

$("#txtCampusCWT").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getClassJson"),
        //url: '@Url.Action("getClassJson", "Attendance")',
        data: { campusId: $("#txtCampusCWT > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassCWT").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassCWT").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getSectionJson"),
        data: {
            campusId: $("#txtCampusCWT > option:selected").attr("value"),
            classId: $("#txtClassCWT > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionCWT").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionCWT").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getSubjectJson"),
        data: {
            campusId: $("#txtCampusCWT > option:selected").attr("value"),
            classId: $("#txtClassCWT > option:selected").attr("value"),
            sectionId: $("#txtSectionCWT > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSubjectCWT").html(items.join(' '));
        }
    });
    return false;
});

$("#txtModuleCWT").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getClasswiseTestJson"),
        data: {
            campusid: $("#txtCampusCWT > option:selected").attr("value"),
            classid: $("#txtClassCWT > option:selected").attr("value"),
            sectionid: $("#txtSectionCWT > option:selected").attr("value"),
            subjectid: $("#txtSubjectCWT > option:selected").attr("value"),
            moduleid: $("#txtModuleCWT > option:selected").attr("value")
        },
        success: function (data) {
            GetClassTestChart(data);

        }
    });
    return false;
});

$("#txtSubjectCWT").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getClasswiseTestJson"),
        data: {
            campusid: $("#txtCampusCWT > option:selected").attr("value"),
            classid: $("#txtClassCWT > option:selected").attr("value"),
            sectionid: $("#txtSectionCWT > option:selected").attr("value"),
            subjectid: $("#txtSubjectCWT > option:selected").attr("value"),
            moduleid: $("#txtModuleCWT > option:selected").attr("value")
        },
        success: function (data) {
            GetClassTestChart(data);
        }
    });
    return false;
});

//Overall Test Char
$("#txtCampusSTD").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getSubjectJson"),
        data: {
            campusId: $("#txtCampusSTD > option:selected").attr("value"),
            classId: "",
            sectionId: ""
        },
        success: function (data) {
            var items = [];
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSubjectSTD").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSubjectSTD").change(function () {
    $.ajax({
        url: encodeURI("../dashboard/getOverallTestJson/"),
        type: "POST",
        data: {
            campusid: $("#txtCampusSTD > option:selected").attr("value"),
            subjectid: $("#txtSubjectSTD > option:selected").attr("value"),
            moduleid: $("#txtModuleSTD > option:selected").attr("value")
        },
        dataType: 'json',
        async: false
    }).done(function (data) {
        GetOverAllTestChart(data);
    });
    return false;
});

$("#txtModuleSTD").change(function () {
    $.ajax({
        url: encodeURI("../dashboard/getOverallTestJson/"),
        type: "POST",
        data: {
            campusid: $("#txtCampusSTD > option:selected").attr("value"),
            subjectid: $("#txtSubjectSTD > option:selected").attr("value"),
            moduleid: $("#txtModuleSTD > option:selected").attr("value")
        },
        dataType: 'json',
        async: false
    }).done(function (data) {
        GetOverAllTestChart(data);
    });
    return false;
});

var randomColorGenerator = function () {
    return '#' + (Math.random().toString(16) + '0000000').slice(2, 8);
};

function GetOverAllTestChart(data) {
    var classname = [];
    var averagecam = [];
    var averagemat = [];
    var color = [];
    var ClassNameMat = [];
    data.forEach(function (obj) {
        color.push(randomColorGenerator());
        if (obj.ClassName != null) {
            classname.push(obj.ClassName);
            averagecam.push(obj.averagecam);
        }
        if (obj.ClassNameMat != null) {
            averagemat.push(obj.averagemat);
            ClassNameMat.push(obj.ClassNameMat);
        }
    });


    //if (JSON.stringify(data[data.length - 1].avgcheckcam) == "true") {
    $('#oc-result-chart').replaceWith('<canvas id="oc-result-chart" height="120"></canvas>');
    var camctx = document.getElementById('oc-result-chart');
    if (camctx != null) {
        var chart = camctx.getContext('2d');

        var camChart = new Chart(chart, {

            type: 'bar',

            data: {
                labels: classname,

                datasets: [
                {
                    label: 'Cambridge Exam Result',
                    data: averagecam,
                    backgroundColor: color,
                },

                ]
            },

        });

        //}
        //if (JSON.stringify(data[data.length - 1].avgcheckmat) == "true") {
        $('#op-result-chart').replaceWith('<canvas id="op-result-chart" height="120"></canvas>');
        var matctx = document.getElementById('op-result-chart').getContext('2d');
        var matChart = new Chart(matctx, {

            type: 'bar',

            data: {
                labels: ClassNameMat,

                datasets: [
                {
                    label: 'Matric Exam Result',
                    data: averagemat,
                    backgroundColor: color,
                },

                ]
            },

        });
        //}
        $("#classavgBox").html(data[data.length - 2].highclass + ": " + data[data.length - 2].maxavg);
    }

}
//Overall Test Char
