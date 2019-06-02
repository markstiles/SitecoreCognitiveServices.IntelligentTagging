
//train classifier
jQuery(document).ready(function () {
    //handles train classifier form
    var trainForm = ".train-classifier-form";
    jQuery(trainForm + " .form-submit")
        .click(function (event) {
            event.preventDefault();

            var idValue = jQuery(trainForm + " #id").val();
            var dbValue = jQuery(trainForm + " #db").val();

            jQuery(".result-failure").hide();
            jQuery(".result-success").hide();
            jQuery(trainForm + " .form-submit").hide();
            jQuery(trainForm).hide();
            jQuery(".progress-indicator").show();

            jQuery.post(
                jQuery(trainForm).attr("action"),
                {
                    id: idValue,
                    database: dbValue,
                }
            ).done(function (r) {
                if (r.Failed) {
                    jQuery(".result-failure .message").text(r.Message);
                    jQuery(".result-failure").show();
                } else {
                    jQuery(".result-success").show();
                }

                jQuery(".progress-indicator").hide();
                jQuery(trainForm).show();
            });
        });
});