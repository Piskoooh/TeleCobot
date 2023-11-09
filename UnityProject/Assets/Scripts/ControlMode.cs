
//UI????????????????????????????

public enum PhotonConnection
{
    //NetworkConnection
    Connect=1,
    Disconnect=0,
}
public enum RosConnection
{
    Connect=1,
    Disconnect=0,
}
public enum ControlMode
{
    //How to Control Locobot?
    ManualControl = 900,
    SemiAutomaticControl = 801,
}
public enum SemiAutoMaticCommands
{
    //ArmPose
    Sleep=10,
    Home=11,

    //If SemiAutomaticMode
    //Arm
    PlaceTarget=800,
    PublishTarget=801,
    //Base
    PlaceGoal=810,
    PublishGoal=811,
    BackHome = 812
}