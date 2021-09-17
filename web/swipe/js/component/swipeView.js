/*
 * Zachary Cook
 *
 * View for swiping ids.
 */

class SwipeView extends React.Component {
    /*
     * Creates the swipe view.
     */
    constructor(props) {
        super(props);
        this.state = {
            "status": "open",
        };

        // Listen to key presses.
        let swipeView = this;
        let buffer = "";
        document.addEventListener("keypress", (event) => {
            // Add a key to the buffer.
            var key = event.key;
            buffer += key;
            if (buffer.length) {
                buffer = buffer.substring(buffer.length - 16);
            }

            // Handle the id if the buffer is valid.
            if (staticApp.state.view != "swipe") return;
            if (swipeView.state.status != "open") return;
            if (buffer.length != 16) return;
            if (buffer[0] != ";") return;
            if (buffer[10] != "=") return;
            if (buffer[15] != "?") return;
            var id = buffer.match(/(\d+)/)[0];
            this.handleId(id);
          }, false);

        // Wrap the methods.
        this.setStatus = this.setStatus.bind(this);
        this.setStatusThenReset = this.setStatusThenReset.bind(this);
        this.handleId = this.handleId.bind(this);
    }

    /*
     * Sets the status.
     */
    setStatus(status) {
        this.state.status = status;
        this.setState(this.state);
    }

    /*
     * Sets the status, then resets the status to open.
     */
    setStatusThenReset(status, delaySeconds) {
        // Set the status.
        this.setStatus(status);

        // Reset the status after a delay.
        let swipeView = this;
        setTimeout(function() {
            swipeView.setStatus("open");
        }, delaySeconds * 1000);
    }

    /*
     * Handles a university id.
     */
    handleId(id) {
        // Set the status as processing.
        this.setStatus("processing");

        // Send the information request.
        let swipeView = this;
        $.ajax({
            url: "/name?" + $.param({
                universityId: id,
            }),
            cache: false,
            success: function(result) {
                result = JSON.parse(result);
                if (result.result == "success" && result.name != null) {
                    // Add the swipe log.
                    $.ajax({
                        type: "POST",
                        url: "/appendswipelog?" + $.param({
                            "universityId": id,
                        }),
                        data: "",
                        cache: false,
                        success: function(data){
                            // Store the name and get the balance.
                            swipeView.state.currentName = result.name;
                            $.ajax({
                                url: "/userbalance?" + $.param({
                                    universityId: id,
                                }),
                                cache: false,
                                success: function(result) {
                                    result = JSON.parse(result);
                                    if (result.result == "success") {
                                        // Store the balance and display the result.
                                        swipeView.state.currentBalance = result.balance;
                                        if (swipeView.state.currentBalance != null && swipeView.state.currentBalance > 0) {
                                            swipeView.setStatusThenReset("displayUser", 4);
                                        } else {
                                            swipeView.setStatusThenReset("displayUser", 2);
                                        }
                                    } else {
                                        // Display an error occured.
                                        swipeView.setStatusThenReset("error", 4);
                                    }
                                },
                                error: function() {
                                    // Display an error occured.
                                    swipeView.setStatusThenReset("error", 4);
                                }
                            });
                        },
                        error: function(data) {
                            // Display an error occured.
                            swipeView.setStatusThenReset("error", 4);
                        }
                    });
                } else {
                    // Prompt the form.
                    staticApp.state.currentId = id;
                    swipeView.setStatusThenReset("form", 2);
                    setTimeout(function() {
                        staticApp.setVisibleView("form");
                    }, 1000);
                }
            },
            error: function() {
                // Display an error occured.
                swipeView.setStatusThenReset("error", 4);
            }
        });
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Determine the text.
        let topText = "Please swipe your Student Id";
        let bottomText = "";
        if (this.state.status == "processing") {
            topText = "Please wait...";
        } else if (this.state.status == "error") {
            topText = "An eror has occured!";
            bottomText = "Please try again later.";
        } else if (this.state.status == "displayUser") {
            topText = "Welcome back " + this.state.currentName +"!";
            if (this.state.currentBalance != null && this.state.currentBalance > 0) {
                let amountDue = new Intl.NumberFormat("en-US", {
                    style: "currency",
                    currency: "USD",
                }).format(this.state.currentBalance);
                bottomText = "Reminder: You have " + amountDue + " due.";
            }
        } else if (this.state.status == "form") {
            topText = "Please fill out the form that pops up.";
        }

        // Render the app.
        return <div class="View" style={{left: (this.props.position * 100) + "%"}}>
            <span class="SwipeText" style={{top: "10px", "font-size": "30px"}}>Welcome to The Construct!</span>
            <img class="SwipeLogo" src="/swipe/img/ConstructDotsLogoWhite.jpg"/>
            <span class="SwipeText" style={{top: "560px", "font-size": "36px"}}>{topText}</span>
            <span class="SwipeText" style={{top: "610px", "font-size": "30px"}}>{bottomText}</span>
        </div>
    }
}