[![Build](https://github.com/TheConstructRIT/Makerspace-Database-Server/actions/workflows/dotnet-code-coverage.yml/badge.svg)](https://github.com/TheConstructRIT/Makerspace-Database-Server/actions/workflows/dotnet-code-coverage.yml) [![Coverage Status](https://coveralls.io/repos/github/TheConstructRIT/Makerspace-Database-Server/badge.svg?branch=master)](https://coveralls.io/github/TheConstructRIT/Makerspace-Database-Server?branch=master)

# Makerspace Database Server
The Makerspace Database Server is the central server used by
[The Construct @ RIT](https://hack.rit.edu/) for managing
users and prints. It replaces the existing Construct Database
Server (closed-source).

# Running
For development and production systems, [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
is required for building and testing the application.
[Python 3](https://www.python.org/) is required for any
of the helper scripts, which may be used while developing or
deploying.

## Development
For development, `Construct.Combined` is intended to be run
with everything together with 1 port. A script can be used to
run it, but it doesn't have any benefits over running in
an IDE like Microsoft Visual Studio or JetBrains Rider.

For the command line, `dotnet` can quickly be used to build it.
```bash
cd Construct.Combined
dotnet run
```

## Deployment
See the [setup document](docs/setup.md).

# Docs
Documentation for the project can be found in the [docs](docs)
directory. For the API intended to be used by other applications,
see the [API specification](https://petstore.swagger.io/?url=https%3A%2F%2Fraw.githubusercontent.com%2FTheConstructRIT%2FMakerspace-Database-Server%2Fmaster%2Fdocs%2Fapi.yaml).

# License
This project uses the [MIT License](LICENSE).