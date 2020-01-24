
$(document).ready(function () {

   


    // code to read selected table row cell data (values).
    $("#purchase_table").on('click', '#view', function () {
        // get the current row
        var currentRow = $(this).closest("tr");

        var ReportsId = currentRow.find("td:eq(1)").text(); // get current row 2nd TD


        window.location.href = "/Reports/FlexData?reportid=" + ReportsId

    });

    loaddata_purchase();
});

function loaddata_purchase() {
    $("#purchase_table").jqGrid({
        url: '/Reports/GetDataFromDB',
        datatype: 'json',
        mtype: 'POST',
        autowidth: true,
        //autoheight: true,
        height: 270,
        postData: { Moduleid: "r5pr" },
        serializeGridData: function (postData) {
        
            return JSON.stringify(postData);
        },

        ajaxGridOptions: { contentType: "application/json" },
        loadonce: true,

        colNames: ['Action', 'reportid', 'reportname'],
        colModel: [
            { name: 'act', index: 'act', width: 120, sortable: false, resizable: false },
            { name: 'reportid', index: 'Id', resizable: false, width: 550, editable: false, editoptions: { readonly: true, size: 10 } },
            { name: 'reportname', index: 'Name', resizable: false, width: 290, editable: true, size: 100 },

            //{ name: 'Gender', index: 'Gender', width: 160, formatter: 'select', editable: true, edittype: "select", editoptions: { value: "0:select;1:male;2:female" } },
            //{ name: 'IsClosed', index: 'IsClosed', width: 180, editable: true, edittype: "checkbox", editoptions: { value: "true:false", formatter: "checkbox" } },

        ],

        gridComplete: function () {

            var ids = jQuery("#purchase_table").getDataIDs();


            for (var i = 0; i < ids.length; i++) {
                var cl = ids[i];


                view = "<input id='view' style='height:20px;width:75px;' type='button' value='View'  class='btn btn-xs btn-danger'>";
         
                jQuery("#purchase_table").setRowData(ids[i], { act: view })
            }
        },




        pager: '#purchase_pager',
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
    $("#purchase_table").jqGrid('navGrid', '#purchase_pager',
        { edit: true, add: true, del: true, search: true },
        { height: 300, reloadAfterSubmit: true }
    );

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.purchase_jqGrid').width();
        $('#purchase_table').setGridWidth(width, false);
    });


    var selRowId = $('#purchase_table').jqGrid('getGridParam', 'selrow');
    var ReportsId = $('#purchase_table').jqGrid('getCell', selRowId, 'reportid');

}

function getSelectedRow_purchase() {

    var selRowId = $('#purchase_table').jqGrid('getGridParam', 'selrow');
    var ReportsId = $('#purchase_table').jqGrid('getCell', selRowId, 'reportid');

    if (ReportsId == undefined) {
        show_err_alert_js('Please Select a row First');
    }
    else {
        window.location.href = "/Reports/FlexData?reportid=" + ReportsId;
    }

} 


