
$(document).ready(function () {




    // code to read selected table row cell data (values).
    $("#Applicant_table").on('click', '#Reject', function () {

        waitingDialog.show('Please Wait: This May Take a While');

        // get the current row
        var currentRow = $(this).closest("tr");

        var reqno = currentRow.find("td:eq(6)").text(); // get current row 2nd TD



        $.ajax({
            type: "POST",
            data: { reqno: reqno, act:'0'},
            url: encodeURI("../ESS/UpdateApl"),
            dataType: "json",
            success: function (data) {

                show_err_alert_js('Applicant has been rejected');

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

        var reqno = currentRow.find("td:eq(6)").text(); // get current row 2nd TD

        $.ajax({
            type: "POST",
            data: { reqno: reqno, act: '1' },
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

        colNames: ['Action', 'Name', 'Email', "Position", 'Location','Documents','Req No'],
        colModel: [
            { name: 'act', index: 'act', width: 150, sortable: false, resizable: true },

            { name: 'Name', index: 'Name', resizable: false, width: 200, resizable: true, },

            { name: 'email', index: 'email', resizable: false, width: 300, resizable: true },

            { name: 'postxt', index: 'postxt', resizable: false, width: 180, resizable: true },

            { name: 'location', index: 'location', resizable: false, width: 100, resizable: true, },

            { name: 'Documents', index: 'Documents', width: 100, sortable: false, resizable: true },

            { name: 'reqno', index: 'reqno', width: 110, sortable: false, resizable: true },



        ],

        gridComplete: function () {

            var ids = jQuery("#Applicant_table").getDataIDs();


            for (var i = 0; i < ids.length; i++) {
                var cl = ids[i];


                view = "<input id='Reject' style='height:20px;width:60px;' type='button' value='Reject'  class='btn btn-xs btn-danger'>";

                view1 = "<input id='Accepted' style='height:20px;width:60px;' type='button' value='Accepted'  class='btn btn-xs btn-success'>";

                cv = "<a href='../ESS/CvView'  target='_blank' class='btn btn-xs btn-warning' style='height:20px;width:75px;'>Resume</a>";

                jQuery("#Applicant_table").setRowData(ids[i], { act: view + '  ' + view1 })

                jQuery("#Applicant_table").setRowData(ids[i], { Documents: cv  })

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


