var total = "";
var amount = "";
var count = 1;
var productArray = new Array();
var amountArray = new Array();
var products = [];

$(document).ready(function () {
    init();
}



    $("#create-invoice-btn").on("click", function () {
        submitForm();
    });

    //--------------------------------- ADD ROW EVENT HANDLER
    $("#add-row-btn").on("click", function () {
        addRow();
    });

    //--------------------------------- REMOVE ROW EVENT HANDLER
    $("#delete-row-btn").on("click", function () {
        deleteRow(this);
    })

  

    //Hide delete button product-control
    $("#products #product-control #delete-row-btn").hide();

$("#amount").val("0.00");


    //Set value of total input
    $("#total").val("0,00");

    function addRow() {
        count++;

        var copy = $("#product-control")
            .clone(true)
            .appendTo("#products")
            .prop("id", "product-row");

        //Empty inputs
        $("#products #product-row:nth-child(" + count + ") #description").empty();
        $("#products #product-row:nth-child(" + count + ") #size").empty();
        $("#products #product-row:nth-child(" + count + ") #grade").empty();
        $("#products #product-row:nth-child(" + count + ") #_FSCclaim").empty();
        $("#products #product-row:nth-child(" + count + ") #_FSCcertificate").empty();

        $("#products #product-row:nth-child(" + count + ") #_pieces").empty();
        $("#products #product-row:nth-child(" + count + ") #_quantity").empty();
        $("#products #product-row:nth-child(" + count + ") #_price").empty();
        $("#products #product-row:nth-child(" + count + ") #amount").prop("value", "");

        //Append option
        $("#products #product-row:nth-child(" + count + ") #description").append(" <option value='type'>Description....</option>");
        $("#products #product-row:nth-child(" + count + ") #grade").append(" <option value='type'>Grade....</option>");
        $("#products #product-row:nth-child(" + count + ") #size").append(" <option value='type'>Size....</option>");
      
        //Show delete button
        $("#products #product-row:nth-child(" + count + ") #delete-row-btn").show();

        //Initialize Materialize CSS dropdowns
        $("#products #product-row:nth-child(" + count + ") select").material_select();
        $("#products #product-control select").material_select();

        return false;
    }

    //Remove product row
    function deleteRow(obj) {
        $(obj).closest(".row").remove();

        count--;
        calcTotal();
    }

    //Calculate total price of all products
    function calcTotal() {
        var amounts = new Array();
        var pids = new Array();
        var discount = 0;
        var totalPrice = Number.parseFloat("0");
        var totalAmount = Number.parseFloat("0");
        var continueCalc = false;

        $("#products div div:nth-child(2) #description option:selected").each(function () {
            if ($(this).val() != "") {
                pids.push($(this).val());
                continueCalc = true;
            }
        });
        $("#products div div:nth-child(2) #grade option:selected").each(function () {
            if ($(this).val() != "") {
                pids.push($(this).val());
                continueCalc = true;
            }
        });
        $("#products div div:nth-child(2) #size option:selected").each(function () {
            if ($(this).val() != "") {
                pids.push($(this).val());
                continueCalc = true;
            }
        });
        $("#products div div:nth-child(2) #_FSCclaim option:selected").each(function () {
            if ($(this).val() != "") {
                pids.push($(this).val());
                continueCalc = true;
            }
        });
        $("#products div div:nth-child(2) #_FSCcertificate option:selected").each(function () {
            if ($(this).val() != "") {
                pids.push($(this).val());
                continueCalc = true;
            }
        });

        $("#products div div:nth-child(2) #_pieces option:selected").each(function () {
            if ($(this).val() != "") {
                pids.push($(this).val());
                continueCalc = true;
            }
        });

        $("#products div div:nth-child(2) #_quantity option:selected").each(function () {
            if ($(this).val() != "") {
                pids.push($(this).val());
                continueCalc = true;
            }
        });

        $("#products div div:nth-child(2) #_price option:selected").each(function () {
            if ($(this).val() != "") {
                pids.push($(this).val());
                continueCalc = true;
            }
        });


        $("#products div div:nth-child(3) #_amount").each(function () {
            var amount = 0;

            if ($(this).val() != "") {
                amount = $(this).val();
            } else {
                amount = 0;
            }

            amounts.push(amount);
        });

        $("#products div div:nth-child(3) #total").each(function () {
            var amount = 0;

            if ($(this).val() != "") {
                amount = $(this).val();
            } else {
                amount = 0;
            }

            amounts.push(amount);
        });

       
        if (amounts.length != pids.length) {
            $("#total").prop("readonly", false);
            $("#total").val("0,00");
            $("#total").prop("readonly", true);
        }

        if (continueCalc == true) {
            for (var i = 0; i < pids.length; i++) {
                var id = pids[i].split('_')[0];
                var p = pids[i].split('_')[1];
                var price = Number(p.replace(/,/, '.'));
                var amount = Number.parseInt(amounts[i]);

                var discountPercentage = 100 - discount;
                var totalBeforeDiscount = (price * amount);
                var totalAfterDiscount = (totalBeforeDiscount * discountPercentage) / 100;

                totalPrice += totalAfterDiscount;
            }

            $("#total").prop("readonly", false);
            $("#total").val(totalPrice.toLocaleString("nl-NL", { minimumFractionDigits: 2 }));

            total = totalPrice.toLocaleString("nl-NL", { minimumFractionDigits: 2 });
            $("#total").prop("readonly", true);
        }
        if (continueCalc == false && pids.length == 0 && ($("#total").val() != 0 || $("#total").val() != "0")) {
            $("#total").prop("readonly", false);
            $("#total").val("0,00");
            $("#total").prop("readonly", true);
        }
    }

    //Execute when save button is clicked
    function submitForm() {
        $("#products div div:nth-child(2) #_product option:selected").each(function () {
            productArray.push($(this).val().split('_')[0]);
        })

        $("#products div div:nth-child(3) #_amount").each(function () {
            amountArray.push($(this).val());
        })

        var totalPrice = $("#total").val();

        $("#form").attr("action", "Create/?pids=" + productArray.toString() + "&amounts=" + amountArray.toString() + "&total=" + totalPrice.toString());
        $("#form").submit();
    }
