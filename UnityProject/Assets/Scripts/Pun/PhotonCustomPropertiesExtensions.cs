using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
/// <summary>
/// アバターに対して、選択されたRoleを設定するための拡張メソッド
/// Roleを共有する。
/// </summary>
public static class PhotonCustomPropertiesExtension
{
    private const string ROLE = "R";
    private const string CONTROL_MODE = "CM";
    private const string MANUAL_COMMAND = "MC";
    private const string SEMI_AUTO_COMMAND = "SAC";
    private const string ROS_CONNECTION = "RC";

    private static readonly Hashtable playerPropsToSet = new Hashtable();
    private static readonly Hashtable roomPropsToSet = new Hashtable();

    public static int GetRole(this Player avatar)
    {
        return (avatar.CustomProperties[ROLE] is int role) ? role : 0;
    }

    public static void SetRole(this Player avatar, int role)
    {
        playerPropsToSet[ROLE] = role;
    }

    public static int GetControlMode(this Room currentRoom)
    {
        return (currentRoom.CustomProperties[CONTROL_MODE] is int controlMode) ? controlMode : 0;
    }
    public static void SetControlMode(this Room currentRoom, int controlMode)
    {
        roomPropsToSet[CONTROL_MODE] = controlMode;
    }

    public static int GetManualCmd(this Room currentRoom)
    {
        return (currentRoom.CustomProperties[MANUAL_COMMAND] is int manualCommand) ? manualCommand : 0;
    }
    public static void SetManualCmd(this Room currentRoom, int manualCommand)
    {
        roomPropsToSet[MANUAL_COMMAND] = manualCommand;
    }
    public static int GetSemiAutoCmd(this Room currentRoom)
    {
        return (currentRoom.CustomProperties[SEMI_AUTO_COMMAND] is int semiAutoCmd) ? semiAutoCmd : 0;
    }
    public static void SetSemiAutoCmd(this Room currentRoom, int semiAutoCmd)
    {
        roomPropsToSet[SEMI_AUTO_COMMAND] = semiAutoCmd;
    }

    public static int GetRosConnection(this Room currentRoom)
    {
        return (currentRoom.CustomProperties[ROS_CONNECTION] is int rosConnection) ? rosConnection : 0;
    }

    public static void SetRosConnection(this Room currentRoom, int rosConnection)
    {
        roomPropsToSet[ROS_CONNECTION] = rosConnection;
    }

    // プレイヤーのカスタムプロパティを送信する
    public static void SendProperties(this Player player)
    {
        if (playerPropsToSet.Count > 0)
        {
            player.SetCustomProperties(playerPropsToSet);
            playerPropsToSet.Clear();
        }
    }

    // ルームのカスタムプロパティを送信する
    public static void SendProperties(this Room room)
    {
        if (roomPropsToSet.Count > 0)
        {
            room.SetCustomProperties(roomPropsToSet);
            roomPropsToSet.Clear();
        }
    }
}
