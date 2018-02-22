(function() {
    $(function () {
        bindInstagramFeed();
        bindCustomFileInputs();
        bindDropDownOther();
        bindImageSubmit();
        bindModals();
        bindSearch();
        bindTooltips();
    });

    function bindCustomFileInputs() {
        $('.custom-file-input').on('change',function() {
            var files = $(this)[0].files;
            var fileNames = [];
            for (var i = 0; i < files.length; i++) {
                fileNames.push(files[i].name);
            }

            $(this).next('.custom-file-label').html(fileNames.join('; '));
        });
    }

    function bindDropDownOther() {
        $('[data-other-for]').each(function() {
            var other = $(this);
            var parent = $('#' + other.data('other-for'));
            var match = other.data('other-key');

            setOtherVisibility(parent, match, other);
            parent.on('change', function () {
                setOtherVisibility(parent, match, other);
            });
        });
    }

    function bindImageSubmit() {
        $('[data-submit]').each(function() {
            var submitter = $(this);
            var form = submitter.closest('form');
            if (form.length === 0) {
               return;
            }
           
            submitter.on('click', function() {
                var input = $('[data-submit-value]', form);
                if (input.length === 1) {
                    input.val($(this).data('value'));
                }
                
                var cssClass = submitter.data('active-class');
                
                $.ajax({
                    data: form.serialize(),
                    method: 'POST',
                    url: form.attr('action'),
                    success: function() {
                        $('[data-submit]', form).removeClass(cssClass);
                        submitter.addClass(cssClass);
                    }
                });
            });
        });
    }

    function bindInstagramFeed() {
        $('[data-instagram-url]').each(function() {
            var container = $(this);
            var maxItems = container.data('instagram-maxitems');
            var template = $('.media-template--instagram').children().first();
        
            var url = container.data('instagram-url');
            $.ajax({
                url: url,
                success: function(data) {
                    var items = data.user.media.nodes;
                    for (var i = 0; i < Math.min(items.length, maxItems); i++) {
                        var clone = template.clone();
                
                        var img = $('img', clone);
                        img[0].src = items[i].thumbnail_src;
                
                        var link = $('a', clone);
                        link[0].href = link[0].href.replace('{code}', items[i].code);
                
                        clone.appendTo(container);
                    }
                }
            });
        });
    }

    function bindModals() {
        $('.modal.show').modal('show');
    }

    function bindSearch() {
        var targets = $('.search-target');
        if (targets.length === 0) {
            return;
        }

        $('.search').on('input', function () {
            var term = $(this).val().toLowerCase();
            targets.each(function () {
                var target = $(this);

                var show = false;
                $('.search-field', target).each(function() {
                    var field = $(this);
                    if (field.html().toLowerCase().indexOf(term) >= 0) {
                        show = true;
                    }
                });

                if (show) {
                    target.removeClass('d-none');
                } else {
                    target.addClass('d-none');
                }
            });
        });
    }

    function bindTooltips() {
        $('[data-toggle="tooltip"]').tooltip();
    }

    function setOtherVisibility(parent, match, other) {
        var parentValue = parent.val();
        if (parentValue == match) {
            other.show();
        } else {
            other.hide();
        }
    }
})();