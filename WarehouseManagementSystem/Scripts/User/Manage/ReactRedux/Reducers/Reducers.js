function UserReducer(state = {
    currentUser: null,
    currentUserForEmailAddress: null,
    users: null,
    updateEmailAddressInputVisibility: false,
}, action) {
    switch (action.type) {
        case "GET_USERS": {
            return { ...state, users: action.payload }
        }
        case "UPDATE_USER_IN_AVAILABLE_USERS": {
            var user = action.payload;
            var availableUsers = state.users;

            for (let i = 0; i < availableUsers.length; i++) {
                if (user.Id == availableUsers[i].Id) {
                    availableUsers[i] = user;
                }
            }
            return { ...state, users: availableUsers }
        }
        case "TOGGLE_UPDATE_EMAIL_ADDRESS_INPUT_VISIBILITY": {
            if (state.updateEmailAddressInputVisibility) {
                return { ...state, updateEmailAddressInputVisibility: false }
            }

            return { ...state, updateEmailAddressInputVisibility: true }
        }
        case "SET_CURRENT_USER": {
            var users = state.users;
            var currentUser = null;
            var userId = action.payload;
            for (let i = 0; i < users.length; i++) {
                if (userId == users[i].ID) {
                    currentUser = users[i];
                }
            }

            return { ...state, currentUser: currentUser }
        }
        case "SET_USERS": {
            return { ...state, users: action.payload }
        }
        case "UPDATE_EMAIL_ADDRESS": {
            var userId = action.payload.userId
            var emailAddress = action.payload.emailAddress;
            var userIndex = null;
            var state = state;

            for (let i = 0; i < state.users.length; i++) {
                if (state.users[i].ID == userId) {
                    userIndex = i;
                }
            }

            var updatedUser = Object.assign({}, state.users[userIndex]);
            updatedUser.EmailAddress = emailAddress;
            var stateWithRemovedUser = [...state.users.slice(0, userIndex), ...state.users.slice(userIndex + 1)];

            var updatedUsers = {  ...state, users: stateWithRemovedUser.concat(updatedUser) }

            updatedUsers.users.sort(function (a, b) {
                var textA = a.Employee.FirstName.toUpperCase();
                var textB = b.Employee.FirstName.toUpperCase();
                return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
            });


            return {
                ...state, users: updatedUsers.users,
                currentUser: {
                    ...state.currentUser,
                    EmailAddress: emailAddress
                }
            }
            //return {
            //    ...state, users: stateWithRemovedUser.concat(updatedUser),
            //    currentUser: {
            //        ...state.currentUser,
            //        EmailAddress: emailAddress
            //    }
            //}
        }
        case "UPDATE_HIRE_DATE": {
            var userId = action.payload.userId
            var hireDate = action.payload.hireDate;
            var userIndex = null;
            var state = state;

            for (let i = 0; i < state.users.length; i++) {
                if (state.users[i].ID == userId) {
                    userIndex = i;
                }
            }

            var updatedUser = Object.assign({}, state.users[userIndex]);
            updatedUser.Employee.HireDate = hireDate;
            var stateWithRemovedUser = [...state.users.slice(0, userIndex), ...state.users.slice(userIndex + 1)];

            var updatedUsers = { ...state, users: stateWithRemovedUser.concat(updatedUser) }

            updatedUsers.users.sort(function (a, b) {
                var textA = a.Employee.FirstName.toUpperCase();
                var textB = b.Employee.FirstName.toUpperCase();
                return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
            });

            return {
                ...state, users: updatedUsers.users,
                currentUser: {
                    ...state.currentUser,
                    Employee: {
                        ...state.currentUser.Employee, HireDate: hireDate
                    }
                }
            }
        }
    }
    return state
}
function RoleReducer(state = {
    availableRoles: null,
}, action) {
    switch (action.type) {
        case "GET_AVAILABLE_ROLES": {
            return { ...state, availableRoles: action.payload }
        }

    }
    return state;
}
function UserRoleReducer(state = {
    userRoles: null,
    addingNewRole: false,
    newRoleDropdownVisibility: false,
    selectedNewRole: null,
    availableUsersUserRole: null,
}, action) {
    switch (action.type) {

        case "TOGGLE_ADDING_NEW_USER_TO_ROLE": {
            if (state.addingNewRole) {
                return { ...state, addingNewRole: false }
            }
            else {
                return { ...state, addingNewRole: true }
            }
        }
        case "ADD_USER_TO_ROLE": {
            var newRole = {
                UserID: action.payload.userId,
                RoleID: action.payload.roleId,
                RoleName: action.payload.roleName
            }
            return {
                ...state,
                userRoles: state.userRoles.concat(newRole)
            }
        }
        case "REMOVE_USER_FROM_ROLE": {
            var userRoles = state.userRoles;
            var userId = action.payload.userId;
            var roleId = action.payload.roleId;

            var index = null;

            for (let i = 0; i < userRoles.length; i++) {
                if (userRoles[i].UserID == userId) {
                    if (userRoles[i].RoleID == roleId) {
                        index = i;
                    }
                }
            }
            return {
                userRoles: [...state.userRoles.slice(0, index), ...state.userRoles.slice(index + 1)]
            }
        }
        case "SET_ADDING_NEW_USER_TO_ROLE": {
            return { ...state, addingNewRole: action.payload }
        }
        case "TOGGLE_NEW_ROLE_DROPDOWN_VISIBILITY": {
            if (state.newRoleDropdownVisibility) {
                return { ...state, newRoleDropdownVisibility: false }
            }
            else {
                return { ...state, newRoleDropdownVisibility: true }
            }
        }
        case "SET_NEW_ROLE_DROPDOWN_VISIBILITY": {
            return { ...state, newRoleDropdownVisibility: action.payload }
        }
        case "SET_SELECTED_NEW_ROLE": {
            var roleId = action.payload;
            var availableRoles = store.getState().role.availableRoles;
            var foundRole = null;
            for (let i = 0; i < availableRoles.length; i++) {
                if (roleId == availableRoles[i].Id) {
                    foundRole = availableRoles[i]
                }
            }
            return { ...state, selectedNewRole: foundRole }
        }
        case "RESET_SELECTED_ROLE": {
            return { ...state, selectedNewRole: null }
        }
        case "SET_USER_ROLES": {
            return { ...state, userRoles: action.payload }
        }
 
    }
    return state;
}
function EmployeeReducer(state = {
    employees: null,
    currentUser: null,
    currentEmployee: null,
    updateHireDateInputVisibility: false,
    currentUserForHireDate: null,

}, action) {
    switch (action.type) {
        case "SET_EMPLOYEES": {
            return { ...state, employees: action.payload }
        }
        case "SET_CURRENT_EMPLOYEE": {
            var employees = state.employees;
            var currentEmployee = null;
            var userId = action.payload;
  
            for (let i = 0; i < employees.length; i++) {
                if (userId == employees[i].UserId) {
                    currentEmployee = employees[i];
                }
            }
            return { ...state, currentEmployee: currentEmployee }
        }
        case "TOGGLE_UPDATE_HIRE_DATE_INPUT_VISIBILITY": {
            if (state.updateHireDateInputVisibility) {
                return { ...state, updateHireDateInputVisibility: false }
            }
            return { ...state, updateHireDateInputVisibility: true }
        }
        case "UPDATE_HIRE_DATE": {
            var state = state;
 
            return {
                ...state,
                currentEmployee: {
                    ...state.currentEmployee,
                    HireDate: action.payload.hireDate
                }
            }
        }
    }
    return state;
}

var user = UserReducer;
var role = RoleReducer;
var userRole = UserRoleReducer
var employee = EmployeeReducer

var reducer = Redux.combineReducers({
    user,
    role,
    userRole,
    employee,
})