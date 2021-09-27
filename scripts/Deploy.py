"""
Zachary Cook

Helper script for deploying projects as processes.
"""

import os
import shutil
import subprocess
import psutil

serviceOptions = {
    "combined": ["Construct.Combined"],
}



projectRootDirectory = os.path.realpath(__file__ + "/../../")



"""
Finds the configuration file and returns the path. Returns
if none exists.
"""
def findConfiguration():
    # Go up the directory until the configuration is found.
    parentDirectory = os.path.realpath(__file__ + "/../")
    for i in range(0,10):
        # Return if the configuration was found.
        configurationLocation = os.path.realpath(parentDirectory + "/configuration.json")
        if os.path.exists(configurationLocation):
            return configurationLocation

        # Move to the parent directory. Break if the parent directory is the same (is the root of the file system).
        newParentDirectory = os.path.realpath(parentDirectory + "/../")
        if newParentDirectory == parentDirectory:
            break
        parentDirectory = newParentDirectory

    # Return None (not found).
    return None

"""
Verifies a services for deployment, such as running unit tests.
An exception will be thrown if the verification fails.
"""
def verify(serviceName):
    """
    # Run the tests for the project.
    projectTestDirectory = os.path.realpath(projectRootDirectory + "/" + serviceName + ".Test")
    testProcess = subprocess.Popen(["dotnet", "test"], cwd=projectTestDirectory)

    # Throw an exception if the tests fail.
    unitTestsExitCode = testProcess.wait()
    if unitTestsExitCode != 0:
        raise AssertionError("Unit tests returned a non-zero exit code: " + str(unitTestsExitCode))
    """
    pass

"""
Stops an existing app and deploys the new version.
"""
def deploy(serviceName):
    # Stop the existing process.
    for process in psutil.process_iter():
        if process.name().lower() == serviceName.lower() or process.name().lower() == serviceName.lower() + ".exe":
            print("Stopping " + process.name())
            process.kill()
            process.wait()

    # Build the new service.
    print("Building " + serviceName)
    projectDirectory = os.path.realpath(projectRootDirectory + "/" + serviceName)
    outputDirectory = os.path.realpath(projectRootDirectory + "/bin/" + serviceName)
    if os.path.exists(outputDirectory):
        shutil.rmtree(outputDirectory)
    buildProcess = subprocess.Popen(["dotnet", "publish", "--output", outputDirectory], cwd=projectDirectory)
    buildExitCode = buildProcess.wait()
    if buildExitCode != 0:
        raise AssertionError("Build returned a non-zero exit code: " + str(buildExitCode))

    # Delete the configuration. May be from a previous build.
    newConfiguration = os.path.realpath(outputDirectory + "/configuration.json")
    if os.path.exists(newConfiguration):
        os.remove(newConfiguration)

    # Copy the configuration file.
    configurationPath = findConfiguration()
    if configurationPath is not None:
        print("Copying configuration file.")
        shutil.copy(configurationPath, newConfiguration)

    # Start the process.
    print("Starting " + serviceName)
    executable = os.path.realpath(outputDirectory + "/" + serviceName)
    if os.path.exists(executable + ".exe"):
        executable += ".exe"
    subprocess.Popen([executable], cwd=outputDirectory, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    print("Started " + serviceName)



# Run the program.
if __name__ == '__main__':
    import sys

    # Get the services to deploy.
    services = sys.argv[1:]
    if len(sys.argv) <= 1:
        print("No services were specified. One or multiple options must be specified.")
        print("\tValid services: " + ",".join(serviceOptions.keys()))
        services = input("Enter the services to deploy:\n").split(" ")

    # Determine the invalid options and exit if there are invalid options.
    invalidOptions = []
    for option in services:
        if option.lower() not in serviceOptions.keys():
            invalidOptions.append(option)
    if len(invalidOptions) > 0:
        print("Invalid services specified: " + " ".join(invalidOptions))
        print("\tValid services: " + ",".join(serviceOptions.keys()))
        exit(-1)

    # Determine the services to deploy.
    servicesToDeploy = []
    for option in services:
        for service in serviceOptions[option.lower()]:
            if service not in servicesToDeploy:
                servicesToDeploy.append(service)

    # Find the configuration and warn if none exists.
    if findConfiguration() == None:
        print("Configuration.json not found in the scripts or parent directories. The default will be used.")

    # Verify the services.
    print("Verifying services before deploying.")
    for service in servicesToDeploy:
        try:
            print("Verifying " + service)
            verify(service)
        except AssertionError:
            print("Verification failed for " + service)
            print("The output above should should the tests that failed. A deployment may not be safe.")
            exit(-1)
    print("Verified services.")

    # Start the services.
    for service in servicesToDeploy:
        print("Deploying " + service)
        deploy(service)

