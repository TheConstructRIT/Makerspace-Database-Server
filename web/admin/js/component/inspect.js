/*
 * Zachary Cook
 *
 * Input for inspecting inputs that allows modifying.
 */

let INSPECT_ELEMENTS = {
    "Print": {
        "InspectPrintName": "filename",
        "InspectPurpose": "purpose",
        "InspectWeight": "weight",
        "InspectMSD": "msdnumber",
        "InspectOwed": "owed",
    },
    "User": {
        "InspectName": "name",
        "InspectEmail": "email",
    },
}



class Inspect extends React.Component {
    /*
     * Creates the swipe component.
     */
    constructor(props) {
        super(props);
        this.state = {
            modifying: false,
        };

        // Bind the function.
        this.updateState = this.updateState.bind(this);
        this.setInformation = this.setInformation.bind(this);
        this.close = this.close.bind(this);
        this.toggleModify = this.toggleModify.bind(this);
        this.promptClearBalance = this.promptClearBalance.bind(this);
    }

    /*
     * Updates the state.
     */
    updateState() {
        this.setState(this.state);
    }

    /*
     * Sets the information to display.
     */
    setInformation(data) {
        this.props.summary.state.inspectData = data;
        this.props.summary.updateState();
    }

    /*
     * Closes the inspection.
     */
    close(event) {
        if (event.target == this.refs.BackgroundCover) {
            this.setInformation(null);
        }
    }

    /*
     * Toggles modifying information.
     */
    toggleModify() {
        // Save the entries if they are valid and modifying is active.
        if (this.state.modifying) {
            // Get the entry names to save.
            let entryType = "Print";
            if (this.props.data.filename == null) {
                entryType = "User";
            }
            let entryNames = INSPECT_ELEMENTS[entryType];
            
            // Read the entries.
            let entries = {};
            for (let entry in entryNames) {
                let value = this.refs[entry].getValue();
                entries[entryNames[entry]] = value;

                // Return if a value isn't valid.
                if (!this.refs[entry].isValid(value)) {
                    return;
                }
            }

            // Store the identifiers.
            entries["session"] = getCookie("session");
            if (entryType == "Print") {
                entries["initialFileName"] = this.props.data.filename;
                entries["time"] = this.props.data.time;
            } else if (entryType == "User") {
                entries["hashedUniversityId"] = this.props.data.hashedUniversityId;
            }

            // Send the network request to update.
            let inspectObject = this;
            let url = "/admin/changeprint?" + $.param(entries);
            if (entryType == "User") {
                url = "/admin/changeuser?" + $.param(entries);
            }
            $.ajax({
                url: url,
                method: "POST",
                success: function() {
                    // TODO: Future enhancement - reload data from server, such as changing the balance for users.
                    inspectObject.props.summary.loadData();
                }
            })

            // Update the display.
            this.props.summary.updateInspectValues(entries);
        }

        // Toggle modifying.
        this.state.modifying = !this.state.modifying;
        this.setState(this.state);
    }

    /*
     * Prompts to clear the print balance of the current user.
     */
    promptClearBalance() {
        this.state.clearBalanceActive = true;
        this.updateState();
    }

    /*
     * Clears the print balance of the current user.
     */
    clearBalance(tigerBucksUsed,completeCallback) {
        let inspectObject = this;
        $.ajax({
            url: "/admin/clearbalance?" + $.param({
                "hasheduniversityid": this.props.data.hashedUniversityId,
                "tigerbucks": tigerBucksUsed,
                "session": getCookie("session"),
            }),
            method: "POST",
            success: function(result) {
                result = JSON.parse(result);
                if (result.result == "success") {
                    // Clear the balance.
                    inspectObject.props.data.totalOwedCost = 0;
                    inspectObject.props.data.totalOwedPrints = 0;
                    inspectObject.props.summary.updateInspectValues(inspectObject.props.data);
                    inspectObject.updateState();

                    // Reload the data.
                    inspectObject.props.summary.loadData();
                    inspectObject.refs.InspectSummary.loadData();

                    // End the promnpt.
                    completeCallback(true);
                } else {
                    completeCallback(false);
                }
            },
            error: function() {
                completeCallback(false);
            }
        })
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Return if there is no data.
        if (this.props.data == null) {
            this.state.modifying = false;
            return null;
        }

        if (this.props.data.filename != null) {
            // Format the date.
            let date = new Date(this.props.data.time * 1000);
            date = (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear() + " " + date.toLocaleTimeString("en-US");

            // Format the cost information.
            let cost = new Intl.NumberFormat("en-US",{
                style: "currency",
                currency: "USD",
            }).format(this.props.data.cost);
            let owed = "No";
            if (this.props.data.owed) {
                owed = "Yes";
            }

            // Create the component.
            return <div ref="BackgroundCover" class="InspectBackgroundCover" onClick={this.close}>
                <div class="InspectPrintBackground Beveled">
                    <InspectInput ref="InspectPrintName" modify={this.state.modifying} class="InspectTextBig" name="Print Name" value={this.props.data.filename}/>
                    <InspectInput class="InspectText" name="Export Time" value={date}/>
                    <InspectInput class="InspectText" name="User" value={this.props.data.user}/>
                    <InspectInput ref="InspectPurpose" modify={this.state.modifying} class="InspectText" name="Purpose" value={this.props.data.purpose}/>
                    <div class="InspectDivider"/>
                    <InspectInput ref="InspectWeight" modify={this.state.modifying} class="InspectText" name="Weight (Grams)" type="Integer" value={this.props.data.weight}/>
                    <InspectInput class="InspectText" name="Material" value={this.props.data.material}/>
                    <InspectInput class="InspectText" name="Cost (USD)" value={cost}/>
                    <InspectInput ref="InspectMSD" modify={this.state.modifying} class="InspectText" name="MSD" type="MSDNumber" value={this.props.data.msdnumber}/>
                    <InspectInput ref="InspectOwed" modify={this.state.modifying} class="InspectText" name="Owed" type="Bool" value={owed}/>
                    <div class="InspectDivider"/>
                    <center>
                        <button class="InspectModifyButton" onClick={this.toggleModify}>Modify</button>
                    </center>
                </div>
            </div>
        } else if (this.props.data.email) {
            // Format the owed cost.
            let owedCost = new Intl.NumberFormat("en-US",{
                style: "currency",
                currency: "USD",
            }).format(this.props.data.totalOwedCost);

            // Create the component.
            return <div ref="BackgroundCover" class="InspectBackgroundCover" onClick={this.close}>
                <div class="InspectUserBackground Beveled">
                    <InspectInput ref="InspectName" modify={this.state.modifying} class="InspectTextBig" name="Name" value={this.props.data.name}/>
                    <InspectInput ref="InspectEmail" modify={this.state.modifying} class="InspectText" name="Email" type="Email" value={this.props.data.email}/>
                    <div class="InspectDivider"/>
                    <InspectInput class="InspectText" name="Total Owed Cost (USD)" value={owedCost}/>
                    <InspectInput class="InspectText" name="Total Owed Prints" value={this.props.data.totalOwedPrints}/>
                    <InspectInput class="InspectText" name="Total Prints" value={this.props.data.totalPrints}/>
                    <InspectInput class="InspectText" name="Total Weight (Grams)" value={this.props.data.totalWeight}/>
                    <div class="InspectSummaryContainer">
                        <Summary user={this.props.data.hashedUniversityId} ref="InspectSummary"/>
                    </div>
                    <center>
                        <button class="InspectModifyButton" onClick={this.toggleModify}>Modify</button>
                        {(this.props.data.totalOwedCost > 0) ? <button class="InspectClearBalanceButton" onClick={this.promptClearBalance}>Clear Balance</button> : null}
                    </center>
                </div>
                <ClearBalancePrompt active={this.state.clearBalanceActive} data={this.props.data} inspectWindow={this}/>
            </div>
        }
    }
}




/*
 * Sets the inspect data for a summary reference.
 */
function setInspect(summaryObject,inspectData) {
    // Detemrine the existing object.
    let existingId = null;
    let existingInspect = null;
    for (let i = 0; i < inspectItems.length; i++) {
        if (inspectItems[i].summary == summaryObject) {
            existingId = i;
            existingInspect = inspectItems[i];
            break;
        }
    }

    if (existingInspect != null) {
        // Update the inspect item.
        if (inspectData == null) {
            // Remove the inspect data.
            inspectItems.splice(existingId);
            staticApp.updateState();
        } else if (existingInspect.data != inspectData) {
            // Update the data.
            existingInspect.data = inspectData;
            staticApp.updateState();
        }
    } else {
        // Add a new inspect item if it doesn't exist.
        if (inspectData != null) {
            inspectItems.push({
                "summary": summaryObject,
                "data": inspectData
            });
            staticApp.updateState();
        }
    }
}