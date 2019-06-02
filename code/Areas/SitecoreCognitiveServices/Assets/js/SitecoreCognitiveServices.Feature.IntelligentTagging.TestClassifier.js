
//test classifier
jQuery(document).ready(function () {
    //handles test classifier form
    var testForm = ".test-classifier-form";
    jQuery(testForm + " .form-submit")
        .click(function (event) {
            event.preventDefault();

            var idValue = jQuery(testForm + " #id").val();
            var languageValue = jQuery(testForm + " #language").val();
            var dbValue = jQuery(testForm + " #db").val();

            jQuery(".result-failure").hide();
            jQuery(".result-success").hide();
            jQuery(testForm).hide();
            jQuery(".progress-indicator").show();

            jQuery.post(
                jQuery(testForm).attr("action"),
                {
                    id: idValue,
                    language: languageValue,
                    database: dbValue,
                }
            ).done(function (r) {
                if (r.Failed) {
                    jQuery(".result-failure .message").text(r.Message);
                    jQuery(".result-failure").show();
                } else {
                    jQuery(".result-success .result-accuracy").text(r.Accuracy);
                    jQuery(".result-success .result-overage").text(r.Overage);
                    jQuery(".result-success .result-confidence").text(r.Confidence);
                    jQuery(".result-success").show();
                }

                jQuery(".progress-indicator").hide();
                jQuery(testForm).show();
            });
        });
});