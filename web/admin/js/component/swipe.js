/*
 * Zachary Cook
 *
 * Manages sessions by asking for users to swipe.
 */

let staticSwipe = null;



class Swipe extends React.Component {
    /*
     * Creates the swipe component.
     */
    constructor(props) {
        super(props);
        this.state = {ProcessState: "Checking"};

        // Bind the functions.
        this.setProcessState = this.setProcessState.bind(this);
        this.onInputChanged = this.onInputChanged.bind(this);
        this.processId = this.processId.bind(this);
        this.checkSession = this.checkSession.bind(this);
        this.logout = this.logout.bind(this);

        // Check the session.
        this.checkSession();

        // Occasionally check the session.
        staticSwipe = this;
        let swipeObject = this;
        setInterval(function() {
            swipeObject.checkSession();
        },5000)
    }

    /*
     * Sets the process state.
     */
    setProcessState(state) {
        this.state.ProcessState = state;
        this.setState(this.state);
    }

    /*
     * Asynchronously checks the current session.
     */
    checkSession() {
        // Return if the window isn't checking or hidden.
        if (this.state.ProcessState != "Checking" && this.state.ProcessState != "Hidden") {
            return;
        }

        // Set the state if there is no session.
        let session = getCookie("session");
        if (session == null) {
            this.setProcessState("Open");
            return;
        }

        // Check the session.
        let swipeObject = this;
        $.ajax({
            url: "/admin/checksession?" + $.param({
                session: session,
            }),
            success: function(result) {
                if (result.status == "success") {
                    swipeObject.setProcessState("Hidden");
                } else {
                    swipeObject.setProcessState("Open");
                }
            },
            error: function() {
                swipeObject.setProcessState("Error");
            }
        })
    }

    /*
     * Logs out the session.
     */
    logout() {
        deleteCookie("session");
        this.setProcessState("Open");
    }

    /*
     * Asynchronously processes an id.
     */
    processId(id) {
        // Set the process state as processing.
        this.setProcessState("Processing")

        // Run the request.
        let swipeObject = this;
        $.ajax({
            url: "/admin/authenticate?" + $.param({
                hashedid: sha256(id),
            }),
            success: function(result) {
                if (result.status == "success") {
                    setCookie("session",result.session);
                    swipeObject.setProcessState("Hidden");
                    staticSummary.loadData();
                } else {
                    swipeObject.setProcessState("Unauthorized");
                }
            },
            error: function() {
                swipeObject.setProcessState("Error");
            }
        })
    }

    /*
     * Invoked when the input changes, such the id being typed.
     */
    onInputChanged(event) {
        // Return if an id is being processed.
        if (this.state.ProcessState == "Hidden" || this.state.ProcessState == "Processing") {
            return;
        }

        // Return if there isn't a valid 9-digit number.
        let id = event.target.value.match(/\d+/);
        if (id == null || id[0].length != 9) {
            return;
        }
        id = id[0];

        // Process the id.
        this.processId(id);
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Return if the element is hidden (session exists and is valid).
        if (this.state.ProcessState == "Hidden") {
            return null;
        }

        // Return if the session is being checked.
        if (this.state.ProcessState == "Checking") {
            return <div class="SwipeBackgroundCover">
                <div class="SwipeBackground Beveled">
                    <div class="SwipeEnterIdText">Please wait...</div>
                </div>
            </div>
        }

        // Reset the views if the swipe form is open (not logged in).
        if (this.state.ProcessState == "Open" || this.state.ProcessState == "Error") {
            staticSummary.clearData();
        }

        // Determine the input item.
        let stateElement;
        if (this.state.ProcessState != "Processing") {
            stateElement = <input class="SwipeIdInputBox" type="text" onChange={this.onInputChanged}></input>
        }

        // Determine the message.
        let inputMessage;
        if (this.state.ProcessState == "Processing") {
            inputMessage = "Please wait...";
        } else if (this.state.ProcessState == "Unauthorized") {
            inputMessage = "Your ID is unauthorized.";
        } else if (this.state.ProcessState == "Error") {
            inputMessage = "An error occured.";
        }

        // Determine the input message element.
        let inputMessageElement;
        if (inputMessage != null) {
            inputMessageElement = <div class="SwipeEnterIdLowerText">{inputMessage}</div>
        }

        // Create the swipe component.
        return <div class="SwipeBackgroundCover">
            <div class="SwipeBackground Beveled">
                <div class="SwipeEnterIdText">Enter your university ID or swipe your ID.</div>
                {inputMessageElement}
                {stateElement}
            </div>
        </div>
    }
}