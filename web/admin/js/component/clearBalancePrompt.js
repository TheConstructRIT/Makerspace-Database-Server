/*
 * Zachary Cook
 *
 * Prompt for clearing user balances.
 */

class ClearBalancePrompt extends React.Component {
    /*
     * Creates the clear balance prompt component.
     */
    constructor(props) {
        super(props);
        this.state = {
            window: "Confirm",
        };

        // Bind the function.
        this.updateState = this.updateState.bind(this);
        this.close = this.close.bind(this);
        this.backgroundClicked = this.backgroundClicked.bind(this);
        this.confirmPrompt = this.confirmPrompt.bind(this);
        this.tigerBucksUsed = this.tigerBucksUsed.bind(this);
        this.tigerBucksNotUsed = this.tigerBucksNotUsed.bind(this);
        this.clearBalanceComplete = this.clearBalanceComplete.bind(this);
    }

    /*
     * Updates the state.
     */
    updateState() {
        this.setState(this.state);
    }

    /*
     * Closes the prompt.
     */
    close() {
        this.props.inspectWindow.state.clearBalanceActive = false;
        this.props.inspectWindow.updateState();
    }

    /*
     * Invoked when the background is clicked.
     */
    backgroundClicked(event) {
        if (event.target == this.refs.BackgroundCover) {
            this.close();
        }
    }

    /*
     * Confirms clearing the balance.
     */
    confirmPrompt() {
        this.state.window = "TigerBucks";
        this.updateState();
    }

    /*
     * Clears the balance using TigerBucks.
     */
    tigerBucksUsed() {
        this.state.window = "PleaseWait";
        this.updateState();
        this.props.inspectWindow.clearBalance(true,this.clearBalanceComplete);
    }

    /*
     * Clears the balance not using TigerBucks.
     */
    tigerBucksNotUsed() {
        this.state.window = "PleaseWait";
        this.updateState();
        this.props.inspectWindow.clearBalance(false,this.clearBalanceComplete);
    }

    /*
     * Callback for when the balance is cleared.
     */
    clearBalanceComplete(success) {
        if (success) {
            this.state.window = "Success";
        } else {
            this.state.window = "Failed";
        }
        this.updateState();
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Return if the prompt isn't active.
        if (this.props.active != true) {
            this.state.window = "Confirm";
            return null;
        }

        // Create and return the element.
        if (this.state.window == "Confirm") {
            // Format how much is owed.
            let formattedOwedCost = new Intl.NumberFormat("en-US",{
                style: "currency",
                currency: "USD",
            }).format(this.props.data.totalOwedCost);

            // Return the initial prompt.
            return <div ref="BackgroundCover" class="ClearBalanceBackgroundCover" onClick={this.backgroundClicked}>
                <div class="ClearBalanceBackground Beveled">
                    <div class="ClearBalanceText">{"Clear balance for " + this.props.data.name + " of " + formattedOwedCost + "?"}</div>
                    <center>
                            <button class="ClearBalanceButton" onClick={this.confirmPrompt}>Yes</button>
                            <button class="ClearBalanceButton" onClick={this.close}>No</button>
                    </center>
                </div>
            </div>
        } else if (this.state.window == "TigerBucks") {
            // Return the prompt for if TigerBucks was used.
            return <div ref="BackgroundCover" class="ClearBalanceBackgroundCover" onClick={this.backgroundClicked}>
                <div class="ClearBalanceBackground Beveled">
                    <div class="ClearBalanceText">Was TigerBucks used?</div>
                    <center>
                        <button class="ClearBalanceButton" onClick={this.tigerBucksUsed}>Yes</button>
                        <button class="ClearBalanceButton" onClick={this.tigerBucksNotUsed}>No</button>
                    </center>
                </div>
            </div>
        } else if (this.state.window == "PleaseWait") {
            // Return the prompt with a message.
            return <div ref="BackgroundCover" class="ClearBalanceBackgroundCover" onClick={this.backgroundClicked}>
                <div class="ClearBalanceBackground Beveled">
                    <div class="ClearBalanceText">Please wait...</div>
                </div>
            </div>
        } else if (this.state.window == "Success") {
            // Return the prompt with a message.
            return <div ref="BackgroundCover" class="ClearBalanceBackgroundCover" onClick={this.backgroundClicked}>
                <div class="ClearBalanceBackground Beveled">
                    <div class="ClearBalanceText">The balance has been cleared.</div>
                    <center>
                        <button class="ClearBalanceButton" onClick={this.close}>Ok</button>
                    </center>
                </div>
            </div>
        } else if (this.state.window == "Failed") {
            // Return the prompt with a message.
            return <div ref="BackgroundCover" class="ClearBalanceBackgroundCover" onClick={this.backgroundClicked}>
                <div class="ClearBalanceBackground Beveled">
                    <div class="ClearBalanceText">An error occured. The balance is not cleared.</div>
                    <center>
                        <button class="ClearBalanceButton" onClick={this.close}>Ok</button>
                    </center>
                </div>
            </div>
        }
    }
}