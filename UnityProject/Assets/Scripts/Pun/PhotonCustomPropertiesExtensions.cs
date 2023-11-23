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

    private static readonly Hashtable propsToSet = new Hashtable();

    public static int GetRole(this Player avatar)
    {
        return (avatar.CustomProperties[ROLE] is int role) ? role : 0;
    }

    public static void SetRole(this Player avatar, int role)
    {
        propsToSet[ROLE] = role;
    }

    // プレイヤーのカスタムプロパティを送信する
    public static void SendProperties(this Player player)
    {
        if (propsToSet.Count > 0)
        {
            player.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }
}
