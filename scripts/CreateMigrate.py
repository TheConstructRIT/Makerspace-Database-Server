"""
Zachary Cook

Helper script for creating Entity Framework database migrates.
"""

import subprocess
import testing.postgresql
import os.path

projectRootDirectory = os.path.realpath(__file__ + "/../../")
coreProjectDirectory = os.path.join(projectRootDirectory, "Construct.Core")
migrationsDirectory = os.path.join(coreProjectDirectory, "Migrations")


"""
Runs a process.
"""
def run(command, workingDirectory):
    process = subprocess.Popen(args=command, cwd=workingDirectory)
    process.wait()

"""
Ensures the dotnet ef (Entity Framework) tool is installed.
"""
def installEntityFrameworkTool():
    run(["dotnet", "tool", "install", "--global", "dotnet-ef"], projectRootDirectory)

"""
Creates a Sqlite migrate.
"""
def createSqliteMigrate(migrateName):
    # Create the migrates.
    run(["dotnet", "ef", "migrations", "add", "Sqlite" + migrateName, "--context", "SqliteContext"], coreProjectDirectory)

"""
Creates a PostgreSQL migrate.
"""
def createPostgresMigrate(migrateName):
    # Create the migrates.
    embeddedPostgres = testing.postgresql.Postgresql(port=39468)
    run(["dotnet", "ef", "migrations", "add", "Postgres" + migrateName, "--context", "PostgresContext"], coreProjectDirectory)
    embeddedPostgres.stop()

"""
Adds ExcludeFromCodeCoverage to the migrate files.
"""
def ignoreMigrationsCodeCoverage():
    # Iterate over the files.
    for folderName in os.listdir(migrationsDirectory):
        providerDirectory = os.path.join(migrationsDirectory, folderName)
        for fileName in os.listdir(providerDirectory):
            file = os.path.join(providerDirectory, fileName)

            # Read the file and ignore it if the import exists.
            if ".Designer.cs" not in fileName:
                with open(file, encoding="utf8") as fileData:
                    fileContents = fileData.read()
                    if "System.Diagnostics.CodeAnalysis" not in fileContents:
                        # Split the lines and add the import.
                        lines = fileContents.split("\n")
                        for i in range(0, len(lines)):
                            line = lines[i]
                            if "using System;" in line or "using Microsoft" in line:
                                lines.insert(i + 1, "using System.Diagnostics.CodeAnalysis;")
                                break

                        # Add the code coverage ignore attributes.
                        for i in range(len(lines) - 1, -1, -1):
                            line = lines[i]
                            if "partial class" in line:
                                lines.insert(i, "    [ExcludeFromCodeCoverage]")

                        # Write the file.
                        with open(file, "w", encoding="utf8") as newFile:
                            newFile.write("\n".join(lines))


# Run the program.
if __name__ == '__main__':
    import sys

    # Get the migrate name to use.
    migrateName = None
    if len(sys.argv) <= 1:
        migrateName = input("Name of the migrate to create not specified. Enter a name to use:\n")
    else:
        migrateName = sys.argv[1]

    # Set up the tool.
    print("Setting up Entity Framework tool (may already be installed).")
    installEntityFrameworkTool()

    # Create the migrates.
    print("Creating Sqlite migrate.")
    createSqliteMigrate(migrateName)
    createPostgresMigrate(migrateName)

    # Ignore the migrations in code coverage.
    print("Finalizing migrates.")
    ignoreMigrationsCodeCoverage()