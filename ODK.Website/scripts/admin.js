(function ($) {
    $(function () {
        bindChosen();
        bindEventDropDown();
        bindHtmlEditor();
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
        }

        events.on('change', function () {
            selectEvent();
        });

        selectEvent();
    }

    function bindHtmlEditor() {
        $('.js-html-editor').trumbowyg({
            semantic: false,
            svgPath: '/css/lib/trumbowyg/trumbowyg.icons.svg'
        });
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
            rows.removeClass('d-none');

            filters.filter('[data-target="' + target + '"]').each(function () {
                var tableFilter = $(this);
                var filterValue = tableFilter.val();
                if (!Array.isArray(filterValue) && filterValue) {
                    filterValue = [filterValue];
                }

                if (filterValue.length) {
                    rows.each(function () {
                        var row = $(this);
                        var rowValue = row.data(tableFilter.data('field')).toString();
                        if (filterValue.indexOf(rowValue) === -1) {
                            row.addClass('d-none');
                        }
                    });
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