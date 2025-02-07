$(document).ready(function () {
    $("#MyCompanyId").change(function ()
    {
        $("BgInvoiceNumberId option").remove();
        var myCompanyId = $(this).val();
        $.ajax({
            type: 'GET',
            url: "/Documents/GetBgInvoiceNumbers",
            data: { id: myCompanyId },
            success: function (selectedNumbers) {
                $("#BgInvoiceNumberId").append('option value="' + "0" + '">' + "Select BgNumber" + '</option>');
                for (var i = 0; i < selectedNumbers.length; i++) {
                    $("#BgInvoiceNumberId").append('<option value="' + selectedNumbers[i].BgInvoiceId + '">' + selectedNumbers[i].BgInvoiceNumber + '</option>');
                }
            },
            error: function (error) {
                alert('Failed to retrieve packinglistNumber' + error);
            }
        });
        return false;
    })
});