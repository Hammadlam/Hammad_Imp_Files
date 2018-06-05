//Bar Chart Average Test Marks


$(function () {
    $.ajax({
        url: "/dashboard/getStudentTestJson/",
        dataType: 'json',
        async: false
    }).done(function (data) {
        GetStudentTestChart(data);
    });
    GetAssistChart();
});

function GetAssistChart() {
    var ctx = document.getElementById('selfassisment-chart');
    if (ctx != null) {
        var data = getAssisData();
        var chart = ctx.getContext('2d');
        var myChart = new Chart(chart, {

            type: 'line',

            data: {
                labels: data.labels,

                datasets: [
                {
                    label: [" 1-Need Improvement", " 2-Satisfactory", " 3-Work Extremely Hard", " 4-Fair", " 5-Work Hard", " 6-Good", " 7-Very Good", "8-Excellent"],
                    data: data.values,
                    //backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#a22615", "#ffcd34", "#0f5b78", "#f36f13", "#958c3d", "#3e95cd", "#8e5ea2", "#3cba9f", "#0f5b78"],
                    fillColor: "rgba(220,220,220,0.5)",
                    strokeColor: "rgba(220,220,220,1)",
                    pointColor: "rgba(220,220,220,1)",
                    pointStrokeColor: "#fff",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: "rgba(220,220,220,1)",
                },

                ]
            },

        });
        myChart.destroy();

    }
}
function GetStudentTestChart(data)
{
    var subjectname = [];
    var obtainmarks = [];
    var color = [];
    var percent; var total;
    data.forEach(function (obj) {
        color.push(randomColorGenerator());
        if (obj.percentage == null) {
            subjectname.push(obj.subjectname);
            obtainmarks.push(obj.obtainmarks);
            total = obj.totalmarks;
        }
        else {
            percent = obj.percentage;
        }
    });
    if (total == undefined)
        total = 0;
    $('#sub-result-chart').replaceWith('<canvas id="sub-result-chart" height="120"></canvas>');
    var camctx = document.getElementById('sub-result-chart');
    if (camctx != null) {
        var chart = camctx.getContext('2d');
        var camChart = new Chart(chart, {

            type: 'bar',

            data: {
                labels: subjectname,

                datasets: [
                    {
                        label: 'Exam Result',
                        data: obtainmarks,
                        backgroundColor: color,
                    },

                ]
            },

        });
    }
    $("label[for='lbltotalmarks'] ").html(total);
    $("#testResultBox").html(percent + "<small>%</small>");
}

$("#txtClassSD").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getModuleJson"),
        data: { classid: $("#txtClassSD > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtModuleSD").html(items.join(' '));
        }
    });
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getStudentNameJson"),
        data: { classid: $("#txtClassSD > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtStudentPD").html(items.join(' '));
        }
    });
    return false;
});


$("#txtModuleSD").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/getStudentTestJson"),
        data: {
            classid: $("#txtClassSD > option:selected").attr("value"),
            studentid: $("#txtStudentPD > option:selected").attr("value"),
            moduleid: $("#txtModuleSD > option:selected").attr("value")
        },
        success: function (data) {
            GetStudentTestChart(data);
        }
    });
    $.ajax({
        type: "POST",
        url: encodeURI("../dashboard/GetAssisment"),
        data: {
            classid: $("#txtClassSD > option:selected").attr("value"),
            studentid: $("#txtStudentPD > option:selected").attr("value"),
            moduleid: $("#txtModuleSD > option:selected").attr("value")
        },
        success: function (data) {
            GetAssistChart(data);
        }
    });
    return false;
});

//Bar Chart Average Test Marks

// Assisment Chart 


function getAssisData() {
    var values = [];
    var labels = [];
    $.ajax({
        url: "/dashboard/GetAssisment/",
        data: {
            classid: $("#txtClassSD > option:selected").attr("value"),
            studentid: $("#txtStudentPD > option:selected").attr("value"),
            moduleid: $("#txtModuleSD > option:selected").attr("value")
        },
        dataType: 'json',
        async: false
    }).done(function (data) {
        if (data[0] != undefined) {
            values.push(data[0].status); labels.push("Sports");
            values.push(data[0].assignppt); labels.push("Assignment");
            values.push(data[0].gk); labels.push("GK");
            values.push(data[0].behave); labels.push("Behavior");
            values.push(data[0].discp); labels.push("Discipline");
            values.push(data[0].clean); labels.push("Cleanliness");
            values.push(data[0].compliance); labels.push("Compliance to School Rules");
            values.push(data[0].task); labels.push("Meeting Task Deadlines");
        }

    });
    return {
        values: values,
        labels: labels
    };
}

// Assisment Chart

//Doughnut-chart No of Admission per Campuses

//new Chart(document.getElementById("calendersection"), {
//    type: 'doughnut',
//    data: {
//        labels: ["Campus-A", "Campus-B", "Campus-C"],
//        datasets: [
//          {
//              label: "",
//              backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#e8c3b9"],
//              data: [365, 325, 360]
//          }
//        ]
//    },
//    options: {
//        title: {
//            display: true,
//            responsive: true,
//            maintainAspectRatio: false,
//            text: 'Number of Admissions in year 2017'
//        }
//    }
//});



//Doughnut-chart No of Admission per Campuses




// Revenue Chart Per Month and Campuses

//var ctx = document.getElementById('revenue-chart').getContext('2d');

//var myChart = new Chart(ctx, {
//    type: 'bar',
//    data: {
//        labels: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", ],
//        datasets: [
//          {
//              label: "Campus-A",
//              backgroundColor: "#33a02c",
//              data: [22000, 2400, 26700, 23500, 28000, 32000, 27600, 29800, 28000, 31000, 27600, 29800 ]
//          },
//          {
//              label: "Campus-B",
//              backgroundColor: "#dd4b39",
//              data: [21500, 23200, 25600, 27000, 28500, 29650, 24600, 27800, 28500, 27480, 24800, 23900]
//          },

//          {
//              label: "Campus-C",
//              backgroundColor: "#00c0ef",
//              data: [24050, 26800, 24600, 29200, 27200, 24150, 23900, 25500, 24600, 23900, 28700, 26500]
//          },
//        ]
//    },

//    options: {
//        title: {
//            display: true,
//            responsive: true,

//            text: 'Revenue Stats by Campuses'
//        }
//    }

//});

// Revenue Chart Per Month and Campuses





//var chart = document.getElementById("otherskill-chart");
//if(chart!=null){
//    new Chart(chart, {
//        type: 'pie',
//        data: {
//            labels: ["Sports Comp", "Drawing Comp", "Spelling Comp", "Writing Comp", "Debate Comp"],
//            datasets: [
//              {
//                  label: "",
//                  backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#e8c3b9", "#a22615"],
//                  data: [55, 80, 30, 70, 40]
//              }
//            ]
//        },
//        options: {
//            title: {
//                display: true,
//                responsive: true,
//                maintainAspectRatio: false,
//                //text: 'Number of Students in a Campus'
//            }
//        }
//    });
//}

//$('#datepicker1').datepicker({
//    changeMonth: true,
//    changeYear: true,
//    showButtonPanel: true,
//    dateFormat: 'MM yy',
//    onClose: function (dateText, inst) {
//        var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
//        var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
//        $(this).datepicker('setDate', new Date(year, month, 1));
//    }
//}); 
