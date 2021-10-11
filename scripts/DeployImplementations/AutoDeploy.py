"""
Zachary Cook

Helper for getting the correct deploy implementation for the system.
"""

from .ProcessDeploy import ProcessDeploy


"""
Returns the deploy object to use for the system.
"""
def getDeploy():
    print("Using Process Deploy method.")
    return ProcessDeploy()