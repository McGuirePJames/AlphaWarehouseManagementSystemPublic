class Actions {
    constructor(userAction, roleAction, userRoleAction, employeeAction) {
        this.UserAction = userAction;
        this.RoleAction = roleAction;
        this.UserRoleAction = userRoleAction;
        this.EmployeeAction = employeeAction;
    }
}
class UserAction {
    getUsers() {
        return function (dispatch) {
            $.ajax({
                url: "/User/GetUsers",
                success: function (response) {
                    dispatch({ type: "GET_USERS", payload: response })
                }
            })
        }
    }
    setCurrentUser(user) {
        return { type: "SET_CURRENT_USER", payload: user }
    }
    getCurrentUser() {
        return { type: "GET_CURRENT_USER" }
    }
    updateUserInAvailableUsers(user) {
        return { type: "UPDATE_USER_IN_AVAILABLE_USERS", payload: user }
    }
    addUserRoleToAvailableUser(userRole) {
        return { type: "ADD_USER_ROLE_TO_AVAILABLE_USER", payload: userRole }
    }
    removeEpochDateFromHireDate(employees) {
        var cleanedEmployees = [];
        for (let i = 0; i < employees.length; i++) {
            var employee = employees[i];
            var epochHireDate = employees[i].HireDate;
            var cleanedDate = formatDate(convertEpochDate(cleanModelDate(epochHireDate) * 1), "-", true);
            employee.HireDate = cleanedDate;
            cleanedEmployees.push(employee);
        }
        return cleanedEmployees;
    }
    removeEpochDatesFromUser(users) {
        var cleanedUsers = [];
        for (let i = 0; i < users.length; i++) {
            var user = users[i];
            var epochHireDate = user.Employee.HireDate;
            var cleanedDate = formatDate(convertEpochDate(cleanModelDate(epochHireDate) * 1), "-", true);
            user.Employee.HireDate = cleanedDate;
            cleanedUsers.push(user);
        }
        return cleanedUsers;
    }
    updateHireDate(userId, hireDate) {
        return function (dispatch) {
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
                        dispatch(actions.UserAction.updateHireDateDispatch(userId, hireDate));
                    }
                },
                complete: function () {
                    $('html').css('cursor', 'default');
                }
            });
        }
    }
    updateHireDateDispatch(userId, hireDate) {
        var response = {
            'userId': userId,
            'hireDate': hireDate
        }
        return { type: "UPDATE_HIRE_DATE", payload: response }
    }
    initializeUserData() {
        return function (dispatch) {
            $.ajax({
                beforeSend: function () {
                    $('html').css('cursor', 'wait');
                },

                url: "/User/GetUserManagementData",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var cleanedEmployees = actions.UserAction.removeEpochDateFromHireDate(response.employees);
                    var cleanedUsers = actions.UserAction.removeEpochDatesFromUser(response.users);
                    dispatch(actions.UserAction.setUsers(cleanedUsers));
                    dispatch(actions.UserRoleAction.setUserRoles(response.userRoles));
                    dispatch(actions.EmployeeAction.setEmployees(cleanedEmployees));
   
                },
                complete: function () {
                    $('html').css('cursor', 'default');
                }
            });
        }
    }
    setUsers(users) {
        return { type: "SET_USERS", payload: users }
    }
    toggleUpdateEmailAddressInputVisibility() {
        return { type: "TOGGLE_UPDATE_EMAIL_ADDRESS_INPUT_VISIBILITY" }
    }
    updateEmailAddress(userId, emailAddress) {
        var data = JSON.stringify({ 'userId': userId, 'email': emailAddress });
        return function (dispatch) {
            $.ajax({
                beforeSend: function () {
                    $('html').css('cursor', 'wait');
                },
                data: data,
                type: "POST",
                url: "/User/UpdateEmailAndUserName",
                data: data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.success) {
                        dispatch(actions.UserAction.updateEmailAddressDispatch(userId, emailAddress));
                        dispatch(actions.UserAction.toggleUpdateEmailAddressInputVisibility());
                    }
                },
                complete: function () {
                    $('html').css('cursor', 'default');
                }
            });
        }
    }    
    updateEmailAddressDispatch(userId, emailAddress) {
        return { type: "UPDATE_EMAIL_ADDRESS", payload: { 'userId': userId, 'emailAddress': emailAddress } };
    }
}
class RoleAction {
    getAvailableRoles() {
        return function (dispatch) {
            $.ajax({
                url: "/Roles/GetRoles",
                success: function (response) {
                    dispatch({ type: "GET_AVAILABLE_ROLES", payload: response });
                }
            })
        }
    }
}
class UserRoleAction {
    addUserToRoleClickHandler() {
        var userRole = {
            userId: store.getState().user.currentUser.ID,
            roleId: store.getState().userRole.selectedNewRole.Id,
            roleName: store.getState().userRole.selectedNewRole.Name,
        }
        return function (dispatch) {
            dispatch(actions.UserRoleAction.addUserToRole(userRole));
        }
    }
    setSelectedNewRole(roleId) {
        return { type: "SET_SELECTED_NEW_ROLE", payload: roleId }
    }
    addUserToRole(userRole) {
        return function (dispatch) {


            let data = JSON.stringify({ 'userId': userRole.userId, 'roleId': userRole.roleId })

            $.ajax({
                beforeSend: function () {
                    $('html').css('cursor', 'wait');
                },
                type: "POST",
                url: "/Roles/AddUserToRole",
                data: data,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.success) {
                        dispatch(actions.UserRoleAction.addUserToRoleDispatch(userRole));
                        dispatch(actions.UserRoleAction.setNewRoleDropdownVisibility(false));
                        dispatch(actions.UserRoleAction.setAddingNewUserToRole(false));
                        dispatch(actions.UserRoleAction.resetSelectedRole());
                    }
                },
                complete: function () {
                    $('html').css('cursor', 'default');
                }
            });
        }
    }
    removeUserFromRole(userId, roleId) {
        var data = JSON.stringify({ 'userId': userId, 'roleId': roleId });
        return function (dispatch) {
            $.ajax({
                beforeSend: function () {
                    $('html').css('cursor', 'wait');
                },
                type: "DELETE",
                data: data,
                url: "/Roles/RemoveUserFromRole",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.success) {
                        dispatch(actions.UserRoleAction.removeUserFromRoleDispatch(userId, roleId));
                    }

                },
                complete: function () {
                    $('html').css('cursor', 'default');
                }
            });
        }

    }
    removeUserFromRoleDispatch(userId, roleId) {
        return {
            type: "REMOVE_USER_FROM_ROLE", payload: { 'userId': userId, 'roleId': roleId }
        }
    }
    addUserToRoleDispatch(userRole) {
        return { type: "ADD_USER_TO_ROLE", payload: userRole }
    }

    toggleAddingNewUserToRole() {
        return { type: "TOGGLE_ADDING_NEW_USER_TO_ROLE" }
    }
    setAddingNewUserToRole(isUserAddingNewUserToRole) {
        return { type: "SET_ADDING_NEW_USER_TO_ROLE", payload: isUserAddingNewUserToRole }
    }
    toggleNewRoleDropdownVisibility() {
        return { type: "TOGGLE_NEW_ROLE_DROPDOWN_VISIBILITY" }
    }
    setNewRoleDropdownVisibility(isDropdownVisible) {
        return { type: "SET_NEW_ROLE_DROPDOWN_VISIBILITY", payload: isDropdownVisible }
    }
    resetSelectedRole() {
        return { type: "RESET_SELECTED_ROLE" }
    }
    setUserRoles(userRoles) {
        return { type: "SET_USER_ROLES", payload: userRoles }
    }

}
class EmployeeAction {
    setEmployees(employees) {
        return {type:"SET_EMPLOYEES", payload: employees}
    }
    setCurrentEmployee(userId) {
        return {type: "SET_CURRENT_EMPLOYEE", payload: userId}
    }
    toggleUpdateHireDateInputVisibility() {
        return { type: "TOGGLE_UPDATE_HIRE_DATE_INPUT_VISIBILITY" }
    }
    updateHireDate(userId, hireDate) {
        return function (dispatch) {
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
                        dispatch(actions.EmployeeAction.updateHireDateDispatch(userId, hireDate));
                    }
                },
                complete: function () {
                    $('html').css('cursor', 'default');
                }
            });
        }
    }
    updateHireDateDispatch(userId, hireDate) {
        var response = {
            'userId': userId,
            'hireDate': hireDate
        }
        return { type: "UPDATE_HIRE_DATE", payload: response }
    }
}


var actions = new Actions(new UserAction, new RoleAction, new UserRoleAction, new EmployeeAction);