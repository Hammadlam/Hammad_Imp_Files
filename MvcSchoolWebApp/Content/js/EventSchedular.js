var selectedEvent = null;
$(document).ready(function () {
    $('#calender').fullCalendar('destroy');
  FetchEventAndRenderCalendar();
});

function FetchEventAndRenderCalendar() {
    var events = [];

    $.ajax({
        type: "POST",
        url: encodeURI("../Event/GetEvents"),
        success: function (data) {
            $.each(data, function (i, v) {
                events.push({
                    title: v.Subject,
                    description: v.Description,
                    start: moment(v.Start),
                    end: v.End != null ? moment(v.End) : null,
                    color: 'red',
                    allDay: v.IsFullDay
                });
            })

            GenerateCalender(events);
        },
        error: function (error) {
            alert('failed');
        }
    });

}

function GenerateCalender(events) {
    $('#calender').fullCalendar('destroy');

    $('#calender').fullCalendar({
        contentHeight: 400,
        defaultDate: new Date(),
        timeFormat: 'h(:mm)a',
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,basicWeek,basicDay'
        },
        eventLimit: true,
        eventColor: '#378006',
        events: events,
        eventClick: function (calEvent, jsEvent, view) {
            selectedEvent = calEvent.start.format("DD-MMM-YYYY HH:mm a");
            $('#myModal #eventTitle').text(calEvent.title);
            var $description = $('<div/>');
            $description.append($('<p/>').html('<b>Start:</b>' + calEvent.start.format("DD-MMM-YYYY HH:mm a")));
            if (calEvent.end != null) {
                $description.append($('<p/>').html('<b>End:</b>' + calEvent.end.format("DD-MMM-YYYY HH:mm a")));
            }
            $description.append($('<p/>').html('<b>Description:</b>' + calEvent.description));
            $('#myModal #pDetails').empty().html($description);

            $('#myModal').modal();
        }
    });
}
$('#btnDelete').click(function () {
    if (selectedEvent != null) {
        $.ajax({
            type: "POST",
            url: '/Event/DeleteEvent',
            data: { eventID: selectedEvent },
            success: function (data) {
                data.forEach(function (obj) {
                    if (obj.status=="true") {
                        FetchEventAndRenderCalendar();
                        $('#myModal').modal('hide');
                    }
                });
            },
            error: function () {
                alert('Failed');
            }
        })
    }
});