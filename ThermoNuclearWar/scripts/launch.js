$(document).ready(function () {
    var getSelectedCodeId = function () { return 1; };
    $("#launch").click(function () {
        $.ajax({
            url: "/api/Launches?launchCodeId=" + getSelectedCodeId(),
            method: "POST"
        })
        .done(function (data) {
                $("#resultPlaceholder").empty();
                $("#resultPlaceholder").append( "<div class=\"alert alert-success alert-dismissible\" role=\"alert\">" +
                    "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>" +
                    "Launch succesful!</div>" );
            })
        .fail(function (data) {
            $("#resultPlaceholder").empty();
            $("#resultPlaceholder").append("<div class=\"alert alert-danger alert-dismissible\" role=\"alert\">" +
                "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>" +
                "Launch error!<br />" + JSON.parse(data.responseText).Message + "</div>");
        });
    });
})