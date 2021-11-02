"""
Zachary Cook

Deploys services using systemd (most Linux distributions).
"""

import os
import subprocess
import sys
import pwd
from .BaseDeploy import BaseDeploy


class SystemdDeploy(BaseDeploy):
    """
    Returns the path of the service file for the given service.
    """
    def getServiceeFile(self, serviceName):
        return "/etc/systemd/system/" + serviceName + ".service"

    """
    Runs a systemctl command.
    """
    def runSystemctl(self, arguments):
        processArguments = ["systemctl"]
        processArguments.extend(arguments)
        process = subprocess.Popen(processArguments)
        process.wait()
        if process.returncode != 0:
            print("Systemctl failed to execute. Elevated permissions may be required.")
            exit(-1)

    """
    Stops a service.
    """
    def stop(self, serviceName):
        # Stop and disable the service.
        if os.path.exists(self.getServiceeFile(serviceName)):
            print("Stopping " + serviceName)
            self.runSystemctl(["stop", serviceName + ".service"])
            self.runSystemctl(["disable", serviceName + ".service"])
            print("Stopped " + serviceName)

    """
    Starts a service.
    """
    def start(self, serviceName):
        # Create the user if it doesn't exist
        try:
            pwd.getpwnam("construct-database")
        except KeyError:
            os.system("useradd -d " + self.projectRootDirectory + " -s /sbin/nologin construct-database")

        # Give the user ownership of the project root directory
        os.system("chown -R construct-database:construct-database " + self.projectRootDirectory)

        # Create the service file if it doesn't exist.
        serviceFileLocation = self.getServiceeFile(serviceName)
        if not os.path.exists(serviceFileLocation):
            with open(serviceFileLocation, "w") as file:
                file.write("[Unit]\n")
                file.write("Description=Runs the service " + serviceName +"\n")
                file.write("\n")
                file.write("[Service]\n")
                file.write("Type=simple\n")
                file.write("User=construct-database\n")
                file.write("Group=construct-database\n")
                file.write("WorkingDirectory=" + self.projectRootDirectory + "/scripts\n")
                file.write("ExecStart=\"" + sys.executable + "\" Run.py " + serviceName + "\n")
                file.write("\n")
                file.write("[Install]\n")
                file.write("WantedBy=multi-user.target\n")
            self.runSystemctl(["daemon-reload"])

        # Start and enable the service.
        print("Starting " + serviceName)
        self.runSystemctl(["start", serviceName + ".service"])
        self.runSystemctl(["enable", serviceName + ".service"])
        print("Started " + serviceName)