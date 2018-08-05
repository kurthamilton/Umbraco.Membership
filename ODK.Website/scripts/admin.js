(function ($) {
    $(function () {
        bindChosen();
        bindEventDropDown();
        bindFormSubmit();
        bindHtmlEditor();
        bindNav();
        bindTableFilters();
        bindTableSort();
    });

    function bindChosen() {
        $('.js-chosen').chosen({
            placeholder_text_multiple: ' ',
            placeholder_text_single: ' '
        });
    }

    function bindEventDropDown() {
        var subject = $('#js-event-invite-subject');
        var body = $('#js-event-invite-body');
        var details = $('.js-event-details');
        var events = $('.js-events');

        var selectEvent = function () {
            var eventId = events.val();
            subject.val($('.js-event-invite-subject[data-event-id="' + eventId + '"]').val());

            var html = $('.js-event-invite-body[data-event-id="' + eventId + '"]').val();
            updateHtml(body, html);

            details.addClass('d-none');
            details.filter('[data-event-id="' + eventId + '"]').removeClass('d-none');
        };

        events.on('change', function () {
            selectEvent();
        });

        selectEvent();
    }

    function bindFormSubmit() {
        $('.js-submit-on-change').on('change', function () {
            var form = $(this).closest('form');
            $.ajax({
                data: form.serialize(),
                method: 'POST',
                url: form.attr('action'),
                success: function () {
                }
            });
        });
    }

    function bindHtmlEditor() {
        $('.js-html-editor').trumbowyg({
            semantic: false,
            svgPath: '/css/lib/trumbowyg/trumbowyg.icons.svg'
        });
    }

    function bindNav() {
        // add current nav item to url
        $('.nav-link[data-toggle="pill"]').on('click', function () {
            window.location.hash = $(this).attr('href');
        });

        // set active nav item from url
        var link = $('.nav-link[data-toggle="pill"][href="' + window.location.hash + '"]');
        if (link.length === 0) {
            return;
        }

        link.tab('show');
    }

    function bindTableFilters() {
        var filters = $('[data-toggle="table-filter"]');
        filters.each(function () {
            var filter = $(this);
            var target = filter.data('target');
            var rows = $('tbody tr', target);
            filter.on('change', function () {
                applyFilter(rows, filters, target);
            });

            applyFilter(rows, filters, target);
        });

        function applyFilter(rows, filters, target) {
            filters = filters.filter('[data-target="' + target + '"]');

            rows.removeClass('d-none');
            rows.each(function () {
                var row = $(this);
                var show = true;

                filters.each(function () {
                    var tableFilter = $(this);
                    var filterValues = tableFilter.val();
                    if (!Array.isArray(filterValues) && filterValues) {
                        filterValues = [filterValues];
                    }

                    if (filterValues.length) {
                        var rowValues = row.data(tableFilter.data('field')).toString().split(',');
                        if (show) {
                            var intersect = filterValues.filter(function (value) {
                                return rowValues.indexOf(value.toString()) !== -1;
                            });
                            show = !!intersect.length;
                        }
                    }
                });

                if (show === false) {
                    row.addClass('d-none');
                }
            });
        }
    }

    function bindTableSort() {
        $(".js-table-sortable").tablesorter({
            // sort on the first column
            sortList: [[0, 0]]
        });
    }

    function updateHtml(el, html) {
        el.trumbowyg('html', html || '');
    }
})(jQuery);