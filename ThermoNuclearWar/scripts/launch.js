$(document).ready(function () {
    checkStatusLoop();

    var getSelectedCodeId = function() { return 1; };
    $("#launch").click(function () {
        $("#launch").addClass("disabled");
        $.ajax({
            url: "/api/Launches?launchCodeId=" + getSelectedCodeId(),
            method: "POST"
        })
            .done(function(data) {
                $("#resultPlaceholder").empty();
                $("#resultPlaceholder").append("<div class=\"alert alert-success alert-dismissible\" role=\"alert\">" +
                    "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>" +
                    "Launch succesful!</div>");
            })
            .fail(function(data) {
                $("#resultPlaceholder").empty();
                $("#resultPlaceholder").append("<div class=\"alert alert-danger alert-dismissible\" role=\"alert\">" +
                    "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>" +
                    "Launch error!<br />" + JSON.parse(data.responseText).Message + "</div>");
            })
            .always(function () {
                $("#launch").removeClass("disabled");
            });
    });
});

function checkStatusLoop() {
    $.ajax({
        url: "/api/Launches"
    })
        .done(function(data) {
            $("#status").removeClass("label-warning label-danger label-success");

            if (data) {
                $("#status").addClass("label-success");
                $("#status").text("Status: online");
            } else {
                $("#status").addClass("label-danger");
                $("#status").text("Status: offline");
            }
        })
        .fail(function(data) {
            $("#status").removeClass("label-warning label-danger label-success");
            $("#status").addClass("label-warning");
            $("#status").text("Couldn't retrieve status!");
        });

    window.setTimeout('checkStatusLoop()', 1000);
}