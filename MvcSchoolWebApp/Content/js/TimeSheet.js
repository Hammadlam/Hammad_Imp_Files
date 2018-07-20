//form type 01 for ESS
//form type 02 for MSS

var formtype;
var longitude;
var latitude;

$(document).ready(function (e) {

    getLocation();

    $("#txtempnameTS").change(function () {
        getemployeeattendancehistory();
        isactiveemployee();
        filluserinformation();
    });

    getemployeeattendancehistory();

    $("#esssubmitattendanceform").submit(function (e) {
        e.preventDefault();
        getLocation();
        confirm_dialogue_attd();
    });

    $("#msssubmitattendanceform").submit(function (e) {
        e.preventDefault();
        getLocation();
        confirm_dialogue_attd();
    });

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
            show_alert_js();
            $("#empattendance_div").css("display", "none");
        }
    });
}

function setformtype(type)
{
    formtype = type;
}

function getformtype()
{
    return formtype;
}

function insertempattendance() {
    waitingDialog.show('Please Wait: This May Take a While');
    $.ajax({
        url: encodeURI("../TM/insertattendancerecord"),
        data: {
            empid: $("#txtempnameTS > option:selected").attr("value"),
            clientid: $("#txtclientnameTS > option:selected").attr("value"),
            date: $("#txtdateTS").val(),
            time: $("#txttimeTS").val(),
            type: getformtype(),
            lattd: getLatitude(),
            lngtd: getLongtitude()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
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
                colNames: ['Employee Name', 'Date', 'Check In', 'Check Out', 'Client'],
                colModel: [
                    { name: 'employeename', index: 'employeename', width: $size * 20, resizable: false, align: 'left' },
                    { name: 'date', index: 'date', width: $size * 20, resizable: false, align: 'left' },
                    { name: 'checkintime', index: 'checkintime', width: $size * 15, resizable: false, align: 'left' },
                    { name: 'checkouttime', index: 'checkouttime', width: $size * 15, resizable: false, align: 'left' },
                    { name: 'cliendname', index: 'cliendname', width: $size * 30, resizable: false, align: 'left' }
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
            empid: $("#txtempnameTS > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data == true) {
                getclientid();
                $("#btntimeout").css("display", "none");
                $("#btntimein").css("display", "block");
            }
            else {
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
            empid: $("#txtempnameTS > option:selected").attr("value")
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
                colNames: ['Employee Name', 'Date', 'Check In', 'Check Out', 'Client'],
                colModel: [
                    { name: 'employeename', index: 'employeename', width: $size * 20, resizable: false, align: 'left' },
                    { name: 'date', index: 'date', width: $size * 20, resizable: false, align: 'left' },
                    { name: 'checkintime', index: 'checkintime', width: $size * 15, resizable: false, align: 'center' },
                    { name: 'checkouttime', index: 'checkouttime', width: $size * 15, resizable: false, align: 'center' },
                    { name: 'cliendname', index: 'cliendname', width: $size * 30, resizable: false, align: 'left' }
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
            show_alert_js();
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
        alert('Geo Location feature is not supported in this browser.');
    }
}

function showPosition(position) {
    longitude = position.coords.longitude;
    latitude = position.coords.latitude;
}

function getLatitude()
{
    return latitude;
}

function getLongtitude() {
    return longitude;
}