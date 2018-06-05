$("#campusCapacity_div").hide();

$("#campusCapacity_table").jqGrid('setGridState', 'hidden');//or 'hidden' 

$("#campusCapacityshowGrid").click(function (e) {
    $.ajax({
        url: encodeURI("../admission/getCapacityJQGridJson"),
        data: {
            campus: $("#txtCampusCC > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            jQuery('#campusCapacity_table').jqGrid('clearGridData');
            $('.camp-capacity').show();
            $('#campusCapacity_table').jqGrid('setGridParam', { data: data });
            $("#campusCapacity_div").show();
            $("#campusCapacity_table").jqGrid('setGridState', 'visible');//or 'hidden' 

            var $screensize = $("#campusCapacity_div").width();
            var $size = ($screensize / 100);
            $("#campusCapacity_table").jqGrid({
                datatype: "local",
                colNames: ['Classes/Levels', 'Total Seats', 'Seats Occupied', 'Seats Available'],
                colModel: [
                { name: 'classes', index: 'classes', align: 'center', width: $size * 30, search: true, resizable: false },
                { name: 'total', index: 'total', align: 'center', width: $size * 20, resizable: false },
                { name: 'occupied', index: 'occupied', align: 'center', width: $size * 20, resizable: false },
                { name: 'available', index: 'available', align: 'center', width: $size * 26, resizable: false },
                ],
                data: data,
                styleUI: 'Bootstrap',
                pager: "#campusCapacity_pager_list",
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                rowNum: 10,
                //rowList: [10, 20, 30],
                viewrecords: true,
                caption: "Campus Capacity",
                shrinkToFit: false,
                gridview: true,
                hidegrid: false,
                loadonce: true
            }).trigger('reloadGrid', [{ current: true }]);

            $("label[for='lbltotalcap'] ").html(data[data.length - 1].totseats);
            $("label[for='lbloccupied'] ").html(data[data.length - 1].totoccupied);
            $("label[for='lblavailable'] ").html(data[data.length - 1].totavaible);
        },
        error: function (error) {
            show_err_alert_js('No Record Found');
            $("#campusCapacity_div").css("display", "none");
            //$(".camp-strength").css("display", "none");
        }
    });

    return false;
});

$(window).bind('resize', function () {
    var width = $('.campusCapacity_jqGrid').width();
    $('#campusCapacity_table').setGridWidth(width, false);
});
//$(document).ready(function () {
//    "use strict";
//    // Examle data for jqGrid
//    var mydata = [
//        { classes: "I", totalseats: "23", occupied: "13", available: "10" },
//        { classes: "II", totalseats: "23", occupied: "13", available: "10" },
//        { classes: "III-A", totalseats: "20", occupied: "10", available: "10" },
//        { classes: "III-B", totalseats: "26", occupied: "11", available: "15" },
//        { classes: "IV-A", totalseats: "18", occupied: "09", available: "09" },
//        { classes: "IV-B", totalseats: "21", occupied: "10", available: "11" },
//        { classes: "IV-C", totalseats: "25", occupied: "15", available: "10" },
//        { classes: "V-A", totalseats: "22", occupied: "12", available: "10" },
//        { classes: "VI-A", totalseats: "20", occupied: "13", available: "7" },

//    ];
//    $("#campusCapacity_table").jqGrid({
//        styleUI: 'Bootstrap',
//        height: 450,
//        autowidth: true,
//        //shrinkToFit: true,
//        rowNum: 20,
//        colNames: ['Classes/Levels', 'Total Seats', 'Seats Occupied', 'Seats Availabe'],
//        colModel: [
//            { name: 'classes', index: 'classes', editable: true, align: 'center', width: 240, search: true, resizable: false },
//            { name: 'totalseats', index: 'totalseats', editable: true, align: 'center', width: 240, resizable: false },
//            { name: 'occupied', index: 'occupied', editable: true, align: 'center', width: 240, resizable: false },
//            { name: 'available', index: 'available', editable: true, align: 'center', width: 239, resizable: false },
//        ],
//        data: mydata,
//        datatype: "local",
//        iconSet: "fontAwesome",
//        idPrefix: "g5_",
//        sortname: "invdate",
//        sortorder: "desc",
//        threeStateSort: true,
//        sortIconsBeforeText: true,
//        headertitles: true,
//        cellEdit: true,
//        cellsubmit: 'clientArray',
//        editable: true,
//        pager: "#campusCapacity_pager_list",
//        viewrecords: true,
//        caption: "Campus Capacity",
//        add: true,
//        edit: true,
//        addtext: 'Add',
//        edittext: 'Edit',
//        hidegrid: false,
//        shrinkToFit: false
//    });

//    // Add selection
//    //$("#attendance_table").setSelection(4, true);


//    // Setup buttons
//    $("#campusCapacity_table").jqGrid('navGrid', '#campusCapacity_pager_list',
//            { edit: true, add: true, del: true, search: true },
//            { height: 300, reloadAfterSubmit: true }
//    );

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.campusCapacity_jqGrid').width();
//        $('#campusCapacity_table').setGridWidth(width, false);
//    });

//    $("#campusCapacity_div").hide();

//    $("#campusCapacity_table").jqGrid('setGridState', 'hidden');//or 'hidden' 

//    $("#campusCapacityshowGrid").click(function (e) {
//        if ($("#campusCapacityForm").valid()) {
//            $("#campusCapacity_div").show();
//            $("#campusCapacity_table").jqGrid('setGridState', 'visible');//or 'hidden' 
//        }

//        return false;
//    });

//    $("#campusCapacityForm").validate({
//        errorClass: "my-error-class",
//        validClass: "my-valid-class",
//        rules: {
//            txtCampus: {
//                required: true,
//            }
//        },
//        messages: {
//            txtCampus: {
//                required: ""
//            }
//        }
//    });

//});

