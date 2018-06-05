"use strict";

$("#campusStrengthForm").submit(function (e) {
        $.ajax({
            url: encodeURI("../admission/getJQGridJson"),
            data: {
                campus: $("#txtCampusStrength > option:selected").attr("value")
            },
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $('.camp-strength').show();
                jQuery('#campusStrength_table').jqGrid('clearGridData');
                $('#campusStrength_table').jqGrid('setGridParam', { data: data });
                $("#campusStrength_div").show();
                $("#campusStrength_table").jqGrid('setGridState', 'visible');//or 'hidden' 

                var $screensize = $("#campusStrength_div").width();
                var $size = ($screensize / 100);
                $("#campusStrength_table").jqGrid({
                    datatype: "local",
                    colNames: ['Classes/Levels', 'Total Strength', 'Boys', 'Girls'],
                    colModel: [
                        { name: 'classes', index: 'classes', align: 'left', width: $size * 40, resizable: false },
                        { name: 'total', index: 'total', align: 'left', width: $size * 20, resizable: false },
                        { name: 'boys', index: 'boys', align: 'left', width: $size * 20, resizable: false },
                        { name: 'girls', index: 'girls', align: 'left', width: $size * 20, resizable: false },
                    ],
                    data: data,
                    styleUI: 'Bootstrap',
                    pager: "#campusStrength_pager_list",
                    mtype: 'GET',
                    height: 250,
                    autoheight: true,
                    autowidth: true,
                    rowNum: 10,
                    //rowList: [10, 20, 30],
                    viewrecords: true,
                    caption: "Campus Strength",
                    shrinkToFit: false,
                    gridview: true,
                    hidegrid: false,
                    loadonce: true
                }).trigger('reloadGrid', [{ current: true }]);
                
               // alert(data[data.length - 1].totalstrength + "" + data[data.length - 1].totalboys + "" + data[data.length - 1].totalgirls);
                
                $("label[for='lbltotal'] ").html(data[data.length - 1].totalstrength);
                $("label[for='lblboys'] ").html(data[data.length - 1].totalboys);
                $("label[for='lblgirls'] ").html(data[data.length - 1].totalgirls);
            },
            error: function (error)
            {
                show_err_alert_js('No Record Found');
                $("#campusStrength_div").css("display", "none");
                $(".camp-strength").css("display", "none");
            }
        });


        

    return false;
});

// Add responsive to jqGrid
$(window).bind('resize', function () {
    var $width = $('#campusStrength_div').width();
    $('#campusStrength_table').setGridWidth($width, false);
});
//$(document).ready(function () {
//    "use strict";
//    // Examle data for jqGrid
//    var mydata = [
//        { classes: "I", strength: "23", boys: "13", girls:"10" },
//        { classes: "II", strength: "23", boys: "13", girls: "10" },
//        { classes: "III-A", strength: "20", boys: "10", girls: "10" },
//        { classes: "III-B", strength: "26", boys: "11", girls: "15" },
//        { classes: "IV-A", strength: "18", boys: "09", girls: "09" },
//        { classes: "IV-B", strength: "21", boys: "10", girls: "11" },
//        { classes: "IV-C", strength: "25", boys: "15", girls: "10" },
//        { classes: "V-A", strength: "22", boys: "12", girls: "10" },
//        { classes: "VI-A", strength: "20", boys: "13", girls: "7" },

//    ];
//    $("#campusStrength_table").jqGrid({
//        styleUI: 'Bootstrap',
//        height: 450,
//        autowidth: true,
//        //shrinkToFit: true,
//        rowNum: 20,
//        colNames: ['Classes/Levels', 'Total Strength', 'Boys', 'Girls'],
//        colModel: [
//            { name: 'classes', index: 'classes', editable: true, align: 'center', width: 240, search: true, resizable: false },
//            { name: 'strength', index: 'strength', editable: true, align: 'center', width: 240, resizable: false },
//            { name: 'boys', index: 'boys', editable: true, align: 'center', width: 240, resizable: false },
//            { name: 'girls', index: 'girls', editable: true, align: 'center', width: 239, resizable: false },
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
//        pager: "#campusStrength_pager_list",
//        viewrecords: true,
//        caption: "Campus Strength",
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
//    $("#campusStrength_table").jqGrid('navGrid', '#campusStrength_pager_list',
//            { edit: true, add: true, del: true, search: true },
//            { height: 300, reloadAfterSubmit: true }
//    );

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.campusStrength_jqGrid').width();
//        $('#campusStrength_table').setGridWidth(width, false);
//    });

//    //$("#campusStrength_div").hide();

//    //$("#campusStrength_table").jqGrid('setGridState', 'hidden');//or 'hidden' 

//    $("#campusStrengthshowGrid").click(function (e) {
//        if ($("#campusStrengthForm").valid()) {
//            $("#campusStrength_div").show();
//            $("#campusStrength_table").jqGrid('setGridState', 'visible');//or 'hidden' 
//        }

//        return false;
//    });

//    $("#campusStrengthForm").validate({
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

