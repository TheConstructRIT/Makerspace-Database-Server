"""
Zachary Cook

Helper script for deploying services.
"""

from DeployImplementations import AutoDeploy
from DeployImplementations import BaseDeploy


# Run the program.
if __name__ == '__main__':
    # Get the services to deploy.
    deployObject = AutoDeploy.getDeploy()
    servicesToDeploy = deployObject.getServicesFromCLI()

    # Determine the projects to verify.
    projectsToTest = []
    for service in servicesToDeploy:
        if service in BaseDeploy.requiredTests.keys():
            for project in BaseDeploy.requiredTests[service]:
                if project not in projectsToTest:
                    projectsToTest.append(project)

    # Verify the services.
    for project in projectsToTest:
        try:
            print("Verifying " + project)
            deployObject.verify(project)
        except AssertionError:
            print("Verification failed for " + project)
            print("The output above should should the tests that failed. A deployment may not be safe.")
            exit(-1)
    print("Verified services.")

    # Deploy the services.
    for service in servicesToDeploy:
        print("Deploying " + service)
        deployObject.deploy(service)