$(document).ready(function () {
    loaddata();
});

function loaddata() {

        $("#hr_table").jqGrid({
            url: '/Reports/GetDataFromDB',
            data: { Moduleid: "r9hr" },
            postData: { Moduleid: "r9hr" },
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

            colNames: ['Action', 'reportid', 'reportname'],
            colModel: [
                { name: 'act', index: 'act', width: 120, sortable: false, resizable: false },
                { name: 'reportid', index: 'Id', resizable: false, width: 550, editable: false, editoptions: { readonly: true, size: 10 } },
                { name: 'reportname', index: 'Name', resizable: false, width: 290, editable: true, size: 100 },

                //{ name: 'Gender', index: 'Gender', width: 160, formatter: 'select', editable: true, edittype: "select", editoptions: { value: "0:select;1:male;2:female" } },
                //{ name: 'IsClosed', index: 'IsClosed', width: 180, editable: true, edittype: "checkbox", editoptions: { value: "true:false", formatter: "checkbox" } },

            ],

            gridComplete: function () {
                var ids = jQuery("#hr_table").getDataIDs();
                for (var i = 0; i < ids.length; i++) {
                    var cl = ids[i];

                    view = "<input style='height:18px;width:75px;' type='button' value='View' onclick='getSelectedRow_hr()' class='btn btn-xs btn-danger'>";

                    jQuery("#hr_table").setRowData(ids[i], { act: view })
                }
            },

            pager: '#hr_pager',
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
        $("#hr_table").jqGrid('navGrid', '#hr_pager',
            { edit: true, add: true, del: true, search: true },
            { height: 300, reloadAfterSubmit: true }
        );

        // Add responsive to jqGrid
        $(window).bind('resize', function () {
            var width = $('.hr_jqGrid').width();
            $('#hr_table').setGridWidth(width, false);
        });

}

function getSelectedRow_hr() {
    debugger
    var selRowId = $('#hr_table').jqGrid('getGridParam', 'selrow');
    var ReportsId = $('#hr_table').jqGrid('getCell', selRowId, 'reportid');

    if (ReportsId == undefined) {
        show_err_alert_js('Please Select a row First');
    }
    else
    {
        window.location.href = "/Reports/FlexData?reportid=" + ReportsId;
    }
}