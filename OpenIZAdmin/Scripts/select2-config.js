$(document).ready(function () {
    // Get the selected language and use that select2 localization
    var match = document.cookie.match(new RegExp('(^| )__openiz_language=([^;]+)'));
    var locale = match ? match[2] : "en";
    $.fn.select2.defaults.set('language', locale);
});