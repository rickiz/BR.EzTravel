function disableSubmit(form) {
    if ($(form).valid()) {
        $('button[type="submit"]').attr('disabled', 'disabled');
    }
}