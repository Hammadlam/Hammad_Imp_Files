$("#passpopupform").on('submit', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $.ajax({
        url: encodeURI("../Home/changepassword"),
        data: {
            currpass: $("#txtoldpass").val(),
            newpass: $("#txtnewpass").val(),
            conpass: $("#txtconpass").val()
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data)
        {
            $('#ChangePassword').modal('hide');

            if (data == "Success")
            {
                show_suc_alert_js('Successfully Changed Password');
                $('#txtoldpass').val('');
                $('#txtnewpass').val('');
                $('#txtconpass').val('');
            }
            else if(data == "Error")
            {
                show_err_alert_js('Old Password is not Correct');
            }
        },
        error: function (data)
        {
            $('#ChangePassword').modal('hide');
            show_err_alert_js('Error Occured While Changing Password');
        }
    });
});

$("#txtnewpass").on('input', function (e) {
    var password1 = $("#txtnewpass").val();
    var password2 = $("#txtconpass").val();
    if (password1 == password2 && password1 != "") {
        $("#pass-validator").html("Password Matched");
        $('#savepass').prop('disabled', false);
    }
    else {
        $("#pass-validator").html("");
        $('#savepass').prop('disabled', true);
    }
});

$("#txtconpass").on('input', function (e) {
    var password1 = $("#txtnewpass").val();
    var password2 = $("#txtconpass").val();
    if (password1 == password2 && password2 != "") {
        $("#pass-validator").html("Password Matched");
        $('#savepass').prop('disabled', false);
    }
    else {
        $("#pass-validator").html("");
        $('#savepass').prop('disabled', true);
    }
});

//function validatepass()
//{
//    var password1 = $("#txtnewpass").val();
//    var password2 = $("#txtconpass").val();
//    if (password1 == password2) {
//        alert('Password Matched');
//        $('#savepass').prop('disabled', false);
//    }
//    else {
//        $('#savepass').prop('disabled', true);
//    }
//}