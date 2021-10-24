/*
 * Zachary Cook
 *
 * Top bar for switching between views.
 */

class TopBar extends React.Component {
    /*
     * Creates the swipe component.
     */
    constructor(props) {
        super(props);
        this.state = {
            view: "Prints",
        };

        // Bind the functions.
        this.setView = this.setView.bind(this);
        this.setPrintsView = this.setPrintsView.bind(this);
        this.setUsersView = this.setUsersView.bind(this);
        this.search = this.search.bind(this);
        this.logout = this.logout.bind(this);
    }

    /*
     * Sets the active view.
     */
    setView(viewName) {
        // Return if the view is the same.
        if (this.state.view == viewName) { 
            return;
        }

        // Set the view.
        this.state.view = viewName;
        this.setState(this.state);
        staticSummary.setView(viewName,this.refs.searchBox.value);
    }

    /*
     * Sets the view to Prints.
     */
    setPrintsView(event) {
        this.setView("Prints");
    }

    /*
     * Sets the view to Users.
     */
    setUsersView(event) {
        this.setView("Users");
    }

    /*
     * Performs a search.
     */
    search(event) {
        staticSummary.search(this.refs.searchBox.value);
    }

    /*
     * Logs out the session.
     */
    logout(event) {
        staticSwipe.logout();
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Determine the classes.
        let printsButtonClasses = "TopBarButton TopBarTabButton";
        let usersButtonClasses = "TopBarButton TopBarTabButton";
        if (this.state.view == "Prints") {
            printsButtonClasses += " TopBarSelectedButton";
        } else if (this.state.view == "Users") {
            usersButtonClasses += " TopBarSelectedButton";
        }

        // Return the top bar.
        return <div class="TopBarContainer">
            <button class={printsButtonClasses} onClick={this.setPrintsView}>Prints</button>
            <button class={usersButtonClasses} onClick={this.setUsersView}>Users</button>
            <input class="TopBarSearchBox" type="text" ref="searchBox"></input>
            <button class="TopBarButton TopBarButtonSearch" onClick={this.search}>Search</button>
            <button class="TopBarButton TopBarLogoutButton" onClick={this.logout}>Logout</button>
        </div>
    }
}