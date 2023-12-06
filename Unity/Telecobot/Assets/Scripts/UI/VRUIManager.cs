﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class VRUIManager : MonoBehaviour
{
    public SceneMaster sceneMaster;

    //controlMode, punConnection, rosConnection, focusRobot, userListを表示する
    public TMP_Text statusText;

    [HideInInspector]
    public Transform xrOrigin;

    [SerializeField]
    private GameObject rotatingUIs;
    public Button right, left;
    public Canvas[] canvases =new Canvas[5];

    private Quaternion startRotation;
    private Quaternion endRotation;
    private float countTime;
    private bool startRotate;
    [SerializeField]
    private CanvasGroup arrowUIs, semiAutomaticControlUI;

    [SerializeField]
    Button SleepPoseBtn, HomePoseBtn, ArmControlBtn, BaseControlBtn, RegisetrBtn, focusBefore, focusAfter;
    [SerializeField]
    TMP_Text moveDescriptionText,focusRobotText;

    [SerializeField]
    private InputAction action;

    // Start is called before the first frame update
    void Start()
    {
        right.onClick.AddListener(() => RotateObject(1));
        left.onClick.AddListener(() => RotateObject(-1));
        //SleepPoseBtn.GetComponent<OnScreenButtonIsPressed>().sceneMaster = this.sceneMaster;
        //HomePoseBtn.GetComponent<OnScreenButtonIsPressed>().sceneMaster = this.sceneMaster;
        //ArmControlBtn.GetComponent<OnScreenButtonIsPressed>().sceneMaster = this.sceneMaster;
        //BaseControlBtn.GetComponent<OnScreenButtonIsPressed>().sceneMaster = this.sceneMaster;
        //RegisetrBtn.GetComponent<OnScreenButtonIsPressed>().sceneMaster = this.sceneMaster;
    }

    // Update is called once per frame
    void Update()
    {
        if(Camera.main == null)
            return;
        // 自身の向きをカメラに向ける
        foreach (var canvas in canvases)
        {
            canvas.transform.rotation = Quaternion.LookRotation(canvas.transform.position - Camera.main.transform.position);
        }

        #region StatusUI
        if (statusText == null)
            return;
        //controlMode, punConnection, rosConnection, focusRobot, userListを表示
        string newtext = "";
        if(PhotonNetwork.IsConnected)
            newtext+="PUNConnection: Connected\n";
        else
            newtext+="PUNConnection: Disconnected\n";
        newtext += "============\n";
        if (sceneMaster.photonMng.focusRobot != null)
        {
            var ram = sceneMaster.photonMng.focusRobot.GetComponent<RobotAvatarSetting>();
            newtext += $"FocusRobotID: {ram.photonView.ViewID}" +
                $"\nROS Network: {(RosConnection)ram.robotRosConnection}\n";
            focusRobotText.text = $"ID : {ram.photonView.ViewID}";
        }
        else
            newtext += "Robot is not connected. Wait for Robot to join.\n";
        newtext += "============\n";
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
            newtext += "ControlMode: " + (ControlMode)sceneMaster.inputMng.controlMode + "\n";
            if (sceneMaster.inputMng.controlMode == ControlMode.ManualControl)
                newtext += "ControlingTarget :" + (ManualCommands)sceneMaster.inputMng.manualCmd + "\n";
            else if (sceneMaster.inputMng.controlMode == ControlMode.SemiAutomaticControl)
                newtext += "Controling :" + (SemiAutomaticCommands)sceneMaster.inputMng.semiAutoCmd + "\n";
        }
        newtext += "============\n";
        newtext += "UserList:\n";
        foreach (var pair in sceneMaster.photonMng.RoleDictionary)
        {
            newtext += $"UserID: {pair.Key}\nUserRole: {(Role)pair.Value}\n";
            newtext += "------------\n";
        }
        newtext += "============\n";
        //newtext += "--END OF STATUS TEXT--";
        statusText.text = newtext;
        #endregion

        #region RemoteControlUI

        if(sceneMaster.userSettings.role == Role.Operator)
        {
            if (sceneMaster.photonMng.focusRobot != null)
            {
                semiAutomaticControlUI.interactable = true;
                semiAutomaticControlUI.blocksRaycasts = true;
            }
            else
            {
                semiAutomaticControlUI.interactable = false;
                semiAutomaticControlUI.blocksRaycasts = false;
                moveDescriptionText.text = "Robot is not connected. Wait for Robot to join.";
            }
        }
        else
        {
            semiAutomaticControlUI.interactable = false;
            semiAutomaticControlUI.blocksRaycasts = false;
            moveDescriptionText.text = "You are not authorized to use this UI.";
        }
        #endregion

        UpdateUIsRotate();

        if(xrOrigin != null)
        {
            transform.position = xrOrigin.position;
            transform.rotation = xrOrigin.rotation;
        }
    }

    // ゲームオブジェクトを指定した軸周りにスムーズに回転させるメソッド
    void RotateObject(int direction)
    {
        countTime = 0f;
        startRotation = rotatingUIs.transform.rotation;
        endRotation = rotatingUIs.transform.rotation * Quaternion.AngleAxis(90f * direction, rotatingUIs.transform.up);
        startRotate = true;
        arrowUIs.interactable = false;
    }

    private void UpdateUIsRotate()
    {
        if (startRotate == false)
        {
            return;
        }
        countTime = Mathf.Clamp(countTime + Time.deltaTime, 0f, 0.5f);
        float rate = countTime / 0.5f;
        rotatingUIs.transform.rotation = Quaternion.Lerp(startRotation, endRotation, rate);

        if (rate >= 1f)
        {
            arrowUIs.interactable = true;
            startRotate = false;
        }
    }

    #region UIの表示非表示
    private void OnEnable()
    {
        action.started += UIVisual;

        action.Enable();
    }

    private void OnDisable()
    {
        action.started -= UIVisual;

        action.Disable();
    }

    private void UIVisual(InputAction.CallbackContext context)
    {
        if (rotatingUIs.activeSelf)
        {
            rotatingUIs.SetActive(false);
        }
        else
        {
            rotatingUIs.SetActive(true);
        }
    }
    #endregion
}
