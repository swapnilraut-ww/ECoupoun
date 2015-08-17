$(document).ready(function () {
    $(document).on("click", ".viewproduct", function () {
        $.ajax({
            method: "POST",
            url: "/Product/SaveProductViewDetails",
            data: { providerId: $(this).data("providerid"), sku: $(this).data("sku") },
            success: function(data)
            {
                alert(data.Message);
            }
    });
  
});
});