jQuery.noConflict();

function GetCheckboxOptions(formClass, optionName) {
    var params = [];

    var options = jQuery(formClass + " input[name='" + optionName + "']:checked");
    if (options.length < 1)
        return params;

    options.each(function () {
        params.push(jQuery(this).val());
    });

    return params;
}