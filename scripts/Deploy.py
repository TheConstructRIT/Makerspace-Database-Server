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

    # Verify the services.
    for service in servicesToDeploy:
        try:
            print("Verifying " + service)
            deployObject.verify(service)
        except AssertionError:
            print("Verification failed for " + service)
            print("The output above should should the tests that failed. A deployment may not be safe.")
            exit(-1)
    print("Verified services.")

    # Deploy the services.
    for service in servicesToDeploy:
        print("Deploying " + service)
        deployObject.deploy(service)