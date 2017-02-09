$(document).ready(function () {

    $("#email-button").click(function (event) {
        $.ajax({
            type: "POST",
            url: "/Account/ConfirmEmail",
            data: {},
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            },
            success: function (result) {
                alert("Email Sent");
            }

        });
        return false;
    });

    $("#refbutton").click(function (event) {
        $("#referral-link").show();
    });

});

/*
function showReferralLink() {
    document.getElementById('referral-link').style.display = "block";
}
*/