$("#txtCampusMU").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Home/getClassJson"),
        data: { campusId: $("#txtCampusMU > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value =''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassMU").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassMU").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Home/getSectionJson"),
        data: { campusId: $("#txtCampusMU > option:selected").attr("value"), classId: $("#txtClassMU > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value =''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionMU").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionMU").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Home/getSubjectJson"),
        data: {
            campusId: $("#txtCampusMU > option:selected").attr("value"),
            classId: $("#txtClassMU > option:selected").attr("value"),
            sectionId: $("#txtSectionMU > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value =''>Subject</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSubjectMU").html(items.join(' '));
        }
    });
    return false;
});

$("#marksuploadform").submit(function (e) {
    var $grid = $('#marks_table').jqGrid('getColProp', 'oralmarks');
    
    $.ajax({
        url: encodeURI("../Home/getMarksJQGridJson"),
        data: {
            campus: $("#txtCampusMU > option:selected").attr("value"),
            classes: $("#txtClassMU > option:selected").attr("value"),
            section: $("#txtSectionMU > option:selected").attr("value"),
            subject: $("#txtSubjectMU > option:selected").attr("value"),
            rsltype: $("#txtModuleMU > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            var combo = $("#txtModuleMU option:selected").attr("value");
            jQuery('#marks_table').jqGrid('clearGridData');
            $("#marks_table").jqGrid("GridUnload");
            $('#marks_table').jqGrid('setGridParam', { data: data });
            $("#marks_div").show();
            $("#marks_table").jqGrid('setGridState', 'visible');
            $value = data[0].totalmarkstxt;
            var $screensize = $("#marks_div").width() - 42;
            var $size = ($screensize / 100);
            var ColModel = new Array();
            ColModel.push({ index: 'studentId', name: 'studentId', width: $size * 15 });
            ColModel.push({ index: 'studentName', name: 'studentName', width: $size * 20 });
            var columnname;
            if ((data[0].classes == 'S1' || data[0].classes == 'S2' || data[0].classes == 'S3' || data[0].classes == 'AS' || data[0].classes == 'A2') &&
                (combo == "5" || combo == "6" || combo == "7" || combo == "8")) {
                columnname = ['Student Id', 'Student Name', 'Paper 1', 'Paper 2', 'Paper 3'];
                ColModel.push({
                    index: 'p1marks', name: 'p1marks', editrules: {
                        number: true,
                        minValue: 0,
                        maxValue: 100,
                        required: true
                    }, width: $size * 15, editable: true });
                ColModel.push({
                    index: 'p2marks', name: 'p2marks', editrules: {
                        number: true,
                        minValue: 0,
                        maxValue: 100,
                        required: true
                    }, width: $size * 15, editable: true });
                ColModel.push({
                    index: 'p3marks', name: 'p3marks', editrules: {
                        number: true,
                        minValue: 0,
                        maxValue: 100,
                        required: true
                    }, width: $size * 35, editable: true });
            }
            else {
                columnname = ['Student Id', 'Student Name', 'Exam Marks', 'Project / Assignment Marks', 'Test Marks', 'Oral Marks', 'Assignment Marks'];
                ColModel.push({
                    index: 'exammarks', name: 'exammarks', editrules: {
                        number: true,
                        minValue: 0,
                        maxValue: 100,
                        required: true
                    }, width: $size * 15 });
                ColModel.push({
                    index: 'projectmarks', name: 'projectmarks', editrules: {
                        number: true,
                        minValue: 0,
                        maxValue: 100,
                        required: true
                    }, width: $size * 20 });
                ColModel.push({
                    index: 'testmarks', name: 'testmarks', editrules: {
                        number: true,
                        minValue: 0,
                        maxValue: 100,
                        required: true
                    }, width: $size * 15 });
                ColModel.push({
                    index: 'oralmarks', name: 'oralmarks', editrules: {
                        number: true,
                        minValue: 0,
                        maxValue: 100,
                        required: true
                    }, width: $size * 13 });
                ColModel.push({
                    index: 'assignmarks', name: 'assignmarks', editrules: {
                        number: true,
                        minValue: 0,
                        maxValue: 100,
                        required: true
                    }, width: $size * 22 });
            }
            

            $("#marks_table").jqGrid({
                datatype: "local",
                colNames: columnname,
                height: 250,
                autoheight: true,
                autowidth: true,
                colModel: ColModel,
                data: data,
                rowNum: data[0].count,
                cellsubmit: 'clientArray',
                mtype: "GET",
                viewrecords: true,
                caption: "Marks Sheet",
                sortIconsBeforeText: true,
                hidegrid: false,
                shrinkToFit: false,
                cellEdit: true,
                editable: true,
                loadonce: true,
            }).trigger('reloadGrid', [{ current: true }]);

            if ($value != null) {
                $('#txtMarksMU').val($value)
            }

            if (combo == "1" || combo == "2" || combo == "3" || combo == "4") {
                $("#marks_table").jqGrid('setColProp', 'testmarks', { editable: true });
                $("#marks_table").jqGrid('setColProp', 'projectmarks', { editable: false });
                $("#marks_table").jqGrid('setColProp', 'oralmarks', { editable: true });
                $("#marks_table").jqGrid('setColProp', 'assignmarks', { editable: true });
                $("#marks_table").jqGrid('setColProp', 'exammarks', { editable: false });
            }
            else if (combo == "5") {
                $("#marks_table").jqGrid('setColProp', 'exammarks', { editable: true });
                $("#marks_table").jqGrid('setColProp', 'projectmarks', { editable: true });
                $("#marks_table").jqGrid('setColProp', 'oralmarks', { editable: false });
                $("#marks_table").jqGrid('setColProp', 'assignmarks', { editable: false });
                $("#marks_table").jqGrid('setColProp', 'testmarks', { editable: false });
            }
            else if (combo == "6") {
                $("#marks_table").jqGrid('setColProp', 'exammarks', { editable: true });
                $("#marks_table").jqGrid('setColProp', 'projectmarks', { editable: true });
                $("#marks_table").jqGrid('setColProp', 'oralmarks', { editable: false });
                $("#marks_table").jqGrid('setColProp', 'assignmarks', { editable: false });
                $("#marks_table").jqGrid('setColProp', 'testmarks', { editable: false });
            }
            else if (combo == "7" || combo == "8") {
                $("#marks_table").jqGrid('setColProp', 'testmarks', { editable: true });
                $("#marks_table").jqGrid('setColProp', 'projectmarks', { editable: false });
                $("#marks_table").jqGrid('setColProp', 'oralmarks', { editable: true });
                $("#marks_table").jqGrid('setColProp', 'assignmarks', { editable: true });
                $("#marks_table").jqGrid('setColProp', 'exammarks', { editable: false });
            }
            else {
                
            }
        },
        error: function (error) {
            show_err_alert_js('Found Some Error! Please Try Again');
            $("#marks_div").css("display", "none");
        }
    });
    return false;
});


$("#btnUploadMarks").click(function (e) {
    e.preventDefault();
    var combo = $("#txtModuleMU option:selected").attr("value");
    var classes = $("#txtClassMU option:selected").attr("value");
    var gridRows = $("#marks_table").jqGrid('getRowData');
    var rowData = new Array();
    waitingDialog.show('Please Wait: This May Take a While');
    var studentid = new Array();
    var exammarks = new Array();
    var projectmarks = new Array();
    var testmarks = new Array();
    var oralmarks = new Array();
    var assignmarks = new Array();
    var p1marks = new Array();
    var p2marks = new Array();
    var p3marks = new Array();
    for (var i = 0; i < gridRows.length; i++) {
        studentid[i] = gridRows[i].studentId;
        if ((classes == 'S1' || classes == 'S2' || classes == 'S3') &&
                (combo == "5" || combo == "6" || combo == "7" || combo == "8")) {
            p1marks[i] = gridRows[i].p1marks;
            p2marks[i] = gridRows[i].p2marks;
            p3marks[i] = gridRows[i].p3marks;
            exammarks[i] = "0";
            projectmarks[i] = "0";
            testmarks[i] = "0";
            oralmarks[i] = "0";
            assignmarks[i] = "0";
        }
        else {
            
            exammarks[i] = gridRows[i].exammarks;
            projectmarks[i] = gridRows[i].projectmarks;
            testmarks[i] = gridRows[i].testmarks;
            oralmarks[i] = gridRows[i].oralmarks;
            assignmarks[i] = gridRows[i].assignmarks;
            p1marks[i] = "0";
            p2marks[i] = "0";
            p3marks[i] = "0";
        }
    }

    $.ajax({
        type: "POST",
        url: encodeURI("../Home/MarksSubmit"),
        data: {
            studentId: studentid,
            exammarks: exammarks,
            projectmarks: projectmarks,
            testmarks: testmarks,
            oralmarks: oralmarks,
            assignmarks: assignmarks,
            p1marks: p1marks,
            p2marks: p2marks,
            p3marks: p3marks,
            campusId: $("#txtCampusMU > option:selected").attr("value"),
            classId: $("#txtClassMU > option:selected").attr("value"),
            moduletxt: $("#txtModuleMU > option:selected").text(),
            sectionId: $("#txtSectionMU > option:selected").attr("value"),
            subjectId: $("#txtSubjectMU > option:selected").attr("value"),
            moduleId: $("#txtModuleMU > option:selected").attr("value"),
            dateId: $("#txtDateMU").val(),
            marks: $("#txtMarksMU").val(),
            marksp2: $("#txtMarksMUP2").val(),
            marksp3: $("#txtMarksMUP3").val()
        },
        success: function (data) {
            
            if(data == "Success")
            {
                waitingDialog.hide();
                show_suc_alert_js('Marks Successfully Uploaded');
                $("#marks_div").css("display", "none");
            }
            else if (data == "Error")
            {
                waitingDialog.hide();
                show_err_alert_js('Found Some Error! Please Try Again');
            }
            else if (data == "Rights")
            {
                waitingDialog.hide();
                show_err_alert_js('You Do Not Have Rights');
                $("#marks_div").css("display", "none");
            }
        },
        error: function (data) {
            waitingDialog.hide();
            show_err_alert_js('Error Occured: Please Press Enter Key after Inserting all Marks');
        },
        dataType: "json"
    });

});



//$(document).ready(function () {

//    // Examle data for jqGrid
//    var mydata = [
//            { id: "22001", name: "Abdul Basit", marks: "74", percentage: "70%" },
//            { id: "22002", name: "Ahmed Zamir", marks: "64", percentage: "60%" },
//            { id: "22003", name: "Ali Manzar", marks: "54", percentage: "70%" },
//            { id: "22004", name: "Amir udin Ayaz", marks: "65", percentage: "65%" },
//            { id: "22005", name: "Azzaz ul Haq", marks: "77", percentage: "77%" },
//            { id: "22006", name: "Danish Walani", marks: "69", percentage: "69%" },
//            { id: "22007", name: "Hamza Iqbal", marks: "68", percentage: "68%" },
//            { id: "22008", name: "Mahboob Alam", marks: "72", percentage: "72%" },
//            { id: "22009", name: "Moiz Iqbal", marks: "70", percentage: "70%" },
//            { id: "22010", name: "Mustafa Quresh", marks: "84", percentage: "84%" },
//            { id: "22011", name: "Salman Khan", marks: "85", percentage: "85%" },
//            { id: "22012", name: "Shaheer Ali Agha", marks: "63", percentage: "63%" },
//            { id: "22013", name: "Syed Saad Ali", marks: "67", percentage: "67%" },
//            { id: "22014", name: "Talha Hussain Chohan", marks: "72", percentage: "72%" },
//            { id: "22015", name: "Taha Nadeem", marks: "83", percentage: "83%" }
//    ];

//    $("#marks_table").jqGrid({
//        data: mydata,
//        datatype: "local",
//        height: 450,
//        autowidth: true,

//        rowNum: 20,
//        rowList: [10, 20, 30],
//        colModel: [
//             { name: "id", label: "Id", width: 120 },
//            { name: "name", label: "Student Name", width: 730  },
//            { name: "marks", label: "Marks", editable: true, width: 100 },
//            { name: "percentage", label: "Percentage", width: 100}
//        ],
//        pager: "#pager_list",
//        viewrecords: true,
//        caption: "Marks Sheet",
//        add: true,
//        edit: true,
//        cellEdit: true,
//        sortIconsBeforeText: true,
//        addtext: 'Add',
//        edittext: 'Edit',
//        hidegrid: false,
//        shrinkToFit: false
//    });

//    // Add selection
// //   $("#marks_table").setSelection(4, true);


//    // Setup buttons
//    $("#marks_table").jqGrid('navGrid', '#pager_list',
//            { edit: true, add: true, del: true, search: true },
//            { height: 300, reloadAfterSubmit: true }
//    );

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.marks_jqGrid').width();

//        $('#marks_table').setGridWidth(width, false);
//    });

//    $("#marks_div").hide();

//    $("#marks_table").jqGrid('setGridState', 'hidden');//or 'hidden'

//    $("#showGrid1").click(function (e) {
//        if ($("#marksuploadform").valid()){
//            $("#marks_div").show();
//            $("#marks_table").jqGrid('setGridState', 'visible');//or 'hidden' 
//        }
//        return false;
//    });

//    $('#datepickerMarks').datepicker();

//    $("#datepickerAttendance").datepicker();

//    jQuery.validator.addMethod("lettersonly", function (value, element) {
//        return this.optional(element) || /^[a-zA-Z.\s]*$/.test(value);
//    }, "Only alphabetical characters");

//$("#marksuploadform").validate({
//        errorClass: "my-error-class",
//        validClass: "my-valid-class",
//        rules: {
//            txtMarksMU: {
//                number: true,
//            }
//        },
//        messages: {
//            txtMarksMU: {

//                number: "Enter numbers only",
//            }
//        }
//    });
//});

