$(function () {
    //var data = getData();
    //AutoFollow(data);
    //getAverageData();
    var ctx = document.getElementById("total-students-chart");
    if (ctx != null) {
        var data = getData();

        var chart = ctx.getContext('2d');
        var myChart = new Chart(chart,
            {
                type: 'line',
                data: {
                    labels: data.classname, // here i show my ClassName
                    datasets: [
                        {

                            label: 'Number of Students',
                            data: data.strength, // here i show my ClassStrngth

                            fillColor: "rgba(220,220,220,0.2)",
                            strokeColor: "rgba(220,220,220,1)",
                            pointColor: "rgba(220,220,220,1)",
                            pointStrokeColor: "#fff",
                            pointHighlightFill: "#fff",
                            pointHighlightStroke: "rgba(220,220,220,1)",
                        }

                    ]
                }
            });
        getAverageData();
    }

});

//start testing      GetAveragGraph()
function getData() {
    //var dateValue = [];
    //var countValue = [];
    var classname = [];
    var strength = [];

    $.ajax({
        url: "/dashboard/GetCount/",
        dataType: 'json',
        async: false
    }).done(function (data) {
        data.forEach(function (obj) {
            classname.push(obj.ClassName);
            strength.push(obj.Strength);
        });
    });
    return {
        classname: classname,
        strength: strength
    };
}

function getAverageData() {
    //var dateValue = [];
    //var countValue = [];
    var classname = [];
    var strength = [];

    $.ajax({
        url: "/dashboard/getAverageTestJson/",
        dataType: 'json',
        async: false
    }).done(function (data) {
        GetAverageTestChart(data);
    });
}
//function AutoFollow(data) {
//    var ctx = document.getElementById("total-students-chart");
//    if (ctx != null) {
//        var chart = ctx.getContext('2d');
//        var myChart = new Chart(chart,
//            {
//                type: 'line',
//                data: {
//                    labels: data.classname, // here i show my ClassName
//                    datasets: [
//                        {

//                            label: 'Number of Students',
//                            data: data.strength, // here i show my ClassStrngth

//                            fillColor: "rgba(220,220,220,0.2)",
//                            strokeColor: "rgba(220,220,220,1)",
//                            pointColor: "rgba(220,220,220,1)",
//                            pointStrokeColor: "#fff",
//                            pointHighlightFill: "#fff",
//                            pointHighlightStroke: "rgba(220,220,220,1)",
//                        }

//                    ]
//                }
//            });
//    }
//}



//Bar Chart Average Test Marks

$("#txtCampusAD").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getModuleJson"),
        data: { campusid: $("#txtCampusAD > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtModuleAD").html(items.join(' '));
        }
    });
    return false;
});


$("#txtModuleAD").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getAverageTestJson"),
        data: {
            campusid: $("#txtCampusAD > option:selected").attr("value"),
            moduleid: $("#txtModuleAD > option:selected").attr("value")
        },
        success: function (data) {
            GetAverageTestChart(data);

        }
    });
    return false;
});

var randomColorGenerator = function () {
    return '#' + (Math.random().toString(16) + '0000000').slice(2, 8);
};


function GetAverageTestChart(data) {
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
    $('#cam-test-result-chart').replaceWith('<canvas id="cam-test-result-chart" height="120"></canvas>');
    var camctx = document.getElementById('cam-test-result-chart');
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
        $('#mat-test-result-chart').replaceWith('<canvas id="mat-test-result-chart" height="120"></canvas>');
        var matctx = document.getElementById('mat-test-result-chart').getContext('2d');
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
    }
}

//Bar Chart Average Test Marks
var chart = document.getElementById("admission-chart");

//Doughnut-chart No of Admission per Campuses
if (chart != null) {
    new Chart(chart, {
        type: 'doughnut',
        data: {
            labels: ["Campus-A", "Campus-B", "Campus-C"],
            datasets: [
                {
                    label: "",
                    backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#e8c3b9"],
                    data: [365, 325, 360]
                }
            ]
        },
        options: {
            title: {
                display: true,
                responsive: true,
                maintainAspectRatio: false,
                text: 'Number of Admissions in year 2017'
            }
        }
    });
}




