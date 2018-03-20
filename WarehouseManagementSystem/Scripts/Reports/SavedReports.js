-$(document).ready(function (e) {

    $(document).on('click', '.user-row .btn-run-report svg ', function (e) {
            var ele = $(e.target).closest('.user-row')[0]
            runReport(ele.dataset.reportId);
    });
    $(document).on('click', '.user-row .btn-delete-report svg', function (e) {
        deleteReport(e);
    });
    $(document).on('click', '.btn-edit-report', function (e) {
        ajaxEditReport(e);
    });
});

function runReport(reportId) {
    ajaxRunReport(reportId);
}

function deleteReport(e) {
    var clickedEle = e.target;
    var userRow = $(clickedEle).closest(".user-row");
    var reportID = userRow[0].dataset.reportId;
    //Need to add if success statement
    ajaxDeleteReport(reportID)
    $(userRow).remove();
}

function ajaxDeleteReport(reportID) {
    let data = JSON.stringify({ 'reportID': reportID });
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/Reports/DeleteReport",
        data: data,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
                alert('success');
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

function ajaxEditReport() {
    $.ajax({
        type: "POST",
        url: "/Reports/EditReport",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.success) {
            }
        },
        complete: function () {
            $('html').css('cursor', 'default');
        }
    });
} 