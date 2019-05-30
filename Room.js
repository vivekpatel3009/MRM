$(".RoomTableDiv").on("click", "a[id*='DeleteRoombtn_']", function () {
      
    var id = this.id.split('_')[1];
    RoomDeleteCall(id);
});
function RoomDeleteCall(id) {
      
    $.ajax({
        type: 'POST',
        data: { RoomId: id },
        url: '/Rooms/CheckRoomReference',
        success: function (response) {
            var $description = $('<div/>');
            if (response.result == 'ReferenceExist') {
                $description.append($('<p/>').html('Are you sure you want to permanently Delete Room?</br>'));
                $description.append($('<p/>').html('All Booking associated with this Room will also be permanently Deleted.'));

            } else if (response.result == 'ReferenceNotExist') {
                $description.append($('<p/>').html('Are you sure you want to permanently Delete Room?</br>'));
            }
            $description.append($('<p/>').html('<input type="hidden" id="HiddenRoomid"  value=' + id + '></br>'));
            $('#myModal #pDetails').empty().html($description);
            $('#myModal').modal();
        }
    });
}
$("#DeleteRoomAllRef").click(function () {
    $.blockUI({
        css: {
            border: '5px solid #3c8dbc',
            borderRadius: '5px',
            padding: '20px 0'
        },
        message: $('#domMessage')
    });
    var valyuRoom = $("#HiddenRoomid").val();
    $.ajax({
        type: 'POST',
        data: { RoomId: valyuRoom },
        url: '/Rooms/DeleteRoomReference',
        success: function (response) {
            if (response.result == 'ReferenceDeleted') {
                toastr.success('Room Record Deleted Successfully');
                $('#myModal').modal('hide');
                var millisecondsToWait = 500;
                setTimeout(function () {
                    window.location = response.url;
                }, millisecondsToWait);

            }
        }
    });
})



$(".RoomTableDiv").on("click", "button[id*='DeActiveRoombtn_']", function () {
      
    var id = this.id.split('_')[1];
    RoomDeActDACTDeleteCall(id);
});
function RoomDeActDACTDeleteCall(id) {      
    $.ajax({
        type: 'POST',
        data: { RoomId: id },
        url: '/Rooms/CheckRoomReference',
        success: function (response) {
            var $description = $('<div/>');
            if (response.result == 'ReferenceExist') {
                if (response.status == 'WantDACT') {
                    $description.append($('<p/>').html('Are you sure you want to DeActivate Room?</br>'));
                    $description.append($('<p/>').html('All Booking associated with this Room will also be permanently Deleted.'));
                } if (response.status == 'WantACT') {
                    $description.append($('<p/>').html('Are you sure you want to Activate Room?</br>'));
                }
            } else if (response.result == 'ReferenceNotExist') {
                if (response.status == 'WantDACT') {
                    $description.append($('<p/>').html('Are you sure you want to DeActivate Room?'));
                }
                if (response.status == 'WantACT') {
                    $description.append($('<p/>').html('Are you sure you want to Activate Room?</br>'));
                }
            }
            $description.append($('<p/>').html('<input type="hidden" id="HiddenRoomidDACT"  value=' + id + '></br>'));
            $('#myModalACTDACT #pDetails').empty().html($description);
            $('#myModalACTDACT').modal();
        }
    });
}

$("#ACTDACTRoomAllRef").click(function () {
    $.blockUI({
        css: {
            border: '5px solid #3c8dbc',
            borderRadius: '5px',
            padding: '20px 0'
        },
        message: $('#domMessage')
    });
    var valyuRoom = $("#HiddenRoomidDACT").val();
    $.ajax({
        type: 'POST',
        data: { RoomId: valyuRoom },
        url: '/Rooms/DeActivateRoomReference',
        success: function (response) {
            if (response.result == 'roomStatusupdated') {
                toastr.success('Room Status Changed Successfully');
                $('#myModalACTDACT').modal('hide');
                var millisecondsToWait = 2000;
                setTimeout(function () {
                    window.location = response.url;
                }, millisecondsToWait);

            }
        }
    });
})
$("#roomNumber").keypress(function (evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode != 43 && charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
});
function activepopupClose() {
    window.location = '/Rooms/Index';
}
function fonctionTest(id, status) {
    console.log(id, status);
    if (status == 0) {
        RoomDeActDACTDeleteCall(id);
    }
    else {
        console.log(status);
        RoomDeActDACTDeleteCall(id);
    }
}  