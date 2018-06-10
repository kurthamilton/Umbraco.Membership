(function() {
    $(function () {
        $('.paypal-button').each(function () {
            var button = $(this);
            var paymentForm = button.closest('form');
            var executeForm = $(paymentForm.data('execute-form'));

            var payment;
            paypal.Button.render({
                env: 'production',

                commit: true,

                payment: function () {
                    var request =
                        post(paymentForm)
                        .then(function (data) {
                            payment = data;
                            return data.id;
                        });
                    return request;
                },

                onAuthorize: function (data) {
                    post(executeForm, {
                        payerId: data.payerID,
                        paymentId: data.paymentID
                    })
                    .then(function (data) {

                    });
                }

            }, button[0]);
        });
    });

    function post(form, data) {
        var formData = form.serializeArray();

        if (data) {
            for (var prop in data) {
                if (data.hasOwnProperty(prop)) {
                    formData.push({
                        name: prop,
                        value: data[prop]
                    });
                }
            }
        }

        return $.when(
            $.ajax({
                url: form.attr('action'),
                method: 'POST',
                data: formData
            })
        );
    }
})();