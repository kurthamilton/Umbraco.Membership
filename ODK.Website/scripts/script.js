(function() {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
        $('.modal.show').modal('show');
    });

    $('[data-other-for]').each(function() {
        var other = $(this);
        var parent = $('#' + other.data('other-for'));
        var match = other.data('other-key');

        setOtherVisibility(parent, match, other);
        parent.on('change', function () {
            setOtherVisibility(parent, match, other);
        });
    });

    function setOtherVisibility(parent, match, other) {
        var parentValue = parent.val();
        if (parentValue == match) {
            other.show();
        } else {
            other.hide();
        }
    }
})();