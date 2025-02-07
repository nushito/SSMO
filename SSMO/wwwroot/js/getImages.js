$(document).ready(function () {
    $("#MyCompanyId").change(function (){
        $("#Header option").remove();
        $("#Footer option").remove();
        var myCompany = $(this).val();
        $.ajax({
            type: 'GET',
            url: "/CustomerOrders/GetImages",
            data: { id: myCompany },
            success: function (selectedHeaders) {
                $("#Header").append('<option value="' + "0" + '">' + "Select Header" + '</option>');
                $("#Footer").append('<option value="' + "0" + '">' + "Select Footer" + '</option>');
                debugger;
                for (var i = 0; i < selectedHeaders.length; i++) {
                    $("#Header").append('<option value="' + selectedHeaders[i].Id + '">' + selectedHeaders[i].ImageTitle + '</option>');
                    $("#Footer").append('<option value="' + selectedHeaders[i].Id + '">' + selectedHeaders[i].ImageTitle + '</option>');
                }
            },
            error: function (ex) {
                alert('Failed to retrieve header' + ex);
            }
        });    
       
        return false;
    })

});