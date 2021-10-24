/*
 * Zachary Cook
 *
 * Main application for running the website.
 */

var inspectItems = [];
var staticApp = null;



class App extends React.Component {
    /*
     * Creates the app.
     */
    constructor(props) {
        super(props);
        this.state = {};
        staticApp = this;

        this.updateState = this.updateState.bind(this);
    }

    /*
     * Updates the state.
     */
    updateState() {
        this.setState(this.state);
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Convert the inspect items.
        let inspectElements = [];
        for (let i = 0; i < inspectItems.length; i++) {
            let data = inspectItems[i];
            inspectElements.push(<Inspect summary={data.summary} data={data.data}/>)
        }

        // Render the app.
        return <div style={{"height": "100%"}}>
            <TopBar/>
            <Summary/>
            <BottomBar/>
            {inspectElements}
            <Swipe/>
        </div>
    }
}



// Render the website application using React.
ReactDOM.render(
    <App/>,
    document.getElementById("root")
);