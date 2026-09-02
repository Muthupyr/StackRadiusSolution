/* jshelper.js */
//
// function GridDelete(event) 


function GridDelete(event) {

    // alert ('value ' + event.value);
    // alert ('url ' + event.url);
    // alert ('table ' + alert('data: ' + JSON.stringify(event.table));
    swal({
        title: "Are you sure to delete this record?",
        text: "",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes",
        cancelButtonText: "No",
        closeOnConfirm: true, //false,
        closeOnCancel: true
    }, function (isConfirm) {
        if (isConfirm) {
            $.ajax({
                url: event.url,
                type: 'POST',
                data: {
                    mode: "Delete",
                    value: event.value
                },
                success: function (result) {

                    if (result.status == "Success") {
                        event.table
                            .row($(event.maintainvalue).parents('tr'))
                            .remove()
                            .draw(false);
                        swal("", result.message, "success");
                    }
                    else {
                        if (result.status == "Failure") {
                            swal("", result.message, "error");
                        }
                    }
                }
            })
        }
    });
}

//todo
function GridRefresh(event) {

    // alert ('value ' + event.value);
    // alert ('url ' + event.url);
    // alert ('table ' + alert('data: ' + JSON.stringify(event.table));
    swal({
        title: "Are you sure to delete this record?",
        text: "",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes",
        cancelButtonText: "No",
        closeOnConfirm: true, //false,
        closeOnCancel: true
    }, function (isConfirm) {
        if (isConfirm) {
            $.ajax({
                url: event.url,
                type: 'POST',
                data: {
                    mode: "Delete",
                    value: event.value
                },
                success: function (result) {

                    if (result.status == "Success") {
                        event.table
                            .row($(event.maintainvalue).parents('tr'))
                            .remove()
                            .draw(false);
                        swal("", result.message, "success");
                    }
                    else {
                        if (result.status == "Failure") {
                            swal("", result.message, "error");
                        }
                    }
                }
            })
        }
    });
}
