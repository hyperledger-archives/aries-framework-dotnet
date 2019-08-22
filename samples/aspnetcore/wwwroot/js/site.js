// Write your JavaScript code.
function copyToCliboard(element) {
    /* Select the text field */
    element.select();

    /* Copy the text inside the text field */
    document.execCommand("copy");
}

$(function () {
    $('[data-toggle="tooltip"]').tooltip();

    $('#invitation-details-form').submit(function(e){
        var inputElement = $("#invitation-details");
        if (inputElement.val() != "")
        {
            return true;
        }
        inputElement.addClass("is-invalid");
        return false;
    });
})