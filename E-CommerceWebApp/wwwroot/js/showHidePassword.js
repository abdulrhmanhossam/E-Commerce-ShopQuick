$(document).ready(function () {

    $("#hideShowPassword").removeClass("far fa-eye").addClass("far fa-eye-slash");

    $("#hideShowPassword").click(function () {
        const passwordInput = $("#password-field");
        const type = passwordInput.attr("type");

        if (type == "password") {
            passwordInput.attr("type", "text");
            $("#hideShowPassword").removeClass("far fa-eye-slash").addClass("far fa-eye");
        } else {
            passwordInput.attr("type", "password");
            $("#hideShowPassword").removeClass("far fa-eye").addClass("far fa-eye-slash");
        }
    });

});