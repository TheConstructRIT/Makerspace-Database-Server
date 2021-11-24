/*
 * Zachary Cook
 *
 * Background of the web application.
 */

var MAX_ENTRIES_PER_PAGE = 25;
var staticSummary;



var DEFAULT_ORDERS = {
    Prints: [0,"desc"],
    Users: [0,"asc"],
};
var COLUMNS = {
    Prints: [
        {
            title: "Time",
            data: "time",
            width: "60pt",
            render: function(value, type) { 
                if(type == "display"){
                    let date = new Date(value * 1000);
                    return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear() + " " + date.toLocaleTimeString("en-US");
                }
                return value;
            },
        },
        {
            title: "File Name",
            data: "filename",
        },
        {
            title: "Purpose",
            data: "purpose",
            width: "70pt",
        },
        {
            title: "Material",
            data: "material",
            width: "30pt",
        },
        {
            title: "Weight (Grams)",
            data: "weight",
            width: "30pt",
        },
        {
            title: "Cost (USD)",
            data: "cost",
            width: "30pt",
            render: function(value) {
                return new Intl.NumberFormat("en-US",{
                    style: "currency",
                    currency: "USD",
                }).format(value);
            },
        },
        {
            title: "Owed",
            data: "owed",
            width: "30pt",
            render: function(value) {
                if (value) {
                    return "Yes";
                } else {
                    return "No";
                }
            },
        },
        {
            title: "MSD Number",
            data: "billto",
            width: "40pt",
        },
        {
            title: "User",
            data: "user",
        },
    ],
    PrintsNoUsers: [
        {
            title: "Time",
            data: "time",
            width: "60pt",
            render: function(value, type) { 
                if(type == "display"){
                    let date = new Date(value * 1000);
                    return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear() + " " + date.toLocaleTimeString("en-US");
                }
                return value;
            },
        },
        {
            title: "File Name",
            data: "filename",
        },
        {
            title: "Purpose",
            data: "purpose",
            width: "70pt",
        },
        {
            title: "Material",
            data: "material",
            width: "30pt",
        },
        {
            title: "Weight (Grams)",
            data: "weight",
            width: "30pt",
        },
        {
            title: "Cost (USD)",
            data: "cost",
            width: "30pt",
            render: function(value) {
                return new Intl.NumberFormat("en-US",{
                    style: "currency",
                    currency: "USD",
                }).format(value);
            },
        },
        {
            title: "Owed",
            data: "owed",
            width: "30pt",
            render: function(value) {
                if (value) {
                    return "Yes";
                } else {
                    return "No";
                }
            },
        },
        {
            title: "MSD Number",
            data: "billto",
            width: "40pt",
        },
    ],
    Users: [
        {
            title: "Name",
            data: "name",
        },
        {
            title: "Email",
            data: "email",
        },
        {
            title: "Total Prints",
            data: "totalPrints",
            width: "30pt",
        },
        {
            title: "Total Weight",
            data: "totalWeight",
            width: "30pt",
        },
        {
            title: "Prints Owed",
            data: "totalOwedPrints",
            width: "30pt",
        },
        {
            title: "Balance Due",
            data: "totalOwedCost",
            width: "30pt",
            render: function(value) {
                return new Intl.NumberFormat("en-US",{
                    style: "currency",
                    currency: "USD",
                }).format(value);
            },
        },
    ],
};



class Summary extends React.Component {
    /*
     * Creates the summary.
     */
    constructor(props) {
        super(props);
        this.state = {
            view: "Prints",
            loading: true,
            allowInspection: true,
            currentPage: 0,
            searchTerm: "",
        };

        // Bind the functions.
        this.loadPrints = this.loadPrints.bind(this);
        this.loadUsers = this.loadUsers.bind(this);
        this.loadData = this.loadData.bind(this);
        this.clearData = this.clearData.bind(this);
        this.updateState = this.updateState.bind(this);
        this.updateTable = this.updateTable.bind(this);
        this.updateInspectValues = this.updateInspectValues.bind(this);
        this.inspect = this.inspect.bind(this);
        this.setView = this.setView.bind(this);
        this.search = this.search.bind(this);

        // Load the initial data.
        this.loadData();
        if (this.props.user == null) {
            staticSummary = this;
        }
    }

    /*
     * Force updates the state object.
     */
    updateState() {
        this.setState(this.state);
    }

    /*
     * Sets the information to view.
     */
    setView(viewName,searchTerm) {
        // Set the search term if present.
        if (searchTerm != null) {
            this.state.searchTerm = searchTerm;
        }
        
        // Set the view and remove the existing table.
        this.state.view = viewName;
        this.state.entries = [];
        this.componentWillUnmount();
        this.updateState();

        // Create the new table element.
        let newTable = document.createElement("table");
        newTable.className = "SummaryTable";
        this.refs.summaryContainer.insertBefore(newTable,this.refs.summaryContainer.firstChild)
        this.refs.summaryView = newTable;

        // Create the new table.
        this.componentDidMount();
        this.loadData();
    }

    /*
     * Invoked when the requested page changes.
     */
    pageChanged(page) {
        this.state.currentPage = page;
        this.loadData();
    }

    /*
     * Updates the prints to show.
     */
    loadPrints() {
        // Set the prints as loading.
        let currentTime = new Date().getTime();
        this.state.failed = false;
        this.state.loading = true;
        this.state.unathorized = false;
        this.state.currentlyLoading = currentTime;
        this.updateState();

        // Determine how the sorting is set up.
        let column = "time";
        let ascending = false;
        if (this.table != null) {
            var order = this.table.order();
            column = COLUMNS.Prints[order[0][0]].data;
            ascending = order[0][1] == "asc";
        }

        // Form the url parameters.
        let urlParameters = {
            session: getCookie("session"),
            maxprints: MAX_ENTRIES_PER_PAGE,
            offsetprints: Math.max(this.state.currentPage - 1, 0) * MAX_ENTRIES_PER_PAGE,
            order: column,
            ascending: ascending,
            search: this.state.searchTerm,
        };
        if (this.props.user) {
            urlParameters.hashedId = this.props.user;
        }

        // Start loading the data.
        let summaryObject = this;
        $.ajax({
            url: "/admin/prints?" + $.param(urlParameters),
            success: function(result) {
                // Return if the current loading request doesn't match.
                if (summaryObject.state.currentlyLoading != currentTime) {
                    return;
                }

                // Display an unauthorized message if there is no prints.
                if (result.prints == null) {
                    summaryObject.state.loading = false;
                    summaryObject.state.unathorized = true;
                    summaryObject.updateState();
                    return;
                }

                // Convert the result.
                summaryObject.counter.setMaxPage(Math.ceil(result.totalPrints / MAX_ENTRIES_PER_PAGE));
                summaryObject.state.entries = [];
                result.prints.forEach(function(entry) {
                    // Determine the user.
                    let user = "Unknown";
                    if (entry.user != null) {
                        user = entry.user.name + " (" + entry.user.email + ")";
                    }

                    // Add the entry.
                    summaryObject.state.entries.push({
                        id: entry.print.id,
                        time: entry.print.timestamp,
                        filename: cleanString(entry.print.name),
                        purpose: cleanString(entry.print.purpose),
                        weight: entry.print.weight,
                        material: cleanString(entry.print.material),
                        cost: entry.print.cost,
                        owed: entry.print.owed,
                        billto: cleanString(entry.print.billTo),
                        user: cleanString(user),
                    });
                });
                
                // Set the entries.
                summaryObject.state.loading = false;
                summaryObject.state.failed = false;
                summaryObject.updateState();
            },
            error: function() {
                // Return if the current loading request doesn't match.
                if (summaryObject.state.currentlyLoading != currentTime) {
                    return;
                }

                // Set the loading as failed.
                summaryObject.state.loading = false;
                summaryObject.state.failed = true;
                summaryObject.updateState();
            }
        })
    }

    /*
     * Updates the users to show.
     */
    loadUsers() {
        // Set the users as loading.
        let currentTime = new Date().getTime();
        this.state.failed = false;
        this.state.loading = true;
        this.state.unathorized = false;
        this.state.currentlyLoading = currentTime;
        this.updateState();

        // Determine the sorting.
        let column = "name";
        let ascending = true;
        if (this.table != null) {
            var order = this.table.order();
            column = COLUMNS.Users[order[0][0]].data;
            ascending = order[0][1] == "asc";
        }

        // Start loading the data.
        let summaryObject = this;
        $.ajax({
            url: "/admin/users?" + $.param({
                session: getCookie("session"),
                maxusers: MAX_ENTRIES_PER_PAGE,
                offsetusers: Math.max(this.state.currentPage - 1, 0) * MAX_ENTRIES_PER_PAGE,
                order: column,
                ascending: ascending,
                search: summaryObject.state.searchTerm,
            }),
            
            success: function(result) {
                // Return if the current loading request doesn't match.
                if (summaryObject.state.currentlyLoading != currentTime) {
                    return;
                }

                // Display an unauthorized message if there is no users.
                if (result.users == null) {
                    summaryObject.state.loading = false;
                    summaryObject.state.unathorized = true;
                    summaryObject.updateState();
                    return;
                }

                // Convert the result.
                summaryObject.counter.setMaxPage(Math.ceil(result.totalUsers / MAX_ENTRIES_PER_PAGE));
                summaryObject.state.entries = result.users;
                result.users.forEach(function(entry) {
                    entry.hashedId = cleanString(entry.hashedId);
                    entry.name = cleanString(entry.name);
                    entry.email = cleanString(entry.email);
                });
                
                // Set the entries.
                summaryObject.state.loading = false;
                summaryObject.state.failed = false;
                summaryObject.updateState();
            },
            error: function() {
                // Return if the current loading request doesn't match.
                if (summaryObject.state.currentlyLoading != currentTime) {
                    return;
                }

                // Set the loading as failed.
                summaryObject.state.loading = false;
                summaryObject.state.failed = true;
                summaryObject.updateState();
            }
        })
    }

    /*
     * Loads the data for the current view.
     */
    loadData() {
        if (this.state.view == "Prints") { 
            this.loadPrints();
        } else {
            this.loadUsers();
        }
    }

    /*
     * Clears the data for the current view.
     */
    clearData() {
        this.state.entries = [];
        this.updateState();
    }

    /*
     * Performs a search on the data.
     */
    search(searchTerm) {
        this.state.searchTerm = searchTerm;
        this.loadData();
    }

    /*
     * Inspects an entry.
     */
    inspect(entry) {
        // Return if inspection is disabled (viewing inspection).
        if (!this.state.allowInspection) {
            return;
        }

        // Set the data to inspect.
        this.state.inspectData = entry;
        this.updateState();
    }

    /*
     * Updates the values to inspect.
     */
    updateInspectValues(values) {
        // Set the values.
        for (let valueName in values) {
            this.state.inspectData[valueName] = values[valueName];
        }

        // Update the state.
        this.updateState();
    }

    /*
     * Updates the data in the table.
     */
    updateTable() {
        let summaryView = this.table;
        if (summaryView) {
            if (this.state.loading) {
                // Show that the data is loading.
                this.table.context[0].oLanguage.sEmptyTable = "Loading. Please wait.";
                this.table.clear().draw();
            } else if (this.state.unathorized) {
                // Show that the data is unauthorized.
                this.table.context[0].oLanguage.sEmptyTable = "Unauthorized. Your session may be old.";
                this.table.clear().draw();
            } else if (this.state.failed) {
                // Show that the data failed to load.
                this.table.context[0].oLanguage.sEmptyTable = "Failed to load data.";
                this.table.clear().draw();
            } else {
                // Show the data.
                this.table.context[0].oLanguage.sEmptyTable = "No entries found.";
                this.table.clear();
                if (this.state.entries.length > 0) {
                    this.table.rows.add(this.state.entries);
                }
                this.table.draw();
            }
        }
    }

    /*
     * Returns the HTML structure of the element.
     */
    render() {
        // Update the table.
        this.updateTable();

        // Update the inspect item.
        setInspect(this,this.state.inspectData);

        // Return the object.
        return <div ref="summaryContainer" class="SummaryContainer">
            <table ref="summaryView" class="SummaryTable"/>
            <PageCounter table={this}/>
        </div>
    }

    /*
     * Invoked when the component is mounted.
     */
    componentDidMount() {
        // Create the table.
        let columns = COLUMNS[this.state.view];
        if (this.props.user != null) {
            columns = COLUMNS["PrintsNoUsers"];
        }
        this.table = $(this.refs.summaryView).DataTable({
           dom: "<\"SummaryDataTableContainer\">",
           pageLength: MAX_ENTRIES_PER_PAGE,
           language: {
               "emptyTable": "<span class=\"datatables-empty-message\"></span>"
           },
           data: [],
           order: [DEFAULT_ORDERS[this.state.view]],
           columns: columns,
        });
        this.updateTable();

        // Set up clicking to inspect.
        let summaryObject = this;
        $('.dataTable').on("click","tbody tr",function() {
            // Return if there is no table (destroyed).
            if (summaryObject.table == null) {
                return;
            }

            // Inspect the row.
            summaryObject.inspect(summaryObject.table.row(this).data());
        })

        // Set up changing th order.
        let lastColumn = DEFAULT_ORDERS[this.state.view][0];
        let lastColumnOrdering = DEFAULT_ORDERS[this.state.view][1];
        $('.dataTable').on("order.dt",function () {
            // Return if there is no table (destroyed).
            if (summaryObject.table == null) {
                return;
            }

            // Return if the order didn't change.
            let order = summaryObject.table.order()[0];
            if (order[0] == lastColumn && order[1] == lastColumnOrdering) {
                return;
            }
            lastColumn = order[0]
            lastColumnOrdering = order[1];

            // Reload the data.
            summaryObject.loadData();
        } );
    }

    /*
     * Invoked when the component is about to be unmounted.
     */
    componentWillUnmount(){
        this.table.destroy(true);
        this.table = null;
    }
}