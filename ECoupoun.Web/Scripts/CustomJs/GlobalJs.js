$(document).ready(function () {
    $(document).on("click", ".product_detail", function () {
        $.ajax({
            method: "POST",
            datatype: "JSON",
            url: "/Ajax/Product/SaveProductViewDetails",
            data: { providerId: $(this).data("providerid"), sku: $(this).data("sku"), productUrl: $(this).data("producturl") },
            success: function (data) {
                if (data.Success) {
                    var win = window.open(data.ProductUrl, '_blank');
                    if (win) {
                        //Browser has allowed it to be opened
                        win.focus();
                    } else {
                        //Broswer has blocked it
                        alert('Please allow popups for this site');
                    }
                }
            }
        });

    });
});