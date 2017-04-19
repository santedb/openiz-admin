$(document).ready(function () {
    $(".reference-term-search").select2({
        ajax: {
            url: "/Concept/SearchAjax",
            dataType: 'json',
            method: "GET",
            delay: 500,
            data: function (params) {
                return {
                    searchTerm: params.term
                };
            },
            processResults: function (data, params) {
                params.page = params.page || 1;

                return {
                    results: $.map(data, function (o) {
                        return { id: o.Id, text: o.Mnemonic };
                    }),
                    pagination: {
                        more: (params.page * 30) < data.length
                    }
                };
            },
            cache: true
        },
        maximumSelectionLength: 6,
        keepSearchResults: true,
        escapeMarkup: function (markup) { return markup; },
        minimumInputLength: 2,
        templateResult: function (data) {
            return "<span class='glyphicon glyphicon-list'> " + data.text + "</span>";
        },
        templateSelection: function (data) {
            return data.text;
        }
    });
});