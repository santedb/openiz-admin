// courtesy of https://stackoverflow.com/questions/8757963/avoiding-duplicate-form-submission-in-asp-net-mvc-by-clicking-submit-twice

// on a valid form, we want to find the submit button and re-enable it.
$(document).on("invalid-form.validate", "form", function ()
{
    var button = $(this).find('button[type="submit"]');

    setTimeout(function ()
    {
        button[0].textContent = button[0].innerText;
        button.removeAttr("disabled");
    }, 1);
});

// on an invalid form state, we want to find the submit button and disable it.
$(document).on("submit", "form", function ()
{
    var button = $(this).find('button[type="submit"]');

    if (button[0] !== undefined)
    {
        var disableWait = button[0].attributes["disable-wait"];

        // this is to prevent ajax form submit buttons from being disabled
        if (disableWait === undefined || disableWait === null || disableWait.value === "false")
        {
            setTimeout(function()
            {
                button[0].textContent = "Please wait";
                button.attr("disabled", "disabled");
            }, 0);
        }
    }
});

$(document).ready(function()
{
    $("#confirmDeleteYes").on("click", function (e)
    {
        var options = $("#confirm-delete-modal").data('bs.modal').options;

        $(this).text("Please wait");
        $(this).attr("disabled", "disabled");
        $(options.form).submit();
    });
});

function showModal(form)
{
    $("#confirm-delete-modal").modal({
        backdrop: 'static',
        form: "#" + form
    });
}