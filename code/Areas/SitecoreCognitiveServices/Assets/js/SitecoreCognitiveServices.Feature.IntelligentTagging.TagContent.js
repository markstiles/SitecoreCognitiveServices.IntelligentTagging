
//tag content
jQuery(document).ready(function () {
    //handles tag content form
    var tagForm = ".tag-form";
    var tagItemForm = ".tag-item-form";
    jQuery(tagForm + " .form-submit")
        .click(function (event) {
            event.preventDefault();

            var idValue = jQuery(tagForm + " #id").val();
            var dbValue = jQuery(tagForm + " #db").val();
            var contentValue = jQuery(tagForm + " #page-content").val();
            var classIdValue = jQuery(tagForm + " input[name='classifierOption']:checked").val();
            
            jQuery(tagForm + " .result-failure").hide();
            jQuery(".result-success").hide();
            jQuery(tagForm).hide();
            jQuery(".progress-indicator").show();

            jQuery.post(
                jQuery(tagForm).attr("action"),
                {
                    id: idValue,
                    database: dbValue,
                    content: contentValue,
                    classifierId: classIdValue
                }
            ).done(function (r) {
                if (r.Failed) {
                    jQuery(tagForm + " .result-failure").text(r.Message);
                    jQuery(tagForm + " .result-failure").show();
                } else {
                    var tagList = "";
                    for (var i = 0; i < r.Tags.length; i++) {
                        var isChecked = true ? "checked" : "";
                        tagList += "<div class='tag-item " + isChecked + "'>";
                        tagList += "<input type='checkbox' id='tagOption-" + r.Tags[i] + "' name='tagOption' value='" + r.Tags[i] + "'" + isChecked + " />";
                        tagList += "<label for='tagOption-" + r.Tags[i] + "'>" + r.Tags[i] + "</label>";
                        tagList += "</div>";
                    }
                    jQuery(tagItemForm + " .result-list").html(tagList);

                    jQuery(".tag-item")
                        .on("click", function () {
                            jQuery(this).removeClass("checked");
                            if (jQuery(this).find("input").is(':checked'))
                                jQuery(this).addClass("checked");
                        });
                }

                jQuery(".progress-indicator").hide();
                jQuery(tagItemForm).show();
            });
        });

    jQuery(tagItemForm + " .form-submit")
        .click(function (event) {
            event.preventDefault();

            var idValue = jQuery(tagForm + " #id").val();
            var languageValue = jQuery(tagForm + " #language").val();
            var dbValue = jQuery(tagForm + " #db").val();
            var tagsValue = GetCheckboxOptions(tagItemForm, "tagOption");
            var classIdValue = jQuery(tagForm + " input[name='classifierOption']:checked").val();

            jQuery(tagItemForm).hide();
            jQuery(tagItemForm + " .result-failure").hide();
            jQuery(".result-success").hide();
            jQuery(".progress-indicator").show();

            jQuery.post(
                jQuery(tagItemForm).attr("action"),
                {
                    id: idValue,
                    language: languageValue,
                    database: dbValue,
                    tags: tagsValue,
                    classifierId: classIdValue
                }
            ).done(function (r) {
                if (r.Failed) {
                    jQuery(tagItemForm + " .result-failure").text(r.Message);
                    jQuery(tagItemForm + " .result-failure").show();
                } else {
                    jQuery(".result-success .tags").text(tagsValue.join(', '));
                    jQuery(".result-success").show();
                }

                jQuery(".progress-indicator").hide();
            });
        });
    
    var timer = setInterval(function () {
        var charCount = jQuery(tagForm + " #page-content").val().length;
        jQuery(tagForm + " .character-count").text(charCount + " characters (max of 1022)");
    }, 250);
});