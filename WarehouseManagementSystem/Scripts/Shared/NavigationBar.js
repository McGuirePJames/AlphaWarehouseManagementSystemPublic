$(document).ready(function (e) {
    $(document).on('click', '.icon-container-sign-out svg', function (e) {
        signOut();
    })
})
function signOut() {
    $.ajax({
        beforeSend: () => {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/User/Logout",
        success: (response) => {
            if (response.success) {
                window.location.href = "/User/Login"
            }
        },
        error: () => {
            $('html').css('cursor', 'default');
        },
        complete: () => {
            $('html').css('cursor', 'default');
        }
    })
}