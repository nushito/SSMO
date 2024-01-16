$(document).ready
    (
        function () {
            //Dropdownlist Selectedchange event
            $("#CustomerId").change(function () {
                $("#CustomerOrderNumber option").remove();
                var customerId = $(this).val();
                var mycompanyId = $("#MyCompanyId").val();
                $.ajax({
                    type: 'GET',
                    url: "/Documents/GetCustomerOrders",
                    dataType: 'json',
                    data: { id: customerId, myCompanyId: mycompanyId },
                    success: function (selectedCustomerOrders) {
                        $("#CustomerOrderNumber").append('<option value="' + "0" + '">' + "Select CustomerOrderNumber" + '</option>');
                        debugger;
                        for (var i = 0; i < selectedCustomerOrders.length; i++) {
                            $("#CustomerOrderNumber").append('<option value="' + selectedCustomerOrders[i].CustomerOrderId + '">' + selectedCustomerOrders[i].CustomerOrderNumber + '</option>');
                        }

                        debugger;
                    },
                    error: function (ex) {
                        alert('Failed to retrieve selectedCustomerOrders' + ex);
                    }
                });
                return false;
            })
        }
    );