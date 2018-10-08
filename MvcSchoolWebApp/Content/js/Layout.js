

/******************************* Refresh Notifications Start *****************************/
window.setInterval(function (e) {
    $.ajax({
        url: encodeURI("../Message/RefreshMsg"),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data.num != null) {

                $("#nof_layout").html(data.num);
                if (data.notf != null) {
                    debugger
                    for (var i = 0; i < Object.keys(data.notf).length; i++) {
                        if (data.notf[i].recordno == null && data.notf[i].Groupid == null) {
                            data.notf[i].recordno = "";
                            data.notf[i].Groupid = "";
                        }
                        var alldata = data.notf[i].MsgId + "?" + data.notf[i].Groupid + "?" + data.notf[i].recordno;
                        $("#notf_menu").prepend('<li style="background-color:#e5e5e5;border:1px solid #ffffff;"> <a id="MsgSubject" onclick="GetMsgid(this);" value=' + alldata + '> <label style="font-weight:600; color:#ca434d;">' + data.notf[i].senderId + '</label><br /> ' + data.notf[i].Subject + ' <p class="pull-right msg-time"> ' + data.notf[i].msgdate + ' </p> </a> </li>');
                    }
                    var x = document.getElementById("snackbar");
                    x.className = "show";
                    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
                }
            }
            else {

            }
        },
        error: function (error) {

        }
    });
}, 10000);

/******************************* Refresh Notifications End *****************************/


function destroy_session() {
    window.location.href = "/dashboard/destroy_session";
}



function Demo(anchor) {
    var value = anchor.getAttribute('value')
    $.ajax({
        type: 'POST',
        url: '/result/fncresulttype',
        data: "dataToSave=" + value,
        success: function (result) {
            window.location.href = result;
        },
        error: function (err, result) {
            //alert("Error in assigning dataToSave" + err.responseText);
        }
    });
}

function down(anchor) {
    var value = anchor.getAttribute('value')
    $.ajax({
        type: 'POST',
        url: '/download/fundownload',
        data: "dataToSave=" + value,
        success: function (result) {
            window.location.href = result;
        },
        error: function (err, result) {
        }
    });
}

function redirectpersonalinfo(anchor) {
    var value = anchor.getAttribute('value');
    $.ajax({
        type: 'POST',
        url: '/personalinfo/funcpersonal',
        data: "dataToSave=" + value,
        success: function (result) {
            window.location.href = result;
        },
        error: function (err, result) {
        }
    });
}

$("#popupform_prsnlinfo").submit(function (e) {
    e.preventDefault();
    var value = $("#popupform_prsnlinfo input:radio:checked").val();
    $.ajax({
        type: 'POST',
        url: '/personalinfo/funcpersonal',
        data: "dataToSave=" + value,
        success: function (result) {
            debugger
            window.location.href = result;
        },
        error: function (err, result) {
        }
    });
});

function GetMsgid(anchor) {
    var value = anchor.getAttribute('value')

    $.ajax({
        type: 'POST',
        url: '/Message/GetMsgId',
        data: "sendmsgid=" + value,
        success: function (result) {
            if (result != "") {
                var getid = result;
                window.location.href = getid;
            }
        },
        error: function (err, result) {
        }
    });
}

$(document).ready(function () {
    $(".treeview ul li").on('click', 'li', function () {
        debugger
        $(".treeview").removeClass("active");
        // adding classname 'active' to current click li
        $(this).parent("treeview").addClass("active");
    });
});