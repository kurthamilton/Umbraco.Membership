(function() {
    $(function () {
        $('[data-payment-form]').on('submit', function(e) {
            var externalForm = $(this);
            var internalForm = $(externalForm.data('payment-form'));

            $.ajax({
                url: internalForm.attr('action'),
                method: 'POST',
                data: internalForm.serialize(),
                success: function (token) {
                    externalForm.off('submit');
                    $('[data-payment-token]', externalForm).val(token);
                    externalForm.submit();
                }
            });

            e.preventDefault();
            return false;
        });
    });
})();