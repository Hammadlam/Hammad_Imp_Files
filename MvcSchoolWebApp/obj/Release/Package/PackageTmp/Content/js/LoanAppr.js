$(document).ready(function () {
    loadLoanAppdata();
});



function loadLoanAppdata() {
    var ctx = document.getElementById("loanapp_table");
    if (ctx != null) {
        var $screensize = $("#loanapp_div").width() - 42;
        var $size = ($screensize / 100);
        $("#loanapp_table").jqGrid({
            url: '/MSS/GetLoanAppr',
            postData: { reqtyp: "10" },
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

            colNames: ['Action', 'Employee Id', 'Employee Name', 'Date', 'Loan Amount', 'Loan Status', 'Overall Status'],
            colModel: [
                { name: 'act', index: 'act', width: $size * 20, sortable: false, resizable: false },
                { name: 'empid', index: 'empid', resizable: false, width: $size * 15, editable: false, editoptions: { readonly: true, size: 10 }, sortable: true },
                { name: 'empname', index: 'empname', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true, align: 'bottam', },
                { name: 'dates', index: 'dates', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true },
                { name: 'loanamt', index: 'loanamt', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true },
                { name: 'loanstat', index: 'loanstat', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true },
                { name: 'overalstat', index: 'overalstat', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true }
            ],

            gridComplete: function () {
                var ids = jQuery("#loanapp_table").getDataIDs();
                for (var i = 0; i < ids.length; i++) {
                    var cl = ids[i];
                    recomendloan = "<input style='height:18px;' type='button' value='Acceptance' onclick= 'getSelectedRow_recodloan();' class='btn btn-xs btn-danger'>";
                    apprloan = "<input style='height:18px;margin-left: 5px;' type='button' value='Approval' onclick= 'getSelectedRow_apploan();' class='btn btn-xs btn-danger'>";
                    jQuery("#loanapp_table").setRowData(ids[i], { act: recomendloan + apprloan })
                }
            },

            pager: '#loanapp_pager',
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
        $("#loanapp_table").jqGrid('navGrid', '#loanapp_pager',
            { edit: true, add: true, del: true, search: true },
            { height: 300, reloadAfterSubmit: true }
        );

        // Add responsive to jqGrid
        $(window).bind('resize', function () {
            var width = $('.loanapp_jqGrid').width();
            $('#loanapp_table').setGridWidth(width, false);
        });
    }
}


function getSelectedRow_recodloan() {
    var selRowId = $('#loanapp_table').jqGrid('getGridParam', 'selrow');
    var empId = $('#loanapp_table').jqGrid('getCell', selRowId, 'empid');
    var loanstat = $('#loanapp_table').jqGrid('getCell', selRowId, 'loanstat');
    if (loanstat == "Pending") {
        if (empId == undefined) {
            show_err_alert_js('Please Select a row First');
        }
        else {

            $.ajax({
                type: "POST",
                url: encodeURI("../MSS/getloanDetails"),
                data: { empId: empId },
                success: function (data) {
                    fillRecLoanpopup(data);

                },
                error: function (data) {
                    show_err_alert_js('Found some error! Please try again');
                }
            });
        }
    }
}


function getSelectedRow_apploan() {
    var selRowId = $('#loanapp_table').jqGrid('getGridParam', 'selrow');
    var empId = $('#loanapp_table').jqGrid('getCell', selRowId, 'empid');
    var loanstat = $('#loanapp_table').jqGrid('getCell', selRowId, 'loanstat');
    if (loanstat == "Processing") {
        if (empId == undefined) {
            show_err_alert_js('Please Select a row First');
        }
        else {
            $.ajax({
                type: "POST",
                url: encodeURI("../MSS/getApprRecodType"),
                data: { empId: empId, reqtype: "10" },
                success: function (data) {
                    if (data.length != 0) {
                        var items = [];
                        items.push("<option value =''>Recommend</option>");
                        $.each(data, function () {
                            items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                        });
                        $("#txtApprRecomend").html(items.join(' '));
                    }
                    else {
                        $("#apprcombo").hide();
                        $("#btnloancsubmit").hide();
                    }
                },
                error: function (data) {
                    show_err_alert_js('Found some error! Please try again');
                }
            });
            $.ajax({
                type: "POST",
                url: encodeURI("../MSS/getloanDetails"),
                data: { empId: empId },
                success: function (data) {
                    fillloanLoanpopup(data);

                },
                error: function (data) {
                    show_err_alert_js('Found some error! Please try again');
                }
            });
        }
    }
}

function fillRecLoanpopup(data) {
    if (data.length != 0) {
        $('#txtrecEmpid').val(data[0].empid);
        $('#txtrecloanreqno').val(data[0].recordno);
        $('#txtrecEmpname').val(data[0].empname);
        $('#txtrecloanjoin').val(data[0].joindate);
        $('#txtrecloandesig').val(data[0].design);
        $('#txtrecLoantyp').val(data[0].loanid);
        $('#txtrecloanAmt').val(data[0].loanamt);
        $('#txtrecloanappbdate').val(data[0].begdate);
        $('#txtrecloanappedate').val(data[0].lastdate);
        var items = [];
        items.push("<option value=" + data[0].loantyp[0].Value + ">" + data[0].loantyp[0].Text + "</option>");
        $("#txtLoantypRecod").html(items.join(' '));
        $('#LoanRecodPopup').modal('show');
    }
    else
        show_err_alert_js('Request already done!');
}
function fillloanLoanpopup(data) {
    if (data.length != 0) {
        $('#txtloanEmpid').val(data[0].empid);
        $('#txtloanloanreqno').val(data[0].recordno);
        $('#txtloanEmpname').val(data[0].empname);
        $('#txtloanloanjoin').val(data[0].joindate);
        $('#txtloanloandesig').val(data[0].design);
        $('#txtloanLoantyp').val(data[0].loanid);
        $('#txtloanloanAmt').val(data[0].loanamt);
        $('#txtloanloanappbdate').val(data[0].begdate);
        $('#txtloanloanappedate').val(data[0].lastdate);
        var items = [];
        items.push("<option value=" + data[0].loantyp[0].Value + ">" + data[0].loantyp[0].Text + "</option>");
        $("#txtloanLoantypAppr").html(items.join(' '));
        $('#LoanAppPopup').modal('show');
    }
    else
        show_err_alert_js('Request already done!');
}
//Recommend Loan Reject
$("#btnrecodRecrej").click(function () {

    $.ajax({
        type: "POST",
        url: encodeURI("../MSS/rejectRecomendloan"),
        data: {
            empid: $('#txtrecEmpid').val(),
            reqno: $('#txtrecloanreqno').val(),
            subpagtype: $("#txtLoantypRecod > option:selected").attr("value"),
            comment: $('#txtRecodComments').val(),
            loanamt: $('#txtrecloanAmt').val()
        },
        success: function (data) {
            if (data == "Success") {
                $('#LoanRecodPopup').modal('hide');
                $('#loanapp_table').trigger('reloadGrid');
                show_suc_alert_js('Successfully loan rejected!');
                window.location.href = '/MSS/loanApproval';
            }
        },
        error: function (data) {
            show_err_alert_js('Found some error! Please try again');
        }
    });

});


//Recommended Loan Accepted
$("#btnloanAccpt").click(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../MSS/acceptLoanByRecomder"),
        data: {
            empid: $('#txtloanEmpid').val(),
            reqno: $('#txtloanloanreqno').val(),
            subpagtype: $("#txtloanLoantypAppr > option:selected").attr("value"),
            startdate: $('#txtloanloanappbdate').val(),
            enddate: $('#txtloanloanappedate').val(),
            apprrecodtypid: $("#txtApprRecomend > option:selected").attr("value"),
            comment: $('#txtloanRecodComments').val(),
            reqtype: "10"
        },
        success: function (data) {
            if (data == "Success") {
                $('#LoanAppPopup').modal('hide');
                $('#loanapp_table').trigger('reloadGrid');
                show_suc_alert_js('Successfully loan accepted!');
                window.location.href = '/MSS/loanApproval';
            }

        },
        error: function (data) {
            show_err_alert_js('Found some error! Please try again');
        }
    });
});

//Approver Loan Reject
$("#btnloanRecrej").click(function () {

    $.ajax({
        type: "POST",
        url: encodeURI("../MSS/rejectApprloan"),
        data: {
            empid: $('#txtloanEmpid').val(),
            reqno: $('#txtloanloanreqno').val(),
            subpagtype: $("#txtloanLoantypAppr > option:selected").attr("value"),
            comment: $('#txtloanRecodComments').val(),
            loanamt: $('#txtloanloanAmt').val()
        },
        success: function (data) {
            if (data == "Success") {
                $('#LoanAppPopup').modal('hide');
                $('#loanapp_table').trigger('reloadGrid');
                show_suc_alert_js('Successfully loan rejected!');
                window.location.href = '/MSS/loanApproval';
            }
        },
        error: function (data) {
            show_err_alert_js('Found some error! Please try again');
        }
    });
});
