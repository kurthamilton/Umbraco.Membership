(function () {
    $('.js-stripe-form').each(function() {
        var form = $(this);

        var apiKey = form.data('api-key');

        // Create a Stripe client.
        var stripe = Stripe(apiKey);

        // Create an instance of Elements.
        var elements = stripe.elements();

        // Custom styling can be passed to options when creating an Element.
        // (Note that this demo uses a wider set of styles than the guide below.)
        var style = {
            base: {
                color: '#32325d',
                lineHeight: '18px',
                fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
                fontSmoothing: 'antialiased',
                fontSize: '16px',
                '::placeholder': {
                    color: '#aab7c4'
                }
            },
            invalid: {
                color: '#fa755a',
                iconColor: '#fa755a'
            }
        };

        // Create an instance of the card Element.
        var card = elements.create('card', { style: style });

        // Add an instance of the card Element into the `card-element` <div>.
        var cardElement = $('.js-card-element', form);
        card.mount('#' + cardElement[0].id);

        // Handle real-time validation errors from the card Element.
        card.addEventListener('change', function (event) {
            var displayError = $('.js-card-errors')[0];
            if (event.error) {
                displayError.textContent = event.error.message;
            } else {
                displayError.textContent = '';
            }
        });

        // Handle form submission.
        form.on('submit', function (e) {
            e.preventDefault();

            stripe.createToken(card).then(function (result) {
                if (result.error) {
                    // Inform the user if there was an error.
                    var errorElement = $('.js-card-errors')[0];
                    errorElement.textContent = result.error.message;
                } else {
                    // Send the token to your server.
                    stripeTokenHandler(result.token, $(this));
                }
            });
        });

        function stripeTokenHandler(token, form) {
            // Insert the token ID into the form so it gets submitted to the server
            var hiddenInput = $('.js-stripe-token', form);
            hiddenInput.val(token.id);

            // Submit the form
            form.submit();
        }
    });
})();