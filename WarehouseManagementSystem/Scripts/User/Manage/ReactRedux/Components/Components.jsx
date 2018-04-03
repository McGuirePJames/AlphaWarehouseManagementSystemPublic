class UserListComponent extends React.Component {
    constructor(props) {
        super(props)
        this.keyCount = 0;
        this.getKey = this.getKey.bind(this);
    }
    getKey() {
        return this.keyCount++;
    }
    componentWillMount() {
        this.props.initializeUserData();

    }
    componentDidMount() {

    }
    render() {
        const setCurrentUserClickHandler = this.props.setCurrentUser;
        const users = this.props.users
        const userState = this.props.userState
        //users != null ? this.props.setCurrentUser(users[0].ID) : null
        users != null && userState.currentUser == null ? this.props.setCurrentUser(users[0].ID) : null
        users != null && this.props.employees != null ? this.props.setCurrentEmployee(users[0].ID) : null
        const userRolesVar = this.props.userRoles;

        return (
            <div id="UserTable">
                {this.props.users != null ?
                    users.map((key, userIndex) => (
                        //<div className="row user-row" key={index} onClick={() => { this.props.setCurrentUser(users[index].ID) }} data-user-id={users[index].ID}>
                        <div className="row user-row" key={userIndex} onClick={(event) => { this.props.setCurrentUser(users[userIndex].ID); this.props.setCurrentEmployee(users[userIndex].ID); this.props.setAddingNewUserToRole(false); this.props.setNewRoleDropdownVisibility(false); this.props.resetSelectedRole() }} data-user-id={users[userIndex].ID}>
                            <div className="user-avatar-container">
                                {
                                    users[userIndex].ProfilePicturePath != "" ?
                                        (
                                            <img src={users[userIndex].ProfilePicturePath} />
                                        ) : null
                                }
                            </div>

                            <div className="user-information col-sm-8">
                                <div className="user-name">
                                    <p>{users[userIndex].Employee.FirstName} {users[userIndex].Employee.LastName}</p>
                                </div>
                                <div className="user-roles row col-sm-12">
                                    {/*{
                                        userRoles != null ?
                                            (
                                            userRoles.map((userRoleKey, userRoleIndex) => {
                                                    userRoles[userRoleIndex].UserId == users[index].ID ? (
                                                            <div key={userRoleKey} className="user-role">
                                                                <p data-role-Id={userRoles[userRoleIndex].RoleID}>{userRoles[userRoleIndex].RoleName}</p>
                                                            </div>                                             
                                                        ) : null
                                            })
                                            ) : null
                                    } */}
                                    {
                                        userRolesVar != null ? (
                                            userRolesVar.map((userRoleKey, userRoleIndex) =>
                                                users[userIndex].ID == this.props.userRoles[userRoleIndex].UserID ? (
                                                    (<div key={userRoleIndex} className="user-role" data-user-Id={users[userIndex].ID}>
                                                        <p data-role-Id={this.props.userRoles[userRoleIndex].RoleID}>{this.props.userRoles[userRoleIndex].RoleName}</p>
                                                    </div>
                                                    )) : null
                                            )) : null
                                    }
                                    {/*
                                    {users[index].UserRoles.length > 0 ?
                                        users[index].UserRoles.map((userRoleKey, userRoleIndex) => (
                                            <div key={userRoleKey} className="user-role">
                                                <p data-role-Id={users[index].UserRoles[userRoleIndex].RoleId}>{users[index].UserRoles[userRoleIndex].RoleName}</p>
                                            </div>
                                        )) : null
                                    } */}
                                </div>
                            </div>{/*
                            <div className="more-actions-container">
                                <i className="fas fa-ellipsis-v"></i>
                            </div> */}
                        </div>
                    )) : null
                }
            </div>
        )
    }
}


class UpdateEmailAddressComponent extends React.Component {
    render() {
        return (
            <div className="user-detail-user-login user-detail-container" data-edit-type="text">
                <div className="row">
                    <label>User Name</label>
                    <div className="edit-container" onClick={this.props.toggleInputVisibility}>
                        <i className="fas fa-pencil-alt"></i>
                    </div>
                </div>
                {!this.props.updateEmailAddressInputVisibility ? (<p className="user-detail-value">{this.props.currentUser.EmailAddress}</p>) :
                    (
                        <div className="user-information-update-actions">
                            <div className="user-information-update-input-container">
                                <input type="text" id="UserDetailEmailAddress" className="user-detail-input" data-property-name="EmailAddress" defaultValue={this.props.currentUser.EmailAddress} />
                            </div>
                            <div className="icon-container-checkmark" onClick={() => { this.props.updateEmailAddress(this.props.currentUser.ID, document.getElementById('UserDetailEmailAddress').value) }}>
                                <i className="fas fa-check"></i>
                            </div>
                        </div>
                    )
                }

            </div>
        )
    }
}

class UpdateHireDateComponent extends React.Component {
    render() {

        return (
            <div className="user-detail-user-hire-date user-detail-container" data-edit-type="date">
                <div className="row">
                    <label>Hire Date</label>
                    <div className="edit-container" onClick={this.props.toggleInputVisibility}>
                        <i className="fas fa-pencil-alt"></i>
                    </div>
                </div>
                {!this.props.inputVisibility ?
                    (<p className="user-detail-value custom-date-picker">{formatToAmerican(this.props.userState.currentUser.Employee.HireDate)}</p>) : (
                        <div className="user-information-update-actions">
                            <div className="user-information-update-input-container">
                                <input id="HireDateInput" type="date" defaultValue={this.props.userState.currentUser.Employee.HireDate} />
                            </div>
                            <div className="icon-container-checkmark" onClick={() => {this.props.updateHireDate(this.props.userState.currentUser.ID, document.getElementById('HireDateInput').value)}}>
                                <i className="fas fa-check"></i>
                            </div>
                        </div>
                    )
                }
            </div>
        )
    }
}
class CurrentUserComponent extends React.Component {
    componentDidMount() {
        var dfd = $.Deferred();
        var checkSelector = setInterval(function () {
            if ($("#UserRoleContainer").length) {
                dfd.resolve();
                ReactDOM.render(
                    <ReactRedux.Provider store={store}>
                        <CurrentUserRoles />
                    </ReactRedux.Provider>,
                    document.getElementById('UserRoleContainer')
                )
                clearInterval(checkSelector);
            }
        }, 100);

        var dfdEmployeeName = $.Deferred();
        var checkSelectoremployeeName = setInterval(function () {
            if ($("#EmployeeNameMountPoint").length) {
                dfdEmployeeName.resolve();
                ReactDOM.render(
                    <ReactRedux.Provider store={store}>
                        <UpdateEmailAddress />
                    </ReactRedux.Provider>,
                    document.getElementById('EmployeeNameMountPoint')
                )
                clearInterval(checkSelectoremployeeName);
            }
        }, 100);

        var dfdHireDate = $.Deferred();
        var checkSelectorHireDate = setInterval(function () {
            if ($("#HireDateMount").length) {
                dfdHireDate.resolve();
                ReactDOM.render(
                    <ReactRedux.Provider store={store}>
                        <UpdateHireDate />
                    </ReactRedux.Provider>,
                    document.getElementById('HireDateMount')
                )
                clearInterval(checkSelectorHireDate);
            }
        }, 100);
    }
    render() {
        var currentUser = this.props.currentUser
        var employees = this.props.employees;
        var currentEmployee = null;
        var test = this.props.test;
        if (employees != null) {
            for (var i = 0; i < employees.length; i++) {
                if (currentUser.ID == employees[i].UserId) {
                    currentEmployee = employees[i];
                }
            }
        }
        if (currentUser != null) {
            return (
                <div id="UserInformation">
                    <div className="col-sm-12">
                        <div className="user-detail-avatar-container">
                            <div className="user-detail-avatar">
                                {
                                    currentUser.ProfilePicturePath != "" ? (
                                        <img src={currentUser.ProfilePicturePath} />
                                    ) : null
                                }
                            </div>
                            <div className="user-detail-avatar-overlay">
                                <input type="file" id="ProfilePictureUpload" className="user-detail-avatar-upload" />
                                <label className="user-detail-avatar-upload-label" for="ProfilePictureUpload">Select picture</label>
                            </div>
                        </div>
                        {
                            this.props.currentEmployee != null ?
                                (
                                    <div className="user-detail-user-name">
                                        <p className="user-detail-value">{this.props.currentUser.Employee.FirstName} {this.props.currentUser.Employee.LastName}</p>
                                    </div>
                                ) : null
                        }
                        <div className="user-sub-details">
                            <div id="EmployeeNameMountPoint">

                            </div>

                            <div className="user-detail-user-id">
                                <div className="row">
                                    <label>User Id</label>
                                </div>
                                <p id="UserDetailUserId" className="user-detail-value">{currentUser.ID}</p>
                            </div>
         
                            <div className="user-detail-user-password user-detail-container">
                                <div className="row">
                                    <label>Password</label>
                                    {/*<div className="edit-container">
                                        <i className="fas fa-pencil-alt"></i>
                                    </div> */}
                                </div>
                                <p id="UserDetailPassword" className="user-detail-value">********</p>
                            </div>
                            <div id="HireDateMount">
                            </div>
                        </div>

                        <div id="UserRoleContainer" className="user-detail-user-roles user-detail-container" data-edit-type="dropdown">
   
                        </div>

                    </div>
                </div>
            )
        }
        return (
            <div></div>
        )
    }
}


class UserRoleComponent extends React.Component {
    componentDidMount() {
        this.props.getAvailableRoles();
    }
    render() {
        const currentUser = this.props.currentUser;
        const availableRoles = this.props.availableRoles;
        const availableusers = this.props.availableUsersUserRole;
        const userRoles = this.props.userRoles;
        const testF = this.props.test;

        if (availableRoles != null) {
            return (
                <div className="user-detail-user-roles user-detail-container" data-edit-type="dropdown">
                    <div className="row">
                        <label>User Roles</label>
                        <div className="edit-container row">
                            <i className="fas fa-pencil-alt"></i>
                            <div id="AddRoleToUser" onClick={this.props.toggleAddingNewUserRole}>
                                <i className="fas fa-plus"></i>
                            </div>
                        </div>
                    </div>
                    {
                        this.props.addingNewRole ? (
                            <div className="container-custom-dropdown">
                                <div className="dropdown-custom">
                                    {
                                        this.props.selectedNewRole != null ? (
                                            <p>{this.props.selectedNewRole.Name}</p>
                                        ) : (
                                                <p className="placeholder">Select a role</p>
                                            )
                                    }
                                    <div className="actions-dropdown">
                                        <div className="icon-container-down-angle" onClick={this.props.toggleNewRoleDropdownVisibility}>
                                            <i className="fas fa-angle-down"></i>
                                        </div>
                                        <div className="icon-container-checkmark" onClick={() => { this.props.addUserToRoleClickHandler() }}>
                                            <i className="fas fa-check"></i>
                                        </div>
                                    </div>
                                </div>
                                {
                                    this.props.newRoleDropdownVisibility ? (
                                        <div className="dropdown-items-custom" >
                                            {
                                                availableRoles.map((key, index) => (
                                                    <div
                                                        className="dropdown-item-custom"
                                                        data-role-Id={availableRoles[index].Id}
                                                        data-role-Name={availableRoles[index].Name}
                                                        onClick={() => this.props.setSelectedNewRole(availableRoles[index].Id)}
                                                        key={index}
                                                    >
                                                        <p>{availableRoles[index].Name}</p>
                                                    </div>
                                                ))
                                            }
                                        </div>
                                    ) : null

                                }

                            </div>
                        ) : null
                    }
                    {userRoles.map((key, index) =>
                        currentUser.ID == userRoles[index].UserID ? (
                            //    (
                            (<div key={index} className="user-detail-user-role user-detail-container" data-role-Id={userRoles[index].RoleID} data-user-Id={userRoles[index].UserID}>
                                <p className="user-detail-value">{userRoles[index].RoleName}</p>
                                <div className="icon-container-delete-row" onClick={() => this.props.removeFromRole(userRoles[index].UserID, userRoles[index].RoleID)}>
                                    <i className="far fa-trash-alt"></i>
                                </div>
                            </div>)
                            //)
                        ) : null
                    )}
                    {/*
                    {currentUser.UserRoles.map((key, index) => (
                        <div key={index} className="user-detail-user-role user-detail-container" data-role-Id={currentUser.UserRoles[index].RoleId} data-user-Id={currentUser.UserRoles[index].UserId}>
                            <p className="user-detail-value">{currentUser.UserRoles[index].RoleName}</p>
                            <div className="icon-container-delete-row">
                                <i className="far fa-trash-alt"></i>
                            </div>
                        </div>
                    ))} */}
                </div>

            )
        }
        else {
            return <div></div>
        }

    }
}

