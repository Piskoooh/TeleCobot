using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class VRUIManager : MonoBehaviour
{
    public SceneMaster sceneMaster;

    //controlMode, punConnection, rosConnection, focusRobot, userListを表示する
    public TMP_Text statusText;

    private RectTransform myRectTfm;
    // Start is called before the first frame update
    void Start()
    {
        myRectTfm = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Camera.main == null)
            return;
        // 自身の向きをカメラに向ける
        myRectTfm.rotation = Quaternion.LookRotation(myRectTfm.position - Camera.main.transform.position);

        if(statusText == null)
            return;
        //controlMode, punConnection, rosConnection, focusRobot, userListを表示
        string newtext = "";
        if(PhotonNetwork.IsConnected)
            newtext+="PUNConnection: Connected\n";
        else
            newtext+="PUNConnection: Disconnected\n";
        newtext += "------\n";
        if (sceneMaster.photonMng.focusRobot != null)
        {
            var ram = sceneMaster.photonMng.focusRobot.GetComponent<RobotAvatarSetting>();
            newtext += $"FocusRobotID: {ram.photonView.ViewID}" +
                $"\nROS Network: {(RosConnection)ram.robotRosConnection}\n";
        }
        else
            newtext += "Robot is not connected. Wait for Robot to join.\n";
        newtext += "------\n";
        if(sceneMaster.inputMng==null)
            newtext+="Operetor is not connected. Wait for Operator to join.\n";
        else
        {
            foreach (var pair in sceneMaster.photonMng.RoleDictionary)
            {
                if (pair.Value == (int)Role.Operator)
                {
                    newtext += $"OperatorID: {pair.Key}\n";
                    break;
                }
            }
            newtext+="ControlMode: "+sceneMaster.inputMng.playerInput.currentActionMap.name+"\n";
        }
        newtext += "------\n";
        newtext += "UserList:\n";
        foreach (var pair in sceneMaster.photonMng.RoleDictionary)
        {
            newtext += $"UserID: {pair.Key}\nUserRole: {(Role)pair.Value}\n";
        }
        newtext += "------\n";
        newtext += "--END OF STATUS TEXT--";
        statusText.text = newtext;
    }
}
