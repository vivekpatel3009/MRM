

$("#FilterRooms").click(function () {

    var isvalid = true;
    var FloorId = $("#FloorId :selected").val();
    var RoomTypeId = $("#RoomTypeId  :selected").val();
    var StartDate = $("#datetimepickerStart").find("input").val();
    var EndDate = $("#datetimepickerEnd").find("input").val();
    var sMin = parseInt($("#sMin").val());
    var eMin = parseInt($("#eMin").val());
    var sHour = parseInt($("#sHour").val());
    var eHour = parseInt($("#eHour").val());
    var MaxCapacity = parseInt($("#MAxCapacity").val());
    var MinCapacity = parseInt($("#MINCapacity").val());
    if (process(StartDate) > process(EndDate)) {
        isvalid = false;
        toastr.warning('StartDate Should Not Be Grater than to EndDate  !!');
    }
    if (MinCapacity > MaxCapacity) {
        isvalid = false;
        toastr.warning('Min person capacity should be less than to Max person capacity !!');
    }
    if (MinCapacity == 0 || MinCapacity == "" || MinCapacity < 2) {
        isvalid = false;
        toastr.warning('Min person capacity alteast 3 or more !!');
    }
    if ((sHour != "" || sHour!= 0 ) && sHour > 23) {
        isvalid = false;
        toastr.warning('start hour should be less than 24  !!');
    }
    if (sHour < 0) {
        isvalid = false;
        toastr.warning('start hour should  positive number  !!');
    }

    if (sMin >59 ) {
        isvalid = false;
        toastr.warning(' start minute should be less than 60 !!');
    }
    if (sMin < 0) {
        isvalid = false;
        toastr.warning('start minute should  positive number !!');
    }

    if (eHour > 23 ) {
        isvalid = false;
        toastr.warning('end hour should be less than 24 !!');
    }
    if (eHour < 0) {
        isvalid = false;
        toastr.warning('end hour should  positive number  !!');
    }
    if (eMin > 59) {
        isvalid = false;
        toastr.warning('end minute should be less than 60 !!');
    }
    if (eMin < 0) {
        isvalid = false;
        toastr.warning('end minute should  positive number !!');
    }

    if (StartDate == EndDate) {
        if (sHour > eHour) {
            isvalid = false;
            toastr.warning('StartDate Should Not Be Grater than to EndDate  !!');
        }
        if (sHour == eHour) {
            if (sMin > eMin) {
                isvalid = false;
                toastr.warning('StartDate Should Not Be Grater than to EndDate  !!');
            }
        }
    }
    if (EndDate == "") {
        isvalid = false; toastr.warning('Please Select EndDate   !!'); }
    if (StartDate == "") {
        isvalid = false;
        toastr.warning('Please Select StartDate   !!');
    }
    if (isvalid == true) {
        $.blockUI({
            css: {
                border: '5px solid #3c8dbc',
                borderRadius: '5px',
                padding: '20px 0'
            },
            message: $('#domMessage')
        });
        $.ajax({
            type: 'POST',
            data: { FloorId: FloorId, RoomTypeId: RoomTypeId, Start: StartDate, End: EndDate, MaxCapacity: MaxCapacity, MinCapacity: MinCapacity, sHour: sHour, sMin: sMin, eHour: eHour, eMin:eMin  },
            url: '/BookRoom/PartialFilterRoom',
            success: function (Data) {
                $("#SectionPartialDiv").html('');
                $("#SectionPartialDiv").html(Data);
               
                $.unblockUI();
            }
        });
    } else {
        $.unblockUI();
    }
});
function process(date) {
    var parts = date.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}
$("#CalenderBody").on("click", "[id*='smallimgroom']", function () {
    var src = this.src;
    $("#bigimgroom").attr("src", src);
});
