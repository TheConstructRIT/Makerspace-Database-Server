"""
Zachary Cook

Base deploy script used for the system.
"""

import os
import shutil
import subprocess
import sys


serviceOptions = {
    "all": ["Construct.Swipe","Construct.User","Construct.Compatibility","Construct.Admin","Construct.Print"],
    "combined": ["Construct.Combined"],
    "compatibility": ["Construct.Compatibility"],
    "swipe": ["Construct.Swipe"],
    "user": ["Construct.User"],
    "admin": ["Construct.Admin"],
    "print": ["Construct.Print"],
}
requiredTests = {
    "Construct.Combined": ["Construct.Core.Test","Construct.Combined.Test","Construct.User.Test","Construct.Swipe.Test","Construct.Admin.Test","Construct.Compatibility.Test"],
    "Construct.User": ["Construct.Core.Test","Construct.User.Test"],
    "Construct.Swipe": ["Construct.Core.Test","Construct.Swipe.Test"],
    "Construct.Admin": ["Construct.Core.Test","Construct.Admin.Test"],
    "Construct.Print": ["Construct.Core.Test","Construct.Print.Test"],
    "Construct.Compatibility": ["Construct.Core.Test","Construct.Compatibility.Test"],
}


class BaseDeploy:
    """
    Creates the base deploy class.
    """
    def __init__(self):
        self.projectRootDirectory = os.path.realpath(__file__ + "/../../../")

    """
    Returns the output directory for a service.
    """
    def getOutputDirectory(self, serviceName):
        return os.path.realpath(self.projectRootDirectory + "/bin/" + serviceName)

    """
    Finds the configuration file and returns the path. Returns
    if none exists.
    """
    def findConfiguration(self):
        # Go up the directory until the configuration is found.
        parentDirectory = os.path.realpath(__file__ + "/../")
        for i in range(0, 10):
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
    Verifies a project for deployment, such as running unit tests.
    An exception will be thrown if the verification fails.
    """
    def verify(self, projectName):
        # Run the tests for the project.
        projectTestDirectory = os.path.realpath(self.projectRootDirectory + "/" + projectName)
        testProcess = subprocess.Popen(["dotnet", "test"], cwd=projectTestDirectory)

        # Throw an exception if the tests fail.
        unitTestsExitCode = testProcess.wait()
        if unitTestsExitCode != 0:
            raise AssertionError("Unit tests returned a non-zero exit code: " + str(unitTestsExitCode))

    """
    Stops an existing app and deploys the new version.
    """
    def deploy(self, serviceName):
        # Stop the existing service.
        self.stop(serviceName)

        # Build the new service.
        self.build(serviceName)

        # Start the service.
        self.start(serviceName)

    """
    Returns the services to deploy from the CLI arguments.
    """
    def getServicesFromCLI(self):
        # Get the services to deploy.
        services = sys.argv[1:]
        if len(sys.argv) <= 1:
            print("No services were specified. One or multiple options must be specified.")
            print("\tValid services: " + ",".join(serviceOptions.keys()))
            services = input("Enter the services to deploy:\n").split(" ")

        # Compile all the service options.
        # This is done for services that call to start the service names directly.
        allServiceOptions = {}
        for optionName in serviceOptions.keys():
            allServiceOptions[optionName] = serviceOptions[optionName]
            for serviceName in serviceOptions[optionName]:
                if serviceName.lower() not in allServiceOptions.keys():
                    allServiceOptions[serviceName.lower()] = [serviceName]

        # Determine the invalid options and exit if there are invalid options.
        invalidOptions = []
        for option in services:
            if option.lower() not in allServiceOptions.keys():
                invalidOptions.append(option)
        if len(invalidOptions) > 0:
            print("Invalid services specified: " + " ".join(invalidOptions))
            print("\tValid services: " + ",".join(allServiceOptions.keys()))
            exit(-1)

        # Determine the services to deploy.
        servicesToDeploy = []
        for option in services:
            for service in allServiceOptions[option.lower()]:
                if service not in servicesToDeploy:
                    servicesToDeploy.append(service)

        # Find the configuration and warn if none exists.
        if self.findConfiguration() is None:
            print("Configuration.json not found in the scripts or parent directories. The default will be used.")

        # Return the services.
        return servicesToDeploy

    """
    Builds a service.
    """
    def build(self, serviceName):
        # Build the new service.
        print("Building " + serviceName)
        projectDirectory = os.path.realpath(self.projectRootDirectory + "/" + serviceName)
        outputDirectory = self.getOutputDirectory(serviceName)
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
        configurationPath = self.findConfiguration()
        if configurationPath is not None:
            print("Copying configuration file.")
            shutil.copy(configurationPath, newConfiguration)

    """
    Stops a service.
    """
    def stop(self, serviceName):
        pass

    """
    Starts a service.
    """
    def start(self, serviceName):
        pass