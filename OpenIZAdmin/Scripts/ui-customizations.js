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

// on form submit, we want to find the submit button and disable it.
$(document).on("submit", "form", function (data)
{
    console.log(data);
    var button = $(this).find('button[type="submit"]');

    if (button[0] !== undefined)
    {
        var disableWait = button[0].attributes["disable-wait"];

        // this is to prevent ajax form submit buttons from being disabled
        if (disableWait === undefined || disableWait === null || disableWait.value === "false")
        {
            var content = button[0].attributes["data-locale-text"];

            var message;

            if (content === undefined || content === null || content.value === "")
            {
                message = "Please wait";
            }
            else
            {
                message = content.value;
            }
            
            setTimeout(function()
            {
                button[0].textContent = message;
                button.attr("disabled", "disabled");
            }, 0);
        }
    }
});