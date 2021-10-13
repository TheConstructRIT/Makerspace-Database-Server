"""
Zachary Cook

Helper script for editing the configuration file.
"""

import os
import shutil
import subprocess

projectRootDirectory = os.path.realpath(__file__ + "/../../")
testProjectDirectory = projectRootDirectory + "/Construct.Core.Test"
buildConfigurationLocation = testProjectDirectory + "/bin/Debug/net5.0/configuration.json"
configurationLocation = projectRootDirectory + "/configuration.json"



# Run the program.
if __name__ == '__main__':
    # Create the configuration if none exists.
    # This is generated in the Construct.Core unit tests.
    if not os.path.exists(configurationLocation):
        # Run the tests.
        print("Preparing clean configuration file.")
        cleanProcess = subprocess.Popen(["dotnet", "clean"], cwd=testProjectDirectory)
        cleanProcess.wait()
        if cleanProcess.returncode != 0:
            print("Construct.Core.Test failed to clean.")
            exit(-1)
        testProcess = subprocess.Popen(["dotnet", "test"], cwd=testProjectDirectory)
        testProcess.wait()
        if testProcess.returncode != 0:
            print("Construct.Core.Test failed to run.")
            exit(-1)

        # Copy the configuration file.
        if not os.path.exists(buildConfigurationLocation):
            print("Configuration file not created from build.")
            exit(-1)
        shutil.copy(buildConfigurationLocation, configurationLocation)
        print("Configuration file copied to: " + configurationLocation)

    # Open the configuration file in the system default text editor.
    print("Opening configuration file: " + configurationLocation)
    os.startfile(configurationLocation)
    print("NOTE: Changes to the configuration will not take effect without re-deploying.")