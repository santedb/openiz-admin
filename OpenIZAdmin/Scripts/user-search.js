$(document).ready(function ()
{
    $(".user-search").select2({
        ajax: {
            url: "/User/SearchAjax",
            dataType: 'json',
            method: "GET",
            delay: 500,
            data: function (params)
            {
                return {
                    searchTerm: params.term
                };
            },
            processResults: function (data, params)
            {
                params.page = params.page || 1;

                return {
                    results: $.map(data, function (o)
                    {
                        return { id: o.UserId, text: o.Username };
                    }),
                    pagination: {
                        more: (params.page * 30) < data.length
                    }
                };
            },
            cache: true
        },
        maximumSelectionLength: 1,
        keepSearchResults: true,
        escapeMarkup: function (markup) { return markup; },
        minimumInputLength: 4,
        templateResult: function (data)
        {
            return "<span class='glyphicon glyphicon-map-marker'>" + data.text + "</span>";
        },
        templateSelection: function (data)
        {
            return data.text;
        }
    });
});