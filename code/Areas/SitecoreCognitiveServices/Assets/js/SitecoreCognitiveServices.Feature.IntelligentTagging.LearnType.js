
//learn type
jQuery(document).ready(function () {
    //handles learn type form
    var learnForm = ".learn-form";
    jQuery(learnForm + " .form-submit")
        .click(function (event) {
            event.preventDefault();
            
            var idValue = jQuery(learnForm + " #id").val();
            var languageValue = jQuery(learnForm + " #language").val();
            var dbValue = jQuery(learnForm + " #db").val();
            var contentValue = GetCheckboxOptions(learnForm, "contentOption");
            
            jQuery(".result-failure").hide();
            jQuery(".result-success").hide();
            jQuery(learnForm).hide();
            jQuery(".form-set").hide();
            jQuery(".scWizardButtons button").hide();
            jQuery(".progress-indicator").show();

            jQuery.post(
                jQuery(learnForm).attr("action"),
                {
                    id: idValue,
                    language: languageValue,
                    db: dbValue,
                    contentFields: contentValue
                }
            ).done(function (r) {
                if (r.Failed) {
                    jQuery(".result-failure").text(r.Message);
                    jQuery(".result-failure").show();
                    jQuery(".form-set").show();
                    jQuery(".scWizardButtons button").show();
                } else {
                    jQuery(".result-success").show();
                }

                jQuery(".progress-indicator").hide();
                jQuery(learnForm).show();
            });
        });
});