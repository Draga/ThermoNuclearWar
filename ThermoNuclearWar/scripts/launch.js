$(document).ready(function () {
    var getSelectedCodeId = function () { return 1; };
    $("#launch").click(function () {
        $.ajax({
            url: "/api/Launches?launchCodeId=" + getSelectedCodeId(),
            method: "POST"
        })
        .done(function (data) {
                $("#resultPlaceholder").empty();
                $("#resultPlaceholder").append( "<div class=\"alert alert-success\" role=\"alert\">Launch succesful!</div>" );
            })
        .fail(function (data) {
            $("#resultPlaceholder").empty();
            $("#resultPlaceholder").append("<div class=\"alert alert-danger\" role=\"alert\">Launch error!<br />" + JSON.parse(data.responseText).Message + "</div>");
        });
    });
})