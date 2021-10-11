"""
Zachary Cook

Helper script for running services. This script is only intended to be
called by services since it does not start a background process.
"""

import os
import subprocess
from DeployImplementations import AutoDeploy


# Run the program.
if __name__ == '__main__':
    # Get the services to run.
    deployObject = AutoDeploy.getDeploy()
    servicesToRun = deployObject.getServicesFromCLI()
    if len(servicesToRun) == 0:
        print("No services were specified.")
    if len(servicesToRun) > 1:
        print("More than 1 service specified. Only the first 1 will start.")
    service = servicesToRun[0]

    # Build the service if it wasn't done already.
    if not os.path.exists(deployObject.getOutputDirectory(service)):
        deployObject.build(service)

    # Start the service.
    print("Starting " + service)
    outputDirectory = deployObject.getOutputDirectory(service)
    executable = os.path.realpath(outputDirectory + "/" + service)
    if os.path.exists(executable + ".exe"):
        executable += ".exe"
    subprocess.Popen([executable], cwd=outputDirectory, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL).wait()