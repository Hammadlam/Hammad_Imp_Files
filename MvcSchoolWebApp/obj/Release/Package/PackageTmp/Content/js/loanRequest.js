var monthno = 0;
$("#txtpayperiodLR").keyup(function () {
    getValues();
    
});

$("#txtloanamtLR").keyup(function () {
    getValues();

});


var date = new Date();
date.setDate(date.getDate() - 1);
$('#txtrpaydateLR').datepicker({
    viewMode: "months",
    minViewMode: "months",
    format: "MM yyyy",
    autoclose: true,
    startDate: date
});

function getValues() {
    var payperiod = $("#txtpayperiodLR").val();
    var loanamt = $("#txtloanamtLR").val();
    var total = parseInt(loanamt / payperiod);
    if (isNaN(total))
        total = "0";
    $("#txtinstalLR").val("" + total);

    var getdate = $("#txtrpaydateLR").val();
    var month = getdate.split("-");
    monthno = getMonthFromString(month[1]);
    if (getdate != "") {
        var currentdate = new Date(getdate);
        currentdate.setMonth(currentdate.getMonth() + parseInt(payperiod));
        if (payperiod != "")
            $("#txtlastdateLR").val($.datepicker.formatDate('dd-MM-yy', currentdate));
        else
            $("#txtlastdateLR").val("");
    }
}

$("#txtrpaydateLR").change(function () {
    var payperiod = $("#txtpayperiodLR").val();
    var getdate = $("#txtrpaydateLR").val();
    var month = getdate.split("-");
    monthno = getMonthFromString(month[1]);
    if (getdate != "") {
        var currentdate = new Date(getdate);
        currentdate.setMonth(currentdate.getMonth() + parseInt(payperiod));

        
        if (payperiod != "")
            $("#txtlastdateLR").val($.datepicker.formatDate('dd-MM-yy', currentdate));
        else
            $("#txtlastdateLR").val("");
    }
});
function getMonthFromString(mon) {

    var d = Date.parse(mon + "1, 2012");
    if (!isNaN(d)) {
        return new Date(d).getMonth() - (Date(d).getDate()-1);
    }
    return -1;
}






