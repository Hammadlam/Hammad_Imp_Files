"use strict";
// Examle data for jqGrid


$("#campusStatus_div").hide();

$("#campusStatus_table").jqGrid('setGridState', 'hidden');//or 'hidden' 


//$("#campusStatusForm").submit(function (e) {
//    $.ajax({
//        url: encodeURI("../admission/getStatusJQGridJson"),
//        data: {
//            campus: $("#txtCampusStatus > option:selected").attr("value")
//        },
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (data) {
//            jQuery('#campusStatus_table').jqGrid('clearGridData');

//            $('#campusStatus_table').jqGrid('setGridParam', { data: data });
//            $("#campusStatus_div").show();
//            $("#campusStatus_table").jqGrid('setGridState', 'visible');//or 'hidden' 

//            var $screensize = $("#campusStatus_div").width();
//            var $size = ($screensize / 100);
//            $("#campusStatus_table").jqGrid({
//                datatype: "local",
//                colNames: ['S No.', 'Student Name', 'Father Name', 'Classes/Levels', 'Reason'],
//                colModel: [
//                    { name: 'studentId', index: 'studentId', align: 'left', width: $size * 10, resizable: false },
//                    { name: 'studentName', index: 'studentName', width: $size * 20, search: true, resizable: false },
//                    { name: 'fatherName', index: 'fatherName', width: $size * 20, search: true, resizable: false },
//                    { name: 'classes', index: 'classes', align: 'center', width: $size * 10, search: true, resizable: false },
//                    { name: 'status', index: 'status', width: $size * 40, resizable: false, classes: 'wrap' }

//                ],
//                data: data,
//                styleUI: 'Bootstrap',
//                pager: "#campusStatus_pager_list",
//                mtype: 'GET',
//                height: 250,
//                autoheight: true,
//                autowidth: true,
//                rowNum: 10,
//                rowList: [10, 20, 30],
//                viewrecords: true,
//                caption: "Addmission Status",
//                shrinkToFit: false,
//                gridview: true,
//                hidegrid: false,
//                loadonce: true
//            }).trigger('reloadGrid', [{ current: true }]);
//        },
//        error: function (error) {
//            show_err_alert_js('No Record Found');
//            $("#campusStatus_div").css("display", "none");
//        }
//    });

//    return false;
//});

$("#campusStatusForm").submit(function (e) {
        $("#campusStatus_div").show();
        $("#campusStatus_table").jqGrid('setGridState', 'visible');//or 'hidden' 
        var mydata = [
        { id: '01', studentname: "Abdul Basit", fathername: "Basit", classes: "I", reason: "Admission Test is not cleared by students. Undertaking will be taken." },
        { id: '02', studentname: "Ahmed Zamir", fathername: "Zamir", classes: "II", reason: "Admission fees is not paid." },
        { id: '03', studentname: "Ali Manzar", fathername: "Manzar", classes: "III", reason: "Concession Case" },
        { id: '04', studentname: "Amir udin Ayaz", fathername: "Ayaz", classes: "III", reason: "Late Admission" },
        { id: '05', studentname: "Azzaz ul Haq", fathername: "Azaz", classes: "IV", reason: "Late Admission" },
        { id: '06', studentname: "Danish Walani", fathername: "Walani", classes: "IV", reason: "Admission fees is not paid." },
        { id: '07', studentname: "Hamza Iqbal", fathername: "Iqbal", classes: "IV", reason: "Admission Test is not cleared by students. Undertaking will be taken." },
        { id: '08', studentname: "Mahboob Alam", fathername: "Alam", classes: "V", reason: "Concession Case" },
        { id: '09', studentname: "Moiz Iqbal", fathername: "Iqbal", classes: "VI", reason: "Admission fees is not paid." },

        ];

        var $screensize = $("#campusStatus_div").width() - 42;
        var $size = ($screensize / 100);

        $("#campusStatus_table").jqGrid({
            styleUI: 'Bootstrap',
            height: 250,
            autoheight: true,
            autowidth: true,
            //shrinkToFit: true,
            rowNum: 10,
            colNames: ['S No.', 'Student Name', 'Father Name', 'Classes/Levels', 'Reason'],
            colModel: [
                { name: 'id', index: 'id', align: 'center', width: $size*10, resizable: false },
                { name: 'studentname', index: 'studentname', width: $size*20, search: true, resizable: false },
                { name: 'fathername', index: 'fathername', width: $size*20, search: true, resizable: false },
                { name: 'classes', index: 'classes', align: 'center', width: $size*15, search: true, resizable: false },
                { name: 'reason', index: 'reason', width: $size*35, resizable: false, classes: 'wrap' }

            ],
            data: mydata,
            datatype: "local",
            iconSet: "fontAwesome",
            loadonce: true,
            //height: "auto",
            idPrefix: "g5_",
            sortname: "invdate",
            sortorder: "desc",
            threeStateSort: true,
            sortIconsBeforeText: true,
            headertitles: true,
            cellsubmit: 'clientArray',
            pager: "#campusStatus_pager_list",
            viewrecords: true,
            caption: "Admission Status",
            hidegrid: false,
            shrinkToFit: false
        });

        // Add responsive to jqGrid
        $(window).bind('resize', function () {
            var width = $('.campusStatus_jqGrid').width();
            $('#campusStatus_table').setGridWidth(width, false);
        });
    return false;
});

//$(document).ready(function () {
//    "use strict";
//    // Examle data for jqGrid
//    var mydata = [
//        { id: '01', studentname: "Abdul Basit", fathername: "Basit", classes: "I", section: "A", reason: "Admission Test is not cleared by students. Undertaking will be taken." },
//        { id: '02', studentname: "Ahmed Zamir", fathername: "Zamir", classes: "II", section: "B", reason: "Admission fees is not paid." },
//        { id: '03', studentname: "Ali Manzar", fathername: "Manzar", classes: "III", section: "D", reason: "Concession Case" },
//        { id: '04', studentname: "Amir udin Ayaz", fathername: "Ayaz", classes: "III", section: "A", reason: "Late Admission" },
//        { id: '05', studentname: "Azzaz ul Haq", fathername: " Azaz", classes: "IV", section: "C", reason: "Late Admission" },
//        { id: '06', studentname: "Danish Walani", fathername: "Walani", classes: "IV", section: "B", reason: "Admission fees is not paid." },
//        { id: '07', studentname: "Hamza Iqbal", fathername: "Iqbal", classes: "IV", section: "D", reason: "Admission Test is not cleared by students. Undertaking will be taken." },
//        { id: '08', studentname: "Mahboob Alam", fathername: "Alam", classes: "V", section: "C", reason: "Concession Case" },
//        { id: '09', studentname: "Moiz Iqbal", fathername: "Iqbal", classes: "VI", section: "A", reason: "Admission fees is not paid." },

//    ];
//    $("#campusStatus_table").jqGrid({
//        styleUI: 'Bootstrap',
//        height: 450,
//        autowidth: true,
//        //shrinkToFit: true,
//        rowNum: 20,
//        colNames: ['S No.', 'Student Name', 'Father Name', 'Classes/Levels', 'Seaction', 'Reason'],
//        colModel: [
//            { name: 'id', index: 'id', editable: true, align: 'center', width: 100, search: true, resizable: false },
//            { name: 'studentname', index: 'studentname', editable: true, width: 240, search: true, resizable: false },
//            { name: 'fathername', index: 'fathername', editable: true, width: 240, search: true, resizable: false },
//            { name: 'classes', index: 'classes', editable: true, align: 'center', width: 100, search: true, resizable: false },
//            { name: 'section', index: 'section', editable: true, align: 'center', width: 100, resizable: false },
//            { name: 'reason', index: 'reason', editable: true, width: 240, resizable: false }
            
//        ],
//        data: mydata,
//        datatype: "local",
//        iconSet: "fontAwesome",
//        height:"auto",
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
//    $("#campusStatus_table").jqGrid('navGrid', '#campusStatus_pager_list',
//            { edit: true, add: true, del: true, search: true },
//            { height: 300, reloadAfterSubmit: true }
//    );

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.campusStatus_jqGrid').width();
//        $('#campusStatus_table').setGridWidth(width, false);
//    });

//    $("#campusStatus_div").hide();

//    $("#campusStatus_table").jqGrid('setGridState', 'hidden');//or 'hidden' 

//    $("#campusStatusshowGrid").click(function (e) {
//        if ($("#campusStatusForm").valid()) {
//            $("#campusStatus_div").show();
//            $("#campusStatus_table").jqGrid('setGridState', 'visible');//or 'hidden' 
//        }

//        return false;
//    });

//    $("#campusStatusForm").validate({
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

