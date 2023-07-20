# Human&RobotsRemoteCollaborationSystemUsingLocobots
This system allows to remote control a cobot (works with ROS Noetic) using openXR interface.
# Demo
In this project file, we can remote control Locobot wx200 from Trossen Robotics.

# Features
The remote operator can immerse to high fidelity shared environment in third person perspective.  
Also, the commands to Locobot is declarative.  

Note that we only support controlling physical robot, not in simulational environment(ex. using docker). We expect your contribution of creating a docker file that enables using robots in containers (especially locobots).

# Getting Started
## Hardware Setup
### Remote Computer 1
* Computer for Remote Operator in remote location. This computer conntects to locobot via [photon cloud]() .  
* OS: Windows 11  
* Unity version: 2022.3.4f1  

### Collaboration Robot
* OS: Ubuntu 20.04  
* Unity version: 2022.3.4f1  
* Model of Robot: [Locobot wx200](https://www.trossenrobotics.com/locobot-wx200.aspx) with create3 (developed by [Trossen Robotics](https://www.trossenrobotics.com/))  
* Software: [interbotix_ros_rovers](https://github.com/Interbotix/interbotix_ros_rovers)(developed by [Trossen Robotics](https://www.trossenrobotics.com/))  
* ROS version: Noetic (1.16.0)  

### Remote Computer 2
* Computer in local location. This computer connects to locobot using ssh. Used in developing, not in collaborating. It means this computer is not necessary for remote collaboration.    
* Follow the [instructions](https://docs.trossenrobotics.com/interbotix_xslocobots_docs/ros_interface/ros1/software_setup.html#requirements) in locobot docummentation for remote development.

## Software Setup
### 1. Open Projectfile in UnityEditor
Clone this repository into your remote computer **and** locobot's NUC.  
Open each project file via unity editor.
    
### 2. Import Assets
When you first open the projectfile, compile error will occur because of unimported assets. Enter unity editor in SafeMode. Then import the .unitypackage file shown bellow.

#### Photon Fusion
Go to Photon Fusion [SDK&Release Notes](https://doc.photonengine.com/fusion/current/getting-started/sdk-download).
Then download FusionSDK.
>**Note**
Before pressing download button, create Photon Account or login to PhotonService if you have one.  
The SDK version which we imported was "v1.1.7 stable" during the development of this projectfile. 

Go to [Photon Dashboard](https://dashboard.photonengine.com/) and create your photon cloud app. Select "Fusion" for Photon SDK.  
Copy your new AppID. Go to Unity Editor and open `Asstets/Photon/Fusion/Resources/PhotonAppSetings`. Paste your AppID that you copied to "AppidFusion".

If all .unitypackage file is imported, unity editor will exit SafeMode.

### 3. Import other Packages
Usually, these packages are imporetd automatically via PackageManager.  
>**Note**
If error occur related to the package shown below, please delete `[NameOfYourProjectFile]/Library/PackageCache/` and reopen the project file in unity.

    "com.unity.addressables": "1.21.14",
    "com.unity.robotics.urdf-importer": "https://github.com/Unity-Technologies/URDF-Importer.git?path=/com.unity.robotics.urdf-importer",
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.xr.interaction.toolkit": "2.4.0",

### 4. Build project file in locobot's NUC
Build the project. Set the path to Home.  

# Usage
TBD

# Note
For detailed information of locoobot, please check [locobot documentation](https://docs.trossenrobotics.com/interbotix_xslocobots_docs/index.html) supported by [Trossen Robotics](https://www.trossenrobotics.com/).

# Author
Kohta Seki (Master Student in Waseda University)

# Licence
TBD

# Acknowledgments
TBD

<details><summary>Open Source Licences</summary>
    
* [Interbotix_Ros_Core](https://github.com/Interbotix/interbotix_ros_core/tree/noetic)(noetic)
* [Interbotix_Ros_Rovers](https://github.com/Interbotix/interbotix_ros_rovers/tree/noetic)(noetic)
* [Interbotix_Ros_Toolboxes](https://github.com/Interbotix/interbotix_ros_core/tree/noetic)(noetic)
* [MoveIt Msgs](https://github.com/ros-planning/moveit_msgs/tree/0.11.4)(v.0.11.4)
* [Unity-Robotcs-Hub](https://github.com/Unity-Technologies/Unity-Robotics-Hub)(v.0.7.0)

</details> 
