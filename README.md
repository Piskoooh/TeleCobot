# Human & Robots Remote Collaboration System Using Locobots
This system allows to remote control a cobot (works with ROS Noetic) using openXR interface.
# Demo
In this repository, we can remote control Locobot wx200 from Trossen Robotics through VR Interface. 

# Features
The remote operator can immerse to high fidelity shared environment in third person perspective.  
Also, the commands to Locobot is declarative.  

Note that we only support controlling physical robot, not in simulational environment(ex. using docker). We expect your contribution of creating a docker file that enables using robots in containers (especially locobots).

# Getting Started
Follow 5 steps for getting started with this project.
1. Prepare hardwares.
2. Setup ROS packages in Locobot.
3. Setup Unity Project in Locobot.
4. Setup Unity Project in Remote Computer 1
5. Start Collaborating

## Hardware Requirements
2 hardwares are necessary for the collaboration.  
Additionaly, preparing 1 more hardware is better for developments.
### Remote Computer 1
* Computer for Remote Operator in remote location. This computer conntects to locobot via [photon cloud]() .  
  * OS: Windows 11  
  * Unity version: 2022.3.4f1
  * Head Mounted Display: Vive Pro (Developed with OpenVR Plugins, you can use other HMDs but not checked.)

### Collaboration Robot
* [Locobot wx200](https://www.trossenrobotics.com/locobot-wx200.aspx) with create3 & RPLIDAR(developed by [Trossen Robotics](https://www.trossenrobotics.com/))  
  * OS: Ubuntu 20.04  
  * Unity version: 2022.3.4f1  
  * Software: [interbotix_ros_rovers](https://github.com/Interbotix/interbotix_ros_rovers)(developed by [Trossen Robotics](https://www.trossenrobotics.com/))  
  * ROS version: Noetic (1.16.0)  

### Remote Computer 2
* Computer in local location. This computer connects to locobot using ssh. Used in developing, not in collaborating. It means this computer is not necessary for remote collaboration.    
* Follow the [instructions](https://docs.trossenrobotics.com/interbotix_xslocobots_docs/ros_interface/ros1/software_setup.html#requirements) in locobot docummentation for remote development.
  * OS: same with locobot NUC.

## Software Setup
### Step 1. Clone repository 
Clone this repository into your remote computer **and** locobot's NUC.  
>**Warning**  
!! This repository includes submodules !!  
!! Therefore do not forget to tag `recursive` in terminal !!

Command which clone repo with submodules.
```
git clone --recursive {url of this repository}
```
<details><summary>When you already clone the repo without submodule, check here.</summary>
Command for installing submodule.

```
git submodule update --init --recursive
```

</details>

### Step 2. ROS package setup in locobot
[Click here](https://github.com/Piskoooh/HumanRobotsRemoteCollaborationSystemUsingLocobots/tree/master/ROS) for setting up ROS packages in Locobot.

### Step 3. Unity project setup in locobot
[Click here](https://github.com/Piskoooh/HumanRobotsRemoteCollaborationSystemUsingLocobots/tree/master/UnityProject) for setting up unity projects in Locobot.

### Step 4. Unity project setup in remote computer 1
[Click here](https://github.com/Piskoooh/HumanRobotsRemoteCollaborationSystemUsingLocobots/tree/master/UnityProject) for setting up unity projects in your remote computer.

### Step 5. Start collaborating
Now all of the setup is complete!!  
Let's start collaborating with locobot remotely!  
~~For detail, [click here]() !!~~
>**<h4>More detail will be added soon.**


---
# Usage
TBD

---
# Note
For detailed information of locoobot, please check [locobot documentation](https://docs.trossenrobotics.com/interbotix_xslocobots_docs/index.html) supported by [Trossen Robotics](https://www.trossenrobotics.com/).

---
# Author
Kohta Seki (Master Student in Waseda University)

---
# Licence
TBD

---
# Acknowledgments
TBD

<details><summary><h2>Open Source Licences</h2></summary>
    
* [Interbotix_Ros_Core](https://github.com/Interbotix/interbotix_ros_core/tree/noetic) (noetic)
* [Interbotix_Ros_Rovers](https://github.com/Interbotix/interbotix_ros_rovers/tree/noetic) (noetic)
* [Interbotix_Ros_Toolboxes](https://github.com/Interbotix/interbotix_ros_core/tree/noetic) (noetic)
* [MoveIt Msgs](https://github.com/ros-planning/moveit_msgs/tree/0.11.4) (v.0.11.4)
* [Unity-Robotcs-Hub](https://github.com/Unity-Technologies/Unity-Robotics-Hub) (v.0.7.0)

</details> 
