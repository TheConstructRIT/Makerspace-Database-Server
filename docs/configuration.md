# Setup
This document covers the server configuration file. See the [setup](setup.md)
document for how the file can be modified and deployed.

## Default Configuration
```json
{
  "Database": {
    "Provider": "sqlite",
    "Source": null,
    "SourceDatabase": null,
    "Username": null,
    "Password": null
  },
  "Logging": {
    "ConsoleLevel": "Information"
  },
  "PrintReceipt": {
    "Provider": "GoogleAppScripts",
    "GoogleAppScriptId": "script_id"
  },
  "Email": {
    "ValidEmails": [],
    "EmailCorrections": {}
  },
  "Admin": {
    "MaximumUserSessions": 5,
    "MaximumUserSessionDuration": 3600,
    "ConfigurablePermissions": [
      "LabManager"
    ]
  },
  "Ports": {
    "Combined": 8000,
    "User": 8001,
    "Swipe": 8002,
    "Admin": 8003,
    "Compatibility": 8005
  }
}
```

## Configuration Options
The configuration is split up into sections that contain related fields.

### Database
Configuration for the database used by the server.
* `Provider (String)` - Database provider to use. Depending on the input:
  * `"sqlite"` - SQLite will be used for the database. This is *not*        
    recommended for an actual deployment.
  * `"postgres"` - PostgreSQL will be be used for the database.
* `Source (String)` - Source for the database. Depending on the `Provider`:
  * `Provider` is `"sqlite"` - `Source` will point to the file to use.
    If none is specified, `database.sqlite` will be used.
  * `Provider` is `"postgres"` - `Source` will be the host of PostgreSQL.
* `SourcePort (Integer)` - Post to connect to the host with. If not provided, the
  default port will be used as determined by Microsoft Entity Framework. *Not used
  with `sqlite`.*
* `SourceDatabase (String)` - Database to use on the host. *Not used with `sqlite`.*
* `Username (String)` - Username to connect to the database with. The user
  must have the ability to create tables and query them. *Not used with `sqlite`.*
* `Password (String)` - Password to connect to the database with. *Not
  used with `sqlite`.*

### Logging
Configuration for the logging that the server does.
* `ConsoleLevel (String)` - Minimum console log level to use in the services.
  The accepted values are: `"Trace"`, `"Debug"`, `"Information"`, `"Warning"`,
  `"Error"`, `"Critical"`, and `"None"`.

### PrintReceipt
Configuration for receipted from users who print.
* `Provider (String)` - Provider to use for sending print receipts to users.
  Currently, only `"GoogleAppScripts"` is supported, but others are planned.
  This is a legacy system that is expected to be replaced.
* `GoogleAppScriptId (String)` - If the `Provider` is `"GoogleAppScripts"`,
  this is the id of the script to use to send print receipt emails.

### Email
Configuration for the emails accepted by the registration service.
* `ValidEmails (List<String>)` - List of emails that are accepted and stored
  for registering users. For example, allowing `@rit.edu` emails would be
  `["rit.edu"]`.
* `EmailCorrections (Dictionary<String, String>)` - Mapping of emails to other
  emails for correcting inputs. At RIT, both `@mail.rit.edu` and `@g.rit.edu`
  are valid, so they can be normalized to `@rit.edu` if `EmailCorrections`
  is set to `{"mail.rit.edu": "rit.edu", "g.rit.edu": "rit.edu"}`.

### Admin
Configuration for the admin UI for server.
* `MaximumUserSessions (Integer)` - The maximum amount of sessions a user can have
  in the admin user interface. This can allow multiple computers to be logged in
  as the same user. If a new session is created and the user is at the limit, the
  oldest session will be removed.
* `MaximumUserSessionDuration (Integer)` - The maximum duration, in seconds, a session
  can be open with no activity. This is intended to let inactive sessions, such as
  lab managers who forget to log out, expire without others using them.
* `ConfigurablePermissions (List<String>)` - List of permissions to display in the
  admin UI. *Time-based permissions are supported in the database but not by the UI.**

### Ports
Configuration for the ports used by the system. *The default configuration
includes 2 ports between `"Admin"` and `"Compatibility"`. Up to 2 more
services are planned.*
* `Combined (Integer)` - Port used by the service that runs everything together.
  This is only intended for systems where connecting directly to the server is required,
  or for local testing.
* `User (Integer)` - Port used by the service for registering and fetching users.
* `Swipe (Integer)` - Port used by the service for registering user visits.
* `Admin (Integer)` - Port used for the admin UI service.
* `Compatibility (Integer)` - Port used for the service with legacy endpoints for
  legacy applications at The Construct @ RIT. **The API for this service is not
  documented and is deprecated. It will be removed in the future.**