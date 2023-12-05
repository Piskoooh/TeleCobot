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

    [HideInInspector]
    public LocalArrow localArrow;

    public ControlMode controlMode = ControlMode.ManualControl;
    public ManualCommands manualCmd = ManualCommands.Disable;
    public SemiAutomaticCommands semiAutoCmd = SemiAutomaticCommands.Disable;
    public PlayerInput playerInput;
    InputActionMap manual_arm;
    InputActionMap manual_base;
    InputActionMap semiAuto;

    private void Awake()
    {
        sceneMaster = GameObject.FindGameObjectWithTag("SceneMaster").GetComponent<SceneMaster>();
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

    #region ゲームコントローラーのみでの制御
    #region マニュアルモード
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

    public void OnCameraRightLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnEeCameraRightLeft called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(CameraRightLeftPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if(sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(CameraRightLeftPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnPanTiltHome(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnPanTiltHome called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(PanTiltHomePun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(PanTiltHomePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnEeZ(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("OnEeZ called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(EeZPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(EeZPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnEeX(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("OnEeX called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(EeXPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(EeXPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnEeRoll(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("OnEeRoll called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(EeRollPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(EeRollPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnEePitch(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("OnEePitch called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(EePitchPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(EePitchPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnWaistRotate(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("OnWaistRotate called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(WaistRotatePun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(WaistRotatePun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnBaseX(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("OnBaseX called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(BaseXPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if(context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(BaseXPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnBaseRotate(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("OnBaseRotate called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(BaseRotatePun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if(context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(BaseRotatePun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }
    #endregion　マニュアルモード

    #region 切り替えコマンド
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
    #endregion 切り替えコマンド

    #region セミオートマモード
    //SemiAutomaticモード固有の入力
    public void OnMoveBase(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnMoveBase called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(MoveBasePun), RpcTarget.AllViaServer, b);
                if (controlMode == ControlMode.SemiAutomaticControl)
                {
                    semiAutoCmd = SemiAutomaticCommands.PlaceGoal;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if(context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(MoveBasePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnMoveArm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnMoveArm called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(MoveArmPun), RpcTarget.AllViaServer, b);
                if (controlMode == ControlMode.SemiAutomaticControl)
                {
                    semiAutoCmd = SemiAutomaticCommands.PlaceTarget;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(MoveArmPun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnEeRollLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnEeRoll called");
            if(sceneMaster.userSettings.role== Role.Operator)
            {
                float f = -context.ReadValue<float>();
                photonView.RPC(nameof(EeRollPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if(context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(EeRollPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnEeRollRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnEeRoll called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(EeRollPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if(context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(EeRollPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnEePitchDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnEePitch called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = -context.ReadValue<float>();
                photonView.RPC(nameof(EePitchPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if(context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(EePitchPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnEePitchUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnEePitch called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(EePitchPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if(context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(EePitchPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnMoveX(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("OnMoveTargetX called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(MoveTargetXPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if(context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(MoveTargetXPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnMoveY(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnMoveTargetY called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(MoveTargetYPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if(context.canceled)
            if(sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(MoveTargetYPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnMoveZ(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnMoveTargetZ called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                float f = context.ReadValue<float>();
                photonView.RPC(nameof(MoveTargetZPun), RpcTarget.AllViaServer, f);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if(context.canceled)
            if(sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(MoveTargetZPun), RpcTarget.AllViaServer, 0f);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnSetGoalOrTarget(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           Debug.Log("OnSetGoalOrTarget called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                if (semiAutoCmd == SemiAutomaticCommands.PlaceGoal)
                {
                    semiAutoCmd = SemiAutomaticCommands.PublishGoal;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
                else if (semiAutoCmd == SemiAutomaticCommands.PlaceTarget)
                {
                    semiAutoCmd = SemiAutomaticCommands.PublishTarget;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
                else Debug.LogWarning("Goal or Target is not set in the scene. " +
                    "Press 'L' and 'b' to set base goal or 'L' and 'a' to set arm target.");
            }
        }
        else if (context.canceled)
            if (semiAutoCmd == SemiAutomaticCommands.PublishGoal
                || semiAutoCmd == SemiAutomaticCommands.PublishTarget)
            {
                semiAutoCmd = SemiAutomaticCommands.Available;
                PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
            }
    }

    /// <summary>
    /// グリッパーのロールとピッチを指定するための回転の基準となる軸の方向を取得する。
    /// InputSystemのロールとピッチのアクションにコールバックとして呼び出し設定。
    /// </summary>
    /// <param name="context"></param>
    public void OnEeCall(InputAction.CallbackContext context)
    {
        if (context.started)
            if(sceneMaster.userSettings.role == Role.Operator)
            {
                Debug.Log("OnEeCall called");
                photonView.RPC(nameof(EeCallPun), RpcTarget.AllViaServer);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
    }
    #endregion セミオートマモード

    #region 両モード共通のコマンド
    //モーターのエラー時に使用する入力。本来1回の処理は10秒かからないが、
    //1回の入力で複数回Pubされてしまうので入力後は30秒ほど待つ必要がある。
    public void OnArmHomePose(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnArmHomePose called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(ArmHomePosePun), RpcTarget.AllViaServer, b);
                if (controlMode == ControlMode.SemiAutomaticControl)
                {
                    semiAutoCmd = SemiAutomaticCommands.Available;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(ArmHomePosePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnArmSleepPose(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnArmSleepPose called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(ArmSleepPosePun), RpcTarget.AllViaServer, b);
                if (controlMode == ControlMode.SemiAutomaticControl)
                {
                    semiAutoCmd = SemiAutomaticCommands.Available;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(ArmSleepPosePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnGripperPwmInc(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnGripperPwmInc called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(GripperPwmIncPun), RpcTarget.AllViaServer, b);

            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(GripperPwmIncPun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnGripperPwmDec(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnGripperPwmDec called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(GripperPwmDecPun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(GripperPwmDecPun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnGripperOpen(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnGripperOpen called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(GripperOpenPun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(GripperOpenPun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnGripperClose(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnGripperClose called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(GripperClosePun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(GripperClosePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnSpeedInc(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnSpeedInc called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(SpeedIncPun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(SpeedIncPun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnSpeedDec(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnSpeedDec called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(SpeedDecPun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(SpeedDecPun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnSpeedCourse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnSpeedCourse called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(SpeedCoursePun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(SpeedCoursePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnSpeedFine(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnSpeedFine called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(SpeedFinePun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(SpeedFinePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnRebootError(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnRebootError called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(RebootErrorPun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(RebootErrorPun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnRebootAll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnRebootAll called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(RebootAllPun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(RebootAllPun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnTorqueEnable(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnTorqueEnable called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(TorqueEnablePun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(TorqueEnablePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnTorqueDisable(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("OnTorqueDisable called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                bool b = context.ReadValueAsButton();
                photonView.RPC(nameof(TorqueDisablePun), RpcTarget.AllViaServer, b);
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (context.canceled)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(TorqueDisablePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }
    #endregion 両モード共通のコマンド
    #endregion

    #region VRコントローラーで操作するUIからの制御
    #region セミオートマモード

    public void OnMoveBaseVR(bool b)
    {
        if (b)
        {
            Debug.Log("OnMoveBaseVR called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                photonView.RPC(nameof(MoveBasePun), RpcTarget.AllViaServer, true);
                if (controlMode == ControlMode.SemiAutomaticControl)
                {
                    semiAutoCmd = SemiAutomaticCommands.PlaceGoal;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (!b)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(MoveBasePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnMoveArmVR(bool b)
    {
        if (b)
        {
            Debug.Log("OnMoveArmVR called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                photonView.RPC(nameof(MoveArmPun), RpcTarget.AllViaServer, true);
                if (controlMode == ControlMode.SemiAutomaticControl)
                {
                    semiAutoCmd = SemiAutomaticCommands.PlaceTarget;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (!b)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(MoveArmPun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnSetTargetOrGoalVR(bool b)
    {
        if (b)
        {
            Debug.Log("OnSetTargetOrGoalVR called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                if (semiAutoCmd == SemiAutomaticCommands.PlaceGoal)
                {
                    semiAutoCmd = SemiAutomaticCommands.PublishGoal;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
                else if (semiAutoCmd == SemiAutomaticCommands.PlaceTarget)
                {
                    semiAutoCmd = SemiAutomaticCommands.PublishTarget;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
                else Debug.LogWarning("Goal or Target is not set in the scene. " +
                                   "Press 'L' and 'b' to set base goal or 'L' and 'a' to set arm target.");
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
    }

    public void OnArmHomePoseVR(bool b)
    {
        if (b)
        {
            Debug.Log("OnArmHomePoseVR called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                photonView.RPC(nameof(ArmHomePosePun), RpcTarget.AllViaServer, true);
                if (controlMode == ControlMode.SemiAutomaticControl)
                {
                    semiAutoCmd = SemiAutomaticCommands.Available;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (!b)
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(ArmHomePosePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
    }

    public void OnArmSleepPoseVR(bool b)
    {
        if (b)
        {
            Debug.Log("OnArmSleepPoseVR called");
            if (sceneMaster.userSettings.role == Role.Operator)
            {
                photonView.RPC(nameof(ArmSleepPosePun), RpcTarget.AllViaServer, true);
                if (controlMode == ControlMode.SemiAutomaticControl)
                {
                    semiAutoCmd = SemiAutomaticCommands.Available;
                    PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)semiAutoCmd);
                }
            }
            else
            {
                Debug.LogWarning("You are not authorized to operate.");
            }
        }
        else if (!b)
        {
            if (sceneMaster.userSettings.role == Role.Operator)
                photonView.RPC(nameof(ArmSleepPosePun), RpcTarget.AllViaServer, false);
            else
                Debug.LogWarning("You are not authorized to operate.");
        }
    }

    #endregion
    #endregion

    #region PunRPCs for Manual Mode

    [PunRPC]
    private void CameraUpDownPun(float value, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("CameraUpDownPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved CameraUpDownPun from {info.Sender.NickName}\n{value}");
            tilt = value;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void CameraRightLeftPun(float value, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("CameraRightLeftPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved CameraRightLeftPun from {info.Sender.NickName}\n{value}");
            pan = value;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void PanTiltHomePun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("PanTiltHomePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved PanTiltHomePun from {info.Sender.NickName}");
            panTiltHome = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void WaistRotatePun(float value, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("WaistRotatePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved WaistRotatePun from {info.Sender.NickName}\n{value}");
            waistRotate = value;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void BaseXPun(float value, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("BaseXPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved BaseXPun from {info.Sender.NickName}\n{value}");
            baseX = value;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void BaseRotatePun(float value, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("BaseRotatePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved BaseRotatePun from {info.Sender.NickName}\n{value}");
            baseRotate = value;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    #endregion PunRPCs for Manual Mode

    #region PunRPCs for SemiAutomatic Mode

    [PunRPC]
    private void MoveBasePun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("MoveBasePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved MoveBasePun from {info.Sender.NickName}");
            moveBase = cmd;
            armSleepPose = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void MoveArmPun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("MoveArmPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved MoveArmPun from {info.Sender.NickName}");
            moveArm = cmd;
            armHomePose = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void MoveTargetXPun(float f, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("MoveTargetXPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved MoveTargetXPun from {info.Sender.NickName}");
            targetX = f;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    [PunRPC]
    private void MoveTargetYPun(float f, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("MoveTargetYPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved MoveTargetYPun from {info.Sender.NickName}");
            targetY = f;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    [PunRPC]
    private void MoveTargetZPun(float f, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("MoveTargetZPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved MoveTargetZPun from {info.Sender.NickName}");
            targetZ = f;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    [PunRPC]
    private void EeCallPun(PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("EeCallPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved EeCallPun from {info.Sender.NickName}");
            if (localArrow)
            {
                localArrow.UpdateEeArrow();
            }
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    #endregion PunRPCs for SemiAutomatic Mode

    #region PunRPCs for Both Modes

    [PunRPC]
    private void ArmHomePosePun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("ArmHomePosePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved ArmHomePosePun from {info.Sender.NickName}");
            armHomePose = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void ArmSleepPosePun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("ArmSleepPosePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved ArmSleepPosePun from {info.Sender.NickName}");
            armSleepPose = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void EeZPun(float value, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("EeZPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved EeZPun from {info.Sender.NickName}\n{value}");
            eeZ = value;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void EeXPun(float value, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("EeXPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved EeXPun from {info.Sender.NickName}\n{value}");
            eeX = value;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void EeRollPun(float value, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("EeRollPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved EeRollPun from {info.Sender.NickName}\n{value}");
            eeRoll = value;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void EePitchPun(float value, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("EePitchPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved EePitchPun from {info.Sender.NickName}\n{value}");
            eePitch = value;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void GripperPwmIncPun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("GripperPwmIncPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved GripperPwmIncPun from {info.Sender.NickName}");
            gripperPwmInc = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void GripperPwmDecPun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("GripperPwmDecPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved GripperPwmDecPun from {info.Sender.NickName}");
            gripperPwmDec = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void GripperOpenPun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("GripperOpenPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved GripperOpenPun from {info.Sender.NickName}");
            gripperOpen = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }

    [PunRPC]
    private void GripperClosePun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("GripperClosePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved GripperClosePun from {info.Sender.NickName}");
            gripperClose = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command to Robot.");
    }
    [PunRPC]
    private void SpeedIncPun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("SpeedIncPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved SpeedIncPun from {info.Sender.NickName}");
            speedInc = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    [PunRPC]
    private void SpeedDecPun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("SpeedDecPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved SpeedDecPun from {info.Sender.NickName}");
            speedDec = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    [PunRPC]
    private void SpeedCoursePun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("SpeedCoursePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved SpeedCoursePun from {info.Sender.NickName}");
            speedCourse = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    [PunRPC]
    private void SpeedFinePun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("SpeedFinePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved SpeedFinePun from {info.Sender.NickName}");
            speedFine = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    [PunRPC]
    private void RebootErrorPun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("RebootErrorPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved RebootErrorPun from {info.Sender.NickName}");
            rebootError = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    [PunRPC]
    private void RebootAllPun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("RebootAllPun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved RebootAllPun from {info.Sender.NickName}");
            rebootAll = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    [PunRPC]
    private void TorqueEnablePun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("TorqueEnablePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved TorqueEnablePun from {info.Sender.NickName}");
            torqueEnable = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    [PunRPC]
    private void TorqueDisablePun(bool cmd, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal)
            Debug.Log("TorqueDisablePun called");
        else if (sceneMaster.userSettings.role == Role.Robot)
        {
            Debug.Log($"Recieved TorqueDisablePun from {info.Sender.NickName}");
            torqueDisable = cmd;
        }
        else
            Debug.Log($"Operator ( {info.Sender.NickName}, UserID: {info.Sender.UserId} ) is sending command.");
    }

    #endregion PunRPCs for Both Modes

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        controlMode = (ControlMode)PhotonNetwork.CurrentRoom.GetControlMode();
        manualCmd = (ManualCommands)PhotonNetwork.CurrentRoom.GetManualCmd();
        semiAutoCmd = (SemiAutomaticCommands)PhotonNetwork.CurrentRoom.GetSemiAutoCmd();
        if (playerInput.enabled==true)
        {
            if (controlMode == ControlMode.SemiAutomaticControl)
                playerInput.SwitchCurrentActionMap("LocobotSemiAuto");
            else if (controlMode == ControlMode.ManualControl && manualCmd == ManualCommands.Base)
                playerInput.SwitchCurrentActionMap("LocobotManualBase");
            else if (controlMode == ControlMode.ManualControl && manualCmd == ManualCommands.Arm)
                playerInput.SwitchCurrentActionMap("LocobotManualArm");
        }
    }
}
