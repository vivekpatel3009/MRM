$(".FloorTableDiv").on("click", "a[id*='DeleteFloorbtn_']", function () {

    var id = this.id.split('_')[1];
    // var id = $(this).parent().find('#hdnRoomId').val()
    FloorDeleteCall(id);
});
function FloorDeleteCall(id) {
      
    $.ajax({
        type: 'POST',
        data: { FloorId: id },
        url: '/Floor/CheckFloorReference',
        success: function (response) {
            var $description = $('<div/>');
            if (response.result == 'ReferenceExist') {
                $description.append($('<p/>').html('Are you sure you want to permanently Delete Floor?</br>'));
                $description.append($('<p/>').html('All Booking, Room associated with this Floor will also be permanently Deleted.'));             
            } else if (response.result == 'ReferenceNotExist') {
                $description.append($('<p/>').html('<b> Are you sure Want to Delete this Floor ?</b></br>'));
            }
            $description.append($('<p/>').html('<input type="hidden" id="HiddenFloorid" name="custId" value=' + id + '></br>'));
            $('#myModal #pDetails').empty().html($description);
            $('#myModal').modal();
        }
    });
}
$("#DeleteFloorAllRef").click(function () {
    $.blockUI({
        css: {
            border: '5px solid #3c8dbc',
            borderRadius: '5px',
            padding: '20px 0'
        },
        message: $('#domMessage')
    });
    var valyufloor = $("#HiddenFloorid").val();
    $.ajax({
        type: 'POST',
        data: { FloorId: valyufloor },
        url: '/Floor/DeleteFloorReference',
        success: function (response) {
            if (response.result == 'ReferenceDeleted') {
                toastr.success('Floor Records Deleted Successfully');
                var millisecondsToWait = 500;
                setTimeout(function () {
                    window.location = response.url;
                }, millisecondsToWait);
            }
        }
    });
})


$(".FloorTableDiv").on("click", "button[id*='DeActiveFloorbtn_']", function () {
    var id = this.id.split('_')[1];
    DEACTFLOOR(id);
});
function DEACTFLOOR(id) {
 
    $.ajax({
        type: 'POST',
        data: { FloorId: id },
        url: '/Floor/CheckFloorReferenceforDeactive',
        success: function (response) {
            var $description = $('<div/>');
            if (response.result == 'ReferenceExist') {
                $description.append($('<p/>').html('Are you sure you want to DeActivate Floor?</br>'));
                $description.append($('<p/>').html('All Rooms associated with this Floor will also be permanently Deleted.'));
                $description.append($('<p/>').html('All Booking associated with this Floor will also be permanently Deleted.'));
            } else if (response.result == 'ReferenceNotExist') {
                $description.append($('<p/>').html('Are you sure you want to DeActivate Floor?'));
            }
            $description.append($('<p/>').html('<input type="hidden" id="HiddenflooridDACT"  value=' + id + '></br>'));
            $('#myModalDACT #pDetails').empty().html($description);
            $('#myModalDACT').modal();
        }
    });
}
$("#DACTFloorAllRef").click(function () {
    $.blockUI({
        css: {
            border: '5px solid #3c8dbc',
            borderRadius: '5px',
            padding: '20px 0'
        },
        message: $('#domMessage')
    });
    var valyuRoom = $("#HiddenflooridDACT").val();
    $.ajax({
        type: 'POST',
        data: { FloorId: valyuRoom },
        url: '/Floor/DeActivateFloorReference',
        success: function (response) {
            if (response.result == 'DeactFloor') {
                toastr.success('Floor Status changed Successfully');
                $('#myModalDACT').modal('hide');
                var millisecondsToWait = 2000;
                setTimeout(function () {
                    window.location = response.url;
                }, millisecondsToWait);
                
            }
        }
    });
})

$(".FloorTableDiv").on("click", "button[id*='showbtn_']", function () {
      
    var id = this.id.split('_')[1];
    ACTFloor(id);
});
function ACTFloor(id) {

    $.ajax({
        type: 'POST',
        data: { FloorId: id },
        url: '/Floor/CheckFloorReferenceforDeactive',
        success: function (response) {
            var $description = $('<div/>');
            if (response.result == 'ReferenceExist') {
                $description.append($('<p/>').html('Are you sure you want to Activate Floor?</br>'));
            } else if (response.result == 'ReferenceNotExist') {
                $description.append($('<p/>').html('Are you sure you want to Activate Floor?'));
            }
            $description.append($('<p/>').html('<input type="hidden" id="HiddenflooridDACT"  value=' + id + '></br>'));
            $('#myModalACT #pDetails').empty().html($description);
            $('#myModalACT').modal();
        }
    });
}
$("#ActFloorAllRef").click(function () {
    $.blockUI({
        css: {
            border: '5px solid #3c8dbc',
            borderRadius: '5px',
            padding: '20px 0'
        },
        message: $('#domMessage')
    });
    var valyuRoom = $("#HiddenflooridDACT").val();
    $.ajax({
        type: 'POST',
        data: { FloorId: valyuRoom },
        url: '/Floor/ActivateFloorReference',
        success: function (response) {
            if (response.result == 'actFloor') {
                toastr.success('Floor Status changed Successfully');
                $('#myModalACT').modal('hide');
                var millisecondsToWait = 2000;
                setTimeout(function () {
                    window.location = response.url;
                }, millisecondsToWait);

            }
        }
    });
})
function activepopupClose() {
    window.location = '/Floor/Index';
}
function fonctionTest(id, status) {

    console.log(id, status);
    if (status == 0) {
        ACTFloor(id);
    }
    else {
        console.log(status);
        DEACTFLOOR(id);
    }
}  