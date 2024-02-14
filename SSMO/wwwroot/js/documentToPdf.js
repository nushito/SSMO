$(function () {
    $("#btnSubmit").click(function () {
        $("input[name='exportData']").val($("#invoice").html());
    });
});