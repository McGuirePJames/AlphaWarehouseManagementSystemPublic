$(document).ready(function (e) {
    $(document).on('click', '#ForgotPassword', function (e) {
        resetPassword(document.getElementById('ResetPasswordEmailAddress').value)
    })
})
function resetPassword(emailAddress) {
    var data = JSON.stringify({ 'emailAddress': emailAddress });
    $.ajax({
        beforeSend: function () {
            ClearTextFromElements("alert");
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/User/ForgotPassword",
        data: data,
        //data: JSON.stringify('{username: "' + document.getElementById('Username').value + '", password: "' + document.getElementById('Password').value + '"}'),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                document.getElementById('ResetPasswordSuccess').innerHTML = response.responseText;
            }
        }
    });
}
function ClearTextFromElements(className) {
    let eles = document.getElementsByClassName(className);

    for (let i = 0; i < eles.length; i++) {
        eles[i].innerHTML = "";
    }
};
