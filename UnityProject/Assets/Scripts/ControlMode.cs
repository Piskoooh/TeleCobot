
//場合分け管理のためにenumを使用
//定数に名称を付与

public enum PhotonConnection
{
    //NetworkConnection
    Disconnect=0,
    Connect=1,
}

public enum RosConnection
{
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

    //If SemiAutomaticMode
    //Arm
    PlaceTarget=10,
    PublishTarget=11,
    //Base
    PlaceGoal=20,
    PublishGoal=21,
    BackHome = 22
}