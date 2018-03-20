$(document).ready(function (e) {
    $("#ButtonLogin").on('click', function () {
        Login();
    });
});
function Login() {

    if (document.getElementById("Username").value == "") {
        document.getElementById('LoginError').value = "Please enter a username"
    }
    else if (document.getElementById("Password").value == "") {
        document.getElementById('LoginError').value = "Please enter a password"
    }
    else
    {
        let username = document.getElementById('Username').value
        let password = document.getElementById('Password').value
        AjaxLogin(username, password)
    }
}
function AjaxLogin(username, password) {
    let data = JSON.stringify({ 'username': username, 'password': password });

    $.ajax({
        beforeSend: function () {
            ClearTextFromElements("alert");
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/User/Login",
        data: data,
        //data: JSON.stringify('{username: "' + document.getElementById('Username').value + '", password: "' + document.getElementById('Password').value + '"}'),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                window.location = "/Home/Home";
            }
            else {
                document.getElementById('LoginError').innerHTML = response.responseText
                $('html').css('cursor', 'default');
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
