/*
 * Zachary Cook
 *
 * Form for signing up with the system.
 */

class SignUpFormView extends React.Component {
    /*
     * Creates the sign-up form view.
     */
    constructor(props) {
        super(props);
        this.state = {
            "currentId": "",
            "status": "open",
        };

        // Wrap the methods.
        this.formChanged = this.formChanged.bind(this);
        this.submitForm = this.submitForm.bind(this);
    }

    /*
     * Invoked when the form changes.
     */
    formChanged(event) {
        // Store the value in the state.
        this.state[event.target.name] = event.target.value;

        // Determine if the form is valid.
        this.state.ready = true;
        if (this.state.name.indexOf(" ") < 0) {
            this.state.ready = false;
        }
        if (this.state.email.toLowerCase().indexOf("@rit.edu") < 0 && this.state.email.toLowerCase().indexOf("@g.rit.edu") < 0 && this.state.email.toLowerCase().indexOf("@mail.rit.edu") < 0) {
            this.state.ready = false;
        }
        if (this.state.college == "" || this.state.year == "") {
            this.state.ready = false;
        }

        // Set the state to update the element.
        this.setState(this.state);
    }

    /*
     * Submits the form. 
     */
    submitForm() {
        // Correct the email.
        if (this.state.email.toLowerCase().indexOf("@g.rit.edu") < 0) {
            this.state.email = this.state.email.replace("@g.rit.edu", "@rit.edu");
        }
        if (this.state.email.toLowerCase().indexOf("@mail.rit.edu") < 0) {
            this.state.email = this.state.email.replace("@mail.rit.edu", "@rit.edu");
        }

        // Start submitting the form.
        let form = this;
        this.state.status = "inprogress";
        this.setState(this.state);
        $.ajax({
            type: "POST",
            url: "/user/register",
            data: JSON.stringify({ 
                hashedId: this.state.currentId,
                name: this.state.name,
                email: this.state.email,
                college: this.state.college,
                year: this.state.year, 
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            success: function(data){
                form.state.status = "submitted";
                form.setState(form.state);
                staticApp.setVisibleView("swipe");
            },
            error: function(data) {
                form.state.status = "error";
                form.setState(form.state);
            }
        });
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Reset the form if the id changed.
        if (staticApp.state.currentId != this.state.currentId) {
            this.state.currentId = staticApp.state.currentId;
            this.state.status = "open";
            this.state.name = "";
            this.state.email = "";
            this.state.college = "";
            this.state.year = "";
            this.state.ready = false;
        }

        // Determine the button.
        let button = <button class="SignUpButton" style={{top: "380px"}} type="button" onClick={this.submitForm}>Submit</button>;
        if (this.state.status == "inprogress") {
            button = <span class="SignUpFormText" style={{top: "380px", "font-size": "30px"}}>Please wait...</span>
        } else if (this.state.status == "submitted") {
            button = <span class="SignUpFormText" style={{top: "380px", "font-size": "30px"}}>Done</span>
        } else if (!this.state.ready) {
            button = <button class="SignUpButton" style={{top: "380px"}} type="button" disabled>Submit</button>;
        }

        // Render the app.
        return <div class="View" style={{left: (this.props.position * 100) + "%"}}>
            <span class="SignUpFormText" style={{top: "10px", "font-size": "30px"}}>Welcome to The Construct!</span>
            <span class="SignUpFormText" style={{top: "50px", "font-size": "24px"}}>Name (First and Last)</span>
            <input class="SignUpTextBox" style={{top: "84px"}} type="text" name="name" value={this.state.name} onChange={this.formChanged}></input>
            <span class="SignUpFormText" style={{top: "120px", "font-size": "24px"}}>RIT Email (abcd1234@rit.edu)</span>
            <input class="SignUpTextBox" style={{top: "154px"}} type="text" name="email" value={this.state.email} onChange={this.formChanged}></input>
            <span class="SignUpFormText" style={{top: "190px", "font-size": "24px"}}>Which college are you part of?</span>
            <select class="SignUpDropDown" style={{top: "224px"}} name="college" value={this.state.college} onChange={this.formChanged}>
                <option value="">(Please select)</option>
                <option value="KGCOE">KGCOE</option>
                <option value="CET">CET</option>
                <option value="Saunders College of Business">Saunders College of Business</option>
                <option value="GCCIS">GCCIS</option>
                <option value="College of Health Sciences and Tech.">College of Health Sciences and Tech.</option>
                <option value="CAD">CAD</option>
                <option value="COLA">COLA</option>
                <option value="NTID">NTID</option>
                <option value="College of Science">College of Science</option>
                <option value="Other">Other</option>
            </select>
            <span class="SignUpFormText" style={{top: "260px", "font-size": "24px"}}>What year are you?</span>
            <select class="SignUpDropDown" style={{top: "294px"}} name="year" value={this.state.year} onChange={this.formChanged}>
                <option value="">(Please select)</option>
                <option value="First Year">First Year</option>
                <option value="Second Year">Second Year</option>
                <option value="Third Year">Third Year</option>
                <option value="Fourth Year">Fourth Year</option>
                <option value="Fifth Year">Fifth Year</option>
                <option value="PHD">PHD</option>
                <option value="Faculty/Staff">Faculty/Staff</option>
                <option value="Alumni">Alumni</option>
            </select>
            <span class="SignUpFormText" style={{top: "350px", "font-size": "18px"}}>By submitting, you acknowledge that have read and understand RIT's lab safety policies.</span>
            {button}
        </div>
    }
}