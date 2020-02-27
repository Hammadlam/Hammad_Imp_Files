//form type 01 for ESS
//form type 02 for MSS

var formtype;
var longitude;
var latitude;
var datetxt = "";
$(document).ready(function (e) {

    tryGeolocation();
    //getLocation();

    $("#txtempnameTS").change(function () {
        getemployeeattendancehistory();
        isactiveemployee();
        filluserinformation();
    });

        $("#txtdateTS").datepicker({
            format: 'dd-MM-yyyy',
            autoclose: true,
            onSelect: function (dateText) {
            }
        }).on("change", function () {
            datetxt = $("#txtdateTS").val();
            getemployeeattendancehistory();
            isactiveemployee();
            filluserinformation();
        });

    getemployeeattendancehistory();

    $("#esssubmitattendanceform").submit(function (e) {
        e.preventDefault();
        tryGeolocation();
        //getLocation();
        confirm_dialogue_attd();
    });

    $("#msssubmitattendanceform").submit(function (e) {
        e.preventDefault();
        tryGeolocation();
        //getLocation();
        confirm_dialogue_attd();
    });

    //ess confrm attandance Button
    $("#essempattd_dialogue_frm").submit(function (e) {
        e.preventDefault();
        $('#empattd_dialogue').modal('hide');
        setformtype("01");
        insertempattendance();
    });

    $("#mssempattd_dialogue_frm").submit(function (e) {
        e.preventDefault();
        $('#empattd_dialogue').modal('hide');
        setformtype("02");
        insertempattendance();
    });
});

function filluserinformation() {
    $.ajax({
        url: encodeURI("../TM/filluserinformation"),
        data: {
            empid: $("#txtempnameTS > option:selected").attr("value"),
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#txtdesigTS").val(data[0].design);
            $("#txtdepartTS").val(data[0].dept);
        },
        error: function (error) {
            waitingDialog.hide();
            $('#empattd_dialogue').modal('hide');
            //show_alert_js();
            $("#empattendance_div").css("display", "none");
        }
    });
}

function setformtype(type) {
    formtype = type;
}

function getformtype() {
    return formtype;
}

function insertempattendance() {
    $("#btntimein").attr("disabled", "disabled");
    $("#btntimeout").attr("disabled", "disabled");
    waitingDialog.show('Please Wait: This May Take a While');
    $.ajax({
        url: encodeURI("../TM/insertattendancerecord"),
        data: {
            empid: $("#txtempnameTS > option:selected").attr("value"),
            clientid: $("#txtclientnameTS > option:selected").attr("value"),
            date: $("#txtdateTS").val(),
            time: $("#txttimeTS").val(),
            type: getformtype(),
            remarks: $("#txtremarksTS").val(),
            lattd: getLatitude(),
            lngtd: getLongtitude()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data == false) {

                waitingDialog.hide();

                $("#btntimeout").removeAttr("disabled");
                $("#btntimein").removeAttr("disabled");
                show_err_alert_js("Location must be required to perform action");
                return;
            }
            $("#txtremarksTS").val("");
            isactiveemployee();
            waitingDialog.hide();
            $('#empattendance_table').jqGrid('clearGridData');
            $("#empattendance_table").jqGrid("GridUnload");
            $('#empattendance_table').jqGrid('setGridParam', { data: data });
            $("#empattendance_div").show();
            $("#empattendance_table").jqGrid('setGridState', 'visible');

            var $screensize = $("#empattendance_div").width() - 42;
            var $size = ($screensize / 100);

            $("#empattendance_table").jqGrid({
                datatype: "local",
                colNames: ['Employee Name', 'Date', 'Check In', 'Check Out', 'Client', 'Remarks (Time in)', 'Remarks (Time out)'],
                colModel: [
                    { name: 'employeename', index: 'employeename', width: $size * 20, resizable: false, align: 'left' },
                    { name: 'date', index: 'date', width: $size * 20, resizable: false, align: 'left' },
                    { name: 'checkintime', index: 'checkintime', width: $size * 15, resizable: false, align: 'left' },
                    { name: 'checkouttime', index: 'checkouttime', width: $size * 15, resizable: false, align: 'left' },
                    { name: 'client', index: 'client', width: $size * 30, resizable: false, align: 'left' },
                    { name: 'remarks', index: 'remarks', width: $size * 30, resizable: true, align: 'left' },
                    { name: 'remarkstout', index: 'remarkstout', width: $size * 30, resizable: true, align: 'left' }
                ],
                data: data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                loadonce: true,
                viewrecords: true,
                rowNum: data[0].count,
                iconSet: "fontAwesome",
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                cellEdit: true,
                editable: true,
                caption: "Attendance Sheet",
                add: true,
                edit: true,
                addtext: 'Add',
                edittext: 'Edit',
                hidegrid: false,
                shrinkToFit: false,
                cellsubmit: 'clientArray'

            }).trigger('reloadGrid', [{ current: true }]);
            // close_progress();
        },
        error: function (error) {
            $("#btntimeout").removeAttr("disabled");
            $("#btntimein").removeAttr("disabled");
            waitingDialog.hide();
            show_err_alert_js("Found Some Error");
        }
    });
}

function confirm_dialogue_attd() {
    $('.summernote').summernote();
    $('#empattd_dialogue').modal('show');
}

function isactiveemployee() {
    $.ajax({
        url: encodeURI("../TM/isactiveemployee"),
        data: {
            empid: $("#txtempnameTS > option:selected").attr("value"),
            date: $("#txtdateTS").val()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data == true) {
                getclientid();
                $("#btntimeout").css("display", "none");
                $("#btntimein").css("display", "block");
                $("#btntimein").removeAttr("disabled");
            }
            else {
                $("#btntimeout").removeAttr("disabled");
                $("#txtclientnameTS").removeAttr("disabled");
                $("#btntimein").css("display", "none");
                $("#btntimeout").css("display", "block");
            }
        },
        error: function (error) {
        }
    });
}

function getclientid() {
    //get clientid and disable the combo
    $("#txtclientnameTS").attr("disabled", "disabled");
    $.ajax({
        url: encodeURI("../TM/getclientid"),
        data: {
            empid: $("#txtempnameTS > option:selected").attr("value"),
            dateId: datetxt
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#txtclientnameTS").val(data);
        },
        error: function (error) {
        }
    });
}

function getemployeeattendancehistory() {
    $.ajax({
        url: encodeURI("../TM/getempattdlog"),
        data: {
            empid: $("#txtempnameTS > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $('#empattendance_table').jqGrid('clearGridData');
            $("#empattendance_table").jqGrid("GridUnload");
            $('#empattendance_table').jqGrid('setGridParam', { data: data });
            $("#empattendance_div").show();
            $("#empattendance_table").jqGrid('setGridState', 'visible');

            var $screensize = $("#empattendance_div").width() - 42;
            var $size = ($screensize / 100);

            $("#empattendance_table").jqGrid({
                datatype: "local",
                colNames: ['Employee Name', 'Date', 'Check In', 'Check Out', 'Client', 'Remarks (Time in)', 'Remarks (Time out)'],
                colModel: [
                    { name: 'employeename', index: 'employeename', width: $size * 20, resizable: false, align: 'left' },
                    { name: 'date', index: 'date', width: $size * 20, resizable: false, align: 'left' },
                    { name: 'checkintime', index: 'checkintime', width: $size * 15, resizable: false, align: 'center' },
                    { name: 'checkouttime', index: 'checkouttime', width: $size * 15, resizable: false, align: 'center' },
                    { name: 'client', index: 'client', width: $size * 30, resizable: false, align: 'left' },
                    { name: 'remarks', index: 'remarks', width: $size * 30, resizable: true, align: 'left' },
                    { name: 'remarkstout', index: 'remarkstout', width: $size * 30, resizable: true, align: 'left' }
                ],
                data: data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                loadonce: true,
                viewrecords: true,
                rowNum: data.count,
                iconSet: "fontAwesome",
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                cellEdit: true,
                editable: true,
                caption: "Attendance Sheet",
                add: true,
                edit: true,
                addtext: 'Add',
                edittext: 'Edit',
                hidegrid: false,
                shrinkToFit: false,
                cellsubmit: 'clientArray'

            }).trigger('reloadGrid', [{ current: true }]);
            // close_progress();
        },
        error: function (error) {
            //show_alert_js();
            $("#empattendance_div").css("display", "none");
        }
    });
}



function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (p) {
            longitude = p.coords.longitude;
            latitude = p.coords.latitude;
        });
    } else {
        //alert('Geo Location feature is not supported in this browser.');
    }
}

function showPosition(position) {
    longitude = position.coords.longitude;
    latitude = position.coords.latitude;
}

function getLatitude() {
    return latitude;
}

function getLongtitude() {
    return longitude;
}




var apiGeolocationSuccess = function (position) {
    //alert("API geolocation success!\n\nlat = " + position.coords.latitude + "\nlng = " + position.coords.longitude);
};

var tryAPIGeolocation = function () {
    jQuery.post("http://www.googleapis.com/geolocation/v1/geolocate?key=AIzaSyDCa1LUe1vOczX1hO_iGYgyo8p_jYuGOPU", function (success) {
        apiGeolocationSuccess({ coords: { latitude: success.location.lat, longitude: success.location.lng } });
    })
  .fail(function (err) {
      //alert("API Geolocation error! \n\n" + err);
  });
};

var browserGeolocationSuccess = function (position) {
    longitude = position.coords.longitude;
    latitude = position.coords.latitude;
    //alert("Browser geolocation success!\n\nlat = " + position.coords.latitude + "\nlng = " + position.coords.longitude);
};

var browserGeolocationFail = function (error) {
    switch (error.code) {
        case error.TIMEOUT:
            //alert("Browser geolocation error !\n\nTimeout.");
            break;
        case error.PERMISSION_DENIED:
            if (error.message.indexOf("Only secure origins are allowed") == 0) {
                tryAPIGeolocation();
            }
            break;
        case error.POSITION_UNAVAILABLE:
            //alert("Browser geolocation error !\n\nPosition unavailable.");
            break;
    }
};

var tryGeolocation = function () {

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            browserGeolocationSuccess,
          browserGeolocationFail,
          { maximumAge: 50000, timeout: 20000, enableHighAccuracy: true });
    }
};

