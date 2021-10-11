"""
Zachary Cook

Helper script for deploying services.
"""

from DeployImplementations import AutoDeploy


# Run the program.
if __name__ == '__main__':
    # Get the services to deploy.
    deployObject = AutoDeploy.getDeploy()
    servicesToDeploy = deployObject.getServicesFromCLI()

    # Deploy the services.
    for service in servicesToDeploy:
        print("Deploying " + service)
        deployObject.deploy(service)