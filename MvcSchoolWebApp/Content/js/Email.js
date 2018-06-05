$(document).ready(function (e) {
    $("#txtReceiver").on('input', function (e) {
        if ($("#txtReceiver").val() != "") {
            $("#txtgroupid").val("");
            $('#txtgroupid').prop('disabled', true);
        }
        else {
            $('#txtgroupid').removeAttr('disabled');
        }
    });

    $("#txtgroupid").on('input', function (e) {
        if ($("#txtgroupid").val() != "") {
            $("#txtReceiver").val("");
            $('#txtReceiver').prop('disabled', true);
        }
        else {
            $('#txtReceiver').removeAttr('disabled');
        }
    });
});