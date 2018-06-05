$("#txtCampusCW").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../personalinfo/getClassJson"),
        data: { campusId: $("#txtCampusCW > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            
            var i = 1;
            items.push("<option value=''>Class/Level</option>");
            $.each(data, function () {

                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                i++;
                
            });

            $("#txtClassCW").html(items.join(' '));
            
        }
        
    });
    return false;
});
    
$("#txtClassCW").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../personalinfo/getSectionJson"),
        data: { campusId: $("#txtCampusCW > option:selected").attr("value"), classId: $("#txtClassCW > option:selected").attr("value") },
        success: function (data) {
            var items = [];

            var i = 1;
            items.push("<option value=''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                i++;
            });

            $("#txtSectionCW").html(items.join(' '));
        }
    });
    return false;
});


$("#classwise_div").hide();
$("#classwise_table").jqGrid('setGridState', 'hidden');


$("#classWiseForm").submit(function (e) {

    $.ajax({
        url: encodeURI("../personalinfo/getJQGridJson"),
        data: {
            campus: $("#txtCampusCW > option:selected").attr("value"),
            classes: $("#txtClassCW > option:selected").attr("value"),
            section: $("#txtSectionCW > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            jQuery('#classwise_table').jqGrid('clearGridData');
            $('#classwise_table').jqGrid('setGridParam', { data: data });
            $("#classwise_div").show();
            $("#classwise_table").jqGrid('setGridState', 'visible');

            var $screensize = $("#classwise_div").width() - 42;
            var $size = ($screensize / 100);

            $("#classwise_table").jqGrid({
                datatype: "local",
                colNames: ['Student Id', 'Student Name', 'Father Name'],
                colModel: [
                    { name: 'studentId', index: 'studentId', width: $size*20, resizable: false },
                    { name: 'studentName', index: 'studentName', width: $size*40, resizable: false },
                    { name: 'fatherName', index: 'fatherName', width: $size*40, resizable: false },
                ],
                data: data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                //shrinkToFit: true,
                rowNum: 10,
                viewrecords: true,
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                pager: "#pager_list",
                caption: "Profile Sheet",
                hidegrid: false,
                loadonce: true,
                shrinkToFit: false

            }).trigger('reloadGrid', [{ current: true }]);
        },
        error: function ()
        {
            show_err_alert_js('No Record Found');
            $("#classwise_div").css("display", "none");
        }
    });
    

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.classwise_table').width();
        $('#aclasswise_table').setGridWidth(width, false);
    });
    return false;
});

//$(document).ready(function () {
//    "use strict";
//    // Examle data for jqGrid
//    var mydata = [
//        { id: "22001", studentname: "Abdul Basit", fathername: "Basit" },
//        { id: "22002", studentname: "Ahmed Zamir", fathername: "Zamir" },
//        { id: "22003", studentname: "Ali Manzar", fathername: "Manzar" },
//        { id: "22004", studentname: "Amir udin Ayaz", fathername: "Ayaz" },
//        { id: "22005", studentname: "Azzaz ul Haq", fathername: " Azaz" },
//        { id: "22006", studentname: "Danish Walani", fathername: "Walani" },
//        { id: "22007", studentname: "Hamza Iqbal", fathername: "Iqbal" },
//        { id: "22008", studentname: "Mahboob Alam", fathername: "Alam" },
//        { id: "22009", studentname: "Moiz Iqbal", fathername: "Iqbal" },
//        { id: "22010", studentname: "Mustafa Quresh", fathername: "Quresh" },
//        { id: "22011", studentname: "Salman Khan", fathername: "Khan" },
//        { id: "22012", studentname: "Shaheer Ali Agha", fathername: "Ali Agha" },
//        { id: "22013", studentname: "Syed Saad Ali", fathername: "Ali" },
//        { id: "22014", studentname: "Talha Hussain Chohan", fathername: "Chohan" },
//        { id: "22015", studentname: "Taha Nadeem", fathername: "Nadeem" }
//    ];
//    $("#classwise_table").jqGrid({
//        styleUI: 'Bootstrap',
//        height: 450,
//        autowidth: true,
//        //shrinkToFit: true,
//        rowNum: 20,
//        colNames: ['Id No', 'Student Name', 'Father Name'],
//        colModel: [
//            { name: 'id', index: 'id', editable: true, sorttype: "int", width: 180, search: true, resizable: false },
//            { name: 'studentname', index: 'studentname', editable: true, width: 700, resizable: false },
//            { name: 'fathername', index: 'fathername', editable: true, width: 700, resizable: false },
//            ],
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
//        pager: "#pager_list",
//        viewrecords: true,
//        caption: "Profile Sheet",
//        add: true,
//        edit: true,
//        addtext: 'Add',
//        edittext: 'Edit',
//        hidegrid: false,
//        shrinkToFit: false
//    });

//    // Setup buttons
//    $("#classwise_table").jqGrid('navGrid', '#pager_list',
//            { edit: true, add: true, del: true, search: true },
//            { height: 300, reloadAfterSubmit: true }
//    );

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.classwise_jqGrid').width();
//        $('#classwise_table').setGridWidth(width, false);
//    });

//    $("#classwise_div").hide();

//    $("#classwise_table").jqGrid('setGridState', 'hidden');//or 'hidden' 

//    $("#showdiv").click(function (e) {
//        if ($("#classWiseForm").valid()) {

//            $("#classwise_div").show();
//            $("#classwise_table").jqGrid('setGridState', 'visible');//or 'hidden'
//        }
//        return false;
//    });

//    $("#classWiseForm").validate({
//        errorClass: "my-error-class",
//        validClass: "my-valid-class",
//        rules: {
//            txtSection: {
//                required: true,
//            },
//            txtPersonId: {
//                required: true,
//            },
//            txtClass: {
//                required: true,
//            },
//            txtCampus: {
//                required: true,
//            }
//        },
//        messages: {
//            txtSection: {
//                required: ""
//            },
//            txtPersonId: {
//                required: ""
//            },
//            txtClass: {
//                required: ""
//            },
//            txtCampus: {
//                required: ""
//            }
//        }
//    });

//});

