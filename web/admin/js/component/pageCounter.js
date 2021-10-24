/*
 * Zachary Cook
 *
 * Allows iterating through pages.
 */

class PageCounter extends React.Component {
    /*
     * Creates the swipe component.
     */
    constructor(props) {
        super(props);
        props.table.counter = this;
        this.lastPage = 1;
        this.state = {
            page: 1,
            maxPage: 10,
        };

        // Bind the functions.
        this.setPage = this.setPage.bind(this);
        this.setMaxPage = this.setMaxPage.bind(this);
        this.onPageDown = this.onPageDown.bind(this);
        this.onPageUp = this.onPageUp.bind(this);
    }

    /*
     * Sets the current page.
     */
    setPage(page) {
        // Set the page.
        this.state.page = Math.max(1,Math.min(page,this.state.maxPage));
        this.setState(this.state);

        // Invoke the table function if the page changed.
        if (this.lastPage != this.state.page) {
            this.props.table.pageChanged(this.state.page);
            this.lastPage = page;
        }
    }

    /*
     * Sets the max page.
     */
    setMaxPage(page) {
        this.state.maxPage = Math.max(1,page);
        this.setPage(this.state.page);
    }

    /*
     * Invoked when the decreasing the page is invoked.
     */
    onPageDown(event) {
        this.setPage(this.state.page - 1);
    }

    /*
     * Invoked when the decreasing the page is invoked.
     */
    onPageUp(event) {
        this.setPage(this.state.page + 1);
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        return <center>
            <div class="PageCounter">
                <button class="PageCounterButton" disabled={this.state.page == 1} onClick={this.onPageDown}>&lt;</button>
                <span class="PageCounterLabel">{ this.state.page + "/" + this.state.maxPage }</span>
                <button class="PageCounterButton" disabled={this.state.page == this.state.maxPage} onClick={this.onPageUp}>&gt;</button>
            </div>
        </center>
    }
}