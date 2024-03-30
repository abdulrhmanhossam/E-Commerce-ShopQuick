function validateShippingInput() {
    if (document.getElementById("trackingNumber").value == "") {
        swal({
            icon: 'error',
            title: 'Oops....',
            text: 'Please enter the tracking Number'
        })
        return false;
    }

    if (document.getElementById("carrier").value == "") {
        swal({
            icon: 'error',
            title: 'Oops....',
            text: 'Please enter the carrier'
        })
        return false;
    }
}

function EnterDataCarrier() {
    if (document.getElementById("carrier").value == "") {
        document.getElementById("carrier").style.border = "thin solid red"
    } else {
        document.getElementById("carrier").style.border = "thin solid #0000FF"
    }
}

function EnterDataTracking() {
    if (document.getElementById("trackingNumber").value == "") {
        document.getElementById("trackingNumber").style.border = "thin solid red"
    } else {
        document.getElementById("trackingNumber").style.border = "thin solid #0000FF"
    }
}

function RefundIssue(url) {
    swal({
        title: "Click Ok if you want to delete the Order",
        text: "if click ok it will not revent back again",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willRefund) => {
        if (willRefund) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        setTimeout(reloadPage, 2000);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    });
}

function reloadPage() {
    window.location.reload();
}



