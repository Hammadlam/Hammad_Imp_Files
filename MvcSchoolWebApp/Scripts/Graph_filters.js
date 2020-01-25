$(document).ready(function () {

    get_filters();

});

function onlyUnique(value, index, self) {
    return self.indexOf(value) === index;
}

var filter_col = [];
var filters = [];

// url: "HomeController.cs/DisplayGraphFilters",

function get_filters() {
     
    $.ajax({
        type: "POST",
        url: "/Home/DisplayGraphFilters",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ id: getAllUrlParams().mid }),
        dataType: "json",
        success: function (result, status) {
            
            var a = [result.length];
            for (var i = 0; i < result.length; i++) {
                a.push(result[i].filter_name);
            }
            filter_col = a.filter(onlyUnique);

            console.log(filter_col);

            for (var i = 1; i < filter_col.length; i++) {
                debugger
                text = "<div class='col-md-4'>" +
                    "<select class='" + filter_col[i] + " form-control multiple-checkboxes' multiple='multiple'></select></div>";

                //text = "<li id='mastermenu'><a onclick='submenuchlidopen(\"" + dmenuclass[i] + "\")'  title='\"" + dmenu[i] + "\"'><i class='fa fa-database'></i> <span class='ellipise'>" + dmenu[i].substring(0, 14) + "..." + "</span><span class='fa fa-chevron-down'></span></a>" +
                //    "<ul class='nav child_menu " + dmenuclass[i] + "'></ul>";
                $('.gr_filters').append(text);
            }

            for (var i = 0; i < result.length; i++) {
                debugger
                text = "<option value='" + result[i].filter_val + "' > " + result[i].filter_val + "</option>";

                $("." + result[i].filter_name).append(text);
            }

            text = "<div class='col-md-4'>" +
                "<input type ='button' class='btn btn-primary aply_filtr' value='Apply filter'>"
                + "</div>";

            $('.gr_filters').append(text);

            $('.aply_filtr').click(function () {
                debugger
                get_filterval();
                applyfilter();

            });

            $('.multiple-checkboxes').multiselect({
                includeSelectAllOption: true,
                buttonWidth: '95%',
            });

        },
        error: function (xhr, status, error) {

            alert("ERROR");
        }

    });

}

function get_filterval() {
    filters.length = 0;

    for (var i = 1; i < filter_col.length; i++) {

        //console.log(filter_col[i]);

        var items = $("." + filter_col[i]).val();
        if (items != null) {
            for (var j = 0; j < items.length; j++) {
                filters.push(filter_col[i] + "_" + items[j]);
                //filters.push(items[j]);
            }
        }

    }

    //console.log(filters);
}

function applyfilter() {
    var val = JSON.stringify(filters);
    var vendorname = [];
    var invcount = [];
    debugger

    $.ajax({
        type: "POST",
        url: "/Home/DisplayApplyGraphFilters",
        contentType: "application/json; charset=utf-8",
        data: '{filter: ' + val + '}',
        dataType: "json",
        success: function (result, status) {
            debugger
            for (var i = 0; i < result.length; i++) {

                vendorname.push(result[i].Vendername);

                invcount.push(result[i].count);

                col_val.push(getRandomColor());
            }

            showgraph(vendorname, invcount);
        },
        error: function (xhr, status, error) {

            alert("ERROR");
        }

    });
}


function getAllUrlParams(url) {

    // get query string from url (optional) or window
    var queryString = url ? url.split('?')[1] : window.location.search.slice(1);

    // we'll store the parameters here
    var obj = {};

    // if query string exists
    if (queryString) {

        // stuff after # is not part of query string, so get rid of it
        queryString = queryString.split('#')[0];

        // split our query string into its component parts
        var arr = queryString.split('&');

        for (var i = 0; i < arr.length; i++) {
            // separate the keys and the values
            var a = arr[i].split('=');

            // set parameter name and value (use 'true' if empty)
            var paramName = a[0];
            var paramValue = typeof (a[1]) === 'undefined' ? true : a[1];

            // (optional) keep case consistent
            paramName = paramName.toLowerCase();
            if (typeof paramValue === 'string') paramValue = paramValue.toLowerCase();

            // if the paramName ends with square brackets, e.g. colors[] or colors[2]
            if (paramName.match(/\[(\d+)?\]$/)) {

                // create key if it doesn't exist
                var key = paramName.replace(/\[(\d+)?\]/, '');
                if (!obj[key]) obj[key] = [];

                // if it's an indexed array e.g. colors[2]
                if (paramName.match(/\[\d+\]$/)) {
                    // get the index value and add the entry at the appropriate position
                    var index = /\[(\d+)\]/.exec(paramName)[1];
                    obj[key][index] = paramValue;
                } else {
                    // otherwise add the value to the end of the array
                    obj[key].push(paramValue);
                }
            } else {
                // we're dealing with a string
                if (!obj[paramName]) {
                    // if it doesn't exist, create property
                    obj[paramName] = paramValue;
                } else if (obj[paramName] && typeof obj[paramName] === 'string') {
                    // if property does exist and it's a string, convert it to an array
                    obj[paramName] = [obj[paramName]];
                    obj[paramName].push(paramValue);
                } else {
                    // otherwise add the property
                    obj[paramName].push(paramValue);
                }
            }
        }
    }

    return obj;
}