$(document).ready(function () {

    $("#email-button").click(function (event) {
        $.ajax({
            type: "POST",
            url: "/account/ConfirmEmail",
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#email-button").replaceWith("Error, try again later")
            },
            success: function (result) {
                $("#email-button").replaceWith("Sent!")
            }

        });
        return false;
    });

    $("#refbutton").click(function (event) {
        $("#referral-link").show();
    });

    $('#Country').change(function () {
        
        var countryName = $('#Country').find(":selected").text();
        var usStatesHtml = '<select class="form-control" id="State" name="state"><option value="AL">Alabama</option><option value="AK">Alaska</option><option value="AZ">Arizona</option><option value="AR">Arkansas</option><option value="CA">California</option><option value="CO">Colorado</option><option value="CT">Connecticut</option><option value="DE">Delaware</option><option value="DC">District Of Columbia</option><option value="FL">Florida</option><option value="GA">Georgia</option><option value="HI">Hawaii</option><option value="ID">Idaho</option><option value="IL">Illinois</option><option value="IN">Indiana</option><option value="IA">Iowa</option><option value="KS">Kansas</option><option value="KY">Kentucky</option><option value="LA">Louisiana</option><option value="ME">Maine</option><option value="MD">Maryland</option><option value="MA">Massachusetts</option><option value="MI">Michigan</option><option value="MN">Minnesota</option><option value="MS">Mississippi</option><option value="MO">Missouri</option><option value="MT">Montana</option><option value="NE">Nebraska</option><option value="NV">Nevada</option><option value="NH">New Hampshire</option><option value="NJ">New Jersey</option><option value="NM">New Mexico</option><option value="NY">New York</option><option value="NC">North Carolina</option><option value="ND">North Dakota</option><option value="OH">Ohio</option><option value="OK">Oklahoma</option><option value="OR">Oregon</option><option value="PA">Pennsylvania</option><option value="RI">Rhode Island</option><option value="SC">South Carolina</option><option value="SD">South Dakota</option><option value="TN">Tennessee</option><option value="TX">Texas</option><option value="UT">Utah</option><option value="VT">Vermont</option><option value="VA">Virginia</option><option value="WA">Washington</option><option value="WV">West Virginia</option><option value="WI">Wisconsin</option><option value="WY">Wyoming</option></select>'
        var notUsHtml = '<input class="form-control" id="State" name="State" placeholder="State" type="text" value="" />'
        if (countryName == "United States") {
            $('#State').replaceWith(usStatesHtml);
        }
        if (countryName != "United States"){
            $('#State').replaceWith(notUsHtml);
        }

    });

    $("#unsubscribe-button").click(function(event){  
        var input = confirm("Do you wish to unsubscribe from unsubscribing from the unsubscribe?")

        if (input == true) {
            $.ajax({
                type: "POST",
                url: "/services/unsubscribe",
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert("There was an error. Please try again later")
                },
                success: function (result) {
                    $('#accountinfo').load('/account/settings #accountinfo')
                }

            });
        }
        else {
            alert("There was an error. Please try again later")
        }
    });

    $("#upload-button").click(function (event) {
        $("#uploadphoto").show();
        $("#uploadphotobtn").hide();
    });
    $("#upload-mainbtn").click(function (event) {
        $("#uploadmainphoto").show();
        $("#upload-mainbtn").hide();
    });

    $("#editbio").click(function (event) {
        $("#save-user-bio").show();
        $("#editbio").hide();
    });

    $("#editbio").click(function (event) {
        var id = $("#editbio").attr("user-id")

        var datavalue = { "id" : id};

        $.ajax({
            url: '/Home/ajaxUserProfileBio',
            type: 'POST',
            data: datavalue,
            dataType: 'json',
            success: function (data) {
                $('#bio-aboutme').replaceWith('<textarea rows="5" cols="30"  id = "bioAboutMe"> ' + data['Bio'].AboutMe + ' </textarea>')
                $('#bio-location').replaceWith('<textarea rows="1" cols="30"  id = "bioAboutMe"> ' + data['Bio'].Location+ ' </textarea>');
                $('#bio-gender').replaceWith('<textarea rows="1" cols="30"  id = "bioAboutMe"> ' + data['Bio'].Gender + ' </textarea>');
                $('#bio-quote').replaceWith('<textarea rows="5" cols="30"  id = "bioAboutMe"> ' + data['Bio'].Quote + ' </textarea>');
            },
            error: function (request, error) {
                alert("Request: " + JSON.stringify(request));
            }
        });
    });

    $("#savebio").click(function (event) {
        var id = $("#savebio").attr("user-id")

        var aboutme = $("#bioAboutMe").val();
        var location = $("#bioLocation").val();
        var gender = $("#bioGender").val();
        var quote = $("#bioQuote").val();

        var datavalue = { "id": id,"AboutMe": aboutme, "Location": location, "Gender": gender, "Quote": quote};

        $.ajax({
            url: '/Home/saveBio',
            type: 'POST',
            data: datavalue,
            success: function (data) {
                window.location.reload();
            },
            error: function (request, error) {
                alert("Request: " + JSON.stringify(request));
            }
        });
    });

    $("#search-button").click(function (event) {
        var searchdata = $("#search-input").val();
        
        if (searchdata.length != 0) {
            window.location.href = "/search/users?query=" + searchdata;
        }
    });

    $('#search-input').keypress(function (e) {
        if (e.keyCode == 13)
            $('#search-button').click();
    });

});