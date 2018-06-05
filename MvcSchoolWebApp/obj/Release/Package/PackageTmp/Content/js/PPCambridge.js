$("#txtCampusPPC").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Preprimary/getClassJson"),
        data: { campusId: $("#txtCampusPPC > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtClassPPC").html(items.join(' '));
        }
    });
    return false;
});

$("#txtClassPPC").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Preprimary/getSectionJson"),
        data: {
            campusId: $("#txtCampusPPC > option:selected").attr("value"),
            classId: $("#txtClassPPC > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSectionPPC").html(items.join(' '));
        }
    });
    return false;
});

$("#txtSectionPPC").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Preprimary/getSubjectJson"),
        data: {
            campusId: $("#txtCampusPPC > option:selected").attr("value"),
            classId: $("#txtClassPPC > option:selected").attr("value"),
            sectionId: $("#txtSectionPPC > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Subject</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#txtSubjectPPC").html(items.join(' '));
        }
    });
    return false;
});
/**************** Dynamic Grid   *******************/
var colModels = new Array();
var colName = new Array();
var colId = new Array();

$("#PPCForm").submit(function (e) {
    var $subname = $("#txtSubjectPPC > option:selected").text();

    $.ajax({
        url: encodeURI("../Preprimary/getPPCJQGridJson"),
        data: {
            campus: $("#txtCampusPPC > option:selected").attr("value"),
            classes: $("#txtClassPPC > option:selected").attr("value"),
            section: $("#txtSectionPPC > option:selected").attr("value"),
            subject: $("#txtSubjectPPC > option:selected").attr("value"),
            teacher: $("#txtTeacherPPC").val(),
            date: $("#txtDatePPC > option:selected").attr("value"),
            module: $("#txtModulePPC > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#PPC_div").show();
            colModels = BuildColumnModelPPC(data);
            colName = BuildColumnNamePPC(data);
            colId = BuildColumnIdPPC(data);
            BuildStudentDataPPC();
        },
        error: function (error) {
            show_err_alert_js('Error Occured');
            $("#PPC_div").css("display", "none");
        }
    });

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.PPC_jqGrid').width();
        $('#PPC_table').setGridWidth(width, false);
    });
    return false;
});

//binding the Dyanamic Colmodel
function BuildColumnModel(data) {
    var $screensize = $("#PPC_div").width() - 42;
    var $size = ($screensize / 100);
    var columns = [];
    for (var i = 0; i < data.length ; i++) {
        if (data[i].studentName == null) {
            if (data[i].field == "col0")
                columns.push({ name: data[i].field, index: data[i].field, width: $size * 20, resizable: false, align: 'left' });
            else {
                columns.push({
                    name: data[i].field, index: data[i].field, editable: true, align: 'left', resizable: false, width: $size * 20, edittype: "select",
                    formatter: 'select',
                    editoptions: { value: "Excellent:Excellent;Very Good:Very Good;Good:Good;Fair:Fair;Work Hard:Work Hard;Work Extremely Hard:Work Extremely Hard" },
                });
            }
        }
    }
    return columns;
}

function BuildColumnName(data) {
    var labels = [];
    for (var i = 0; i < data.length ; i++) {
        if (data[i].collabl != null)
            labels.push(data[i].collabl);
    }
    return labels;
}

function BuildColumnIdPPC(data) {
    var labelsid = [];
    for (var i = 0; i < data.length ; i++) {
        if (data[i].fieldid != null)
            labelsid.push(data[i].fieldid);
    }
    return labelsid;
}

function BuildStudentData() {
    $.ajax({
        url: encodeURI("../Preprimary/getPPCDataJQGridJson"),
        data: {
            campus: $("#txtCampusPPC > option:selected").attr("value"),
            classes: $("#txtClassPPC > option:selected").attr("value"),
            section: $("#txtSectionPPC > option:selected").attr("value"),
            subject: $("#txtSubjectPPC > option:selected").attr("value"),
            teacher: $("#txtTeacherPPC").val(),
            date: $("#txtDatePPC > option:selected").attr("value"),
            module: $("#txtModulePPC > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var caption = $("#txtSubjectPPC > option:selected").text();
            jQuery('#PPC_table').jqGrid('clearGridData');
            $("#PPC_table").jqGrid("GridUnload");
            $('#PPC_table').jqGrid('setGridParam', { data: data });
            $("#PPC_table").jqGrid('setGridState', 'visible');

            $("#PPC_table").jqGrid({
                datatype: "local",
                colNames: colName,
                colModel: colModels,
                data: data,
                styleUI: 'Bootstrap',
                mtype: 'GET',
                height: 250,
                autoheight: true,
                autowidth: true,
                loadonce: true,
                rowNum: 10,
                viewrecords: true,
                iconSet: "fontAwesome",
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                cellEdit: true,
                editable: true,
                pager: "#PPC_pager_list",
                caption: caption,
                add: true,
                edit: true,
                addtext: 'Add',
                edittext: 'Edit',
                hidegrid: false,
                shrinkToFit: false,
                cellsubmit: 'clientArray'

            }).trigger('reloadGrid', [{ current: true }]);
        },
        error: function (error) {
            show_err_alert_js('Error Occured');
            $("#PPC_div").css("display", "none");
        }
    });
}

/**************** Dynamic Grid   ****************/

/**************** Dynamic Grid Insertion ****************/
$("#btnUploadMarksppc").click(function (e) {
    e.preventDefault();
    waitingDialog.show('Please Wait: This May Take a While');
    var gridRows = $("#PPC_table").jqGrid('getRowData');
    var rowData = new Array();

    var griddata = Create2DArray(gridRows.length, colName.length);
    debugger
    for (var i = 0; i < gridRows.length; i++) {
        for (var j = 0; j < colName.length; j++) {
            var name = "col" + j;
            griddata[i][j] = gridRows[i][name];
        }
    }

    $.ajax({
        type: "POST",
        url: encodeURI("../Preprimary/PPMarksSubmit"),
        data: {
            campusId: $("#txtCampusPPC > option:selected").attr("value"),
            classId: $("#txtClassPPC > option:selected").attr("value"),
            sectionId: $("#txtSectionPPC > option:selected").attr("value"),
            subjectId: $("#txtSubjectPPC > option:selected").attr("value"),
            dateId: $("#txtDatePPC").val(),
            griddata: griddata,
            colName: colName
        },
        success: function (data) {

            if (data == "Success") {
                waitingDialog.hide();
                show_suc_alert_js('Marks Successfully Uploaded');
                $("#dvborder").css("display", "none");
            }
            else if (data == "Error") {
                waitingDialog.hide();
                show_err_alert_js('Found Some Error! Please Try Again');
            }
            else if (data == "Rights") {
                waitingDialog.hide();
                show_err_alert_js('You Do Not Have Rights');
            }
        },
        error: function (data) {
            waitingDialog.hide();
            show_err_alert_js('Error Occured: Please Press Enter Key after Inserting all Marks');
        },
        dataType: "json"
    });

});

function Create2DArray(rows, columns) {
    var x = new Array(rows);
    for (var i = 0; i < rows; i++) {
        x[i] = new Array(columns);
    }
    return x;
}
/**************** Dynamic Grid Insertion ****************/