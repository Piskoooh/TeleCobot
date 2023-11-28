# Unity Project Directory
This directory includes project files for unity editor. For starting collaboration, it is necessary to setup this project files in both remote computer and locobot. To do so, this file will introduce how to setup Unity project file.  
>**Warning**  
Most of the steps are same in remote computer and locobot.  
Note that different points are highlighted in **Warning**.

---
## Step 1. Open "UnityProject" from Unity Hub
Open project file from "Add project from disk".
The directory of unity project is `~/{PATH}/{TO}/HumanRobotsRemoteCollaborationSystemUsinglocobots/UnityProject`.

---
## Step 2. Import Assets
When you first open the projectfile, compile error will occur because of unimported assets.  
Enter unity editor in SafeMode. Then import the .unitypackage file shown bellow.

---

### Photon Fusion
Go to Photon Fusion [SDK&Release Notes](https://doc.photonengine.com/fusion/current/getting-started/sdk-download).
Then download FusionSDK.
>**Note**  
Before pressing download button, create Photon Account or login to PhotonService if you have one.  

>**Note** 
The SDK version which we imported was "v1.1.7 stable" during the development of this project. 

Import Fusion SDK in your unity project. (Drag `.unitypackage` to Project window.)

If all `.unitypackage` is imported completely, unity editor will exit SafeMode.

---

## Step 3. Import Packages
Usually, these packages are imporetd automatically via PackageManager.  

    "com.unity.addressables": "1.21.14",
    "com.unity.robotics.urdf-importer": "https://github.com/Unity-Technologies/URDF-Importer.git?path=/com.unity.robotics.urdf-importer",
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.xr.interaction.toolkit": "2.4.0",
>**Note**
If some errors occur related to the package shown above, please delete `[NameOfYourProjectFile]/Library/PackageCache/` and reopen the project.

---

## Step 4. Set variables
### 4.1 Photon Fusion AppID
Go to [Photon Dashboard](https://dashboard.photonengine.com/) and create your photon cloud app. Select "Fusion" for Photon SDK.  
Copy your new AppID. Go back to Unity Editor and open `Asstets/Photon/Fusion/Resources/PhotonAppSetings`.  
Paste your AppID that you copied to "AppidFusion".

---
### 4.2 ROS IP
Open Terminal in Locobot (`Ctrl+Alt+T`) and enter command shown below.

    echo $ROS_IP

The number shown in your terminal,`xxx.xxx.xxx.xxx`, is your `ROS_IP`. Copy it and go back to UnityEditor.  

In `Hierarchy` window, select `ROSConnectionPrefab`. Then, check `Inspector` window. `ROSConnection` component is attached. Paste your `ROS_IP` to `Ros IP Address` variable.  

---
## Step 5. Build project file.
Build the project. Set the path to Home.  
>**<h4>More detail will be added soon.**

---