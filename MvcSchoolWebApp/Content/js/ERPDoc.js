var ctx = document.getElementById("erdoc_grid");
if (ctx != null) {
    $.ajax({
        url: encodeURI("../CompanyDoc/getJQGridJsonERDoc"),
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            jQuery('#erdoc_grid').jqGrid('clearGridData');
            var role = data[data.length - 1].user_role;
            $('#erdoc_grid').jqGrid('setGridParam', { data: data });

            var $screensize = $("#erdoc_div").width() - 42;
            var $size = ($screensize / 100);

            $("#erdoc_grid").jqGrid({
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
                    var ids = jQuery("#erdoc_grid").getDataIDs();
                    for (var i = 0; i < ids.length; i++) {
                        var cl = ids[i];

                        //view = "<input style='height:18px;width:75px;' type='button' value='View' class='btn btn-xs btn-primary'>";
                        Download = "<i style='margin-left:30%; color:#e60000;' title='Download' class='fa fa-download fa-lg' onclick='getSelected_ERDoc()'></i>";
                        Delete = "<i style='margin-left:15%; color:#e60000;' title='Delete' class='fa fa-trash-o fa-lg' onclick='getDeleted_ERDoc()'></i>";
                        jQuery("#erdoc_grid").setRowData(ids[i], { status: Download + Delete });
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
                caption: "ER Documents",
                pager: "#erdoc_pager",
                viewrecords: true,
                hidegrid: false,
                shrinkToFit: false
            }).trigger('reloadGrid', [{ current: true }]);

        },
        error: function (error) {
            show_err_alert_js('No Record Found');
        }
    });


    $("#erdoc_grid").setSelection(4, true);

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.erdoc_wrapper').width();

        $('#erdoc_grid').setGridWidth(width, false);
    });

}

function getSelected_ERDoc() {

    var selRowId = $('#erdoc_grid').jqGrid('getGridParam', 'selrow');

    var ReportsId = $('#erdoc_grid').jqGrid('getCell', selRowId, 'fileName');

    window.location.href = "/CompanyDoc/DownloadHRDoc?reportid=" + ReportsId;

}

function getDeleted_ERDoc() {

    var selRowId = $('#erdoc_grid').jqGrid('getGridParam', 'selrow');
    var fileName = $('#erdoc_grid').jqGrid('getCell', selRowId, 'fileName');
    $.ajax({
        type: "POST",
        url: encodeURI("../CompanyDoc/DeleteHRDoc"),
        data: {
            filename: fileName,
            category: "5000"
        },
        success: function (data) {
            if (data)
                $('#erdoc_grid').delRowData(selRowId[0]);
            else
                show_err_alert_js("You Do Not Have Rights");
        }
    });

}