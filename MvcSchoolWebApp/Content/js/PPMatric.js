$("#ppmCampus").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Preprimary/getClassJson"),
        data: { campusId: $("#ppmCampus > option:selected").attr("value") },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Class/Level</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#ppmClass").html(items.join(' '));
        }
    });
    return false;
});

$("#ppmClass").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Preprimary/getSectionJson"),
        data: {
            campusId: $("#ppmCampus > option:selected").attr("value"),
            classId: $("#ppmClass > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Section</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#ppmSection").html(items.join(' '));
        }
    });
    return false;
});

$("#ppmSection").change(function () {
    $.ajax({
        type: "POST",
        url: encodeURI("../Preprimary/getSubjectJson"),
        data: {
            campusId: $("#ppmCampus > option:selected").attr("value"),
            classId: $("#ppmClass > option:selected").attr("value"),
            sectionId: $("#ppmSection > option:selected").attr("value")
        },
        success: function (data) {
            var items = [];
            items.push("<option value=''>Subject</option>");
            $.each(data, function () {
                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
            });
            $("#ppmSubject").html(items.join(' '));
        }
    });
    return false;
});




/**************** Dynamic Grid   *******************/
var colModels = new Array();
var colName = new Array();
var colId = new Array();

$("#ppmform").submit(function (e) {
    var $subname = $("#ppmSubject > option:selected").text();

    $.ajax({
        url: encodeURI("../Preprimary/getJQGridJson"),
        data: {
            campus: $("#ppmCampus > option:selected").attr("value"),
            classes: $("#ppmClass > option:selected").attr("value"),
            section: $("#ppmSection > option:selected").attr("value"),
            subject: $("#ppmSubject > option:selected").attr("value"),
            teacher: $("#ppmTeacher").val(),
            date: $("#ppmDate > option:selected").attr("value"),
            module: $("#ppmModule > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#ppm_upload_div").show();
            colModels = BuildColumnModelPPM(data);
            colName = BuildColumnNamePPM(data);
            colId = BuildColumnIdPPM(data);
            BuildStudentDataPPM();
        },
        error: function (error) {
            show_err_alert_js('Error Occured');
            $("#ppm_upload_div").css("display", "none");
        }
    });

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.ppmac_jqGrid').width();
        $('#ppmac_table').setGridWidth(width, false);
    });
    return false;
});

//binding the Dyanamic Colmodel
function BuildColumnModelPPM(data) {
    var $screensize = $("#ppm_upload_div").width() - 42;
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

function BuildColumnNamePPM(data) {
    var labels = [];
    for (var i = 0; i < data.length ; i++) {
        if (data[i].collabl != null)
            labels.push(data[i].collabl);
    }
    return labels;
}

function BuildColumnIdPPM(data) {
    var labelsid = [];
    for (var i = 0; i < data.length ; i++) {
        if (data[i].fieldid != null)
            labelsid.push(data[i].fieldid);
    }
    return labelsid;
}

function BuildStudentDataPPM() {
    $.ajax({
        url: encodeURI("../Preprimary/getDataJQGridJson"),
        data: {
            campus: $("#ppmCampus > option:selected").attr("value"),
            classes: $("#ppmClass > option:selected").attr("value"),
            section: $("#ppmSection > option:selected").attr("value"),
            subject: $("#ppmSubject > option:selected").attr("value"),
            teacher: $("#ppmTeacher").val(),
            date: $("#ppmDate > option:selected").attr("value"),
            module: $("#ppmModule > option:selected").attr("value")
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var caption = $("#ppmSubject > option:selected").text();
            jQuery('#ppmac_table').jqGrid('clearGridData');
            $("#ppmac_table").jqGrid("GridUnload");
            $('#ppmac_table').jqGrid('setGridParam', { data: data });
            $("#ppmac_table").jqGrid('setGridState', 'visible');

            $("#ppmac_table").jqGrid({
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
                rowNum: 100000,
                viewrecords: true,
                iconSet: "fontAwesome",
                sortorder: "desc",
                threeStateSort: true,
                sortIconsBeforeText: true,
                headertitles: true,
                cellEdit: true,
                editable: true,
                pager: "#pager_list",
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
            $("#ppm_upload_div").css("display", "none");
        }
    });
}

/**************** Dynamic Grid   ****************/



/**************** Dynamic Grid Insertion ****************/
$("#btnUploadMarksppm").click(function (e) {
    e.preventDefault();
    waitingDialog.show('Please Wait: This May Take a While');
    var gridRows = $("#ppmac_table").jqGrid('getRowData');
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
            campusId: $("#ppmCampus > option:selected").attr("value"),
            classId: $("#ppmClass > option:selected").attr("value"),
            sectionId: $("#ppmSection > option:selected").attr("value"),
            subjectId: $("#ppmSubject > option:selected").attr("value"),
            dateId: $("#ppmDate").val(),
            griddata: griddata,
            colName: colName
        },
        success: function (data) {

            if (data == "Success") {
                waitingDialog.hide();
                show_suc_alert_js('Marks Successfully Uploaded');
                $("#ppm_upload_div").css("display", "none");
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