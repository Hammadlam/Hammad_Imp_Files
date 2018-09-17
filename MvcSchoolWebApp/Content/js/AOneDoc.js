var ctx = document.getElementById("aonedoc_grid");
if (ctx != null) {
    $.ajax({
        url: encodeURI("../CompanyDoc/getJQGridJsonAOneDoc"),
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            jQuery('#aonedoc_grid').jqGrid('clearGridData');
            var role = data[data.length - 1].user_role;
            $('#aonedoc_grid').jqGrid('setGridParam', { data: data });

            var $screensize = $("#aonedoc_div").width() - 42;
            var $size = ($screensize / 100);

            $("#aonedoc_grid").jqGrid({
                datatype: "local",
                loadonce: true,
                colNames: ['Action', 'S No', 'Name', 'Date'],
                colModel: [
                    { name: 'status', index: 'status', editable: false, width: $size * 15, align: 'left', resizable: false },
                    { name: 'serialNo', index: 'serialNo', editable: false, width: $size * 10, align: 'left', resizable: false },
                    { name: 'fileName', index: 'fileName', editable: false, width: $size * 40, align: 'left', resizable: false },
                    { name: 'date', index: 'date', editable: false, width: $size * 35, align: 'left', resizable: false }
                ],

                gridComplete: function () {
                    var ids = jQuery("#aonedoc_grid").getDataIDs();
                    for (var i = 0; i < ids.length; i++) {
                        var cl = ids[i];

                        //view = "<input style='height:18px;width:75px;' type='button' value='View' class='btn btn-xs btn-primary'>";
                        Download = "<i style='margin-left:30%; color:#e60000;' title='Download' class='fa fa-download fa-lg' onclick='getSelected_AOneDoc()'></i>";
                        Delete = "<i style='margin-left:15%; color:#e60000;' title='Delete' class='fa fa-trash-o fa-lg' onclick='getDeleted_AOneDoc()'></i>";
                        jQuery("#aonedoc_grid").setRowData(ids[i], { status: Download + Delete });
                    }
                },
                data: data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                rowNum: 10,
                viewrecords: true,
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                caption: "A-One Documents",
                pager: "#aonedoc_pager",
                viewrecords: true,
                hidegrid: false,
                shrinkToFit: false
            }).trigger('reloadGrid', [{ current: true }]);

        },
        error: function (error) {
            show_err_alert_js('No Record Found');
        }
    });


    $("#aonedoc_grid").setSelection(4, true);

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.aonedoc_wrapper').width();

        $('#aonedoc_grid').setGridWidth(width, false);
    });

}

function getSelected_AOneDoc() {

    var selRowId = $('#aonedoc_grid').jqGrid('getGridParam', 'selrow');

    var ReportsId = $('#aonedoc_grid').jqGrid('getCell', selRowId, 'fileName');

    window.location.href = "/CompanyDoc/DownloadHRDoc?reportid=" + ReportsId;

}

function getDeleted_AOneDoc() {

    var selRowId = $('#aonedoc_grid').jqGrid('getGridParam', 'selrow');
    var fileName = $('#aonedoc_grid').jqGrid('getCell', selRowId, 'fileName');
    $.ajax({
        type: "POST",
        url: encodeURI("../CompanyDoc/DeleteHRDoc"),
        data: {
            filename: fileName,
            category: "4000"
        },
        success: function (data) {
            if (data)
                $('#aonedoc_grid').delRowData(selRowId[0]);
            else
                show_err_alert_js("You Do Not Have Rights");
        }
    });

}