/*
 * Zachary Cook
 *
 * Main application for running the website.
 */

var staticApp = null;



class App extends React.Component {
    /*
     * Creates the app.
     */
    constructor(props) {
        super(props);
        this.state = {
            "view": "swipe",
        };
        staticApp = this;

        this.setVisibleView = this.setVisibleView.bind(this);
    }

    /*
     * Sets the visible view.
     */
    setVisibleView(view) {
        this.setState({
            "view": view,
        })
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Render the app.
        let startPosition = (this.state.view == "swipe" ? 0 : -1);
        return <div style={{"height": "100%"}}>
            <SwipeView position={startPosition}/>
            <SignUpFormView position={startPosition + 1} currentId={this.state.currentId}/>
            <FocusBlock/>
        </div>
    }
}



// Render the website application using React.
ReactDOM.render(
    <App/>,
    document.getElementById("root")
);