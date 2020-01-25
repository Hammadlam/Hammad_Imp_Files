
$(document).ready(function () {




    // code to read selected table row cell data (values).
    $("#Applicant_table").on('click', '#Reject', function () {

        waitingDialog.show('Please Wait: This May Take a While');

        // get the current row
        var currentRow = $(this).closest("tr");

        var userid = currentRow.find("td:eq(2)").text(); // get current row 2nd TD



        $.ajax({
            type: "POST",
            data: { userid: userid,reject:'1'},
            url: encodeURI("../ESS/UpdateApl"),
            dataType: "json",
            success: function (data) {

                show_suc_alert_js('Applicant has been selected');

            },
            error: function (data) {

                show_err_alert_js('Error Occured!');
            },

        });



        waitingDialog.hide();
    });

    // code to read selected table row cell data (values).
    $("#Applicant_table").on('click', '#Accepted', function () {

        waitingDialog.show('Please Wait: This May Take a While');

        // get the current row
        var currentRow = $(this).closest("tr");

        var userid = currentRow.find("td:eq(2)").text(); // get current row 2nd TD

        $.ajax({
            type: "POST",
            data: { userid: userid, reject: '1' },
            url: encodeURI("../ESS/UpdateApl"),
            dataType: "json",
            success: function (data) {

                show_suc_alert_js('Applicant has been selected');

            },
            error: function (data) {

                show_err_alert_js('Error Occured!');
            },

        });




        waitingDialog.hide();

    });


    loaddata_purchase('0');
});

function loaddata_purchase(aplid) {
    $("#Applicant_table").jqGrid({
        url: '/ESS/GetAppl',
        datatype: 'json',
        mtype: 'POST',
        autowidth: true,
        //autoheight: true,
        height: 270,
        postData: { aplid: aplid },
        serializeGridData: function (postData) {

            return JSON.stringify(postData);
        },

        ajaxGridOptions: { contentType: "application/json" },
        loadonce: true,

        colNames: ['Action', 'Name', 'Email'],
        colModel: [
            { name: 'act', index: 'act', width: 220, sortable: false, resizable: true },
            { name: 'Name', index: 'Id', resizable: false, width: 550, resizable: true, },
            { name: 'userid', index: 'Name', resizable: false, width: 350, resizable: true },

            //{ name: 'Gender', index: 'Gender', width: 160, formatter: 'select', editable: true, edittype: "select", editoptions: { value: "0:select;1:male;2:female" } },
            //{ name: 'IsClosed', index: 'IsClosed', width: 180, editable: true, edittype: "checkbox", editoptions: { value: "true:false", formatter: "checkbox" } },

        ],

        gridComplete: function () {

            var ids = jQuery("#Applicant_table").getDataIDs();


            for (var i = 0; i < ids.length; i++) {
                var cl = ids[i];


                view = "<input id='Reject' style='height:20px;width:75px;' type='button' value='Reject'  class='btn btn-xs btn-danger'>";

                view1 = "<input id='Accepted' style='height:20px;width:75px;' type='button' value='Accepted'  class='btn btn-xs btn-success'>";

                jQuery("#Applicant_table").setRowData(ids[i], { act: view +'  '+ view1})

            }
        },




      
        rowNum: 10,
        rowList: [10, 20, 30],
        viewrecords: true,
        gridview: true,
        jsonReader: {
            page: function (obj) { return 1; },
            total: function (obj) { return 1; },
            //records: function (obj) { return obj.d.length; },
            root: function (obj) { return obj.d; },
            repeatitems: false,
            id: "0"
        },
        shrinkToFit: false

    });

    // Setup buttons
    $("#Applicant_table").jqGrid('navGrid', '#Applicant_pager',
        { edit: true, add: true, del: true, search: true },
        { height: 300, reloadAfterSubmit: true }
    );

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.purchase_jqGrid').width();
        $('#Applicant_table').setGridWidth(width, false);
    });


    var selRowId = $('#Applicant_table').jqGrid('getGridParam', 'selrow');
    var ReportsId = $('#Applicant_table').jqGrid('getCell', selRowId, 'reportid');

}

function getSelectedRow_purchase() {

    var selRowId = $('#Applicant_table').jqGrid('getGridParam', 'selrow');
    var ReportsId = $('#Applicant_table').jqGrid('getCell', selRowId, 'reportid');

    if (ReportsId == undefined) {
        show_err_alert_js('Please Select a row First');
    }
    else {
        window.location.href = "/Reports/FlexData?reportid=" + ReportsId;
    }

}


