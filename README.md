# HumanRobotsRemoteCollaborationSystemUsingLocobots
This system allows to remote control a cobot (works with ROS Noetic) using openXR interface.
# Demo
In this project file, we can remote control Locobot wx200 from Trossen Robotics.

# Features
The remote operator can immerse to high fidelity shared environment in third person perspective.
Also, the commands to Locobot is declarative. 
Note that we only support controlling real robot, not in simulational environment(ex. using docker).

# Getting Started
## Requirement
### Remote Computer
OS: Windows 11

Unity version: 2022.3.4f1

### Robot
Model: Locobot wx200 (Trossen Robotics)

OS: Ubuntu 20.04

ROS version: Noetic

## Installation
### Open Projectfile
Clone this repository into your remote computer and locobot's NUC.
Then, open with unity editor.

**In locobot's NUC, build this project file.** We use this built .exe file during the collaboration.

### Import Assets
#### via Package Manager
    "com.unity.addressables": "1.21.14",
    "com.unity.robotics.urdf-importer": "https://github.com/Unity-Technologies/URDF-Importer.git?path=/com.unity.robotics.urdf-importer",
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.xr.interaction.toolkit": "2.4.0",

#### via .unitypackage
##### Photon Fusion
Go to Photon Fusion [SDK&Release Notes](https://doc.photonengine.com/fusion/current/getting-started/sdk-download).

We imported "v1.1.7 stable" during we develop this projectfile. 

# Usage
TBD

# Note
TBD

# Author
Kohta Seki (Master Student in Waseda University)

# Licence
TBD

# Acknowledgments
TBD
