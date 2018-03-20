//class MyComponent extends React.Component {
//    constructor(props) {
//        super(props);
//        this.state = {
//            error: null,
//            isLoaded: false,
//            items: []
//        };
//    }

//    componentDidMount() {
//        fetch("/Reports/GetUsers")
//            .then(res => res.json())
//            .then(
//            (result) => {
//                this.setState({
//                    isLoaded: true,
//                    items: result
//                });
//            },
//            // Note: it's important to handle errors here
//            // instead of a catch() block so that we don't swallow
//            // exceptions from actual bugs in components.
//            (error) => {
//                this.setState({
//                    isLoaded: true,
//                    error
//                });
//            }
//            )
//    }

//    render() {
//        const { error, isLoaded, items } = this.state;
//        if (error) {
//            return <div>Error: {error.message}</div>;
//        } else if (!isLoaded) {
//            return <div>Loading...</div>;
//        } else {
//            return (
//                <div id="ContainerUserNotificationDropdown" className="custom-input-group container-custom-dropdown">
//                    <label htmlFor="UserNotificationDropdown">Users to notify</label>
//                    <div id="UserNotificationDropdown" className="dropdown-custom">
//                        <div id="UserNotificationUsers" className="dropdown-custom-multi-items placeholder row">

//                        </div>
//                        <div id="UserNotificationIconDropdownDownAngle" className="icon-container-down-angle">
//                            <i className="fas fa-angle-down"></i>
//                        </div>
//                    </div>
//                    <div id="UserNotificationDropdownItems" className="dropdown-items-custom">
//                        {items.map((key, index) => (
//                            <div className="dropdown-item-custom" key={items[index].ID}>
//                                <p>{items[index].EmailAddress} </p>
//                            </div>
//                        ))}
//                    </div>
//                </div>
//            );
//        }
//    }
//}
//ReactDOM.render(
//    <MyComponent />,
//    document.getElementById('content')
//);


class MyComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            items: [],
            showMenu: false,
            selectedItems: []
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
    }
    componentDidMount() {
        fetch("/Reports/GetUsers")
            .then(res => res.json())
            .then(
            (result) => {
                this.setState({
                    isLoaded: true,
                    items: result
                });
            },
            // Note: it's important to handle errors here
            // instead of a catch() block so that we don't swallow
            // exceptions from actual bugs in components.
            (error) => {
                this.setState({
                    isLoaded: true,
                    error
                });
            }
        )
    }

    render() {
        var randVar = Math.random();
        const { error, isLoaded, items } = this.state;
        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div>Loading...</div>;
        } else {
            return (
                <div id="ContainerUserNotificationDropdown"
                    className="custom-input-group container-custom-dropdown" ref={(element) => {
                    this.dropdownMenu = element;
                }}>
                    <label htmlFor="UserNotificationDropdown">Users to notify</label>
                    <div id="UserNotificationDropdown" className="dropdown-custom" onClick={this.showMenu}>
                        <div id="UserNotificationUsers" className="dropdown-custom-multi-items placeholder row">
                            { this.state.selectedItems.length > 0 ? 
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
                                    {items.map((key, index) => (
                                            <div
                                                className="dropdown-item-custom"
                                                data-user-Id={items[index].ID}
                                                data-user-EmailAddress={items[index].EmailAddress}
                                                key={index}
                                                onClickCapture={this.addSelectedItem.bind(this)}
                                            >
                                            <p>{items[index].EmailAddress} </p>
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
ReactDOM.render(
    <MyComponent />,
    document.getElementById('content')
);
