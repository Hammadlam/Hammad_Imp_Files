$(document).ready(function (e) {
    $('.dataTables-example').dataTable({
        //responsive: true,
        "dom": 'T<"clear">lfrtip',
        "tableTools": {
            "sSwfPath": "js/plugins/dataTables/swf/copy_csv_xls_pdf.swf"
        }
    });
});

$("#publishresult").submit(function () {
    waitingDialog.show('Please Wait: This May Take a While');
    var pubstat = new Array();
    var classid = new Array();
    var sectionid = new Array();
    var campusid;
    var moduleid;
    var radios = $(".gradeX :input[type=radio]").length/2;
    for (var i = 0; i < radios; i++ )
    {
        var pub = 'pub ' + i;
        var cls = 'cls ' + i;
        var sec = 'sec ' + i;
        campusid = $("#txtcampuspr > option:selected").attr("value");
        moduleid = $("#txtmodulepr > option:selected").attr("value");
        pubstat[i] = $("input[type='radio'][name='"+pub+"']:checked").val();
        classid[i] = $("input[type='hidden'][name='"+cls+"']").val();
        sectionid[i] = $("input[type='hidden'][name='"+sec+"']").val();
    }
    
    $.ajax({
        type: "POST",
        url: encodeURI("../Result/submitpublishresult"),
        data: { campusid: campusid, moduleid: moduleid, pubstat: pubstat, classid: classid, sectionid: sectionid},
        success: function (data) {
            if (data == "Success") {
                waitingDialog.hide();
                show_suc_alert_js('Successfully Updated');
            }
            else if (data == "Error")
            {
                waitingDialog.hide();
                show_err_alert_js('Found Some Error! Please Try Again');
            }
        },
        error: function (data)
        {
            waitingDialog.hide();
            show_err_alert_js('Found Some Error! Please Try Again');
        }
    });
    return false;
});

//$("#txtcampuspr").change(function () {
//    $.ajax({
//        type: "POST",
//        url: encodeURI("../Result/getClassJson"),
//        data: { campusId: $("#txtcampuspr > option:selected").attr("value") },
//        success: function (data) {
//            var items = [];
//            items.push("<option value =''>Class</option>");
//            $.each(data, function () {
//                items.push("<option value=" + this.Value + ">" + this.Text + "</option>");
//            });
//            $("#txtclasspr").html(items.join(' '));
//        }
//    });
//    return false;
//});

//$("#formpublishresult").submit(function () {
//    $.ajax({
//        type: "POST",
//        url: encodeURI("../Result/getGridJsonresult"),
//        data: { campusId: $("#txtcampuspr > option:selected").attr("value"), classId: $("#txtclasspr > option:selected").attr("value"), moduleId: $("#txtmodulepr > option:selected").attr("value") },
//        success: function (data) {
//            var result = "";

//            booksDiv.html('');

//            $.each(data, function (class, section, subject) {

//                result +=   '<tr>' +
//                            '<td>' + book.Title + '</td>' +
//                            '<td>' + book.Title + '</td>' +
//                            '<td>' + book.Title + '</td>' +
//                            '<td>' + book.Title + '</td>' +
//                            '</tr>';
//            });

//            booksDiv.html(result);
//        }
//    });
//    return false;
//});
    