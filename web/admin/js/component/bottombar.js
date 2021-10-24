/*
 * Zachary Cook
 *
 *  Displays information at the bottom of the page.
 */

class BottomBar extends React.Component {
    /*
     * Creates the swipe component.
     */
    constructor(props) {
        super(props);
        this.state = {
            downloadState: "Export Data"
        };

        // Bind the functions.
        this.startDownload = this.startDownload.bind(this);
    }

    /*
     * Starts downloading the CSVs.
     */
    startDownload() {
        // Return if the data is downloading.
        if (this.state.downloadState == "Download Started") {
            return;
        }
        this.state.downloadState = "Download Started";
        this.setState(this.state);

        // Download the data.
        var link = document.createElement("a");
        link.href = "/admin/csvs?" + $.param({
            "session": getCookie("session"),
        });
        link.download = "CSVs.zip";
        link.click();
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Return the bottom bar.
        return <div class="BottomBarContainer">
            <button onClick={this.startDownload}>{this.state.downloadState}</button>
        </div>
    }
}