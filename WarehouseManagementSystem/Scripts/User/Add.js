$(document).ready(function (e) {
    document.getElementById('ButtonAddUser').addEventListener("click", createUser);
});

function toggleCheckBox(checkbox) {
    if (checkbox.classList.contains("checked")) {
        checkbox.classList.remove("checked");
        checkbox.classList.add("unchecked");
    }
    else {
        checkbox.classList.remove("unchecked");
        checkbox.classList.add("checked");
    }
}
function createUser() {
    ajaxCreateUser(getUserModel(), getEmployeeModel());
}
function ajaxCreateUser(user, employee) {
    let data = JSON.stringify({ 'user': user, 'employee': employee });
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/User/Create",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.Success) {
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

function getEmployeeModel() {
    var employee = {
        'Id': guid(),
        'FirstName': document.getElementById('CreateUserFirstName').value,
        'LastName': document.getElementById('CreateUserLastName').value,
        'HireDate': $('#CreateUserHireDate').val(),
        'Job': getJobModel(),
        'Shift': getShiftModel()
    };
    return employee;
} 
function getJobModel() {
    var job = {
        'Id': document.getElementById('CreateUserJob').options[document.getElementById('CreateUserJob').selectedIndex].dataset.jobId,
        'JobDescription': document.getElementById('CreateUserJob').options[document.getElementById('CreateUserJob').selectedIndex].innerHTML
    }
    return job;
}
function getShiftModel() {
    var shift = {
        'Id': document.getElementById('CreateUserShift').options[document.getElementById('CreateUserShift').selectedIndex].dataset.shiftId,
        'ShiftDescription': document.getElementById('CreateUserShift').options[document.getElementById('CreateUserShift').selectedIndex].innerHTML
    }
    return shift;
}
function getUserModel() {
    var user = {
        'EmailAddress': document.getElementById('CreateUserEmailAddress').value,
        'Password': document.getElementById('CreateUserPassword').value,
        'UserRoles': getSelectedRoles()
    };
    return user;
} 
function getSelectedRoles() {
    let potentialRoles = document.getElementsByClassName('UserRoleTable')[0].getElementsByClassName('user-role');
    let arrayRoles = [];

    for (let i = 0; i < potentialRoles.length; i++) {
        var id = potentialRoles[i].dataset.roleId;
        var name = potentialRoles[i].getElementsByTagName('p')[0].innerHTML;

        var potentialRoleClassList = potentialRoles[i].getElementsByClassName('box')[0].classList;

        if (potentialRoleClassList.contains('checked')) {
            var role = {
                'RoleID': id,
                'RoleName': name
            };
            arrayRoles.push(role);
        }
    }
    return arrayRoles;
}
function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}