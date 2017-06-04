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
                $('#bio-location').replaceWith('<textarea rows="1" cols="30"  id = "bioLocation"> ' + data['Bio'].Location + ' </textarea>');
                $('#bio-gender').replaceWith('<textarea rows="1" cols="30"  id = "bioGender"> ' + data['Bio'].Gender + ' </textarea>');
                $('#bio-quote').replaceWith('<textarea rows="5" cols="30"  id = "bioQuote"> ' + data['Bio'].Quote + ' </textarea>');
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

    $("#follow-button").click(function (event) {
        var id = $("#follow-button").attr("user-id")

        var datavalue = { "id": id };

        $.ajax({
            url: '/Home/addFollowing',
            type: 'POST',
            data: datavalue,
            success: function (data) {
                $("#unfollow-button").show();
                $("#follow-button").hide();
            },
            error: function (request, error) {
                alert("Request: " + JSON.stringify(request));
            }
        });
    });

    $("#unfollow-button").click(function (event) {
        var id = $("#unfollow-button").attr("user-id")

        var datavalue = { "id": id };

        $.ajax({
            url: '/Home/removeFollowing',
            type: 'POST',
            data: datavalue,
            success: function (data) {
                $("#follow-button").show();
                $("#unfollow-button").hide();
            },
            error: function (request, error) {
                alert("Request: " + JSON.stringify(request));
            }
        });
    });

    /*
    $("#add-comment").click(function (event) {
        var id = $("#add-comment").attr("user-id")
        var postid = $("#add-comment").attr("post-id")
        var message = $("#user-comment").val();

        var datavalue = { "id": id, "PostId": postid, "Message": message };

        $.ajax({
            url: '/Home/addComment',
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
    */

    /*
    $(".commentButton").click(function (event) {
        var stringId = $(this).attr('id');
        var id = $(this.id).attr("user-id")
        var postid = $(this.id).attr("post-id")
        var message = document.getElementById(stringId).getElementsByClassName("commentText")[0];

        var datavalue = { "id": id, "PostId": postid, "Message": message };

        $.ajax({
            url: '/Home/addComment',
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
    */

    $(".commentButton").click(function (event) {
        var stringId = $(this).attr('id');
        var elements = document.getElementsByClassName("commentText");
        var final = "blank";

        for (i = 0; i < elements.length; i++) {
            if (elements[i].id == stringId) {
                final = elements[i];
            }
        }




        alert(message);

    });

    function textChangeListener(evt) {
        var id = evt.target.id;
        var text = evt.target.value;

        if (id == "topLineText") {
            window.topLineText = text;
        } else {
            window.bottomLineText = text;
        }

        redrawMeme(window.imageSrc, window.topLineText, window.bottomLineText);
    }
    //------
    function redrawMeme(image, topLine, bottomLine) {
        // Get Canvas2DContext
        var canvas = document.querySelector('canvas');
        var ctx = canvas.getContext("2d");
        if (image != null)
            ctx.drawImage(image, 0, 0, canvas.width, canvas.height);

        // clear previous
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        if (image != null)
            ctx.drawImage(image, 0, 0, canvas.width, canvas.height);

        // Text attributes
        ctx.font = '30pt Impact';
        ctx.textAlign = 'center';
        ctx.strokeStyle = 'black';
        ctx.lineWidth = 3;
        ctx.fillStyle = 'white';

        if (topLine != null) {
            ctx.fillText(topLine, canvas.width / 2, 40);
            ctx.strokeText(topLine, canvas.width / 2, 40);
        }

        if (bottomLine != null) {
            ctx.fillText(bottomLine, canvas.width / 2, canvas.height - 20);
            ctx.strokeText(bottomLine, canvas.width / 2, canvas.height - 20);
        }
    }

    function saveFile() {
        window.open(document.querySelector('canvas').toDataURL());
    }
    //--------

    function handleFileSelect(evt) {
        //make canvas
        var canvasWidth = 400;
        var canvasHeight = 400;
        var file = evt.target.files[0];

        //image upload
        var reader = new FileReader();
        reader.onload = function (fileObject) {
            var data = fileObject.target.result;

            // Create an image object
            var image = new Image();
            image.onload = function () {

                window.imageSrc = this;
                redrawMeme(window.imageSrc, null, null);
            }

            // Set image data to background image.
            image.src = data;
            console.log(fileObject.target.result);
        };
        reader.readAsDataURL(file)
    }
    window.imageSRC = null;
    window.topLineText = "";
    window.bottomLineText = "";
    window.imageSRC = null;
    window.topLineText = null;
    window.bottomLineText = null;

    var file = document.querySelector("#file");
    file.onchange = handleFileSelect;

    var input1 = document.getElementById('topLineText');
    var input2 = document.getElementById('bottomLineText');
    input1.oninput = textChangeListener;
    input2.oninput = textChangeListener;
    document.getElementById('file').addEventListener('change', handleFileSelect, false);
    document.querySelector('button').addEventListener('click', saveFile, false);







});