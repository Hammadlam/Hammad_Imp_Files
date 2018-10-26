
var mydata = [
{ institurename: "", date: "", degree: "", majors: "", percentage: "" },
{ institurename: "", date: "", degree: "", majors: "", percentage: "" },
{ institurename: "", date: "", degree: "", majors: "", percentage: "" },
{ institurename: "", date: "", degree: "", majors: " ", percentage: "" }

];

var $screensize = $("#content").width() - 42;
var $size = ($screensize / 100);
$('#educationid').click(function () {
    $("#education_table").jqGrid({
        styleUI: 'Bootstrap',
        height: 250,
        autoheight: true,
        autowidth: true,
        //shrinkToFit: true,
        rowNum: 10,
        colNames: ['Institute Name', 'Date', 'Degree', 'Majors', "Percentage/ GPA"],
        colModel: [
            { name: 'institurename', index: 'institurename', width: $size * 20, search: true, resizable: false, editable: true },
            { name: 'date', index: 'date', width: $size * 20, search: true, resizable: false, editable: true },
            { name: 'degree', index: 'degree', width: $size * 20, search: true, resizable: false, editable: true },
            { name: 'majors', index: 'majors', classes: "wrap", width: $size * 20, resizable: false, editable: true },
            { name: 'percentage', index: 'percentage', width: $size * 20, resizable: false, editable: true }
        ],
        data: mydata,
        datatype: "local",
        iconSet: "fontAwesome",
        loadonce: true,
        autoheight: true,
        autowidth: true,
        idPrefix: "g5_",
        sortname: "invdate",
        sortorder: "desc",
        threeStateSort: true,
        sortIconsBeforeText: true,
        headertitles: true,
        cellsubmit: 'clientArray',
        viewrecords: true,
        hidegrid: false,
        shrinkToFit: false,
        cellEdit: true,
        editable: true,
    });

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.education_jqGrid').width();
        $('#education_table').setGridWidth(width, false);
    });

});

/* Experience JQgrid */
$("#experienceid").click(function () {
    var $screensize = $("#content").width() - 42;
    var $size = ($screensize / 100);

    $("#experience_table").jqGrid({
        datatype: "local",
        loadonce: true,
        colNames: ['Action', 'Job Title', 'Company', 'From Date', 'To Date', 'Responsibility'],
        colModel: [
            { name: 'status', index: 'status', editable: false, width: $size * 10, align: 'center', resizable: false },
            { name: 'jobtitle', index: 'jobtitle', editable: false, width: $size * 20, align: 'center', resizable: false },
            { name: 'company', index: 'company', editable: false, width: $size * 15, align: 'center', resizable: false },
            { name: 'fdate', index: 'fdate', editable: false, width: $size * 15, align: 'center', resizable: false },
            { name: 'tdate', index: 'tdate', editable: false, width: $size * 20, align: 'center', resizable: false },
            { name: 'resp', index: 'resp', editable: false, width: $size * 20, resizable: false, classes: 'wrap' }
        ],
        gridComplete: function () {
            var ids = jQuery("#experience_table").getDataIDs();
            for (var i = 0; i < ids.length; i++) {
                var cl = ids[i];
                Delete = "<i style='margin-left:15%; color:#e60000;' title='Delete' class='fa fa-trash-o fa-lg' onclick='getDeleted_EXAPPForm()'></i>";
                jQuery("#experience_table").setRowData(ids[i], { status: Delete });
            }
        },
        styleUI: 'Bootstrap',
        mtype: 'GET',
        height: 250,
        autoheight: true,
        autowidth: true,
        //rowList: [10, 20, 30],
        viewrecords: true,
        sortorder: "desc",
        threeStateSort: true,
        sortIconsBeforeText: true,
        headertitles: true,
        viewrecords: true,
        hidegrid: false,
        shrinkToFit: false
    }).trigger('reloadGrid', [{ current: true }]);

    // Add selection
    $("#experience_table").setSelection(4, true);

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('#content').width();

        $('#experience_table').setGridWidth(width, false);
    });

});

//deleting row
function getDeleted_EXAPPForm() {
    var selRowId = $('#experience_table').jqGrid('getGridParam', 'selrow');
    var serialId = $('#experience_table').jqGrid('getCell', selRowId, 'jobtitle');
    $('#experience_table').delRowData(selRowId[0]);
}

$("#btnAddEXAPPForm").click(function () {
    var myData = [{
        "status": "<i style='margin-left:15%; color:#e60000;' title='Delete' class='fa fa-trash-o fa-lg' onclick='getDeleted_EXAPPForm()'></i>",
        "jobtitle": "" + $("#txtjobtitleEXAPP").val(),
        'company': "" + $("#txtcompEXAPP").val(),
        'fdate': "" + $("#txtfromdateEXAPP").val(),
        'tdate': "" + $("#txttodateEXAPP").val(),
        'resp': "" + $("#txtrespEXAPP").val()
    }];
    var rows = $("#experience_table").getDataIDs();
    var gridRows = $("#experience_table").jqGrid('getRowData');
    var matched = false;
    for (var i = 0; i < gridRows.length; i++) {
        if (gridRows[i].jobtitle == $("#txtjobtitleEXAPP").val()
            && gridRows[i].company == $("#txtcompEXAPP").val()
            && gridRows[i].fdate == $("txtfromdateEXAPP").val()
            && gridRows[i].tdate == $("txttodateEXAPP").val()) {
            matched = true;
        }
    }
    if (rows.length == [])
        rows.length = 0;
    if (matched == false) {
        $("#experience_table").jqGrid("addRowData", rows.length + 1, myData[0], "last");
        $("#experience_table").trigger("reloadGrid");
    }
    else {
        show_err_alert_js('Duplicate records!');
    }

});


/* Projects JQgrid */
$("#projectid").click(function () {
    var $screensize = $("#content").width() - 42;
    var $size = ($screensize / 100);

    $("#project_table").jqGrid({
        datatype: "local",
        loadonce: true,
        colNames: ['Action', 'Project Name', 'Project URL', 'Start Date', 'End Date', 'Project Description'],
        colModel: [
            { name: 'status', index: 'status', editable: false, width: $size * 10, align: 'center', resizable: false },
            { name: 'projname', index: 'projname', editable: false, width: $size * 20, align: 'center', resizable: false },
            { name: 'projurl', index: 'projurl', editable: false, width: $size * 15, align: 'center', resizable: false },
            { name: 'sdate', index: 'sdate', editable: false, width: $size * 15, align: 'center', resizable: false },
            { name: 'edate', index: 'edate', editable: false, width: $size * 20, align: 'center', resizable: false },
            { name: 'desc', index: 'desc', editable: false, width: $size * 20, resizable: false, classes: 'wrap' }
        ],
        gridComplete: function () {
            var ids = jQuery("#project_table").getDataIDs();
            for (var i = 0; i < ids.length; i++) {
                var cl = ids[i];
                Delete = "<i style='margin-left:15%; color:#e60000;' title='Delete' class='fa fa-trash-o fa-lg' onclick='getDeleted_PRAPPForm()'></i>";
                jQuery("#project_table").setRowData(ids[i], { status: Delete });
            }
        },
        styleUI: 'Bootstrap',
        mtype: 'GET',
        height: 250,
        autoheight: true,
        autowidth: true,
        viewrecords: true,
        sortorder: "desc",
        threeStateSort: true,
        sortIconsBeforeText: true,
        headertitles: true,
        viewrecords: true,
        hidegrid: false,
        shrinkToFit: false
    }).trigger('reloadGrid', [{ current: true }]);

    // Add selection
    $("#project_table").setSelection(4, true);

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('#content').width();

        $('#project_table').setGridWidth(width, false);
    });

});

//deleting row
function getDeleted_EXAPPForm() {
    var selRowId = $('#project_table').jqGrid('getGridParam', 'selrow');
    var serialId = $('#project_table').jqGrid('getCell', selRowId, 'projname');
    $('#project_table').delRowData(selRowId[0]);
}

$("#btnAddPRAPPForm").click(function () {
    var myData = [{
        "status": "<i style='margin-left:15%; color:#e60000;' title='Delete' class='fa fa-trash-o fa-lg' onclick='getDeleted_PRAPPForm()'></i>",
        "projname": "" + $("#txtprojnamePRAPP").val(),
        'projurl': "" + $("#txtprojurlPRAPP").val(),
        'sdate': "" + $("#txtfromdatePRAPP").val(),
        'edate': "" + $("#txttodatePRAPP").val(),
        'desc': "" + $("#txtdescPRAPP").val()
    }];
    var rows = $("#project_table").getDataIDs();
    var gridRows = $("#project_table").jqGrid('getRowData');
    var matched = false;
    for (var i = 0; i < gridRows.length; i++) {
        if (gridRows[i].projname == $("#txtprojnamePRAPP").val()
            && gridRows[i].projurl == $("#txtprojurlPRAPP").val()
            && gridRows[i].sdate == $("txtfromdatePRAPP").val()
            && gridRows[i].edate == $("txttodatePRAPP").val()) {
            matched = true;
        }
    }
    if (rows.length == [])
        rows.length = 0;
    if (matched == false) {
        $("#project_table").jqGrid("addRowData", rows.length + 1, myData[0], "last");
        $("#project_table").trigger("reloadGrid");
    }
    else {
        show_err_alert_js('Duplicate records!');
    }

});

var basicinfo = new Array();
var contactinfo = new Array();

$("#APPForm").submit(function (e) {
    e.preventDefault();
    waitingDialog.show('Please Wait: This May Take a While');

    basicInfo();
    contactInfo();

    $.ajax({
        type: "POST",
        url: encodeURI("../HR/insertAPPForm"),
        data: {
            basicinfo: basicinfo,
            contactinfo: contactinfo

        },
        success: function (data) {

            if (data == "Success") {
                waitingDialog.hide();
                show_suc_alert_js('Form Successfully Uploaded');
                clearFields();
            }
            else if (data == "Error") {
                waitingDialog.hide();
                show_err_alert_js('Found Some Error! Please Try Again');
            }
            else if (data == "Rights") {
                waitingDialog.hide();
                show_err_alert_js('You Do Not Have Rights');
            }
        },
        error: function (data) {
            waitingDialog.hide();
            show_err_alert_js('Error Occured!');
        },
        dataType: "json"
    });


});


function basicInfo() {
    basicinfo[0] = $("#txtempnameAPPForm").val();
    basicinfo[1] = $("#txtcnicAPPForm").val();
    basicinfo[2] = $("input[name='gender']:checked").val();
    basicinfo[3] = $("#martialstatAPPForm > option:selected").attr("value");
    basicinfo[4] = $("#txtdobAPPForm").val();
    basicinfo[5] = $("#txtpobAPPForm").val();
    basicinfo[6] = $("#txtfathernameAPPForm").val();
    basicinfo[7] = $("#txtfathercnicAPPForm").val();
}

function contactInfo() {
    contactinfo[0] = $("#txtstreet1APPForm").val();
    contactinfo[1] = $("#txtstreet2APPForm").val();
    contactinfo[2] = $("#txtcityAPPForm").val();
    contactinfo[3] = $("#txtctryAPPForm").val();
    contactinfo[4] = $("#txtemailAPPForm").val();
    contactinfo[5] = $("#txtphone1APPForm").val();
    contactinfo[6] = $("#txtphone2APPForm").val();
    contactinfo[7] = $("#txtphone3APPForm").val();
    contactinfo[8] = $("#txtlinkedinAPPForm").val();
    contactinfo[9] = $("#txtskypeAPPForm").val();
}

function clearFields() {
    $("#txtempnameAPPForm").val("");
    $("#txtcnicAPPForm").val("");
    $("#martialstatAPPForm > option:selected").attr("value");
    $("#txtdobAPPForm").val("");
    $("#txtpobAPPForm").val("");
    $("#txtfathernameAPPForm").val("");
    $("#txtfathercnicAPPForm").val("");
    $("#txtstreet1APPForm").val("");
    $("#txtstreet2APPForm").val("");
    $("#txtcityAPPForm").val("");
    $("#txtctryAPPForm").val("");
    $("#txtemailAPPForm").val("");
    $("#txtphone1APPForm").val("");
    $("#txtphone2APPForm").val("");
    $("#txtphone3APPForm").val("");
    $("#txtlinkedinAPPForm").val("");
    $("#txtskypeAPPForm").val("");
}