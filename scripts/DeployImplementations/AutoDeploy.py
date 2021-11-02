"""
Zachary Cook

Helper for getting the correct deploy implementation for the system.
"""

import os
from .ProcessDeploy import ProcessDeploy
from .SystemdDeploy import SystemdDeploy


"""
Returns the deploy object to use for the system.
"""
def getDeploy():
    if os.path.exists("/etc/systemd/system"):
        print("Using Systemd Deploy method.")
        return SystemdDeploy()
    else:
        print("Using Process Deploy method.")
        return ProcessDeploy()