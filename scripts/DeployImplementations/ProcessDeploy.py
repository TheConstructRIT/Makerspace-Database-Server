"""
Zachary Cook

Deploys services as Processes. Only intended for local
testing as there is no auto-restarts.
"""

import os
import psutil
import subprocess
from .BaseDeploy import BaseDeploy


class ProcessDeploy(BaseDeploy):
    """
    Stops a service.
    """
    def stop(self, serviceName):
        for process in psutil.process_iter():
            if process.name().lower() == serviceName.lower() or process.name().lower() == serviceName.lower() + ".exe":
                print("Stopping " + process.name())
                process.kill()
                process.wait()
                print("Stopped " + process.name())

    """
    Starts a service.
    """
    def start(self, serviceName):
        print("Starting " + serviceName)
        outputDirectory = self.getOutputDirectory(serviceName)
        executable = os.path.realpath(outputDirectory + "/" + serviceName)
        if os.path.exists(executable + ".exe"):
            executable += ".exe"
        subprocess.Popen([executable], cwd=outputDirectory, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        print("Started " + serviceName)