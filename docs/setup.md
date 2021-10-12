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
(TODO: Add PostgreSQL support and write)

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
### `python3 Setup.py update`
(TODO: Write)

### `python3 setup.py deploy [service1] [service2] [...]`
(TODO: Write)

### `python3 setup.py start [service1] [service2] [...]`
(TODO: Write)

### `python3 setup.py stop [service1] [service2] [...]`
(TODO: Write)

### `python3 setup.py config`
(TODO: Add command and write)