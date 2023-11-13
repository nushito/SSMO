
$(document).ready(function () {
    //Dropdownlist Selectedchange event
    $("#SupplierId").change(function () {
        $("#SupplierOrderNumber option").remove();
        var supplierId = $(this).val();
        $.ajax({
            type: 'GET',
            url: "/ServiceOrders/GetSupplierOrdersNumbers",
            // dataType: 'json',
            data: { id: supplierId },
            success: function (selectedInvoiceNumbers) {
                $("#SupplierOrderNumber").append('<option value="' + "0" + '">' + "Select Supplier Order" + '</option>');
                debugger;
                for (var i = 0; i < selectedInvoiceNumbers.length; i++) {
                    $("#SupplierOrderNumber").append('<option value="' + selectedInvoiceNumbers[i].SupplierOrderId + '">' + selectedInvoiceNumbers[i].SupplierOrderNumber + '</option>');
                }
                debugger;
            },
            error: function (ex) {
                alert('Failed to retrieve selectedInvoiceNumbers' + ex);
            }
        });
        return false;
    })
});