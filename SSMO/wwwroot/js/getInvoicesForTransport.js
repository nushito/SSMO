
$(document).ready(function () {
    //Dropdownlist Selectedchange event
    $("#SupplierId").change(function () {
        $("#PurchaseInvoiceNumber option").remove();
        var supplierId = $(this).val();
        $.ajax({
            type: 'GET',
            url: "/ServiceOrders/GetPurchaseDocumentNumbers",
            // dataType: 'json',
            data: { id: supplierId },
            success: function (selectedInvoiceNumbers) {
                $("#PurchaseInvoiceNumber").append('<option value="' + "0" + '">' + "Select Purchase" + '</option>');
                debugger;
                for (var i = 0; i < selectedInvoiceNumbers.length; i++) {
                    $("#PurchaseInvoiceNumber").append('<option value="' + selectedInvoiceNumbers[i].PurchaseInvoiceId + '">' + selectedInvoiceNumbers[i].PurchaseInvoiceNumber + '</option>');
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