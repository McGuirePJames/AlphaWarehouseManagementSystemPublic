class MultiItemSelectDropdown extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            showMenu: false,
            selectedItems: [],
            availableItems: []
        };
        this.showMenu = this.showMenu.bind(this);
        this.closeMenu = this.closeMenu.bind(this);

    }
    showMenu(event) {
        event.preventDefault();

        this.setState({ showMenu: true }, () => {
            document.addEventListener('click', this.closeMenu);
        });
    }
    closeMenu() {
        if (!this.dropdownMenu.contains(event.target)) {
            if (!event.target.classList.contains('multi-item-display-email')) {
                if (!event.target.classList.contains("dropdown-custom-multi-item")) {
                    this.setState({ showMenu: false }, () => {
                        document.removeEventListener('click', this.closeMenu);
                    });
                }
            }
        }
    }
    addAvailableItem(Id, EmailAddress) {
        var availableItemArray = this.state.availableItems;

        var availableItem = {
            'Id': Id,
            'EmailAddress': EmailAddress
        }
        availableItemArray.push(availableItem);
        this.setState({ availableItems: availableItemArray });

    }
    removeAvailableItem(Id) {
        var availableItemsArray = this.state.availableItems;

        for (let i = 0; i < availableItemsArray.length; i++) {
            if (availableItemsArray[i].ID == Id) {
                availableItemsArray.splice(i, 1);
            }
        }
        this.setState({ availableItems: availableItemsArray });
    }
    addSelectedItem(event) {
        var clickedEle = event.currentTarget;

        if (!clickedEle.classList.contains("dropdown-item-customm")) {
            clickedEle = $(clickedEle).closest(".dropdown-item-custom")[0];
        }
        var userId = clickedEle.dataset.userId;
        var emailAddress = clickedEle.dataset.userEmailaddress;
        var newArrayItem = {
            'Id': userId,
            'EmailAddress': emailAddress
        }
        var arrayvar = this.state.selectedItems.slice()
        arrayvar.push(newArrayItem)
        this.setState({ selectedItems: arrayvar })
        this.removeAvailableItem(userId);
    }
    removeSelectedItem(event) {
        var clickedEle = event.currentTarget;
        if (!event.target.classList.contains("dropdown-custom-multi-item")) {
            clickedEle = $(clickedEle).closest(".dropdown-custom-multi-item")[0];
        }
        var newArray = this.state.selectedItems;

        for (let i = 0; i < newArray.length; i++) {
            if (clickedEle.dataset.userId == newArray[i].Id) {
                newArray.splice(i, 1);
            }
        }
        this.setState({ selectedItems: newArray });
        this.addAvailableItem(clickedEle.dataset.userId, clickedEle.dataset.userEmailaddress);
    }
    componentDidMount() {
        fetch("/Reports/GetUsers")
            .then(res => res.json())
            .then(
            (result) => {
                this.setState({
                    isLoaded: true,
                    availableItems: result
                });
            },
            (error) => {
                this.setState({
                    isLoaded: true,
                    error
                });
            }
            )
    }
    getSelectedItems() {
        return this.state.selectedItems;
    }
    render() {
        const { error, isLoaded, availableItems } = this.state;
        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div></div>;
        } else {
            return (
                <div id="ContainerUserNotificationDropdown"
                    className="custom-input-group container-custom-dropdown" ref={(element) => {
                        this.dropdownMenu = element;
                    }}>
                    <label htmlFor="UserNotificationDropdown">Users to notify</label>
                    <div id="UserNotificationDropdown" className="dropdown-custom" onClick={this.showMenu}>
                        <div id="UserNotificationUsers" className="dropdown-custom-multi-items placeholder row">
                            {this.state.selectedItems.length > 0 ?
                                this.state.selectedItems.map((key, index) => (
                                    <div
                                        className="dropdown-custom-multi-item"
                                        data-user-Id={this.state.selectedItems[index].Id}
                                        data-user-EmailAddress={this.state.selectedItems[index].EmailAddress}
                                        onClickCapture={this.removeSelectedItem.bind(this)}
                                        key={index}
                                    >
                                        <p
                                            className="multi-item-display-email"
                                        >{this.state.selectedItems[index].EmailAddress}</p>
                                    </div>
                                ))
                                :
                                null
                            }
                        </div>
                        <div id="UserNotificationIconDropdownDownAngle" className="icon-container-down-angle">
                            <i className="fas fa-angle-down"></i>
                        </div>
                    </div>
                    <div id="UserNotificationDropdownItems" className="dropdown-items-custom">
                        {
                            this.state.showMenu ?
                                (
                                    <div id="UserNotificationDropdownItems" className="dropdown-items-custom">
                                        {availableItems.map((key, index) => (
                                            <div
                                                className="dropdown-item-custom"
                                                data-user-Id={availableItems[index].ID}
                                                data-user-EmailAddress={availableItems[index].EmailAddress}
                                                key={index}
                                                onClickCapture={this.addSelectedItem.bind(this)}
                                            >
                                                <p>{availableItems[index].EmailAddress} </p>
                                            </div>
                                        ))}
                                    </div>
                                )
                                :
                                null
                        }
                    </div>
                </div>
            );
        }
    }
}
class ScheduledReports extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isLoaded: false,
            reports: []
        };

    }
    componentDidMount() {
        fetch("/Reports/GetScheduledReports")
            .then(res => res.json())
            .then(
            (result) => {
                this.setState({
                    isLoaded: true,
                    reports: result
                });
            },
            (error) => {
                this.setState({
                    isLoaded: true,
                    error
                });
            }
            )
    }
    addReport(scheduleId) {
        var newArray = this.state.reports;
        newArray.push(createSchedule(scheduleId));

        this.setState({ reports: newArray });
    }
    removeReport(event) {
        var clickedEle = event.currentTarget;
        clickedEle = $(clickedEle).closest('.list-item')[0];

        var scheduleId = clickedEle.dataset.scheduleId;
        var hangfireJobId = clickedEle.dataset.hangfireId;

        //var data = JSON.stringify({ "scheduleId": scheduleId, "hangfireJobId": hangfireJobId });
        var ajaxRequest = $.ajax({
            beforeSend: function (e) {
                $('html').css('cursor', 'wait');
            },
            url: "/Reports/DeleteScheduledReport/" + '?' + $.param({ "scheduleId": scheduleId}),
            type: "DELETE",
            //dataType: "json",
            success: function (response) {

                if (response.success) {

                    var newArray = this.state.reports;

                    for (let i = 0; i < newArray.length; i++) {
                        if (scheduleId == newArray[i].ScheduleId) {
                            newArray.splice(i, 1);
                        }
                    }
                    this.setState({ reports: newArray });
                }
                else if (!response.success) {
                    alert(response.responseText);
                }

            }.bind(this),
            complete: function (e) {
                $('html').css('cursor', 'default');            
            }
        });
    }
    render() {
        const { error, isLoaded, availableItems } = this.state;
        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div>
                <div className="list-item-group row">
                    <p>Daily</p>
                </div>
                <div className="list-item-group row">
                    <p>Weekly</p>
                </div>
                <div className="list-item-group row">
                    <p>Monthly</p>
                </div>
            </div>
        } else {
            return (

                <div>
                    <div className="list-item-group collapsed" href="#DailyScheduledReports" data-toggle="collapse" aria-expanded="false">
                        <p>Daily</p>
                    </div>
                    <div id="DailyScheduledReports" className="collapse">
                        {this.state.reports.map((key, index) => (
                            this.state.reports[index]._Frequency == "Daily" ?
                                <div className="list-item" data-schedule-Id={this.state.reports[index].ScheduleId} data-hangfire-Id={this.state.reports[index].HangfireJobId} key={index}>
                                    <div className="report-description">
                                        <div className="report-name">
                                            <p>{this.state.reports[index].Report.ReportName}</p>
                                        </div>
                                        <div className="schedule-name">
                                            <p>{this.state.reports[index].Name}</p>
                                        </div>
                                    </div>
                                    <div className="report-actions">
                                        <div className="report-action" onClick={this.removeReport.bind(this)}>
                                            <i className="far fa-trash-alt fa-2x"></i>
                                        </div>
                                    </div>
                                </div>
                                : null
                        ))}
                    </div>
                    <div className="list-item-group collapsed" href="#WeeklyScheduledReports" data-toggle="collapse" aria-expanded="false">
                        <p>Weekly</p>
                    </div>
                    <div id="WeeklyScheduledReports" className="collapse ">
                        {this.state.reports.map((key, index) => (
                            this.state.reports[index]._Frequency == "Weekly" ?
                                <div className="list-item" data-schedule-Id={this.state.reports[index].ScheduleId} data-hangfire-Id={this.state.reports[index].HangfireJobId} key={index}>
                                    <div className="report-description">
                                        <div className="report-name">
                                            <p>{this.state.reports[index].Report.ReportName}</p>
                                        </div>
                                        <div className="schedule-name">
                                            <p>{this.state.reports[index].Name}</p>
                                        </div>
                                    </div>
                                    <div className="report-actions">
                                        <div className="report-action" onClick={this.removeReport.bind(this)}>
                                            <i className="far fa-trash-alt fa-2x"></i>
                                        </div>
                                    </div>
                                </div>
                                : null
                        ))}
                    </div>
                    <div className="list-item-group collapsed" href="#MonthlyScheduledReports" data-toggle="collapse" aria-expanded="false" >
                        <p>Monthly</p>
                    </div>
                    <div id="MonthlyScheduledReports" className="collapse ">
                        {this.state.reports.map((key, index) => (
                            this.state.reports[index]._Frequency == "Monthly" ?
                                <div className="list-item" data-schedule-Id={this.state.reports[index].ScheduleId} data-hangfire-Id={this.state.reports[index].HangfireJobId} key={index}>
                                    <div className="report-description">
                                        <div className="report-name">
                                            <p>{this.state.reports[index].Report.ReportName}</p>
                                        </div>
                                        <div className="schedule-name">
                                            <p>{this.state.reports[index].Name}</p>
                                        </div>
                                    </div>
                                    <div className="report-actions">
                                        <div className="report-action" onClick={this.removeReport.bind(this)}>
                                            <i className="far fa-trash-alt fa-2x"></i>
                                        </div>
                                    </div>
                                </div>
                                : null
                        ))}
                    </div>
                </div>
            );
        }
    }
}

var UsersToNotifyDropdown = ReactDOM.render(
    <MultiItemSelectDropdown />,
    document.getElementById('DropdownMountPoint')
);


var ScheduledReportViewer = ReactDOM.render(
    <ScheduledReports />,
    document.getElementById('ReportsListContainer')
)