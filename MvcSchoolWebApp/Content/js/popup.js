function show_alert() {
    $('#myModal2').fadeIn("toggle").modal('toggle');
    $('#myModal2').css("z-index",3000);
    //setInterval(inactive, 5000)
}

function inactive() {
    var wrapper = document.getElementById('wrap');
    wrapper.style.opacity = 1;
    wrapper.style.background = 'none';
    var model = document.getElementById('demo-popup');
    model.style.visibility = 'hidden';

}

function show_alert_js(msg) {
    $('#myModal3').fadeIn("toggle").modal('toggle');
    $('#myModal3').css("z-index", 3000);
    $('#error-msg').html(msg);
    //setInterval(inactive, 5000)
}

function show_err_alert_js(msg) {
    $('#myModal3').fadeIn("toggle").modal('toggle');
    $('#myModal3').css("z-index", 3000);
    $('#error-msg').html(msg);
    //setInterval(inactive, 5000)
}

function show_suc_alert_js(msg) {
    $('#myModal4').fadeIn("toggle").modal('toggle');
    $('#myModal4').css("z-index", 3000);
    $('#suc-msg').html(msg);
    //setInterval(inactive, 5000)
}

function close_progress() {
    var progressBarContainer = $('#progress-bar');
    progressBarContainer.hide();
}
