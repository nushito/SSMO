$(document).ready(function () {
    //Dropdownlist Selectedchange event
    $("#MyCompanyId").change(function () {
        $("#PackingListNumber option").remove();
        var myCompanyId = $(this).val();
        $.ajax({
            type: 'GET',
            url: "/Documents/GetPackingListNumbers",
            // dataType: 'json',
            data: { id: myCompanyId },
            success: function (selectedNumbers) {
                $("#PackingListNumber").append('<option value="' + "0" + '">' + "Select PackingList Number" + '</option>');
                debugger;
                for (var i = 0; i < selectedNumbers.length; i++) {
                    $("#PackingListNumber").append('<option value="' + selectedNumbers[i].PackingListId + '">' + selectedNumbers[i].PackingListNumber + '</option>');
                }
                debugger;
                //var numbers = '';
                //$(#InvoiceNumber).empty();
                //$.each(selectedInvoiceNumbers, function(i, invoice) {
                //    numbers += '<option value="' + invoice.InvoiceId + '">' + invoice.InvoiceNumber + '</option>';
                //    
                //}); $(#InvoiceNumber).html(invoice);
            },
            error: function (ex) {
                alert('Failed to retrieve packinglistNumber' + ex);
            }
        });
        return false;
    })
});