(function ($) {
    $(function () {
        bindChosen();
        bindEventDropDown();
        bindHtmlEditor();
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
            svgPath: '/css/lib/trumbowyg.icons.svg'
        });
    }

    function updateHtml(el, html) {
        el.trumbowyg('html', html || '');
    }
})(jQuery);