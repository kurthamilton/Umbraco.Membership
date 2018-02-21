(function() {
    $(function () {
        bindCustomFileInputs();
        bindDropDownOther();
        bindModals();
        bindSearch();
        bindTooltips();

        loadInstagramFeed();
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

    function loadInstagramFeed() {
        var container = $('[data-instagram-username]');
        if (container.length === 0) {
            return;
        }

        var username = container.data('instagram-username');
        var maxItems = container.data('instagram-maxitems');
        var template = $('.media-template--instagram').children().first();

        var onload = function() {
            var response = JSON.parse(this.responseText);
            var items = response.user.media.nodes;
            for (var i = 0; i < Math.min(items.length, maxItems); i++) {
                var clone = template.clone();

                var img = $('img', clone);
                img[0].src = items[i].thumbnail_src;

                var link = $('a', clone);
                link[0].href = 'https://instagram.com/p/' + items[i].code;

                clone.appendTo(container);
            }
        };

        var request = new XMLHttpRequest();
        request.addEventListener('load', onload);
        request.open('GET', 'https://www.instagram.com/' + username + '/?__a=1');
        request.send();
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