/*
 * Zachary Cook
 *
 * Shows information about a user or print.
 */

class InspectInput extends React.Component {
    /*
     * Creates the swipe component.
     */
    constructor(props) {
        super(props);
        this.state = {
            valid: true,
        };

        // Bind the function.
        this.isValid = this.isValid.bind(this);
        this.getValue = this.getValue.bind(this);
        this.toggleModify = this.toggleModify.bind(this);
        this.onInputChanged = this.onInputChanged.bind(this);
    }

    /*
     * Returns if the input is valid.
     */
    isValid(value) {
        // Return if the type is a bool or no type is specified.
        if (this.props.type == "Bool" || this.props.type == null) {
            return true;
        }

        // Validate the input.
        if (this.props.type == "MSDNumber") {
            const re = /P[0-9]{5,5}/;
            return value == "" || re.test(String(value));
        } else if (this.props.type == "Integer") {
            return String(parseFloat(value)) == value;
        } else if (this.props.type == "Email") {
            const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(String(value).toLowerCase());
        }

        // Return true (no validator).
        return true;
    }

    /*
     * Returns the current value.
     */
    getValue() {
        if (this.props.type == "Bool") {
            return this.refs.inputBox.checked;
        } else {
            return this.refs.inputBox.value;
        }
    }

    /*
     * Toggles modifying information.
     */
    toggleModify() {
        this.state.modifying = !this.state.modifying;
        this.setState(this.state);
    }

    /*
     * Invoked when the input changes.
     */
    onInputChanged(event) {
        this.props.value = event.target.value;
        this.state.valid = this.isValid(event.target.value);
        this.setState(this.state);
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Return if modifying is disabled.
        if (!this.props.modify) {
            return <div class={this.props.class}>
                <b>{this.props.name + ": "}</b>
                {(this.props.value == "" ? "(None)" : this.props.value)}
            </div>
        }

        // Return the input.
        let classes = "InspectModifyInput";
        if (!this.state.valid) {
            classes += " InspectInputInvalid";
        }
        if (this.props.type == "Bool") {
            if (this.props.value == "Yes") {
                return <div class={this.props.class}>
                    <b>{this.props.name + ": "}</b>
                    <input class={classes} type="checkbox" ref="inputBox" onChange={this.onInputChanged} checked></input>
                </div>
            } else {
                return <div class={this.props.class}>
                    <b>{this.props.name + ": "}</b>
                    <input class={classes} type="checkbox" ref="inputBox" onChange={this.onInputChanged}></input>
                </div>
            }
        } else {
            return <div class={this.props.class}>
                <b>{this.props.name + ": "}</b>
                <input class={classes} type="text" ref="inputBox" value={this.props.value} onChange={this.onInputChanged}></input>
            </div>
        }
    }
}