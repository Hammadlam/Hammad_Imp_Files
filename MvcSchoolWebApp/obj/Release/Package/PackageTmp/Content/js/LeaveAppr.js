$(document).ready(function () {
    loadLeaveAppdata();
});



function loadLeaveAppdata() {
    var ctx = document.getElementById("leaveapp_table");
    if (ctx != null) {
        var $screensize = $("#leaveapp_div").width() - 42;
        var $size = ($screensize / 100);
        $("#leaveapp_table").jqGrid({
            url: '/MSS/GetLeaveAppr',
            postData: { reqtyp: "20" },
            datatype: 'json',
            mtype: 'POST',
            autowidth: true,
            //autoheight: true,
            height: 270,

            serializeGridData: function (postData) {
                return JSON.stringify(postData);
            },

            ajaxGridOptions: { contentType: "application/json" },
            loadonce: true,

            colNames: ['Action', 'Employee Id', 'Employee Name', 'Date', 'Leave Amount', 'Leave Status', 'Overall Status'],
            colModel: [
                { name: 'act', index: 'act', width: $size * 20, sortable: false, resizable: false },
                { name: 'empid', index: 'empid', resizable: false, width: $size * 15, editable: false, editoptions: { readonly: true, size: 10 }, sortable: true },
                { name: 'empname', index: 'empname', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true, align: 'bottam', },
                { name: 'dates', index: 'dates', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true },
                { name: 'lvdays', index: 'lvdays', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true },
                { name: 'leavestat', index: 'leavestat', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true },
                { name: 'overalstat', index: 'overalstat', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true }
            ],

            gridComplete: function () {
                var ids = jQuery("#leaveapp_table").getDataIDs();
                for (var i = 0; i < ids.length; i++) {
                    var cl = ids[i];
                    recomendleave = "<input style='height:18px;' type='button' value='Acceptance' onclick= 'getSelectedRow_recodleave();' class='btn btn-xs btn-danger'>";
                    apprleave = "<input style='height:18px;margin-left: 5px;' type='button' value='Approval' onclick= 'getSelectedRow_appleave();' class='btn btn-xs btn-danger'>";
                    jQuery("#leaveapp_table").setRowData(ids[i], { act: recomendleave + apprleave })
                }
            },

            pager: '#leaveapp_pager',
            rowNum: 10,
            rowList: [10, 20, 30],
            viewrecords: true,
            gridview: true,
            jsonReader: {
                page: function (obj) { return 1; },
                total: function (obj) { return 1; },
                root: function (obj) { return obj.d; },
                repeatitems: false,
                id: "0"
            },
            shrinkToFit: false

        });

        // Setup buttons
        $("#leaveapp_table").jqGrid('navGrid', '#leaveapp_pager',
            { edit: true, add: true, del: true, search: true },
            { height: 300, reloadAfterSubmit: true }
        );

        // Add responsive to jqGrid
        $(window).bind('resize', function () {
            var width = $('.leaveapp_jqGrid').width();
            $('#leaveapp_table').setGridWidth(width, false);
        });
    }
}


function getSelectedRow_recodleave() {
    var selRowId = $('#leaveapp_table').jqGrid('getGridParam', 'selrow');
    var empId = $('#leaveapp_table').jqGrid('getCell', selRowId, 'empid');
    var leavestat = $('#leaveapp_table').jqGrid('getCell', selRowId, 'leavestat');
    if (leavestat == "Pending") {
        if (empId == undefined) {
            show_err_alert_js('Please Select a row First');
        }
        else {

            $.ajax({
                type: "POST",
                url: encodeURI("../MSS/getleaveDetails"),
                data: { empId: empId },
                success: function (data) {
                    fillRecLeavepopup(data);

                },
                error: function (data) {
                    show_err_alert_js('Found some error! Please try again');
                }
            });
        }
    }
}


function getSelectedRow_appleave() {
    var selRowId = $('#leaveapp_table').jqGrid('getGridParam', 'selrow');
    var empId = $('#leaveapp_table').jqGrid('getCell', selRowId, 'empid');
    var leavestat = $('#leaveapp_table').jqGrid('getCell', selRowId, 'leavestat');
    if (leavestat == "Processing") {
        if (empId == undefined) {
            show_err_alert_js('Please Select a row First');
        }
        else {
            $.ajax({
                type: "POST",
                url: encodeURI("../MSS/getApprRecodType"),
                data: { empId: empId, reqtype: "20" },
                success: function (data) {
                    if (data.length != 0) {
                        var items = [];
                        items.push("<option value =''>Recommend</option>");
                        $.each(data, function () {
                            items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                        }); 
                        $("#txtApprRecomendLvA").html(items.join(' '));
                    }
                    else {
                        $("#apprcomboLvA").hide();
                        $("#btnleavecsubmit").hide();
                    }
                },
                error: function (data) {
                    show_err_alert_js('Found some error! Please try again');
                }
            });
            $.ajax({
                type: "POST",
                url: encodeURI("../MSS/getleaveDetails"),
                data: { empId: empId },
                success: function (data) {
                    fillleaveLeavepopup(data);

                },
                error: function (data) {
                    show_err_alert_js('Found some error! Please try again');
                }
            });
        }
    }
}

function fillRecLeavepopup(data) {
    if (data.length != 0) {
        $('#txtrecEmpidLvA').val(data[0].empid);
        $('#txtrecloan0reqnoLvA').val(data[0].recordno);
        $('#txtrecEmpnameLvA').val(data[0].empname);
        $('#txtrecloanjoinLvA').val(data[0].joindate);
        $('#txtrecloandesigLvA').val(data[0].design);
        $('#txtLoantypRecodLvA').val(data[0].loanid);
        $('#txtrecloanAmtLvA').val(data[0].totdays);
        $('#txtrecloanappbdateLvA').val(data[0].begdate);
        $('#txtrecloanappedateLvA').val(data[0].lastdate);
        var items = [];
        items.push("<option value=" + data[0].loantyp[0].Value + ">" + data[0].loantyp[0].Text + "</option>");
        $("#txtLoantypRecodLvA").html(items.join(' '));
        $('#LeaveRecodPopup').modal('show');
    }
    else
        show_err_alert_js('Request already done!');
}
function fillleaveLeavepopup(data) {
    if (data.length != 0) {
        $('#txtleaveEmpid').val(data[0].empid);
        $('#txtleaveleavereqno').val(data[0].recordno);
        $('#txtleaveEmpname').val(data[0].empname);
        $('#txtleaveleavejoin').val(data[0].joindate);
        $('#txtleaveleavedesig').val(data[0].design);
        //$('#txtleaveleavetypAppr').val(data[0].loanid);
        $('#txtleaveleaveday').val(data[0].totdays);
        $('#txtleaveleaveappbdate').val(data[0].begdate);
        $('#txtleaveleaveappedate').val(data[0].lastdate);
        var items = [];
        items.push("<option value=" + data[0].loantyp[0].Value + ">" + data[0].loantyp[0].Text + "</option>");
        $("#txtleaveleavetypAppr").html(items.join(' '));
        $('#LeaveAppPopup').modal('show');
    }
    else
        show_err_alert_js('Request already done!');
}
//Recommend Leave Reject
$("#btnrecodRecrejLvA").click(function () {

    $.ajax({
        type: "POST",
        url: encodeURI("../MSS/rejectRecomendleave"),
        data: {
            empid: $('#txtrecEmpidLvA').val(),
            reqno: $('#txtrecloanreqnoLvA').val(),
            subpagtype: $("#txtLoantypRecodLvA > option:selected").attr("value"),
            comment: $('#txtRecodCommentsLvA').val(),
            totdays: $('#txtrecloanAmtLvA').val()
        },
        success: function (data) {
            if (data == "Success") {
                $('#LeaveRecodPopup').modal('hide');
                $('#leaveapp_table').trigger('reloadGrid');
                show_suc_alert_js('Successfully leave rejected!');
                window.location.href = '/MSS/leaveApproval';
            }
        },
        error: function (data) {
            show_err_alert_js('Found some error! Please try again');
        }
    });

});


//Recommended Loan Accepted
$("#btnleaveAccpt").click(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../MSS/acceptLeaveByRecomder"),
        data: {
            empid: $('#txtleaveEmpid').val(),
            reqno: $('#txtleaveleavereqno').val(),
            subpagtype: $("#txtleaveleavetypAppr > option:selected").attr("value"),
            startdate: $('#txtleaveleaveappbdate').val(),
            enddate: $('#txtleaveleaveappedate').val(),
            apprrecodtypid: $("#txtApprRecomendLvA > option:selected").attr("value"),
            comment: $('#txtleaveRecodComments').val(),
            reqtype: "20"
        },
        success: function (data) {
            if (data == "Success") {
                $('#LeaveAppPopup').modal('hide');
                $('#leaveapp_table').trigger('reloadGrid');
                show_suc_alert_js('Successfully leave accepted!');
                window.location.href = '/MSS/leaveApproval';
            }

        },
        error: function (data) {
            show_err_alert_js('Found some error! Please try again');
        }
    });
});

//Approver Loan Reject
$("#btnleaveRecrej").click(function () {

    $.ajax({
        type: "POST",
        url: encodeURI("../MSS/rejectApprleave"),
        data: {
            empid: $('#txtleaveEmpid').val(),
            reqno: $('#txtleaveleavereqno').val(),
            subpagtype: $("#txtleaveleavetypAppr > option:selected").attr("value"),
            comment: $('#txtleaveRecodComments').val(),
            lvdays: $('#txtleaveleaveday').val()
        },
        success: function (data) {
            if (data == "Success") {
                $('#LeaveAppPopup').modal('hide');
                $('#leaveapp_table').trigger('reloadGrid');
                show_suc_alert_js('Successfully leave rejected!');
                window.location.href = '/MSS/leave Approval';
            }
        },
        error: function (data) {
            show_err_alert_js('Found some error! Please try again');
        }
    });
});
