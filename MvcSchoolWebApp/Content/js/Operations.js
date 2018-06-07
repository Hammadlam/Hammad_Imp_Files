
$('#pop_table').jqGrid('clearGridData');
$("#pop_table").jqGrid("GridUnload");
$("#popdiv").show();
$("#pop_table").jqGrid('setGridState', 'visible');

var $screensize = $("#popdiv").width() - 42;
var $size = ($screensize / 100);

$("#pop_table").jqGrid({
    datatype: "local",
    colNames: ['OP No.', 'Ctrl Key', 'WrkCtr', 'Plant', 'Operation Name', 'Base Qty', 'Opr Unit', 'FOH Rate', 'FOH Time', 'FOH Unit', 'Machine Rate', 'M. Time', 'M. Unit', 'Labor Rate', 'L. Time', 'L. Unit'],
    colModel: [
        {
            name: 'OP No.', index: 'OP No.', width: $size * 15, resizable: false, align: 'left'
        },
        {
            name: 'Ctrl Key', index: 'Ctrl Key', editable: true, align: 'left', resizable: false, width: $size * 10, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'WrkCtr', index: 'WrkCtr', editable: true, align: 'left', resizable: false, width: $size * 20, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'Plant', index: 'Plant', editable: true, align: 'left', resizable: false, width: $size * 20, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'Operation Name', index: 'Operation Name', width: $size * 40, resizable: false, align: 'left'
        },
        {
            name: 'Base Qty', index: 'Base Qty', editable: true, width: $size * 10, resizable: false, align: 'left'
        },
        {
            name: 'Opr Unit', index: 'Opr Unit', editable: true, align: 'left', resizable: false, width: $size * 20, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'FOH Rate', index: 'FOH Rate', editable: true, align: 'left', resizable: false, width: $size * 20, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'FOH Time', index: 'FOH Time', editable: true, width: $size * 10, resizable: false, align: 'left'
        },
        {
            name: 'FOH Unit', index: 'FOH Unit', editable: true, align: 'left', resizable: false, width: $size * 10, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'Machine Rate', index: 'Machine Rate', editable: true, align: 'left', resizable: false, width: $size * 10, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'M. Time', index: 'FOH Time', editable: true, width: $size * 10, resizable: false, align: 'left'
        },
        {
            name: 'M. Unit', index: 'FOH Unit', editable: true, align: 'left', resizable: false, width: $size * 10, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'Labor Rate', index: 'Machine Rate', editable: true, align: 'left', resizable: false, width: $size * 10, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

        },
        {
            name: 'L. Time', index: 'FOH Time', editable: true, width: $size * 10, resizable: false, align: 'left'
        },
        {
            name: 'L. Unit', index: 'FOH Unit', editable: true, align: 'left', resizable: false, width: $size * 10, edittype: "select",
            formatter: 'select',
            editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

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