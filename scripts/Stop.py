"""
Zachary Cook

Helper script for stopping services.
"""

from DeployImplementations import AutoDeploy


# Run the program.
if __name__ == '__main__':
    # Get the services to stop.
    deployObject = AutoDeploy.getDeploy()
    servicesToDeploy = deployObject.getServicesFromCLI()

    # Stop the services.
    for service in servicesToDeploy:
        deployObject.stop(service)