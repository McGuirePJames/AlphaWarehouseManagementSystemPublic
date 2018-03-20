$(document).ready(function (e) {
    $(document).on('change', ".user-detail-avatar-upload", function (e) {
        ajaxUploadImage(e);
    });
    $(document).on('click', '.user-row', function (e) {
        var row = e.target;

        if (!row.classList.contains("user-row")) {
            row = $(row).closest(".user-row")[0];
        }
        displayUserDetails(row.dataset.userId);
    });

    $(document).on('click', '.edit-container svg', function (e) {
        var ele = $(e.target).closest(".user-detail-container").get(0);
        var inputType = ele.dataset.editType;

        var previousValue = ele.getElementsByClassName('user-detail-value')[0].innerHTML;
        ele.getElementsByClassName('user-detail-value')[0].setAttribute('style', 'display:none');
        ele.getElementsByClassName('user-detail-input')[0].setAttribute('style', 'display:inline-block');

        if (inputType === "date") {
            ele.getElementsByClassName('user-detail-input')[0].value = formatDate(previousValue);
        }
        else {
            ele.getElementsByClassName('user-detail-input')[0].value = previousValue;
        }

        ele.getElementsByClassName('user-detail-input')[0].focus();


        ele.getElementsByClassName('user-detail-value')[0].setAttribute('contenteditable', 'true');

    });
    $(document).on('blur', '#UserDetailEmailAddress', function (e) {
        var blurredEle = document.getElementById('UserDetailEmailAddress');
        blurredEle.removeAttribute('contenteditable');
        var userId = document.getElementById('UserDetailUserId').innerHTML;
        var attributeName = blurredEle.dataset.propertyName;
        var attributeValue = blurredEle.value;

        var ajaxRequestUpdateEmail = ajaxUpdateEmail(userId, attributeValue);
        var ajaxRequestUpdateDisplayName = ajaxUpdateUserName(userId, attributeValue);

        //ajaxRequestUpdateEmail.success(function (e) {
        updateUserModel(userId, attributeName, attributeValue);
        //})

        var parent = $(e.target).closest(".user-detail-container").get(0);
        parent.getElementsByClassName('user-detail-input')[0].setAttribute('style', 'display:none');
        parent.getElementsByClassName('user-detail-value')[0].removeAttribute('style');
        parent.getElementsByClassName('user-detail-value')[0].innerHTML = attributeValue;

    });
    $(document).on('blur', '#UserDetailHireDate', function (e) {
        var blurredEle = document.getElementById('UserDetailHireDate');
        var userId = document.getElementById('UserDetailUserId').innerHTML;
        var attributeName = blurredEle.dataset.propertyName;
        var attributeValue = document.getElementById('UserDetailHireDate').value;

        var ajaxRequestUpdateHireDate = ajaxUpdateHireDate(userId, attributeValue);

        //ajaxRequestUpdateHireDate.success(function (e) {
            updateUserModel(userId, attributeName, attributeValue);
        //})

        var parent = $(e.target).closest(".user-detail-container").get(0);
        parent.getElementsByClassName('user-detail-input')[0].setAttribute('style', 'display:none');
        parent.getElementsByClassName('user-detail-value')[0].removeAttribute('style');
        parent.getElementsByClassName('user-detail-value')[0].innerHTML = attributeValue;
    });
    $(document).on('blur', '#UserDetailJob', function (e) {
        var blurredEle = document.getElementById('UserDetailJob');
        var userId = document.getElementById('UserDetailUserId').innerHTML;
        var attributeName = blurredEle.dataset.propertyName;
        var selectedIndex = document.getElementById('UserDetailJob').selectedIndex;
        var attributeInnerHtml = document.getElementById('UserDetailJob').options[selectedIndex].value;
        var attributeValue = document.getElementById('UserDetailJob').options[selectedIndex].dataset.jobId


        var ajaxRequestUpdateHireDate = ajaxUpdateJob(userId, attributeValue);

        //ajaxRequestUpdateHireDate.success(function (e) {
        updateUserModel(userId, attributeName, attributeValue);
        //})

        var parent = $(e.target).closest(".user-detail-container").get(0);
        parent.getElementsByClassName('user-detail-input')[0].setAttribute('style', 'display:none');
        parent.getElementsByClassName('user-detail-value')[0].removeAttribute('style');
        parent.getElementsByClassName('user-detail-value')[0].innerHTML = attributeInnerHtml;
    });
    $(document).on('blur', '#UserDetailShift', function (e) {
        var blurredEle = document.getElementById('UserDetailShift');
        var userId = document.getElementById('UserDetailUserId').innerHTML;
        var attributeName = blurredEle.dataset.propertyName;
        var selectedIndex = document.getElementById('UserDetailShift').selectedIndex;
        var attributeInnerHtml = document.getElementById('UserDetailShift').options[selectedIndex].value;
        var attributeValue = document.getElementById('UserDetailShift').options[selectedIndex].dataset.shiftId;

        ajaxUpdateShift(userId, attributeValue);

        //ajaxRequestUpdateHireDate.success(function (e) {
        updateUserModel(userId, attributeName, attributeValue);
        //})

        var parent = $(e.target).closest(".user-detail-container").get(0);
        parent.getElementsByClassName('user-detail-input')[0].setAttribute('style', 'display:none');
        parent.getElementsByClassName('user-detail-value')[0].removeAttribute('style');
        parent.getElementsByClassName('user-detail-value')[0].innerHTML = attributeValue;
        parent.getElementsByClassName('user-detail-value')[0].innerHTML = attributeInnerHtml;
    });
});
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
function displayUserDetails(userId) {
    clearUserDetailValues();
    var selectedUserModel = getUserModel(userId);

    if (selectedUserModel.ProfilePicturePath !== "") {
        var profilePicture = document.createElement('img');
        profilePicture.setAttribute('src', selectedUserModel.ProfilePicturePath);
        document.getElementsByClassName('user-detail-avatar')[0].appendChild(profilePicture);
    }

    document.getElementsByClassName('user-detail-user-id')[0].getElementsByClassName('user-detail-value')[0].innerHTML = selectedUserModel.ID;
    document.getElementsByClassName('user-detail-user-name')[0].getElementsByClassName('user-detail-value')[0].innerHTML = selectedUserModel.Employee.FirstName + " " + selectedUserModel.Employee.LastName;
    document.getElementsByClassName('user-detail-user-id')[0].getElementsByClassName('user-detail-value')[0].innerHTML = selectedUserModel.ID;
    document.getElementsByClassName('user-detail-user-login')[0].getElementsByClassName('user-detail-value')[0].innerHTML = selectedUserModel.EmailAddress;
    document.getElementsByClassName('user-detail-user-password')[0].getElementsByClassName('user-detail-value')[0].innerHTML = "********";
    document.getElementsByClassName('user-detail-user-hire-date')[0].getElementsByClassName('user-detail-value')[0].innerHTML = formatDate(convertEpochDate(parseInt(cleanModelDate(selectedUserModel.Employee.HireDate))));
    document.getElementsByClassName('user-detail-user-job-type')[0].getElementsByClassName('user-detail-value')[0].innerHTML = selectedUserModel.Employee.Job.JobDescription;
    document.getElementsByClassName('user-detail-user-shift')[0].getElementsByClassName('user-detail-value')[0].innerHTML = selectedUserModel.Employee.Shift.ShiftDescription;

    for (let i = 0; i < selectedUserModel.UserRoles.length; i++) {
        var userRoleContainer = document.createElement('div');
        userRoleContainer.setAttribute('class', 'user-detail-user-role');

        var userRole = document.createElement('p');
        userRole.setAttribute('class', 'user-detail-value');
        userRole.innerHTML = selectedUserModel.UserRoles[i].RoleName;
        userRoleContainer.appendChild(userRole);

        document.getElementsByClassName('user-detail-user-roles')[0].appendChild(userRoleContainer);
    }
}
function clearUserDetailValues() {
    var userDetails = document.getElementsByClassName("user-detail-value");

    $(document.getElementsByClassName('user-detail-avatar')[0].getElementsByTagName('img')[0]).remove();

    for (let i = 0; i < userDetails.length; i++) {
        userDetails[i].innerHTML = "";
    }
    $(".user-detail-user-role").remove()

}
function getUserModel(userId) {
    for (let i = 0; i < serverUserModels.length; i++) {
        if (serverUserModels[i].ID === userId) {
            return serverUserModels[i]
        }
    }
}
function convertEpochDate(epochDate) {
    var date = new Date(epochDate);
    return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear()
}
function cleanModelDate(dateString) {
    return dateString.replace(/\//g, '').replace('Date', '').replace(')', '').replace('(', '')
}
function updateUserModel(userId, attributeName, attributeValue) {
    let userModel = getUserModel(userId);
    setObjectKeyValue(userModel, attributeName, attributeValue);
}
function setObjectKeyValue(obj, key, value) {

    for (let i = 0; i < Object.keys(obj).length; i++) {
        var objKey = Object.entries(obj)[i][0];
        var objValue = obj[objKey];
        if (objValue != null && obj.hasOwnProperty(objKey) && typeof obj[objKey] == "object") {
            setObjectKeyValue(obj[objKey], key, value);
        }
        else if (objKey === key) {
            obj[key] = value;
        }
    }
}
function ajaxUpdateEmail(userId, email) {
    let data = JSON.stringify({ 'userId': userId, 'email': email });
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/User/UpdateEmail",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                alert("Finished Success");
            }
            else {
                alert("Finished Failure");
            }
        },
        complete: function () {
            $('html').css('cursor', 'default');
        }
    });
}
function ajaxUpdateUserName(userId, username) {
    let data = JSON.stringify({ 'userId': userId, 'username': username });
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/User/UpdateUserName",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                alert("Finished Success");
            }
            else {
                alert("Finished Failure");
            }
        },
        complete: function () {
            $('html').css('cursor', 'default');
        }
    });
}
function ajaxUpdateHireDate(userId, hireDate) {
    let data = JSON.stringify({ 'userId': userId, 'hireDate': hireDate });
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/User/UpdateHireDate",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                alert("Finished Success");
            }
            else {
                alert("Finished Failure");
            }
        },
        complete: function () {
            $('html').css('cursor', 'default');
        }
    });
}
function ajaxUpdateJob(userId, jobId) {
    let data = JSON.stringify({ 'userId': userId, 'jobId': jobId });
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/User/UpdateJob",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                alert("Finished Success");
            }
            else {
                alert("Finished Failure");
            }
        },
        complete: function () {
            $('html').css('cursor', 'default');
        }
    });
}
function ajaxUpdateShift(userId, shiftId) {
    let data = JSON.stringify({ 'userId': userId, 'shiftId': shiftId });
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/User/UpdateShift",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                alert("Finished Success");
            }
            else {
                alert("Finished Failure");
            }
        },
        complete: function () {
            $('html').css('cursor', 'default');
        }
    });
}
function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('-');
}
