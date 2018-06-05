// Revenue Chart by each Campuses
var ctx = document.getElementById('revenue-chart');
if (ctx != null) {
    var chart = ctx.getContext('2d')
    var myChart = new Chart(chart, {

        type: 'line',

        data: {
            labels: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],

            datasets: [
                {
                    label: 'Revenue by Campuses',
                    data: [24050, 26800, 24600, 29200, 27200, 24150, 23900, 25500, 24600, 23900, 28700, 26500],
                    //backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#a22615", "#ffcd34", "#0f5b78", "#f36f13", "#958c3d", "#3e95cd", "#8e5ea2", "#3cba9f", "#0f5b78"],
                    borderColor: "#0f5b78",
                    pointBackgroundColor: "#b40e1a",
                    pointBorderColor: "#b40e1a",
                    //backgroundColor:"black",
                    fill: false
                },


            ]
        },

    });
}
// Revenue Chart by each Campuses

//Bar Chart

var chart = new CanvasJS.Chart("barchartContainer", {
    animationEnabled: true,
    axisY: {
        title: "Current Strength",
        titleFontColor: "#4F81BC",
        lineColor: "#4F81BC",
        labelFontColor: "#4F81BC",
        tickColor: "#4F81BC"
    },
    axisY2: {
        title: "Current Capacity",
        titleFontColor: "#C0504E",
        lineColor: "#C0504E",
        labelFontColor: "#C0504E",
        tickColor: "#C0504E"
    },
    toolTip: {
        shared: true
    },
    legend: {
        cursor: "pointer",
        itemclick: toggleDataSeries
    },
    data: [{
        type: "column",
        name: "Current Strength",
        legendText: "Current Strength",
        showInLegend: true,
        dataPoints: [
			{ label: "Campus-1", y: 266.21 },
			{ label: "Campus-2", y: 302.25 },
			{ label: "Campus-3", y: 157.20 },
			{ label: "Campus-4", y: 148.77 },
			{ label: "Campus-5", y: 101.50 },
			{ label: "Campus-6", y: 97.8 }
        ]
    },
	{
	    type: "column",
	    name: "Current Capacity",
	    legendText: "Current Capacity",
	    axisYType: "secondary",
	    showInLegend: true,
	    dataPoints: [
			{ label: "Campus-1", y: 10.46 },
			{ label: "Campus-2", y: 2.27 },
			{ label: "Campus-3", y: 3.99 },
			{ label: "Campus-4", y: 4.45 },
			{ label: "Campus-5", y: 2.92 },
			{ label: "Campus-6", y: 3.1 }
	    ]
	}]
});
chart.render();

function toggleDataSeries(e) {
    if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
        e.dataSeries.visible = false;
    }
    else {
        e.dataSeries.visible = true;
    }
    chart.render();
}