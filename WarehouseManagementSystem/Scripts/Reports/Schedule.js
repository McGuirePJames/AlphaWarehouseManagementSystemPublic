$(document).ready(function (e) {
    $(document).on('click', '#FrequencyDropdown', function (e) {
        toggleFrequencyDropdown();
    });
    $(document).on('mouseleave', '#FrequencyDropdownItems', function (e) {
        toggleFrequencyDropdown();
    });
    $(document).on('click', '#ContainerFrequencyDropdown .dropdown-item-custom', function (e) {
        var clickedEle = e.target;
        if (!clickedEle.classList.contains('dropdown-item-custom')) {
            clickedEle = $(clickedEle).closest('.dropdown-item-custom')[0];
        }
        var reportName = clickedEle.getElementsByTagName('p')[0].innerHTML;
        var reportId = clickedEle.dataset.reportId;
        setDropdownValue(reportId, reportName);
        document.getElementById('FrequencyDropdown').classList.add('picked');
        document.getElementById('FrequencyDropdown').getElementsByTagName('p')[0].classList.remove('placeholder');
    });
    $(document).on('click', '#ContainerFrequencyDropdown .frequency-select', function (e) {
        var clickedEle = e.target;

        if (!clickedEle.classList.contains('frequency-select')) {
            clickedEle = $(clickedEle).closest('.frequency-select')[0];
        }        
        resetFrequencySelectors();
        clickedEle.classList.add('active');
    });
    $(document).on('click', '#UserNotificationDropdown', function (e) {
        toggleUserNotificationDropdown();
    });
    $(document).on('mouseleave', '#UserNotificationDropdownItems', function (e) {
        toggleUserNotificationDropdown();
    });
    $(document).on('click', '#ContainerUserNotificationDropdown .dropdown-item-custom', function (e) {
        var clickedEle = e.target;
        if (!clickedEle.classList.contains('dropdown-item-custom')) {
            clickedEle = $(clickedEle).closest('.dropdown-item-custom')[0];
        }
        document.getElementById('UserNotificationDropdown').classList.add('picked');
    });
    $(document).on('dblclick', '.dropdown-custom-multi-item', function (e) {
        var clickedEle = e.target;
        if (!clickedEle.classList.contains('dropdown-custom-multi-item')) {
            clickedEle = $(clickedEle).closest('.dropdown-custom-multi-item')[0];
        }
        $(clickedEle).remove();
    });
    
    $(document).on('click', '.frequency-select', function (e) {
        var clickedEle = e.target;

        if (!clickedEle.classList.contains('frequency-select')) {
            clickedEle = $(clickedEle).closest('.frequency-select')[0];
        }
        resetFrequencySelectors();
        clickedEle.classList.add('active');
    });
    $(document).on('click', '#FrequencyRunImmediately', function (e) {
        var clickedEle = e.target;

        if (clickedEle.checked == true) {
            document.getElementById('FrequencySelectDateTime').disabled = true;
            document.getElementById('FrequencySelectDateTime').value = "";
        }
        else {
            document.getElementById('FrequencySelectDateTime').disabled = false;
        }
    });
    $(document).on('click', '#CreateScheduledReport', function (e) {
        var schedule = createSchedule();
        if (schedule.StartImmediately) {
            ajaxCreateScheduledReport(schedule);
        }
        else {
            ajaxCreateDelayedJob(schedule);
        }
    });

    flatpickr("#FrequencySelectDateTime", {
        enableTime: true,
        dateFormat: "Y-m-d H:i",
    });
});





class Report {
    constructor(reportId, reportName) {
        this.ReportId = reportId;
        this.ReportName = reportName;
    }
}

class Schedule{
    constructor(scheduleId, reportId, hangfireJobId, name, frequency, startImmediately, startDate, usersToNotify, report) {
        this.ScheduleId = scheduleId;
        this.ReportId = reportId;
        this.HangfireJobId = hangfireJobId;
        this.Name = name;
        this._Frequency = frequency;
        this.StartImmediately = startImmediately;
        this.StartDate = startDate;
        this.UsersToNotify = usersToNotify;
        this.Report = report;
    }
}
class UsersToNotify {
    constructor(userId, emailAddress) {
        this.EmailAddress = emailAddress;
        this.UserId = userId;
    }
}
class User {
    constructor(id, email) {
        this._id = id;
        this._email = email;
    }
}


function ajaxCreateDelayedJob(schedule) {
    let data = JSON.stringify(schedule);
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/Reports/CreateDelayedJob",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                ScheduledReportViewer.addReport(response.responseText);
                alert("Finished Success");
            }
            else {
                alert(response.responseText);
            }
        },
        complete: function () {
            $('html').css('cursor', 'default');
        }
    });
}
function ajaxCreateScheduledReport(schedule) {
    let data = JSON.stringify(schedule);
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/Reports/CreateScheduledReport",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                ScheduledReportViewer.addReport(response.responseText);
                alert("Finished Success");
            }
            else {
                alert(response.responseText);
            }
        },
        complete: function () {
            $('html').css('cursor', 'default');
        }
    });
}


function createSchedule(scheduleId) {
    var reportId = document.getElementById('FrequencyDropdown').dataset.reportId;
    var hangfireJobId = guid();
    var reportName = document.getElementById('ScheduleCreationScheduleName').value;
    var frequency = document.getElementsByClassName('frequency-select active')[0].getElementsByTagName('p')[0].innerHTML;    

    var schedule = new Schedule();
    schedule.ReportId = reportId;
    schedule.HangfireJobId = hangfireJobId;
    schedule.Name = reportName;
    schedule._Frequency = frequency;
    schedule.UsersToNotify = getUsersToNotify();
    schedule.ScheduleId = scheduleId;

    var report = new Report();
    report.ReportId = document.getElementById('FrequencyDropdown').dataset.reportId;
    report.ReportName = document.getElementById('FrequencyDropdown').getElementsByTagName('p')[0].innerHTML;

    schedule.Report = report;


    if (document.getElementById('FrequencySelectDateTime').value == "") {
        schedule.StartImmediately = true;
    }
    else {
        schedule.StartImmediately = false;
        schedule.StartDate = document.getElementById('FrequencySelectDateTime').value;
    }
    return schedule;
}
function getUsersToNotify() {
    var selectedUsers = UsersToNotifyDropdown.getSelectedItems();
    var usersToReturn = [];
    for (let i = 0; i < selectedUsers.length; i++) {
        usersToReturn.push({ "UserId": selectedUsers[i].Id, "EmailAddress": selectedUsers[i].EmailAddress });
    }
    return usersToReturn;
}

function toggleUserNotificationDropdown() {
    var dropdown = document.getElementById('UserNotificationDropdown');

    if (dropdown.classList.contains('active')) {
        dropdown.classList.remove('active');
    }
    else {
        dropdown.classList.add('active');
    }
}



function toggleFrequencyDropdown() {
    var dropdown = document.getElementById('FrequencyDropdown');

    if (dropdown.classList.contains('active')){
        dropdown.classList.remove('active');
    }
    else {
        dropdown.classList.add('active');
    }
}
function setDropdownValue(reportId, reportName) {
    document.getElementById('FrequencyDropdown').getElementsByTagName('p')[0].innerHTML = reportName;
    document.getElementById('FrequencyDropdown').setAttribute('data-report-Id', reportId);
}
function resetFrequencySelectors() {
    var selectors = document.getElementsByClassName('frequency-select');

    for (let i = 0; i < selectors.length; i++) {
        selectors[i].classList.remove('active');
    }
}




function addUser(user) {
    if (isFirstUser()) {
        removePlaceHolder();
    }
    var parent = document.getElementById('UserNotificationDropdown');
    var container = document.createElement('div');
    container.setAttribute('class', 'dropdown-custom-multi-item');
    container.setAttribute('data-user-Id', user._id);
    container.setAttribute('data-user-email', user._email);

    var email = document.createElement('p');
    email.innerHTML = user._email;
    container.appendChild(email);

    document.getElementById('UserNotificationUsers').appendChild(container);
    //$(container).insertAfter("#UserNotificationIconDropdownDownAngle");


}
function isFirstUser() {
    if (document.getElementById('UserNotificationDropdown').getElementsByClassName('placeholder')[0]) {
        return true;
    }
    return false;
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