


$(document).ready(function () {
    loadLoanAppdata();
    var id = document.getElementById("txtenddateRAPP");
    if (id != null) {
        $('#txtenddateRAPP').datepicker({
            format: 'dd-MM-yyyy hh:ii',
            autoclose: true,
            startDate: date
        });

    }
});



function loadLoanAppdata() {
    var ctx = document.getElementById("resigapp_table");
    if (ctx != null) {
        var $screensize = $("#resigapp_div").width() - 42;
        var $size = ($screensize / 100);
        $("#resigapp_table").jqGrid({
            url: '',///MSS/GetLoanAppr
            postData: { reqtyp: "10" },
            datatype: 'json',
            mtype: 'POST',
            autowidth: true,
            height: 270,

            serializeGridData: function (postData) {
                return JSON.stringify(postData);
            },

            ajaxGridOptions: { contentType: "application/json" },
            loadonce: true,

            colNames: ['Action', 'Employee Id', 'Employee Name', 'Date', 'Status', 'Overall Status'],
            colModel: [
                { name: 'act', index: 'act', width: $size * 20, sortable: false, resizable: false },
                { name: 'empid', index: 'empid', resizable: false, width: $size * 15, editable: false, editoptions: { readonly: true, size: 10 }, sortable: true },
                { name: 'empname', index: 'empname', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true, align: 'bottam', },
                { name: 'dates', index: 'dates', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true },
                { name: 'stat', index: 'stat', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true },
                { name: 'overalstat', index: 'overalstat', resizable: false, width: $size * 20, editable: false, size: 100, sortable: true }
            ],

            gridComplete: function () {
                var ids = jQuery("#resigapp_table").getDataIDs();
                for (var i = 0; i < ids.length; i++) {
                    var cl = ids[i];
                    recomendresig = "<input style='height:18px;' type='button' value='Acceptance' onclick= 'getSelectedRow_recodResig();' class='btn btn-xs btn-danger'>";
                    apprresig = "<input style='height:18px;margin-left: 5px;' type='button' value='Approval' onclick= 'getSelectedRow_appResig();' class='btn btn-xs btn-danger'>";
                    jQuery("#resigapp_table").setRowData(ids[i], { act: recomendresig + apprresig })
                }
            },

            pager: '#resigapp_pager',
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
        $("#resigapp_table").jqGrid('navGrid', '#resigapp_pager',
            { edit: true, add: true, del: true, search: true },
            { height: 300, reloadAfterSubmit: true }
        );

        // Add responsive to jqGrid
        $(window).bind('resize', function () {
            var width = $('.resigapp_jqGrid').width();
            $('#resigapp_table').setGridWidth(width, false);
        });
    }
}
