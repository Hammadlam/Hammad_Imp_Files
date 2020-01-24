
$("#frmcreatedailyrpt2").submit(function (e) {
   
    e.preventDefault();
    $.ajax({
        url: encodeURI("../TM/CreateDailyReport2"),
        data: {
            date: $("#DateTMR").val()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            window.location.href = '../TM/launch_Daily_report';
        },
        error: function (error) {
            show_err_alert_js('No Record Found');
            $("#attendanceViewTMR_div").css("display", "none");
        }
    });
});

