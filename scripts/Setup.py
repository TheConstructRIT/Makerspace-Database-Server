"""
Zachary Cook

Standalone script for setting up the server.
"""

import os
import subprocess
import sys


repositoryUrl = "https://github.com/TheConstructRIT/Makerspace-Database-Server.git"
commands = [
    {
        "command": "deploy",
        "script": "Deploy.py",
        "arguments": "[service1] [service2] [...]",
        "description": "Stops, rebuilds, and deploys a list of services.",
    },
    {
        "command": "start",
        "script": "Start.py",
        "arguments": "[service1] [service2] [...]",
        "description": "Stops and starts a list of services without rebuilding.",
    },
    {
        "command": "stop",
        "script": "Stop.py",
        "arguments": "[service1] [service2] [...]",
        "description": "Stops a list of services.",
    },
]


"""
Checks for a required program. Stops the program
if the program doesn't exist.
"""
def checkRequirement(program):
    # Split the path.
    pathString = os.environ["PATH"]
    if ";" in pathString:
        paths = os.environ["PATH"].split(";")
    else:
        paths = os.environ["PATH"].split(":")

    # Return if the program exists.
    for path in paths:
        if os.path.exists(os.path.join(path, program)) or os.path.exists(os.path.join(path, program) + ".exe"):
            return

    # Exit if the program doesn't exist.
    print("Program not found in the system PATH: " + program)
    exit(-1)

def displayCommands():
    print("\tupdate - Pulls the latest code for the server, but does not re-deploy.")
    for command in commands:
        print("\t" + command["command"] + " " + command["arguments"] + " - " + command["description"])


# Run the program.
if __name__ == '__main__':
    arguments = sys.argv[1:]

    # Check the requirements.
    checkRequirement("git")
    checkRequirement("dotnet")

    # Determine the directory.
    if os.path.exists("/etc"):
        repositoryDirectory = "/etc/construct-database"
    else:
        repositoryDirectory = "C:/Program Files/Construct-Database"

    # Prepare the code.
    if not os.path.exists(repositoryDirectory):
        # Pull the server code.
        print("Pulling server code.")
        gitCloneProcess = subprocess.Popen(["git", "clone", repositoryUrl, repositoryDirectory])
        gitCloneProcess.wait()
        if gitCloneProcess.returncode != 0:
            print("Git clone failed. Elevated permissions may be required.")
            exit(-1)

        # Prepare the Python requirements.
        print("Setting up script requirements.")
        requirementsFile = repositoryDirectory + "/scripts/requirements.txt"
        pipProcess = subprocess.Popen([sys.executable, "-m", "pip", "install", "-r", requirementsFile])
        pipProcess.wait()
        if pipProcess.returncode != 0:
            print("Python requirements install failed. This will not be attempted again.")
            print("See the requirements in " + requirementsFile)
            exit(-1)

    # Return if no arguments were specified.
    if len(arguments) == 0:
        print("No arguments specified. Options:")
        displayCommands()
        exit(-1)

    # Perform the update command.
    commandName = arguments[0].lower()
    if commandName == "update":
        print("Updating the code.")
        gitPullProcess = subprocess.Popen(["git", "pull", "origin", "master"], cwd=repositoryDirectory)
        gitPullProcess.wait()
        if gitPullProcess.returncode != 0:
            print("Git pull failed. Elevated permissions may be required.")
            exit(-1)
        exit(0)

    # Get the command to run.
    command = None
    for commandOption in commands:
        if commandOption["command"] == commandName:
            command = commandOption
            break
    if command is None:
        print("Command \"" + commandName + "\" not found. Options:")
        displayCommands()
        exit(-1)

    # Run the command.
    print("Performing " + command["command"])
    scriptArguments = [sys.executable, command["script"]]
    scriptArguments.extend(arguments[1:])
    scriptProcess = subprocess.Popen(scriptArguments, cwd=repositoryDirectory + "/scripts")
    scriptProcess.wait()
    exit(scriptProcess.returncode)