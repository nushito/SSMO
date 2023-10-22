
    var counter = 1;
    $(function products() {
        $('#addrow').click(function () {
            $('<tr id="tablerow' + counter + '"><td>' +
                ' <select class="text-box single-line" name="DescriptionId[' + counter + ']" asp-for="DescriptionId" ><option value="">All</option>@foreach (var desc in service.DescriptionIdAndNameList()){<option value="@desc.Id">@desc.Name</option>}</select>' +
                '</td>' +
                '<td>' +
                '<select class="text-box single-line" name="SizeId[' + counter + ']" asp-for="SizeId" ><option value="">All</option>@foreach (var size in service.SizeIdAndNameList()){<option value="@size.Id">@size.Name</option>}</select>' +
                '</td>' +
                '<td>' +
                '<select class="text-box single-line" name="GradeId[' + counter + ']" asp-for="GradeId" ><option value="">All</option>@foreach (var grade in service.GradeIdAndNameList()){<option value="@grade.Id">@grade.Name</option>}</select>' +
                '</td>' +
                '<td>' +
                '<input class="text-box single-line" name="FscClaim[' + counter + ']" value="" required="required" />' +
                '</td>' +
                '<td>' +
                '<select class="text-box single-line" name ="FscSertificate[' + counter + ']" asp-for = "FscSertificate"><option value="">-</option> @foreach (var fsc in mycompanyService.MyCompaniesFscList()){<option value="@fsc">@fsc</option>}</select>' +
                '</td>' +
                '<td>' +
                '<select class="text-box single-line" name ="Unit[' + counter + ']" asp-for = "Unit"><option value="">All</option> @foreach (var unit in service.GetUnits()){<option value="@unit">@unit</option>}</select>' +
                '</td>' +

                '<td>' +
                '<input class="text-box single-line" name="Price[' + counter + ']" value="" required="required" />' +
                '</td>' +
                '<td>' +
                '<input asp-for="Products.ElementAt(i).Pallets" class="text-box single-line" name="Pallets[' + counter + ']" value="" required="required" />' +
                '</td>' +
                '<td>' +
                '<input class="text-box single-line" name="SheetsPerPallet[' + counter + ']" value="" required="required" />' +
                '</td>' +
                '<td>' +
                '<input class="text-box single-line" name="Quantity[' + counter + ']" value="" required="required" />' +
                '</td>' +
                '<td>' +
                '<button type="button" class="btn btn-primary" onclick="removeTr(' + counter + ');">Delete</button>' +
                '</td>' +
                '</tr>').appendTo('#clientTable');
            counter++;
            return false;
        });
    });

    function removeTr(index) {
        if (counter > 1) {
            $('#tablerow' + index).remove();
            counter--;
        }
        return false;
    }
