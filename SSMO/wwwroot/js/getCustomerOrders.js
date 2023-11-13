$(document).ready(function () {
    //Dropdownlist Selectedchange event
    $("#CustomerId").change(function () {
        $("#CustomerOrder option").remove();
        var customerId = $(this).val();
        $.ajax({
            type: 'GET',
            url: "/ServiceOrders/GetCustomerOrdersNumbers",
            // dataType: 'json',
            data: { id: customerId },
            success: function (selectedOrders) {
                $("#CustomerOrder").append('<option value="' + "0" + '">' + "Select Customer Order" + '</option>');
                debugger;
                for (var i = 0; i < selectedOrders.length; i++) {
                    $("#CustomerOrder").append('<option value="' + selectedOrders[i].CustomerOrderId + '">' + selectedOrders[i].CustomerOrderNumber + '</option>');
                }
                debugger;
            },
            error: function (ex) {
                alert('Failed to retrieve selectedOrders' + ex);
            }
        });
        return false;
    })
});