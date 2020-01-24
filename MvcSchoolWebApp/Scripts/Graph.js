// Define data set for all charts
var myData;
var col_val = [];
// Default chart defined with type: 'line'
Chart.defaults.global.defaultFontFamily = "monospace";
var ctx = document.getElementById('myChart').getContext('2d');


function getRandomColor() {
    var o = Math.round, r = Math.random, s = 255;
    return 'rgb(' + o(r() * s) + ',' + o(r() * s) + ',' + o(r() * s) + ')';
}

function checkdata() {
    debugger
    $.ajax({
        url: '/Home/DisplayGraphAPIAsync',
        type: "POST",
        dataType: 'json',
        data: '',
        success: function (data) {
            debugger
            console.log(data);
            console.log(data.length);
            console.log(data[2].Vendername);
        },
        error: function (data) {

        }
    })
}

var myChart;

function showgraph(vendors, count) {
    

    var t = getAllUrlParams().gtype;
    if (t == 'polararea') {
        t = 'polarArea';
    }
    if (t == 'line' || t == 'radar') {
        myData = {
            labels: vendors,
            datasets: [
                {
                    label: "Inventory Count",
                    fill: true,
                    backgroundColor: getRandomColor(), //'rgb(190, 99, 255)',
                    borderColor: getRandomColor(), //'rgb(190, 99, 255)',
                    data: count,
                }]
        };
    }
    else {
        myData = {
            labels: vendors,
            datasets: [
                {
                    label: "Inventory Count",
                    fill: true,
                    backgroundColor: col_val, //'rgb(190, 99, 255)',
                    borderColor: col_val, //'rgb(190, 99, 255)',
                    data: count,
                }]
        };
    }

    if (myChart != null) {

        myChart.destroy();
    }

    var fsize = 15;

    if (t == "doughnut") {
        fsize = 20;
    }

    myChart = new Chart(ctx, {
        type: t,
        data: myData,
        options: {
            responsive: true,
            //maintainAspectRatio: false,
            tooltips: {
                titleFontSize: fsize,
                bodyFontSize: fsize
            },
            title: {
                display: true,
                text: 'Inventory Management'
            },
            scales: {
                yAxes: [{
                    display: true,
                    ticks: {
                        suggestedMin: 0,  // minimum will be 0, unless there is a lower value.
                        steps: 10,
                        stepValue: 5,
                        //max: 10
                        // OR //
                        //beginAtZero: true   // minimum value will be 0.
                    }
                }],
                xAxes: [{
                    ticks: {
                        autoSkip: true,
                        maxTicksLimit: 20,
                        maxRotation: 30
                    }
                }]
            }
        }
    });
}

function getGraphData() {

    var vendorname = [];
    var invcount = [];
    //var mid = getAllUrlParams().mid;
    //var mid = getAllUrlParams().mid;
    //console.log(getAllUrlParams().mid);

    $.ajax({
        type: "POST",
        url: "/Home/DisplayGraphAPIAsync",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ id: getAllUrlParams().mid }),
        dataType: "json",
        success: function (result, status) {
   
            for (var i = 0; i < result.length; i++) {

                vendorname.push(result[i].Vendername);

                invcount.push(result[i].count);

                col_val.push(getRandomColor());
            }

            showgraph(vendorname, invcount);
        },
        error: function (xhr, status, error) {
            debugger
            alert("Error in updating notification status");
        }
    });
}





// Function runs on chart type select update
//function updateChartType() {
//    // Since you can't update chart type directly in Charts JS you must destroy original chart and rebuild
//    myChart.destroy();
//    myChart = new Chart(ctx, {
//        type: document.getElementById("chartType").value,
//        data: myData,
//    });
//};

//function upChartType(value) {
//    // Since you can't update chart type directly in Charts JS you must destroy original chart and rebuild
//    myChart.destroy();
//    myChart = new Chart(ctx, {
//        type: value,
//        data: myData,
//    });
//};


$(document).ready(function () {
    
    getGraphData();
    //checkdata();

});

function getAllUrlParams(url) {

    // get query string from url (optional) or window
     var queryString = url ? url.split('?')[1] : window.location.search.slice(1);

    // we'll store the parameters here
     var obj = {};

    // if query string exists
    if (queryString) {

        // stuff after # is not part of query string, so get rid of it
        queryString = queryString.split('#')[0];

        // split our query string into its component parts
        var arr = queryString.split('&');

        for (var i = 0; i < arr.length; i++) {
            // separate the keys and the values
            var a = arr[i].split('=');

            // set parameter name and value (use 'true' if empty)
            var paramName = a[0];
            var paramValue = typeof (a[1]) === 'undefined' ? true : a[1];

            // (optional) keep case consistent
            paramName = paramName.toLowerCase();
            if (typeof paramValue === 'string') paramValue = paramValue.toLowerCase();

            // if the paramName ends with square brackets, e.g. colors[] or colors[2]
            if (paramName.match(/\[(\d+)?\]$/)) {

                // create key if it doesn't exist
                var key = paramName.replace(/\[(\d+)?\]/, '');
                if (!obj[key]) obj[key] = [];

                // if it's an indexed array e.g. colors[2]
                if (paramName.match(/\[\d+\]$/)) {
                    // get the index value and add the entry at the appropriate position
                    var index = /\[(\d+)\]/.exec(paramName)[1];
                    obj[key][index] = paramValue;
                } else {
                    // otherwise add the value to the end of the array
                    obj[key].push(paramValue);
                }
            } else {
                // we're dealing with a string
                if (!obj[paramName]) {
                    // if it doesn't exist, create property
                    obj[paramName] = paramValue;
                } else if (obj[paramName] && typeof obj[paramName] === 'string') {
                    // if property does exist and it's a string, convert it to an array
                    obj[paramName] = [obj[paramName]];
                    obj[paramName].push(paramValue);
                } else {
                    // otherwise add the property
                    obj[paramName].push(paramValue);
                }
            }
        }
    }

    return obj;
}

//function showgraph(val)
//{
//    alert(val);
//    upChartType(val)
//}

// Randomize data button function
//function randomizeData() {
//    let newDataBaby = dataBaby.map(x => Math.floor(Math.random() * 50));
//    let newMoreDataBaby = moreDataBaby.map(x => Math.floor(Math.random() * 40));
//    myData.datasets[0].data = newDataBaby
//    myData.datasets[1].data = newMoreDataBaby
//    myChart.update();
//    console.log(newDataBaby);
//};
