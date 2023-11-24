using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using System;

//InputSystemに入力(Action)があった際に行う処理をまとめたスクリプト
//有効化するActionMapの切り替えはStart()とOnSwitchMode()で制御。
//OnSwitchControl()
//InputSystemについては https://learning.unity3d.jp/8070/　で学べる。
//Unity Event Invokeを使用
public class InputManager : MonoBehaviourPunCallbacks
{
    private SceneMaster sceneMaster;

    [HideInInspector]
    public bool speedInc, speedDec, speedCourse, speedFine,
        gripperPwmInc, gripperPwmDec, gripperOpen, gripperClose,
        armHomePose, armSleepPose, panTiltHome,
        rebootError, rebootAll, torqueEnable, torqueDisable,
        moveBase, moveArm, loopRate, loopRatePreset;

    [HideInInspector]
    public float baseX, baseRotate, waistRotate,
        eeX, eeZ, eeRoll, eePitch, pan, tilt, targetX, targetY, targetZ;

    public ControlMode controlMode = ControlMode.ManualControl;
    public ManualCommands manualCmd = ManualCommands.Disable;
    public SemiAutomaticCommands semiAutoCmd = SemiAutomaticCommands.Disable;
    public PlayerInput playerInput;
    InputActionMap manual_arm;
    InputActionMap manual_base;
    InputActionMap semiAuto;

    private void Awake()
    {
        sceneMaster = GameObject.Find("SceneMaster").GetComponent<SceneMaster>();
        sceneMaster.inputMng = this;
    }

    private void Start()
    {
        manual_arm = playerInput.actions.FindActionMap("LocobotManualArm");
        manual_base = playerInput.actions.FindActionMap("LocobotManualBase");
        semiAuto = playerInput.actions.FindActionMap("LocobotSemiAuto");
        var currentActionMap = playerInput.currentActionMap;

        if (currentActionMap == manual_arm)
        {
            controlMode = ControlMode.ManualControl;
            manualCmd = ManualCommands.Arm;
            semiAutoCmd = SemiAutomaticCommands.Disable;
        }
        else if (currentActionMap == manual_base)
        {
            controlMode = ControlMode.ManualControl;
            manualCmd = ManualCommands.Base;
            semiAutoCmd = SemiAutomaticCommands.Disable;
        }
        else if (currentActionMap == semiAuto)
        {
            controlMode = ControlMode.SemiAutomaticControl;
            manualCmd = ManualCommands.Disable;
            semiAutoCmd = SemiAutomaticCommands.Available;
        }
        else controlMode = ControlMode.Unkown;
    }

    //マニュアルコントロールモード
    public void OnCameraUpDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnEeCameraUpDown called");
            if(sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(CameraUpDownPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if(sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(CameraUpDownPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    [PunRPC]
    private void CameraUpDownPun(float value, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("CameraUpDownPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved CameraUpDownPun from {info.Sender.NickName}");
            tilt = value;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    public void OnCameraRightLeft(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeCameraRightLeft called");
        if (context.performed)
            pan = context.ReadValue<float>();
        else if (context.canceled)
            pan = 0;
    }

    public void OnPanTiltHome(InputAction.CallbackContext context)
    {
        Debug.Log("OnPanTiltHome called");
        if (context.performed)
            panTiltHome = context.ReadValueAsButton();
        else if (context.canceled)
            panTiltHome = false;
    }

    public void OnEeZ(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeZ called");
        if (context.performed)
            eeZ = context.ReadValue<float>();
        else if (context.canceled)
            eeZ = 0;
    }

    public void OnEeX(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeX called");
        if (context.performed)
            eeX = context.ReadValue<float>();
        else if (context.canceled)
            eeX = 0;
    }

    public void OnEeRoll(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeRoll called");
        if (context.performed)
            eeRoll = context.ReadValue<float>();
        else if (context.canceled)
            eeRoll = 0;
    }

    public void OnEePitch(InputAction.CallbackContext context)
    {
        Debug.Log("OnEePitch called");
        if (context.performed)
            eePitch = context.ReadValue<float>();
        else if (context.canceled)
            eePitch = 0;
    }

    public void OnWaistRotate(InputAction.CallbackContext context)
    {
        Debug.Log("OnWaistRotate called");
        if (context.performed)
            waistRotate = context.ReadValue<float>();
        else if (context.canceled)
            waistRotate = 0;
    }

    public void OnBaseX(InputAction.CallbackContext context)
    {
        Debug.Log("OnBaseX called");
        if (context.performed)
            baseX = context.ReadValue<float>();
        else if (context.canceled)
            baseX = 0;
    }

    public void OnBaseRotate(InputAction.CallbackContext context)
    {
        Debug.Log("OnBaseRotate called");
        if (context.performed)
            baseRotate = context.ReadValue<float>();
        else if (context.canceled)
            baseRotate = 0;
    }

    //両モード共通のコマンド
    //モーターのエラー時に使用する入力。本来1回の処理は10秒かからないが、
    //1回の入力で複数回Pubされてしまうので入力後は30秒ほど待つ必要がある。
    public void OnArmHomePose(InputAction.CallbackContext context)
    {
        Debug.Log("OnArmHomePose called");
        if (context.performed)
        {
            armHomePose = context.ReadValueAsButton();
            if (controlMode == ControlMode.SemiAutomaticControl)
            semiAutoCmd = SemiAutomaticCommands.Available;
        }
        else if (context.canceled)
            armHomePose = false;
    }

    public void OnArmSleepPose(InputAction.CallbackContext context)
    {
        Debug.Log("OnArmSleepPose called");
        if (context.performed)
        {
            armSleepPose = context.ReadValueAsButton();
            if (controlMode == ControlMode.SemiAutomaticControl)
            semiAutoCmd = SemiAutomaticCommands.Available;
        }
        else if (context.canceled)
            armSleepPose = false;
    }

    public void OnGripperPwmInc(InputAction.CallbackContext context)
    {
        Debug.Log("OnGripperPwmInc called");
        if (context.started)
            gripperPwmInc = context.ReadValueAsButton();
        else if (context.canceled)
            gripperPwmInc = false;
    }

    public void OnGripperPwmDec(InputAction.CallbackContext context)
    {
        Debug.Log("OnGripperPwmDec called");
        if (context.started)
            gripperPwmDec = context.ReadValueAsButton();
        else if (context.canceled)
            gripperPwmDec = false;
    }

    public void OnGripperOpen(InputAction.CallbackContext context)
    {
        Debug.Log("OnGripperOpen called");
        if (context.performed)
            gripperOpen = context.ReadValueAsButton();
        else if (context.canceled)
            gripperOpen = false;
    }

    public void OnGripperClose(InputAction.CallbackContext context)
    {
        Debug.Log("OnGripperClose called");
        if (context.performed)
            gripperClose = context.ReadValueAsButton();
        else if (context.canceled)
            gripperClose = false;
    }

    public void OnSpeedInc(InputAction.CallbackContext context)
    {
        Debug.Log("OnSpeedInc called");
        if (context.started)
            speedInc = context.ReadValueAsButton();
        else if (context.canceled)
            speedInc = false;
    }

    public void OnSpeedDec(InputAction.CallbackContext context)
    {
        Debug.Log("OnSpeedDec called");
        if (context.started)
            speedDec = context.ReadValueAsButton();
        else if (context.canceled)
            speedDec = false;
    }

    public void OnSpeedCourse(InputAction.CallbackContext context)
    {
        Debug.Log("OnSpeedCourse called");
        if (context.performed)
            speedCourse = context.ReadValueAsButton();
        else if (context.canceled)
            speedCourse = false;
    }

    public void OnSpeedFine(InputAction.CallbackContext context)
    {
        Debug.Log("OnSpeedFine called");
        if (context.performed)
            speedFine = context.ReadValueAsButton();
        else if (context.canceled)
            speedFine = false;
    }

    public void OnRebootError(InputAction.CallbackContext context)
    {
        Debug.Log("OnRebootError called");
        if (context.performed)
            rebootError = context.ReadValueAsButton();
        else if (context.canceled)
            rebootError = false;
    }

    public void OnRebootAll(InputAction.CallbackContext context)
    {
        Debug.Log("OnRebootAll called");
        if (context.performed)
            rebootAll = context.ReadValueAsButton();
        else if (context.canceled)
            rebootAll = false;
    }

    public void OnTorqueEnable(InputAction.CallbackContext context)
    {
        Debug.Log("OnTorqueEnable called");
        if (context.performed)
            torqueEnable = context.ReadValueAsButton();
        else if (context.canceled)
            torqueEnable = false;
    }

    public void OnTorqueDisable(InputAction.CallbackContext context)
    {
        Debug.Log("OnTorqueDisable called");
        if (context.performed)
            torqueDisable = context.ReadValueAsButton();
        else if (context.canceled)
            torqueDisable = false;
    }

    //ActionMapの切り替え
    //マニュアルモードにおける操作対象ベース・アームの切り替え
    public void OnSwitchControl(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("OnSwitchControl called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                if (controlMode == ControlMode.ManualControl
                    && playerInput.currentActionMap == manual_arm)
                {
                    Debug.Log("Switch Action Map to 'Manual Base'.");
                    //playerInput.SwitchCurrentActionMap("LocobotManualBase");
                    manualCmd = ManualCommands.Base;
                }
                else if (controlMode == ControlMode.ManualControl
                    && playerInput.currentActionMap == manual_base)
                {
                    Debug.Log("Switch Action Map to 'Manual Arm'.");
                    //playerInput.SwitchCurrentActionMap("LocobotManualArm");
                    manualCmd = ManualCommands.Arm;
                }
                PhotonNetwork.CurrentRoom.SetManualCmd((int)manualCmd);
            }
            else Debug.LogWarning("You are not authorized to operate.");
        }
    }

    //セミオートモードとマニュアルモードの切り替え
    public void OnSwitchMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("OnSwitchMode called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                if (controlMode == ControlMode.ManualControl
                    && (playerInput.currentActionMap == manual_arm
                    || playerInput.currentActionMap == manual_base))
                {
                    controlMode = ControlMode.SemiAutomaticControl;
                    //playerInput.SwitchCurrentActionMap("LocobotSemiAuto");
                    manualCmd = ManualCommands.Disable;
                    semiAutoCmd = SemiAutomaticCommands.Available;
                }
                else if (controlMode == ControlMode.SemiAutomaticControl
                    && playerInput.currentActionMap == semiAuto)
                {
                    controlMode = ControlMode.ManualControl;
                    //playerInput.SwitchCurrentActionMap("LocobotManualBase");
                    manualCmd = ManualCommands.Base;
                    semiAutoCmd = SemiAutomaticCommands.Disable;
                }
                PhotonNetwork.CurrentRoom.SetControlMode((int)controlMode);
                PhotonNetwork.CurrentRoom.SetManualCmd((int)manualCmd);
                PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                //photonView.RPC(nameof(RPCSwitchMode), RpcTarget.AllViaServer);
            }
            else Debug.LogWarning("You are not authorized to operate.");
        }
    }

    //以下、SemiAutomaticモード固有の入力
    public void OnMoveBase(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveBase called");
        if (context.performed)
        {
            moveBase = context.ReadValueAsButton();
            armSleepPose = context.ReadValueAsButton();
            semiAutoCmd = SemiAutomaticCommands.PlaceGoal;
        }
        else if (context.canceled)
        {
            moveBase = false;
            armSleepPose = false;
        }
    }

    public void OnMoveArm(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveArm called");
        if (context.performed)
        {
            moveArm = context.ReadValueAsButton();
            armHomePose = context.ReadValueAsButton();
            semiAutoCmd = SemiAutomaticCommands.PlaceTarget;
        }
        else if (context.canceled)
        {
            moveArm = false;
            armHomePose = false;
        }
    }

    public void OnEeRollLeft(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeRoll called");
        if (context.performed)
            eeRoll = -context.ReadValue<float>();
        else if (context.canceled)
            eeRoll = 0;
    }

    public void OnEeRollRight(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeRoll called");
        if (context.performed)
            eeRoll = context.ReadValue<float>();
        else if (context.canceled)
            eeRoll = 0;
    }

    public void OnEePitchDown(InputAction.CallbackContext context)
    {
        Debug.Log("OnEePitch called");
        if (context.performed)
            eePitch = -context.ReadValue<float>();
        else if (context.canceled)
            eePitch = 0;
    }

    public void OnEePitchUp(InputAction.CallbackContext context)
    {
        Debug.Log("OnEePitch called");
        if (context.performed)
            eePitch = context.ReadValue<float>();
        else if (context.canceled)
            eePitch = 0;
    }

        public void OnMoveX(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveTargetX called");
        if (context.performed)
            targetX = context.ReadValue<float>();
        else if (context.canceled)
            targetX = 0;
    }

    public void OnMoveY(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveTargetY called");
        if (context.performed)
            targetY = context.ReadValue<float>();
        else if (context.canceled)
            targetY = 0;
    }

    public void OnMoveZ(InputAction.CallbackContext context)
    {
        Debug.Log("OnMoveTargetZ called");
        if (context.performed)
            targetZ = context.ReadValue<float>();
        else if (context.canceled)
            targetZ = 0;
    }

    public void OnSetGoalOrTarget(InputAction.CallbackContext context)
    {
        Debug.Log("OnSetGoalOrTarget called");
        if (context.performed)
        {
            if (semiAutoCmd == SemiAutomaticCommands.PlaceGoal)
                semiAutoCmd = SemiAutomaticCommands.PublishGoal;
            else if (semiAutoCmd == SemiAutomaticCommands.PlaceTarget)
                semiAutoCmd = SemiAutomaticCommands.PublishTarget;
            else Debug.LogWarning("Goal or Target is not set in the scene. " +
                "Press 'L' and 'b' to set base goal or 'L' and 'a' to set arm target.");
        }
        else if (context.canceled)
            if (semiAutoCmd == SemiAutomaticCommands.PublishGoal
                || semiAutoCmd == SemiAutomaticCommands.PublishTarget)
                semiAutoCmd = SemiAutomaticCommands.Available;
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        controlMode = (ControlMode)PhotonNetwork.CurrentRoom.GetControlMode();
        manualCmd = (ManualCommands)PhotonNetwork.CurrentRoom.GetManualCmd();
        semiAutoCmd = (SemiAutomaticCommands)PhotonNetwork.CurrentRoom.GetSemiAutoCmd();
        if (controlMode == ControlMode.SemiAutomaticControl)
            playerInput.SwitchCurrentActionMap("LocobotSemiAuto");
        else if (controlMode == ControlMode.ManualControl && manualCmd == ManualCommands.Base)
            playerInput.SwitchCurrentActionMap("LocobotManualBase");
        else if (controlMode == ControlMode.ManualControl && manualCmd == ManualCommands.Arm)
            playerInput.SwitchCurrentActionMap("LocobotManualArm");

    }
}
