/*** Resultt JS ***/
$("#txtCampusRI").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Result/getClassJson"),
        data: { campusId: $("#txtCampusRI > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtClassRI").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassRI").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Result/getSectionJson"),
        data: {
            campusId: $("#txtCampusRI > option:selected").attr("value"),
            classId: $("#txtClassRI > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtSectionRI").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionRI").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Result/getstudentname"),
        data: {
            campusId: $("#txtCampusRI > option:selected").attr("value"),
            classId: $("#txtClassRI > option:selected").attr("value"),
            sectionId: $("#txtSectionRI > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Student</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtnameRI").html(items.join(' '));
        }
    });
    return false;

});

$("#txtnameRI").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Result/getsubjectJSON"),
        data: {
            campusId: $("#txtCampusRI > option:selected").attr("value"),
            classId: $("#txtClassRI > option:selected").attr("value"),
            sectionId: $("#txtSectionRI > option:selected").attr("value"),
            nameId: $("#txtnameRI > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Subject</option>");
            $.each(data,
                function () {
                    items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
                });
            $("#txtSubjectRI").html(items.join(' '));
        }
    });

    return false;
});


$("#resultform").submit(function (e) {
    e.preventDefault();
    
    $.ajax({
        url: encodeURI("../Result/getGridJson"),
        data: {
            campus: $("#txtCampusRI > option:selected").attr("value"),
            classes: $("#txtClassRI > option:selected").attr("value"),
            section: $("#txtSectionRI > option:selected").attr("value"),
            nameId: $("#txtnameRI > option:selected").attr("value"),
            subjectid: $("#txtSubjectRI > option:selected").attr("value")
        },

        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            jQuery('#table_list_2').jqGrid('clearGridData');
            $('#table_list_2').jqGrid('setGridParam', { data: data });

            $("#quiz_div").show();
            $("#table_list_2").jqGrid('setGridState', 'visible');//or 'hidden'
            //$('#table_list_2').trigger('reloadGrid');
            
            var $screensize = $("#quiz_div").width() - 42;
            var $size = ($screensize / 100);

            $("#table_list_2").jqGrid({
                datatype: "local",
                colNames: ['Name', 'Exam Marks', 'Project/Assignment Marks', 'Test Marks', 'Oral marks', 'Assignment Marks', 'Paper 1 Marks', 'Paper 2 Marks', 'Paper 3 Marks', 'Total Obtained', 'Total Marks', '(%)'],
                colModel: [
                    //width: "350", resizable: false, editable: false, align: 'center'
                    //  { name: 'id', index: 'id', editable: false, width: 100, align: 'left', resizable: false, sorttype: "int", search: true },
                    { name: 'resulttype', index: 'resulttype', editable: false, width: $size*15, align: 'left', resizable: false, search: true },
                    { name: 'examinationmarks', index: 'examinationmarks', editable: false, width: $size*10, align: 'left', resizable: false, search: true },
                    { name: 'projectmarks', index: 'projectmarks', editable: false, width: $size*20, align: 'left', resizable: false, search: true },
                    { name: 'testmarks', index: 'testmarks', editable: false, width: $size*10, align: 'left', resizable: false, search: true },
                    { name: 'oralmarks', index: 'oralmarks', editable: false, width: $size*10, align: 'left', resizable: false, search: true },
                    { name: 'assignmentmarks', index: 'assignmentmarks', editable: false, width: $size * 15, align: 'left', resizable: false, search: true },
                    { name: 'p1marks', index: 'p1marks', editable: false, width: $size * 13, align: 'left', resizable: false, search: true },
                    { name: 'p2marks', index: 'p2marks', editable: false, width: $size * 13, align: 'left', resizable: false, search: true },
                    { name: 'p3marks', index: 'p3marks', editable: false, width: $size * 13, align: 'left', resizable: false, search: true },
                    { name: 'obtainedmarks', index: 'obtainedmarks', editable: false, width: $size*12, align: 'left', resizable: false, sortable: false },
                    { name: 'totalmarks', index: 'totalmarks', editable: false, width: $size * 10, align: 'left', resizable: false, sortable: false },
                    { name: 'percentage', index: 'percentage', editable: false, width: $size * 10, align: 'left', resizable: false, sortable: false },

                ],
                data: data,
                styleUI: 'Bootstrap',
                height: 250,
                autoheight: true,
                autowidth: true,
               // rowNum: 10,
                mtype: 'GET',
                pager: "#pager_list_2",
                viewrecords: true,
                loadonce: true,
                hidegrid: false,
                shrinkToFit: false,
                caption: "Student Performance"

            }).trigger('reloadGrid', [{ current: true }]);

            //.trigger('reloadGrid', [{current: true}])
            //var dataAjax = data.responseText;
            ////return dataAjax;
            //return data;
        },
        error: function (error)
        {
            show_err_alert_js('No Record Found');
            $("#quiz_div").css("display","none");
        }
       
    });
    
});

$(window).bind('resize', function () {
    var width = $('.jqGrid_wrapper').width();

    $('#table_list_2').setGridWidth(width, false);
});
//var gridData = new Array();
//gridData = resData;

//for (var i = 0; i <= gridData.length; i++)
//    $("#table_list_2").jqGrid('addRowData', i + 1, gridData[i]);

//        var $grid = $('#table_list_2');
//        $grid.jqGrid('setGridParam',
//        {
//            data: data,
//            datatype: "json",
//            colNames: [' Name', 'Total', 'Obtained', '(%)'],
//            colModel: [
//                //width: "350", resizable: false, editable: false, align: 'center'
//                //  { name: 'id', index: 'id', editable: false, width: 100, align: 'left', resizable: false, sorttype: "int", search: true },
//                { name: 'resulttype', index: 'resulttype', editable: false, width: 675, align: 'left', resizable: false, search: true },
//                //{ name: 'status', index: 'status', editable: false, width: 50, align: 'left', resizable: false}

//                { name: 'totalmarks', index: 'totalmarks', editable: false, width: 90, align: 'left', resizable: false, sortable: false },
//                { name: 'obtainedmarks', index: 'obtainedmarks', editable: false, width: 90, align: 'left', resizable: false, sortable: false },
//                { name: 'percentage', index: 'percentage', editable: false, width: 90, align: 'left', resizable: false, sortable: false },

//            ],
//            styleUI: 'Bootstrap',
//            height: 250,
//            autoheight: true,
//            autowidth: true,
//            rowNum: 10,
//            mtype: 'GET',
//            pager: "#pager_list_2",
//            viewrecords: true,
//            loadonce: true,
//            hidegrid: false,
//            shrinkToFit: false,
//            caption: "Student Performance"
//        });
//    }
//});

//    datatype: "json",
//    colNames: [' Name', 'Total', 'Obtained', '(%)'],
//    colModel: [
//        //width: "350", resizable: false, editable: false, align: 'center'
//        //  { name: 'id', index: 'id', editable: false, width: 100, align: 'left', resizable: false, sorttype: "int", search: true },
//        { name: 'resulttype', index: 'resulttype', editable: false, width: 675, align: 'left', resizable: false, search: true },
//        //{ name: 'status', index: 'status', editable: false, width: 50, align: 'left', resizable: false}

//        { name: 'totalmarks', index: 'totalmarks', editable: false, width: 90, align: 'left', resizable: false, sortable: false },
//        { name: 'obtainedmarks', index: 'obtainedmarks', editable: false, width: 90, align: 'left', resizable: false, sortable: false },
//        { name: 'percentage', index: 'percentage', editable: false, width: 90, align: 'left', resizable: false, sortable: false },

//    ],
//    styleUI: 'Bootstrap',
//    height: 250,
//    autoheight: true,
//    autowidth: true,
//    rowNum: 10,
//    mtype: 'GET',
//    pager: "#pager_list_2",
//    viewrecords: true,
//    loadonce: true,
//    hidegrid: false,
//    shrinkToFit: false,
//    caption: "Student Performance"

//});




//$("#table_list_2").jqGrid({

//    url: encodeURI("../Result/getGridJson"),
//    postData: {
//        campus: $("#txtCampusRI > option:selected").attr("value"),
//        classes: $("#txtClassRI > option:selected").attr("value"),
//        section: $("#txtSectionRI > option:selected").attr("value"),
//        nameId: $("#txtnameRI > option:selected").attr("value"),
//        subjectid: $("#txtSubjectRI > option:selected").attr("value"),
//    },
//    datatype: "json",
//    colNames: [' Name', 'Total', 'Obtained', '(%)'],
//    colModel: [
//        //width: "350", resizable: false, editable: false, align: 'center'
//        //  { name: 'id', index: 'id', editable: false, width: 100, align: 'left', resizable: false, sorttype: "int", search: true },
//        { name: 'resulttype', index: 'resulttype', editable: false, width: 675, align: 'left', resizable: false, search: true },
//        //{ name: 'status', index: 'status', editable: false, width: 50, align: 'left', resizable: false}

//        { name: 'totalmarks', index: 'totalmarks', editable: false, width: 90, align: 'left', resizable: false, sortable: false },
//        { name: 'obtainedmarks', index: 'obtainedmarks', editable: false, width: 90, align: 'left', resizable: false, sortable: false },
//        { name: 'percentage', index: 'percentage', editable: false, width: 90, align: 'left', resizable: false, sortable: false },

//    ],
//    styleUI: 'Bootstrap',
//    height: 250,
//    autoheight: true,
//    autowidth: true,
//    rowNum: 10,
//    mtype: 'GET',
//    pager: "#pager_list_2",
//    viewrecords: true,
//    loadonce: true,
//    hidegrid: false,
//    shrinkToFit: false,
//    caption: "Student Performance"

//});


//$("#table_list_2").jqGrid('setGridParam', {
//    postData: { "limit": limit }
//}).trigger('reloadGrid');





//$(document).ready(function () {


//    // Examle data for jqGrid
//    var mydata = [
//        { id: "1", name: "Quiz1", total: "50", obtained: "40", percentage: "80%" },
//        { id: "2", name: "Quiz2", total: "50", obtained: "40", percentage: "80%" },
//        { id: "3", name: "Quiz3", total: "50", obtained: "40", percentage: "80%" },
//        { id: "4", name: "Quiz4", total: "50", obtained: "40", percentage: "80%" },
//        { id: "5", name: "Quiz5", total: "50", obtained: "40", percentage: "80%" },
//        { id: "6", name: "Quiz6", total: "50", obtained: "40", percentage: "80%" },

//    ];

//    // Configuration for jqGrid Example 1


//    // Configuration for jqGrid Example 2
//    $("#table_list_2").jqGrid({
//        data: mydata,
//        datatype: "local",
//        height: 450,
//        autowidth: true,
//        rowNum: 20,
//        rowList: [10, 20, 30],
//        colNames: ['S No', ' Name', 'Total', 'Obtained', '(%)'],
//        colModel: [
//            //width: "350", resizable: false, editable: false, align: 'center'
//            { name: 'id', index: 'id', editable: false, width: 100, align: 'left', resizable: false, sorttype: "int", search: true },
//            { name: 'name', index: 'name', editable: false, width: 675, align: 'left', resizable: false, search: true },
//            //{ name: 'status', index: 'status', editable: false, width: 50, align: 'left', resizable: false}

//            { name: 'total', index: 'total', editable: false, width: 90, align: 'left', resizable: false, sortable: false },
//            { name: 'obtained', index: 'obtained', editable: false, width: 90, align: 'left', resizable: false, sortable: false },
//            { name: 'percentage', index: 'percentage', editable: false, width: 90, align: 'left', resizable: false, sortable: false },


//        ],

//        //gridComplete: function () {
//        //    var ids = jQuery("#table_list_2").getDataIDs();
//        //    for (var i = 0; i < ids.length; i++) {
//        //        var cl = ids[i];
//        //        //class='btn btn-xs btn-info'
//        //        view = "<input style='height:18px;width:75px;' type='button' onclick='getOnClick()' value='View' class='btn btn-xs btn-primary'>";
//        //        download = "<input style='height:18px;width:75px;' type='button' class='btn btn-xs btn-primary' value='Download' />";

//        //        jQuery("#table_list_2").setRowData(ids[i], { status: view + download })
//        //    }
//        //},

//        pager: "#pager_list_2",
//        viewrecords: true,
//        //caption: "Quiz",
//        add: true,
//        edit: true,
//        addtext: 'Add',
//        edittext: 'Edit',
//        hidegrid: false,
//        shrinkToFit: false,
//        caption: "Student Performance"
//    });

//    // Add selection
//    $("#table_list_2").setSelection(4, true);


//    // Setup buttons
//    $("#table_list_2").jqGrid('navGrid', '#pager_list_2',
//        { edit: true, add: true, del: true, search: true },
//        { height: 200, reloadAfterSubmit: true }
//    );

//    // Add responsive to jqGrid
//    $(window).bind('resize', function () {
//        var width = $('.jqGrid_wrapper').width();

//        $('#table_list_2').setGridWidth(width, false);
//    });

//    $("#quiz_div").hide();

//    $("#table_list_2").jqGrid('setGridState', 'hidden');//or 'hidden' 

//    $("#btnviewresult").click(function (e) {
//        if ($("#resultform").valid()) {
//            $("#quiz_div").show();
//            $("#table_list_2").jqGrid('setGridState', 'visible');//or 'hidden' 
//        }
//        return false;
//    });

//    $("#resultform").validate({
//        errorClass: "my-error-class",


//        rules: {
//            txtSubject: {
//                required: true,
//            },
//            txtSection: {
//                required: true,
//            },
//            txtName: {
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
//            txtSubject: {
//                required: "",
//            },
//            txtSection: {
//                required: ""
//            },
//            txtName: {
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
//function getOnClick() {

//    //$.ajax({
//    //    type: 'POST',
//    //    url: "/result/getreport",
//    //    success: function (result) {
//    //    },
//    //    error: function () {
//    //        alert("error in alert");
//    //    }
//    //});
//    //  window.location.href = "/result/getreport"; 
//    window.location.href = "/result/getreport";
//}



