/*
 * Zachary Cook
 *
 * Cover for when the browser isn't focused.
 */

class FocusBlock extends React.Component {
    /*
     * Creates the focus block.
     */
    constructor(props) {
        super(props);
        this.state = {
            "focused": true,
        };

        // Listen to the page gaining and losing focus.
        let focusBlock = this;
        window.onfocus = function() {
            focusBlock.setFocused(true);
        };
        window.onblur = function() {
            focusBlock.setFocused(false);
        };

        // Wrap the methods.
        this.setFocused = this.setFocused.bind(this);
    }

    /*
     * Sets the page as focused.
     */
    setFocused(focused) {
        this.state.focused = focused;
        this.setState(this.state);
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Render the block.
        return <div class={this.state.focused ? "FocusBlock FocusBlockTransparent" : "FocusBlock FocusBlockOpague"}>
            <span class={this.state.focused ? "FocusBlockText FocusBlockTextTransparent" : "FocusBlockText FocusBlockTextOpague"}>Browser is not focused.</span>
        </div>
    }
}