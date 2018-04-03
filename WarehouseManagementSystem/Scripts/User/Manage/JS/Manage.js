$(document).ready(function (e) {
    $(document).on('change', ".user-detail-avatar-upload", function (e) {
        ajaxUploadImage(e);
    });
});
function cleanModelDate(dateString) {
    return dateString.replace(/\//g, '').replace('Date', '').replace(')', '').replace('(', '')
}
function convertEpochDate(epochDate) {
    var date = new Date(epochDate);
    return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear()
}
function formatDate(date, joinChar, yearFirstBool) {
    var d = new Date(date.split('/')[2], date.split('/')[0], date.split('/')[1]);
    var month = '' + (d.getMonth() + 1);
    var day = '' + d.getDate();
       var year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    if (yearFirstBool) {
        return [year, month, day].join(joinChar);
    }
    return [month, day, year].join(joinChar);
}
function formatToAmerican(inputDate) {
    var d = new Date(inputDate);
    d = dateToUTC(d);
    if (!isNaN(d.getTime())) {
       
        return d.getMonth() + 1 + '/' + d.getDate() + '/' + d.getFullYear();
    }
}
function dateToUTC(date) {
    return new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(), date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
}

function ajaxUploadImage(e) {
    var data = new FormData();
    var files = document.getElementById('ProfilePictureUpload').files;
    if (files.length > 0) {
        data.append("image", files[0]);
        data.append("userId", document.getElementsByClassName('user-detail-user-id')[0].getElementsByTagName('p')[0].innerHTML);
    }
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        url: "/User/UploadProfilePicture",
        type: "POST",
        processData: false,
        contentType: false,
        data: data,
        success: function (response) {
            if (response.success) {
                alert("uploaded Successfully");
            }
            else {
                alert("uploaded Successfully");
            }

        },
        error: function (er) {
            alert("error");
        },
        complete: function (e) {
            $('html').css('cursor', 'default');
        }

    });
}