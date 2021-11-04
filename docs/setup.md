# Setup
This document covers using the [`Setup.py`](../scripts/Setup.py) script to
deploy and update the server code.

## Pre-Requisites
### Services: systemd
At this point, the deploy scripts only reliably work with systemd on Linux.
A new script can be added to the [`DeploymentImplementations`](../scripts/DeployImplementations/)
folder to support other services, such as the
Non-Sucking Service Manager for use with Windows. There is a backup for
deploying as processes, but they are not recommended for actual deployments
because it doesn't support automatic restarts.

### Packages
Before the script can be used, some additional packages. The specific command
to run depends on the distribution of the environment. From testing, `dotnet-sdk` and
`aspnet-runtime` are required for building the code, running
the tests before deploying, and running the applications, `python-pip` or `python3-pip`
is required to additional Python requirements setup in the `Setup.py` script,
and `git` is required for pulling code.

Debian-based distributions (like Ubuntu):
```bash
sudo apt install -y git python3-pip
```
For `dotnet-sdk-5.0` and `aspnetcore-runtime-5.0`, see Microsoft's documentation for your
version. ([Ubuntu](https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu), 
[Debian](https://docs.microsoft.com/en-us/dotnet/core/install/linux-debian))


Arch-based distributions (like Manjaro):
```bash
sudo pacman -yS git dotnet-sdk aspnet-runtime python-pip
```

CentOS-based distributions (like RedHat Enterprise Linux):
```bash
sudo dnf install -y git python3-pip dotnet-sdk-5.0 aspnetcore-runtime-5.0
```

### Database
By default, SQLite is used as a database, but is only intended for test environments.
PostgreSQL is the only supported database providers, but others like MySQL can be added
if Microsoft Entity Framework supports it. For PostgreSQL, version 9 and older result in
errors on starting, with newer versions recommended for features and security updates.
For non-SQLite database management systems, a user must exist (the default root account
is NOT recommended) and a database much be created with table creation and query
permissions granted to that user (all is recommended to reduce headaches with setup and
changes).  If a database is not created with the database management system or the account
has incorrect permissions, the server will fail to start.

### Setup Script
The setup script can be ran independently of the rest of the code since it manages
downloading and updating the code. `wget` can be used to download the file anywhere
where the permissions are sufficient. Be aware `sudo` may be required when running
most of the actions in the script because of where the files get downloaded and the
interactions with `systemd`.
```bash
wget https://raw.githubusercontent.com/TheConstructRIT/Makerspace-Database-Server/master/scripts/Setup.py
```

Be aware the script does not auto-update with the code.

## Script Usage
`sudo` may be required in most cases because of the file permissions of the `/etc/`
directory and interactions with `systemctl`.

### `python3 Setup.py update`
Updates the files of the database server *without redeploying*. Updating the files
with the server code running is supported.

### `python3 Setup.py config`
Creates the default `configuration.json` file if none exists, then opens the system's
default text editor. On Linux systems, make sure that `xdg-open` is configured to open
a text editor (the text editor to use is deployment-specific) instead of a web browser.
After the configuration is saved, `Setup.py deploy` must be ran to apply the changes
since the applications don't  re-read the configuration, and the file they read is a
copied version.

### `python3 Setup.py deploy (service1) (service2) (...)`
*If no services are specified, the options for services will be displayed and the
services to deploy will be prompted.*

Tests the specified services, stops the specified services, builds the specified services,
and starts the specified services in that order. If a test fails for one of the specified
services, none of the specified services will be stopped, built, or started as it is
considered unsafe to deploy. On `systemd` services, the services will be "enabled"
(will automatically start on reboot).

### `python3 Setup.py start (service1) (service2) (...)`
*If no services are specified, the options for services will be displayed and the
services to deploy will be prompted.*

Stops and starts the specified services without re-testing or re-building. On `systemd`
services, the services will be "enabled" (will automatically start on reboot).

### `python3 Setup.py stop (service1) (service2) (...)`
*If no services are specified, the options for services will be displayed and the
services to deploy will be prompted.*

Stops the specified services. On `systemd` services, the services will be "disabled"
(will not automatically start on reboot).