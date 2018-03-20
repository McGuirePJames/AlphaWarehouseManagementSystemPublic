window.onload = function () {
    $(document).on("click", '.level-one svg', function (e) {
        e.stopPropagation();
    });
    $(document).on("click", '.input-role-description', function (e) {
        e.stopPropagation();
    });

};
$(document).ready(function () {
    $(".action-container .custom-dropdown").on('click', function (e) {
        ToggleDropDown(e);
    });
    $(".action-container .custom-dropdown .row p").on('click', function (e) {
        ToggleDropDownRowStatus(e);
    });
    $(document).on('click', '#RoleTable .slider', function (e) {
        ToggleSlider(e);
    });
    $('#ButtonAddRole').on('click', function (e) {
        InsertNewRole(guid());
    });
    $(document).on('click', '#RoleTable .row-action-container svg:nth-child(1)', function (e) {
        let clickedEle = e.target;
        let row = $(clickedEle).closest('.level-one')[0];
        let id = row.attributes.href.value.replace('#', '');
        if (row.className.indexOf("new-row") >= 0) {
            createRoleAjax(getRow(id));
            SaveRow(e);
        }
        else {
            let arrayModifiedSliders = getModifiedSliders(document.getElementById(id));
            updateRoleAjax(arrayModifiedSliders);
        }
    });
    $(document).on('click', '#RoleTable .row-action-container svg:nth-child(2)', function (e) {
        let clickedEle = e.target;
        let row = $(clickedEle).closest('.level-one')[0];
        let id = row.attributes.href.value.replace('#', '');
        document.getElementById('ConfirmationPopUp').getElementsByClassName('yes')[0].setAttribute("onclick", "DeleteRow(" + "'" + id + "'" + ")");
        showConfirmation();

    });
    $(document).on('click', '#ConfirmationPopUp .hide-icon svg', function (e) {
        document.getElementById('ConfirmationPopUp').setAttribute('style', 'display:none');
        hideConfirmation();
        ResetConfirmationPopUp();
    });
    $(document).on('click', '#ConfirmationPopUp .yes', function (e) {
        hideConfirmation();
        ResetConfirmationPopUp();
    });
    $(document).on('click', '#ConfirmationPopUp .no', function (e) {
        hideConfirmation();
        ResetConfirmationPopUp();
    });
});

function ToggleDropDown(e) {
    let ele = e.target;
    if (ele.className.indexOf("closed") >= 0) {
        ele.className = ele.className.replace("closed", "opened");
    }
    else {
        ele.className = ele.className.replace("opened", "closed");
    }

}
function ToggleDropDownRowStatus(e) {
    let ele = e.target;
    if (ele.className.indexOf("active") >= 0) {
        ele.className = ele.className.replace("active", "dormant");
    }
    else {
        ele.className = ele.className.replace("dormant", "active");
    }
}
function ToggleSlider(e) {
    let ele = e.target.parentNode;
    if (ele.className.indexOf("off") >= 0) {
        ele.className = ele.className.replace("off", "on modified");
    }
    else {
        ele.className = ele.className.replace("on", "off modified");
    }
}

function InsertNewRole(id) {
    this._id = id;

    var firstLevel = new FirstLevel();
    firstLevel._id = id;
    firstLevel._description = "Created Row";

    firstLevel.create();
    firstLevel.insertRow();

    var secondLevel = new SecondLevel();
    secondLevel._id = id;

    secondLevel.create();
    secondLevel.insertRow();
}
function FirstLevel() {
    this._firstLevel = "";
    this._id = "";
    this._description = "";
}
FirstLevel.prototype = {
    create: function () {
        let firstLevel = document.createElement('div');
        firstLevel.setAttribute('href', "#" + this._id);
        firstLevel.setAttribute('class', 'cell col-sm-12 row level-one new-row');
        firstLevel.setAttribute('data-toggle', 'collapse');
        firstLevel.setAttribute('aria-expanded', 'true');

        let containerInputRowDescription = document.createElement('div');
        containerInputRowDescription.setAttribute('class', 'col-sm-12 col-md-6 padding-left-none container-input-role-description');

        let inputRowDescription = document.createElement('input');
        inputRowDescription.setAttribute('type', 'text');
        inputRowDescription.setAttribute('class', 'input-role-description');
        inputRowDescription.setAttribute('maxlength', '20');

        //let rowDescription = document.createElement('p');
        //rowDescription.setAttribute('class', 'col-sm-12 col-md-6 padding-left-none');
        //rowDescription.innerHTML = this._description;

        let caretRight = document.createElement('i');
        caretRight.setAttribute('class', 'fa fa-caret-right');

        containerInputRowDescription.appendChild(inputRowDescription);
        containerInputRowDescription.appendChild(caretRight);

        //rowDescription.appendChild(caretRight);
        firstLevel.appendChild(containerInputRowDescription);

        let actionContainer = document.createElement('div');
        actionContainer.setAttribute('class', 'col-sm-12 col-md-6 row row-action-container');
        let iconCheck = document.createElement('i');
        iconCheck.setAttribute('class', 'fas fa-check');
        let iconTrash = document.createElement('i');
        iconTrash.setAttribute('class', 'far fa-trash-alt');

        actionContainer.appendChild(iconCheck);
        actionContainer.appendChild(iconTrash);

        firstLevel.appendChild(actionContainer);
        this._firstLevel = firstLevel;
    },
    insertRow: function () {
        document.getElementById("RoleTable").appendChild(this._firstLevel);

    }
};
function SecondLevel() {
    this._SecondLevel = "";
    this._id = "";

}
SecondLevel.prototype = {
    create: function () {
        let containerSecondLevel = document.createElement('div');
        containerSecondLevel.setAttribute('id', this._id);
        containerSecondLevel.setAttribute('class', 'row navigation-dropdown collapse show');

        for (let i = 0; i < 7; i++) {
            var sliderRow = new SliderRows();
            if (i === 0) {
                sliderRow._description = "Data Entry:Products";
                containerSecondLevel.appendChild(sliderRow.create());
            }
            else if (i === 1) {
                sliderRow._description = "Reports:Create";
                containerSecondLevel.appendChild(sliderRow.create());
            }
            else if (i === 2) {
                sliderRow._description = "Reports:Manage";
                containerSecondLevel.appendChild(sliderRow.create());
            }
            else if (i === 3) {
                sliderRow._description = "Reports:Schedule";
                containerSecondLevel.appendChild(sliderRow.create());
            }
            else if (i === 4) {
                sliderRow._description = "Roles:Manage";
                containerSecondLevel.appendChild(sliderRow.create());
            }
            else if (i === 5) {
                sliderRow._description = "Users:Add";
                containerSecondLevel.appendChild(sliderRow.create());
            }
            else if (i === 6) {
                sliderRow._description = "Users:Manage";
                containerSecondLevel.appendChild(sliderRow.create());
            }
        }
        this._SecondLevel = containerSecondLevel;
    },
    insertRow: function () {
        document.getElementById("RoleTable").appendChild(this._SecondLevel);
    }
};


function SliderRows(description) {
    this._description = description;
    this._sliders = "";

    var sliders = new Sliders();
    this._sliders = sliders.createArray();

}
SliderRows.prototype = {
    create: function () {
        let row = document.createElement('div');
        row.setAttribute('class', 'col-12 col-sm-12 level-two cell description row');
        row.setAttribute('data-id', guid());
        let rowDescription = document.createElement('p');
        rowDescription.setAttribute('class', 'col-sm-4');
        rowDescription.innerHTML = this._description;
        row.appendChild(rowDescription);

        for (let i = 0; i < this._sliders.length; i++) {
            row.appendChild(this._sliders[i]);
        }

        return row;
    }
};
function Sliders() {

}
Sliders.prototype = {
    create: function () {
        let slideContainer = document.createElement('div');
        slideContainer.setAttribute('class', 'col-12 col-sm-12 col-md-12 col-lg-2 container-slide');
        let slide = document.createElement('div');
        slide.setAttribute('class', 'slide off');
        slide.setAttribute('data-id', guid());
        let slider = document.createElement('div');
        slider.setAttribute('class', 'slider');

        slide.appendChild(slider);
        slideContainer.appendChild(slide);

        return slideContainer;
    },
    createArray: function () {
        let slidersArray = [];
        for (let i = 0; i < 4; i++) {
            sliders = new Sliders();
            slidersArray.push(sliders.create());
        }
        return slidersArray;
    }
};
Sliders.prototype.__proto__ = SliderRows.prototype;

function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}

function DeleteRow(id) {
    deleteRoleAjax(getRow(id));
    $(document.getElementById(id)).remove();
    $(document.querySelectorAll("div[href=" + CSS.escape('#') + id + "]")).remove();
}
function SaveRow(e) {
    let clickedEle = e.target;
    let firstLevel = $(clickedEle).closest('.level-one')[0];

    if (IsNewRow(firstLevel)) {
        let descriptionInputValue = firstLevel.getElementsByClassName('input-role-description')[0].value;
        $(firstLevel.getElementsByClassName('input-role-description')).remove();
        let descriptionP = document.createElement('p');
        descriptionP.setAttribute('class', 'padding-left-none container-description');
        descriptionP.innerHTML = descriptionInputValue;

        $(firstLevel.getElementsByClassName('container-input-role-description')[0]).prepend(descriptionP);
        firstLevel.classList.remove("new-row");
        firstLevel.getElementsByClassName('container-input-role-description')[0].setAttribute('class', 'col-sm-12 col-md-6 container-description');

    }
}
function IsNewRow(firstLevel) {
    if (firstLevel.className.indexOf("new-row") >= 0) {
        return true;
    }
    return false;
}

function ResetConfirmationPopUp() {
    document.getElementById('ConfirmationPopUp').getElementsByClassName('yes')[0].removeAttribute('onclick');

}
function showConfirmation() {
    document.getElementById('ConfirmationPopUp').setAttribute('style', 'display:inline-block');
    $('main').css('filter', 'blur(3px');
    $('nav').css('filter', 'blur(3px');
    $('footer').css('filter', 'blur(3px');


}
function hideConfirmation() {
    document.getElementById('ConfirmationPopUp').setAttribute('style', 'display:none');
    $('main').css('filter', 'blur(0px');
    $('nav').css('filter', 'blur(0px');
    $('footer').css('filter', 'blur(0px');
}
function createRoleAjax(role) {
    let data = JSON.stringify(role);
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/Roles/Create",
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
function deleteRoleAjax(role) {
    let data = JSON.stringify(role);
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/Roles/Delete",
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

function getModifiedSliders(row) {
    let arraymodifiedSliders = [];
    let modifiedSliders = row.getElementsByClassName('modified');

    for (let i = 0; i < modifiedSliders.length; i++) {
        var permission = {
            'Id': modifiedSliders[i].dataset.id,
            'Description': "",
            'Allowed': ""
        }
        if (modifiedSliders[i].className.indexOf("on") >= 0) {
            permission.Allowed = 1;
        }
        else {
            permission.Allowed = 0;
        }
        arraymodifiedSliders.push(permission);
    }
    return arraymodifiedSliders;
}
function updateRoleAjax(modifiedSliders) {
    let data = JSON.stringify(modifiedSliders);
    $.ajax({
        beforeSend: function () {
            $('html').css('cursor', 'wait');
        },
        type: "POST",
        url: "/Roles/Update",
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


function getRow(roleId) {

    let row = document.getElementById(roleId);

    var description = ""
    //if new row was created
    if (row.previousElementSibling.getElementsByClassName('container-input-role-description')[0]) {
        description = row.previousElementSibling.getElementsByClassName('container-input-role-description')[0].getElementsByTagName('input')[0].value
    }
    else {
        description = row.previousElementSibling.getElementsByClassName('container-description')[0].getElementsByTagName('p')[0].innerHTML
    }

    var role = {
        'id': roleId,
        'name': description,
        'PermissionGroups': getPermissionGroups(roleId)
    }
    return role;

}
function getPermissionGroups(roleId) {
    let arrayPermissionGroups = [];
    let permissionGroups = document.getElementById(roleId).getElementsByClassName('level-two');

    for (let i = 0; i < permissionGroups.length; i++) {
        var permissionGroup = {
            'Id': permissionGroups[i].dataset.id,
            'RoleId': roleId,
            'Description': permissionGroups[i].getElementsByTagName('p')[0].innerHTML,
            'PermissionGroupsDetails': getPermissionGroupDetails(permissionGroups[i].dataset.id)
        }
        arrayPermissionGroups.push(permissionGroup);
    }
    return arrayPermissionGroups;
}
function getPermissionGroupDetails(permissionGroupId) {
    let permissionsArray = []
    let permissions = document.querySelectorAll('#RoleTable .navigation-dropdown [data-id="' + permissionGroupId + '"]')[0].getElementsByClassName('container-slide');

    for (let i = 0; i < permissions.length; i++) {

        var elePermission = permissions[i].getElementsByClassName('slide')[0]

        var permission = {
            'Id': elePermission.dataset.id,
            'Description': "",
            'Allowed': 0,
            'PermissionGroupsId': permissionGroupId
        }
        if (elePermission.className.indexOf('on') >= 0) {
            permission.Allowed = 1;
        }
        //psuedo element is on md and below breakpoint.  As a result it isn't "selectable" on anything higher
        if (window.innerWidth >= 992) {
            if (i === 0) {
                permission.Description = "Create";
            }
            else if (i === 1) {
                permission.Description = "Read";
            }
            else if (i === 2) {
                permission.Description = "Update";
            }
            else if (i === 3) {
                permission.Description = "Delete";
            }
        }
        else {
            permission.Description = window.getComputedStyle(document.getElementsByClassName('container-slide')[i].querySelector('.slide'), ':before').getPropertyValue('content')
        }
        permissionsArray.push(permission);
    }
    return permissionsArray;
}
