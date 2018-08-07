
TxtHide();
$("#btnEdit").click(function () {
    LblHide();
    TxtValue();
    TxtShow();
    return false;
});

$("#btnSave").click(function () {
    TxtHide();
    LblValue();
    LblShow();
    return false;
});


function LblHide() {
    $("#lblempid").hide();
    $("#lblfullname").hide();
    $("#lblemplastname").hide();
    $("#lblcnic").hide();
    $("#lbljoindate").hide();
    $("#lblgender").hide();
    $("#lbldesignation").hide();


    $("#lblName1").hide();
    $("#lblName2").hide();
    $("#lblStrt1").hide();
    $("#lblStrt2").hide();
    $("#lblStrt4").hide();
    $("#lblZip").hide();
    $("#lblCity").hide();
    $("#lblProv").hide();
    $("#lblCtry").hide();
    $("#lblPhone").hide();
    $("#lblFex").hide();
    $("#lblEmail").hide();
    $("#lblWeb").hide();
    $("#txtcnic").hide();
}

function LblValue() {
    $("#txtempid").val($("#lblempid").html().trim());
    $("#txtfullname").val($("#lblfullname").html().trim());
    $("#txtemplastname").val($("#lblemplastname").html().trim());
    $("#txtcnic").val($("#lblcnic").html().trim());
    $("#txtjoindate").val($("#lbljoindate").html().trim());
    $("#txtgender").val($("#lblgender").html().trim());
    $("#txtdesignation").val($("#lbldesignation").html().trim());

    $("#lblName1").html($("#txtName1").val());
    $("#lblName2").html($("#txtName2").val());
    $("#lblStrt1").html($("#txtStrt1").val());
    $("#lblStrt2").html($("#txtStrt2").val());
    $("#lblStrt4").html($("#txtStrt4").val());
    $("#lblZip").html($("#txtZip").val());
    $("#lblCity").html($("#txtCity").val());
    $("#lblProv").html($("#txtProv").val());
    $("#lblCtry").html($("#txtCtry").val());
    $("#lblPhone").html($("#txtPhone").val());
    $("#lblFex").html($("#txtFex").val());
    $("#lblEmail").html($("#txtEmail").val());
    $("#lblWeb").html($("#txtWeb").val());
}


function LblShow() {
    $("#lblempid").show();
    $("#lblfullname").show();
    $("#lblemplastname").show();
    $("#lblcnic").show();
    $("#lbljoindate").show();
    $("#lblgender").show();
    $("#lbldesignation").show();

    $("#lblName1").show();
    $("#lblName2").show();
    $("#lblStrt1").show();
    $("#lblStrt2").show();
    $("#lblStrt4").show();
    $("#lblZip").show();
    $("#lblCity").show();
    $("#lblProv").show();
    $("#lblCtry").show();
    $("#lblPhone").show();
    $("#lblFex").show();
    $("#lblEmail").show();
    $("#lblWeb").show();
}


function TxtHide() {
    $("#txtName1").hide();
    $("#txtName2").hide();
    $("#txtfullname").hide();
    $("#txtStrt1").hide();
    $("#txtStrt2").hide();
    $("#txtStrt4").hide();
    $("#txtZip").hide();
    $("#txtCity").hide();
    $("#txtProv").hide();
    $("#txtCtry").hide();
    $("#txtPhone").hide();
    $("#txtFex").hide();
    $("#txtEmail").hide();
    $("#txtWeb").hide();
    $("#txtcnic").hide();
    $("#txtempid").hide();
    $("#txtempname").hide();
    $("#txtemplastname").hide();
    $("#txtjoindate").hide();
    $("#txtgender").hide();
    $("#txtdesignation").hide();
}

function TxtValue() {
    $("#txtempid").val($("#lblempid").html().trim());
    $("#txtfullname").val($("#lblfullname").html().trim());
    $("#txtemplastname").val($("#lblemplastname").html().trim());
    $("#txtcnic").val($("#lblcnic").html().trim());
    $("#txtjoindate").val($("#lbljoindate").html().trim());
    $("#txtgender").val($("#lblgender").html().trim());
    $("#txtdesignation").val($("#lbldesignation").html().trim());

    $("#txtName1").val($("#lblName1").html().trim());
    $("#txtName2").val($("#lblName2").html().trim());
    //$("#txtStrt1").val($("#lblStrt1").html().trim());
    $("#txtStrt2").val($("#lblStrt2").html().trim());
    //$("#txtStrt4").val($("#lblStrt4").html().trim());
    $("#txtZip").val($("#lblZip").html().trim());
    $("#txtCity").val($("#lblCity").html().trim());
    //$("#txtProv").val($("#lblProv").html().trim());
    $("#txtCtry").val($("#lblCtry").html().trim());
    $("#txtPhone").val($("#lblPhone").html().trim());
    //$("#txtFex").val($("#lblFex").html().trim());
    //$("#txtEmail").val($("#lblEmail").html().trim());
    //$("#txtWeb").val($("#lblWeb").html().trim());
}

function TxtShow() {
    $("#txtempid").show();
    $("#txtfullname").show();
    $("#txtemplastname").show();
    $("#txtcnic").show();
    $("#txtjoindate").show();
    $("#txtgender").show();
    $("#txtdesignation").show();

    $("#txtName1").show();
    $("#txtName2").show();
    $("#txtStrt1").show();
    $("#txtStrt2").show();
    $("#txtStrt4").show();
    $("#txtZip").show();
    $("#txtCity").show();
    $("#txtProv").show();
    $("#txtCtry").show();
    $("#txtPhone").show();
    $("#txtFex").show();
    $("#txtEmail").show();
    $("#txtWeb").show();
}