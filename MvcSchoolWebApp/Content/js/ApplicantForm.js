$('.next-btn').click(function () {
    $('#tabs a[href=#education]').tab('show');
});

var mydata = [
{ id: 'Post Graduation', institurename: "", date: "", degree: "", majors: "", percentage: "" },
{ id: 'Graduation', institurename: "", date: "", degree: "", majors: "", percentage: "" },
{ id: 'Intermediate', institurename: "", date: "", degree: "", majors: "", percentage: "" },
{ id: 'Others', institurename: "", date: "", degree: "", majors: "", percentage: "" }

];

var $screensize = $("#basicinfo").width() - 42;
var $size = ($screensize / 100);
$('#id').click(function () {
    $("#education_table").jqGrid({
        styleUI: 'Bootstrap',
        height: 250,
        autoheight: true,
        autowidth: true,
        //shrinkToFit: true,
        rowNum: 10,
        colNames: [' ', 'Institute Name', 'Date', 'Degree', 'Majors', "Percentage"],
        colModel: [
            { name: 'id', index: 'id', align: 'center', width: $size * 10, resizable: false },
            { name: 'institurename', index: 'institurename', width: $size * 35, search: true, resizable: false, editable: true },
            { name: 'date', index: 'date', width: $size * 20, search: true, resizable: false, editable: true },
            { name: 'degree', index: 'degree', width: $size * 15, search: true, resizable: false, editable: true },
            { name: 'majors', index: 'majors', classes: "wrap", width: $size * 35, resizable: false, editable: true },
            { name: 'percentage', index: 'percentage', width: $size * 10, resizable: false, editable: true }
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
        pager: "#education_pager_list",
        viewrecords: true,
        hidegrid: false,
        shrinkToFit: false,
        cellEdit: true,
        editable: true,
    });

    // Add responsive to jqGrid
    $(window).bind('resize', function () {
        var width = $('.education_jqGrid').width();
        $('#education_table').setGridWidth(width, false);
    });

});