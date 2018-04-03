
//user list
function mapStateToProps(state) {
    return {
        users: state.user.users,
        userRoles: state.userRole.userRoles,
        employees: state.employee.employees,
        userState: state.user,

    }
}
function mapDispatchToProps(dispatch) {
    return {
        setPersonNameClick: () => dispatch(actions.UserAction.setPersonName()),
        getUsers: () => dispatch(actions.UserAction.getUsers()),
        setCurrentUser: (userIdParam) => dispatch(actions.UserAction.setCurrentUser(userIdParam)),
        setCurrentEmployee: (userIdParam) => dispatch(actions.EmployeeAction.setCurrentEmployee(userIdParam)),
        setAddingNewUserToRole: (isUserAddingNewUserToRole) => dispatch(actions.UserRoleAction.setAddingNewUserToRole(isUserAddingNewUserToRole)),
        setNewRoleDropdownVisibility: (isDropdownVisible) => dispatch(actions.UserRoleAction.setNewRoleDropdownVisibility(isDropdownVisible)),
        resetSelectedRole: () => dispatch(actions.UserRoleAction.resetSelectedRole()),
        initializeUserData: () => dispatch(actions.UserAction.initializeUserData()),

    }
}
const UserList = ReactRedux.connect(
    mapStateToProps,
    mapDispatchToProps
)(UserListComponent)

ReactDOM.render(
    <ReactRedux.Provider store={store}>
        <UserList />
    </ReactRedux.Provider>,
    document.getElementById('ContainerUserTable')
)



//current user 
function mapStateToPropsCurrentUser(state) {
    return {
        currentUser: state.user.currentUser,
        currentEmployee: state.employee.currentEmployee,
        employees: state.employee.employees,
    }
}
function mapDispatchToPropsCurrentUser(dispatch) {
    return {
        getUsers: () => dispatch(actions.UserAction.getUsers()),

    }
}
const CurrentUser = ReactRedux.connect(
    mapStateToPropsCurrentUser,
    mapDispatchToPropsCurrentUser
)(CurrentUserComponent)

ReactDOM.render(
    <ReactRedux.Provider store={store}>
        <CurrentUser />
    </ReactRedux.Provider>,
    document.getElementById('ContainerCurrentUser')
)



//user role manager
function mapStateToPropsCurrentUserRoles(state) {
    return {
        users: state.user.users,
        currentUser: store.getState().user.currentUser,
        availableRoles: state.role.availableRoles,
        addingNewRole: state.userRole.addingNewRole,
        newRoleDropdownVisibility: state.userRole.newRoleDropdownVisibility,
        selectedNewRole: state.userRole.selectedNewRole,
        availableRoles: state.role.availableRoles,
        availableUsersUserRole: state.user.users,
        userRoles: state.userRole.userRoles,
        employees: state.employee.employees,
    }
}
function mapDispatchToPropsCurrentUserRoles(dispatch) {
    return {
        setCurrentUser: (userParam) => dispatch(actions.UserAction.setCurrentUser(userParam)),
        getAvailableRoles: () => dispatch(actions.RoleAction.getAvailableRoles()),
        toggleAddingNewUserRole: () => dispatch(actions.UserRoleAction.toggleAddingNewUserToRole()),
        toggleNewRoleDropdownVisibility: () => dispatch(actions.UserRoleAction.toggleNewRoleDropdownVisibility()),
        setSelectedNewRole: (roleId) => dispatch(actions.UserRoleAction.setSelectedNewRole(roleId)),
        addUserToRoleClickHandler: () => dispatch(actions.UserRoleAction.addUserToRoleClickHandler()),
        removeFromRole: (userId, roleId) => dispatch(actions.UserRoleAction.removeUserFromRole(userId, roleId))
    }
}
const CurrentUserRoles = ReactRedux.connect(
    mapStateToPropsCurrentUserRoles,
    mapDispatchToPropsCurrentUserRoles
)(UserRoleComponent)

//this render is inside the CurrentUserComponent of the ComponentDidMount method
//ReactDOM.render(
//    <ReactRedux.Provider store={store}>
//        <CurrentUserRoles />
//    </ReactRedux.Provider>,
//    document.getElementById('UserRoleContainer')
//)


//email address
function mapStateToPropsCurrentEmailAddress(state) {
    return {
        currentUser: store.getState().user.currentUser,
        currentUserForEmailAddress: store.getState().user.currentUser,
        updateEmailAddressInputVisibility: state.user.updateEmailAddressInputVisibility,
    }
}
function mapDispatchToPropsCurrentEmailAddress(dispatch) {
    return {
        toggleInputVisibility: () => dispatch(actions.UserAction.toggleUpdateEmailAddressInputVisibility()),
        updateEmailAddress: (userId, emailAddress) => dispatch(actions.UserAction.updateEmailAddress(userId, emailAddress)),
    }
}
const UpdateEmailAddress = ReactRedux.connect(
    mapStateToPropsCurrentEmailAddress,
    mapDispatchToPropsCurrentEmailAddress,
)(UpdateEmailAddressComponent)


//hire date update
function mapStateToPropsUpdateHireDate(state) {
    return {
        userState: state.user,
        currentUserForHireDate: store.getState().user.currentUser,
        currentEmployee: state.employee.currentEmployee,
        inputVisibility: state.employee.updateHireDateInputVisibility,
    }
}
function mapDispatchToPropsUpdateHireDate(dispatch) {
    return {
        toggleInputVisibility: () => dispatch(actions.EmployeeAction.toggleUpdateHireDateInputVisibility()),
        updateHireDate: (userId, hireDate) => dispatch(actions.UserAction.updateHireDate(userId, hireDate)),
    }
}
const UpdateHireDate = ReactRedux.connect(
    mapStateToPropsUpdateHireDate,
    mapDispatchToPropsUpdateHireDate
)(UpdateHireDateComponent)




