$(document).ready(function () {

    $("#email-button").click(function (event) {
        $.ajax({
            type: "POST",
            url: "/Account/ConfirmEmail",
            data: {},
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert("Error");
            },
            success: function (result) {
                alert("Email Sent");
            }

        });
        return false;
    });

});