(function () {
    $('.js-payment-button--stripe').on('click', function(e) {
        var button = $(this);

        var handler = StripeCheckout.configure({
            key: button.data('api-key'),
            allowRememberMe: false,
            description: button.data('description'),
            email: button.data('email'),
            locale: 'auto',
            name: button.data('site-name'),
            token: function (token, args) {
                $('.js-payment-token').val(token.id);
                button.closest('form').submit();
            }
        });

        // Open Checkout with further options:
        handler.open({
            currency: button.data('currency'),
            amount: button.data('amount')
        });

        // Close Checkout on page navigation:
        window.addEventListener('popstate', function () {
            handler.close();
        });

        e.preventDefault();
    });
})();