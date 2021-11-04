"""
Zachary Cook

Helper for getting the correct deploy implementation for the system.
"""

import os


"""
Returns the deploy object to use for the system.
"""
def getDeploy():
    if os.path.exists("/lib/systemd/system"):
        print("Using Systemd Deploy method.")
        from .SystemdDeploy import SystemdDeploy
        return SystemdDeploy()
    else:
        print("Using Process Deploy method.")
        from .ProcessDeploy import ProcessDeploy
        return ProcessDeploy()