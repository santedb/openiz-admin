$(document).ready(function ()
{
    $(".place-search").select2({
        ajax: {
            url: "/Place/SearchAjax",
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
                        return { id: o.Id, text: o.Name, data: o };
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
            var retVal = "";
            if (data.data) {

                if (data.data.Type)
                    retVal += "<span class='label label-info'>" + data.data.Type + "</span> ";
                retVal += data.text;

                // Address?
                if (data.data.Address && data.data.Address[0]) {

                    var addrString = "";
                    if (data.data.Address[0].AdditionalLocator)
                        addrString += data.data.Address[0].AdditionalLocator + ", ";
                    if (data.data.Address[0].StreetAddress)
                        addrString += data.data.Address[0].StreetAddress + ", ";
                    if (data.data.Address[0].Precinct)
                        addrString += data.data.Address[0].Precinct + ", ";
                    if (data.data.Address[0].City)
                        addrString += data.data.Address[0].City + ", ";
                    if (data.data.Address[0].County != null)
                        addrString += data.data.Address[0].County + ", ";
                    if (data.data.Address[0].State != null)
                        addrString += data.data.Address[0].State + ", ";
                    if (data.data.Address[0].Country != null)
                        addrString += data.data.Address[0].Country + ", ";

                    addrString = addrString.substring(0, addrString.length - 2);

                    retVal += "<small>(<span class='glyphicon glyphicon-map-marker'></span>" + addrString + ")</small>";
                }
            }
            else
                retVal += "<span class='glyphicon glyphicon-map-marker'></span> "  + data.text;
            return retVal;
        },
        templateSelection: function (data)
        {
            return data.text;
        }
    });
});