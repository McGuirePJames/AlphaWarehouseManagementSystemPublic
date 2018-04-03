$(document).ready(function (e) {
    $(document).on('click', "#ButtonResetPassword", function (e) {
        resetPassword()
    });
});
function resetPassword(userId, token, password, confirmPassword) {
    var token = getUrlParam('code', null);
    var userId = getUrlParam('UserId', null);
    var password = document.getElementById('Password').value;
    var confirmPassword = document.getElementById('PasswordConfirm').value;

    if (!doPasswordsMatch(password, confirmPassword)) {
        document.getElementById('ResetPasswordError').innerHTML = "Passwords do not match";
        return;
    }
    ajaxResetPassword(userId, token, password);
}
function ajaxResetPassword(userId, token, password) {
    var data = JSON.stringify({ 'userId': userId, 'token': token, 'password': password });
    $.ajax({
        beforeSend: function () {
            ClearTextFromElements("alert");
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/User/ResetPassword",
        data: data,
        //data: JSON.stringify('{username: "' + document.getElementById('Username').value + '", password: "' + document.getElementById('Password').value + '"}'),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                document.getElementById('ResetPasswordSuccess').innerHTML = response.responseText;
                window.location.href = "/User/Login"
            }
            else {
                document.getElementById('ResetPasswordError').innerHTML = response.responseText;
            }
        }
    });
}

function doPasswordsMatch(password, confirmPassword) {
    if (password === confirmPassword) {
        return true;
    }
    return false;
}
function getUrlVars() {
    var vars = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
        vars[key] = value;
    });
    return vars;
}
function getUrlParam(parameter, defaultvalue) {
    var urlparameter = defaultvalue;
    if (window.location.href.indexOf(parameter) > -1) {
        urlparameter = getUrlVars()[parameter];
    }
    return urlparameter;
}

function ClearTextFromElements(className) {
    let eles = document.getElementsByClassName(className);

    for (let i = 0; i < eles.length; i++) {
        eles[i].innerHTML = "";
    }
};
