
//$(document).ready(function () {
//    "use strict";
//    // Examle data for jqGrid
//    var mydata = [
//        { id: "Gl000", filename: "Chart of Account", status: "view" },
//        { id: "zgl001", filename: "Test1", status: "view" },
//        { id: "gl010a", filename: "test2", status: "view" },
//        { id: "crp07", filename: "test3", status: "view" },
//        { id: "ar004", filename: "test3", status: "view" },
//        { id: "SM0001", filename: "Customer Master List", status: "view" },
   

//    ];

//    // Configuration for jqGrid Example 2
//    $("#finance_table").jqGrid({
//        data: mydata,
//        datatype: "local",
//        height: 450,
//        autowidth: true,

//        rowNum: 20,
//        rowList: [10, 20, 30],
//        colNames: ['S No', 'Report Name', 'Action'],
//        colModel: [
//            //width: "350", resizable: false, editable: false, align: 'center'
//            { name: 'id', index: 'id', editable: false, width: 120, align: 'left', resizable: false, sorttype: "int", search: true },
//            { name: 'filename', index: 'filename', editable: false, width: 845, align: 'left', resizable: false, search: true },

//            { name: 'status', index: 'status', editable: false, width: 80, align: 'left', resizable: false, sortable: false },


//        ],

//        gridComplete: function () {
//            var ids = jQuery("#finance_table").getDataIDs();
//            for (var i = 0; i < ids.length; i++) {
//                var cl = ids[i];
//                //class='btn btn-xs btn-info'
//                var view = "<input style='height:18px;width:75px;' type='button' value='View' onclick=\"getSelectedRow();\" class='btn btn-xs btn-primary'>";
//                jQuery("#finance_table").setRowData(ids[i], { status: view })
//            }
//        },

//        pager: "#finance_pager",
//        viewrecords: true,
//        //caption: "Quiz",
//        add: true,
//        edit: true,
//        addtext: 'Add',
//        edittext: 'Edit',
//        hidegrid: false,
//        shrinkToFit: false
//    });

//    // Setup buttons
//    $("#finance_table").jqGrid('navGrid', '#finance_pager',
//            { edit: true, add: true, del: true, search: true },
//            { height: 300, reloadAfterSubmit: true }
//    );

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.finance_jqGrid').width();
//        $('#finance_table').setGridWidth(width, false);
//    });

//});

//function getSelectedRow() {


//    debugger
//    var selRowId = $('#finance_table').jqGrid('getGridParam', 'selrow');

//    var ReportsId = $('#finance_table').jqGrid('getCell', selRowId, 'id');
  
//    //window.location.href = '@Url.Action("FlexData", "Reports")' + '?reportid=' + ReportsId;

//    window.location.href = "/Reports/FlexData?reportid=" + ReportsId;

 


//} 

/////// ********************* /////////////////////// 


$(document).ready(function () {

    loaddata_finance();

    //$(window).bind('resize', function () {
    //    var width = $('.jqGrid_wrapper').width();

    //    $('#table_list_2').setGridWidth(width, false);
    //});

    //$("#quiz_div").hide();

    //$("#table_list_2").jqGrid('setGridState', 'hidden');

    //$("#btnviewresult").click(function (e) {
    //    if ($("#resultform").valid()) {
    //        loaddata();
    //        $("#quiz_div").show();
    //        $("#table_list_2").jqGrid('setGridState', 'visible');
    //    }
    //    return false;
    //});


    // code to read selected table row cell data (values).
    $("#finance_table").on('click', '#view', function () {
        // get the current row
        var currentRow = $(this).closest("tr");

        var ReportsId = currentRow.find("td:eq(1)").text().trim(); // get current row 2nd TD


        window.location.href = "/Reports/FlexData?reportid=" + ReportsId

    });


});

function loaddata_finance() {
    $("#finance_table").jqGrid({
        url: '/Reports/GetDataFromDB',
        datatype: 'json',
        mtype: 'POST',
        autowidth: true,
        //autoheight: true,
        height: 270,
        postData: { Moduleid: "r1gl" },
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
            var ids = jQuery("#finance_table").getDataIDs();
            for (var i = 0; i < ids.length; i++) {
                var cl = ids[i];

                view = "<input id='view' style='height:18px;width:75px;' type='button' value='View' class='btn btn-xs btn-danger'>";
              
                jQuery("#finance_table").setRowData(ids[i], { act: view  })
            }
        },

        pager: '#finance_pager',
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
        $("#finance_table").jqGrid('navGrid', '#finance_pager',
                { edit: true, add: true, del: true, search: true },
                { height: 300, reloadAfterSubmit: true }
        );

        // Add responsive to jqGrid
        $(window).bind('resize', function () {
            var width = $('.finance_jqGrid').width();
            $('#finance_table').setGridWidth(width, false);
        });

}

function getSelectedRow_finance() {

    var selRowId = $('#finance_table').jqGrid('getGridParam', 'selrow');
    var ReportsId = $('#finance_table').jqGrid('getCell', selRowId, 'reportid');

    if (ReportsId == undefined) {
        show_err_alert_js('Please Select a row First');
    }
    else {
        window.location.href = "/Reports/FlexData?reportid=" + ReportsId;
    }

} 