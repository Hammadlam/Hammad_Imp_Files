
$('#so_table').jqGrid('clearGridData');
$("#so_table").jqGrid("GridUnload");
$("#sodiv").show();
$("#so_table").jqGrid('setGridState', 'visible');

var $screensize = $("#sodiv").width() - 42;
var $size = ($screensize / 100);

$("#so_table").jqGrid({
    datatype: "local",
    colNames: ['Type', 'Material', 'Description', 'Cust. Mat. No', 'Quantity', 'OU', 'SU', 'Plant', 'Store', 'Net Price', 'S T Code', 'S T Amount', 'Gross Price', 'Net Value'],
    colModel: [
        {
            name: 'Type', index: 'Type', editable: true, align: 'left', resizable: false, width: $size * 10, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'Material', index: 'Material', editable: true, align: 'left', resizable: false, width: $size * 20, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'Description', index: 'Description', width: $size * 40, resizable: false, align: 'left'
        },
        {
            name: 'Cust. Mat. No', index: 'Cust. Mat. No', editable: true, align: 'left', resizable: false, width: $size * 20, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'Quantity', index: 'Quantity', editable: true, width: $size * 10, resizable: false, align: 'left'
        },
        {
            name: 'OU', index: 'OU', editable: true, align: 'left', resizable: false, width: $size * 10, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'SU', index: 'SU', editable: false, width: $size * 10, resizable: false, align: 'left'
        },
        {
            name: 'Plant', index: 'Plant', editable: true, align: 'left', resizable: false, width: $size * 20, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'Store', index: 'Store', editable: true, align: 'left', resizable: false, width: $size * 20, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'Net Price', index: 'Net Price', editable: true, width: $size * 10, resizable: false, align: 'left'
        },
        {
            name: 'S T Code', index: 'S T Code', editable: true, width: $size * 10, resizable: false, align: 'left'
        },
        {
            name: 'S T Amount', index: 'S T Amount', editable: true, width: $size * 10, resizable: false, align: 'left'
        },
        {
            name: 'Gross Price', index: 'Gross Price', editable: true, width: $size * 10, resizable: false, align: 'left'
        },
        {
            name: 'Net Value', index: 'Net Value', editable: true, width: $size * 10, resizable: false, align: 'left'
        }
        
    ],
    //data: data,
    styleUI: 'Bootstrap',
    mtype: 'GET',
    height: 250,
    autoheight: true,
    autowidth: true,
    loadonce: true,
    viewrecords: true,
    //rowNum: data[0].count,
    iconSet: "fontAwesome",
    sortorder: "desc",
    threeStateSort: true,
    sortIconsBeforeText: true,
    headertitles: true,
    cellEdit: true,
    editable: true,
    add: true,
    edit: true,
    addtext: 'Add',
    edittext: 'Edit',
    hidegrid: false,
    shrinkToFit: false,
    cellsubmit: 'clientArray'

}).trigger('reloadGrid', [{ current: true }]);
// Add responsive to jqGrid
$(window).bind('resize', function () {
    var width = $('.so_jqGrid').width();
    $('#so_table').setGridWidth(width, false);
});
//    return false;
//});