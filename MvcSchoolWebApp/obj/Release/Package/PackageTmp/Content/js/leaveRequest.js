var startdate;
var enddate;

var date = new Date();
date.setDate(date.getDate() - 1);
$('#txtbegdateLvR').datepicker({
    format: 'dd-MM-yyyy',
    autoclose: true,
    startDate: date
});

$('#txtlastdateLvR').datepicker({
    format: 'dd-MM-yyyy',
    autoclose: true,
    startDate: date
});

$("#txtbegdateLvR").change(function () {
    startdate = new Date($("#txtbegdateLvR").val());
    enddate = new Date($("#txtlastdateLvR").val());
    $("#txtnumDayLvR").val(calcBusinessDays(startdate, enddate));
});

$("#txtlastdateLvR").change(function () {
    startdate = new Date($("#txtbegdateLvR").val());
    enddate = new Date($("#txtlastdateLvR").val());
    $("#txtnumDayLvR").val(calcBusinessDays(startdate, enddate));
});

function calcBusinessDays(dDate1, dDate2) {         // input given as Date objects

    if (dDate1 != "" && dDate2 != "") {
        var iWeeks, iDateDiff, iAdjust = 0;

        if (dDate2 < dDate1) return 0;                 // error code if dates transposed

        var iWeekday1 = dDate1.getDay();                // day of week
        var iWeekday2 = dDate2.getDay();

        iWeekday1 = (iWeekday1 == 0) ? 7 : iWeekday1;   // change Sunday from 0 to 7
        iWeekday2 = (iWeekday2 == 0) ? 7 : iWeekday2;

        if ((iWeekday1 > 5) && (iWeekday2 > 5)) iAdjust = 1;  // adjustment if both days on weekend

        iWeekday1 = (iWeekday1 > 5) ? 5 : iWeekday1;    // only count weekdays
        iWeekday2 = (iWeekday2 > 5) ? 5 : iWeekday2;

        // calculate differnece in weeks (1000mS * 60sec * 60min * 24hrs * 7 days = 604800000)
        iWeeks = Math.floor((dDate2.getTime() - dDate1.getTime()) / 604800000)

        if (iWeekday1 <= iWeekday2) {
            iDateDiff = (iWeeks * 5) + (iWeekday2 - iWeekday1)
        } else {
            iDateDiff = ((iWeeks + 1) * 5) - (iWeekday1 - iWeekday2)
        }

        iDateDiff -= iAdjust                            // take into account both days on weekend
    }
    return (iDateDiff + 1);                         // add 1 because dates are inclusive

}