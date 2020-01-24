
$(document).ready(function () {




    // code to read selected table row cell data (values).
    $("#chart_table").on('click', '#view', function () {
        // get the current row
        var currentRow = $(this).closest("tr");

        var ReportsId = currentRow.find("td:eq(1)").text().trim(); // get current row 2nd TD


        window.location.href = "/home/graph?mid="+ReportsId+"&gtype=line"

    });

    loaddata_graph();
});

function loaddata_graph() {

    $("#chart_table").jqGrid({
        url: '/Reports/GetGraphDataFromDB',
        datatype: 'json',
        mtype: 'POST',
        autowidth: true,
        //autoheight: true,
        height: 270,
        postData: { Moduleid: "0000" },
        serializeGridData: function (postData) {

            return JSON.stringify(postData);
        },

        ajaxGridOptions: { contentType: "application/json" },
        loadonce: true,

        colNames: ['Action', 'graphid', 'graphname'],
        colModel: [
            { name: 'act', index: 'act', width: 120, sortable: false, resizable: false },
            { name: 'graphid', index: 'Id', resizable: false, width: 550, editable: false, editoptions: { readonly: true, size: 10 } },
            { name: 'graphname', index: 'Name', resizable: false, width: 290, editable: true, size: 100 },

       
        ],

        gridComplete: function () {

            var ids = jQuery("#chart_table").getDataIDs();


            for (var i = 0; i < ids.length; i++) {
                var cl = ids[i];


                view = "<input id='view' style='height:20px;width:75px;' type='button' value='View'  class='btn btn-xs btn-danger'>";

                jQuery("#chart_table").setRowData(ids[i], { act: view })
            }
        },




        pager: '#chart_pager',
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
    $("#chart_table").jqGrid('navGrid', '#chart_pager',
        { edit: true, add: true, del: true, search: true },
        { height: 300, reloadAfterSubmit: true }
    );

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.chart_jqGrid').width();
        $('#chart_table').setGridWidth(width, false);
    });


    var selRowId = $('#chart_table').jqGrid('getGridParam', 'selrow');
    var ReportsId = $('#chart_table').jqGrid('getCell', selRowId, 'reportid');

}

function getSelectedRow_purchase() {

    var selRowId = $('#chart_table').jqGrid('getGridParam', 'selrow');
    var ReportsId = $('#chart_table').jqGrid('getCell', selRowId, 'reportid');

    if (ReportsId == undefined) {
        show_err_alert_js('Please Select a row First');
    }
    else {
        window.location.href = "/Reports/FlexData?reportid=" + ReportsId;
    }

}


