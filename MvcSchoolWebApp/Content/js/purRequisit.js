//$("#prForm").submit(function (e) {
    //$.ajax({
    //    url: encodeURI("../ESS/getPRJQGridJson"),
    //    data: {
    //        campus: $("#txtCampusMA > option:selected").attr("value"),
    //        classes: $("#txtClassMA > option:selected").attr("value"),
    //        section: $("#txtSectionMA > option:selected").attr("value"),
    //        subject: $("#txtSubjectMA > option:selected").attr("value"),
    //        date: $("#txtDateMA").val()
    //    },
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    success: function (data) {
            
    //        // close_progress();
    //    },
    //    error: function (error) {
    //        show_alert_js();
    //        $("#prdiv").css("display", "none");
    //    }
    //});
    $('#pr_table').jqGrid('clearGridData');
    $("#pr_table").jqGrid("GridUnload");
    //$('#pr_table').jqGrid('setGridParam', { data: data });
    $("#prdiv").show();
    $("#pr_table").jqGrid('setGridState', 'visible');

    var $screensize = $("#prdiv").width() - 42;
    var $size = ($screensize / 100);

    $("#pr_table").jqGrid({
        datatype: "local",
        colNames: ['AC', 'Material', 'Material Description', 'Order Quantity', 'OU', 'BU', 'Plant', 'Store', 'Delv. Date', 'Short Text'],
        colModel: [
            {
                name: 'AC', index: 'AC', editable: true, align: 'left', resizable: false, width: $size * 10, edittype: "select",
                formatter: 'select',
                editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

            },
            {
                name: 'Material', index: 'Material', editable: true, align: 'left', resizable: false, width: $size * 20, edittype: "select",
                formatter: 'select',
                editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

            },
            {
                name: 'Material Description', index: 'Material Description', width: $size * 40, resizable: false, align: 'left'
            },
            {
                name: 'Order Quantity', index: 'Order Quantity', editable: true, width: $size * 10, resizable: false, align: 'left'
            },
            {
                name: 'OU', index: 'OU', editable: true, align: 'left', resizable: false, width: $size * 10, edittype: "select",
                formatter: 'select',
                editoptions: { value: "Present:Present;Absent:Absent;Leave:Leave", defaultValue: "Present" },

            },
            {
                name: 'BU', index: 'BU', editable: false, width: $size * 10, resizable: false, align: 'left'
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
                name: 'Delv. Date', index: 'Delv. Date', editable: true, align: 'left', resizable: false, width: $size * 30,
                formatter: 'date', formatoptions: { srcformat: 'd/m/Y', newformat: 'd/m/Y' }
            },
            {
                name: 'Short Text', index: 'Short Text', editable: true, align: 'left', resizable: false, width: $size * 40
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
        var width = $('.pr_jqGrid').width();
        $('#pr_table').setGridWidth(width, false);
    });
//    return false;
//});