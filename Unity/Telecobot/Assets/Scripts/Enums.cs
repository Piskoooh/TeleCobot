//場合分け管理のためにenumを使用
//定数に名称を付与

public enum UserType
{
    //What is the type of the user?
    Remote_nonVR=0,
    Remote_VR=1,
    Local_AR=2,
    Robot=3,
}

public enum Role
{
    //What is the role of the user?
    Unkown=0,
    Operator=1,
    Collaborator=2,
    Robot=3,
}

public enum PhotonConnection
{
    //Is connected to Photon?
    Disconnect=0,
    Connect=1,
}

public enum RosConnection
{
    //Is connected to ROS?
    Disconnect=0,
    Connect=1,
}

public enum ControlMode
{
    //How to Control Locobot?
    Unkown=0,
    ManualControl = 1,
    SemiAutomaticControl = 2
}

public enum ManualCommands
{
    Disable=0,
    Arm=1,
    Base=2
}

public enum SemiAutomaticCommands
{
    Disable=0,
    Available=1,
    //Arm
    PlaceTarget=10,
    PublishTarget=11,
    //Base
    PlaceGoal=20,
    PublishGoal=21,
    BackHome = 22
}

public enum VRControllerType
{
    Universal = 0,
    OculusTouch = 1,
    Vive = 2,
    Pico4 = 3,
}
