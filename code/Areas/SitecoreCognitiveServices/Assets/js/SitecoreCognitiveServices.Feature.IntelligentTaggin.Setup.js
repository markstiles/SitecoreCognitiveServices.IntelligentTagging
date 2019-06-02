
//setup
jQuery(document).ready(function () {
    //handles setup form
    var setupForm = ".setup-form";
    jQuery(setupForm + " .form-submit")
        .click(function (event) {
            event.preventDefault();

            var nlcEndpointValue = jQuery(setupForm + " #nlcEndpoint").val();
            var nlcUsernameValue = jQuery(setupForm + " #nlcUsername").val();
            var nlcPasswordValue = jQuery(setupForm + " #nlcPassword").val();

            jQuery(".result-failure").hide();
            jQuery(".result-success").hide();
            jQuery(".progress-indicator").show();
            
            jQuery.post(
                jQuery(setupForm).attr("action"),
                {
                    naturalLanguageClassifierApiEndpoint: nlcEndpointValue,
                    naturalLanguageClassifierUsername: nlcUsernameValue,
                    naturalLanguageClassifierPassword: nlcPasswordValue
                }
            ).done(function (r) {
                if (r.Failed) {
                    jQuery(".result-failure .item-list").text(r.Items);
                    jQuery(".result-failure").show();
                } else {
                    jQuery(".result-success").show();
                }

                jQuery(".progress-indicator").hide();
            });
        });
});