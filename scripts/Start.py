"""
Zachary Cook

Helper script for starting services.
"""

import os
from DeployImplementations import AutoDeploy


# Run the program.
if __name__ == '__main__':
    # Get the services to start.
    deployObject = AutoDeploy.getDeploy()
    servicesToDeploy = deployObject.getServicesFromCLI()

    # Start the services.
    for service in servicesToDeploy:
        # Stop the service.
        deployObject.stop(service)

        # Build the service if it wasn't done already.
        if not os.path.exists(deployObject.getOutputDirectory(service)):
            deployObject.build(service)

        # Start the service.
        deployObject.start(service)