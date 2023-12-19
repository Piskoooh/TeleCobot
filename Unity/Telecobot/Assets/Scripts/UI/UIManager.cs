using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
/// <summary>
/// UI に関する処理をここで実行
/// </summary>
public class UIManager : MonoBehaviour
{
    public PubUnityControl pubUnityControl;
    public SceneMaster sceneMaster;

    //ネットワーク関連のUI
    public TMP_Text controlMode_Text, punConnection_Text, rosConnection_Text;
    public Button punConnectButton, rosConnectButton;

    //シーン上に生成するUIのプレハブ
    //[HideInInspector]
    public GameObject visualIndicator, eeGripper;

    public float targetMoveSpeed = 0.5f;
    [HideInInspector]
    public GameObject target;
    Pose pose;

    //ロボットアームのターゲット指定で使用する読み取り専用の数値
    Transform baseLinkTf => sceneMaster.rosConnector.base_link.transform;
    Transform armBaseLinkTf => sceneMaster.rosConnector.arm_base_link.transform;
    Transform endEffectorTf => sceneMaster.rosConnector.endEffector.transform;
    Vector2 b_a => new Vector2(armBaseLinkTf.transform.position.x - baseLinkTf.transform.position.x, armBaseLinkTf.transform.position.z - baseLinkTf.transform.position.z);
    Vector2 b_aN => b_a.normalized;
    Vector2 a_ee => new Vector2(endEffectorTf.transform.position.x - armBaseLinkTf.transform.position.x, endEffectorTf.transform.position.z - armBaseLinkTf.transform.position.z);
    Vector2 a_eg => new Vector2(eeGripper.transform.position.x - armBaseLinkTf.transform.position.x, eeGripper.transform.position.z - armBaseLinkTf.transform.position.z);

    //ロボットベースのゴール指定に関連するUI
    //シーン上に生成するUI
    public float goalMoveSpeed = 1f, goalRotateSpeed = 5f;
    [HideInInspector]
    public GameObject goal;

    // Start is called before the first frame update
    void Start()
    {
        punConnection_Text.text = "Photon : Not Connected";
        rosConnection_Text.text = "Ros : Not Connected";
        punConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        rosConnectButton.interactable = false;
    }

    /// <summary>
    /// アームがリーチ可能な範囲を視覚化。ロボットの子オブジェクトとすることでロボットが移動しても追従。
    /// </summary>
    public void VisualRange()
    {
        visualIndicator = PhotonNetwork.Instantiate("ArmRangeVisualizerPun", sceneMaster.rosConnector.arm_base_link.transform.position, Quaternion.Euler(0f, 0f, 0f));
        sceneMaster.photonMng.focusRobot.GetComponent<RobotAvatarSetting>().CallAARVP(visualIndicator.tag);
    }

    /// <summary>
    /// アームの目標位置を指定するためのターゲットオブジェクトを生成する。範囲外にエンドエフェクターがある場合はアームをホームポジションに指定して終了。
    ///     if文前…視覚化した範囲を数式で表現するための変数を用意
    ///     if文　エンドエフェクターが視覚化した範囲内かどうかを判定
    ///         ターゲットがなければ生成。あればターゲット位置を初期値にリセット。
    ///     else　範囲内に収まるようにホームポジションにアームをリセット。
    /// </summary>
    public void CreateOrResetTarget()
    {
        if (visualIndicator == null)
        {
            VisualRange();
        }
        float angle = Vector2.Angle(a_ee, b_aN);
        Vector3 direction = endEffectorTf.position - armBaseLinkTf.position;
        if (direction.magnitude < 0.55f && 90 > angle && sceneMaster.rosConnector.endEffector.transform.position.y > 0 )
        {
            if (target == null)
            {
                target = PhotonNetwork.Instantiate("TargetGripperPun", endEffectorTf.position, Quaternion.Euler(0f, 0f, 0f)); //create
                target.transform.parent = baseLinkTf;
                eeGripper = GameObject.FindGameObjectWithTag("end_effector");
                sceneMaster.photonMng.focusRobot.GetComponent<RobotAvatarSetting>().CallEeA("end_effector",target.tag);
            }

            //gripperプレハブを使う時
            Pose resetPose;
            //resetPose.position = new Vector3(baseLinkTf.position.x + 0.5f * b_aN.x, baseLinkTf.position.y + 0.1f, baseLinkTf.position.z + 0.5f * b_aN.y);
            //resetPose.rotation = baseLinkTf.rotation;
            resetPose.position = sceneMaster.rosConnector.endEffector.transform.position;
            resetPose.rotation = sceneMaster.rosConnector.endEffector.transform.rotation;
            pose = new Pose(resetPose.position,resetPose.rotation);
            AlignChildByMoveParent(target.transform, eeGripper.transform, pose);
        }
        else
        {
            Debug.LogWarning($"End effector is outside the specified range or with invalid y value.\n" +
            $"Direction Magnitude: {direction.magnitude}, Angle: {angle}, Gripper Position Y: {endEffectorTf.position.y}" +
            $"\nReset arm to 'Home Position'.");

            sceneMaster.inputMng.semiAutoCmd = SemiAutomaticCommands.Available;
            PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)sceneMaster.inputMng.semiAutoCmd);
        }
    }

    /// <summary>
    /// 座標数値をPubする前にアームのリーチ可能な範囲内にTargetが存在するかを確認。範囲外の場合はターゲットの位置をリセット。
    /// </summary>
    public void PickTarget()
    {
        if (target == null)
            Debug.Log("Target Object does not exist.");
        else
        {
            float angle = Vector2.Angle(a_eg, b_aN);
            Vector3 direction = eeGripper.transform.position - armBaseLinkTf.position;
            if (direction.magnitude < 0.55f && 90 > angle && eeGripper.transform.position.y > 0)
            {
                pubUnityControl.SetTargetPose();
                return;
            }
            else
                Debug.Log($"Cannot set target. The target is outside the specified range or has an invalid y value.\n" +
                            $"Direction Magnitude: {direction.magnitude}, Angle: {angle}, Gripper Position Y: {eeGripper.transform.position.y}");
        }
        CreateOrResetTarget();
        sceneMaster.inputMng.semiAutoCmd = SemiAutomaticCommands.PlaceTarget;
        PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)sceneMaster.inputMng.semiAutoCmd);
    }

    public void CreateOrResetGoal()
    {
        if (goal == null)
        {
            goal = PhotonNetwork.Instantiate("TargetLocobotPun", baseLinkTf.position, Quaternion.Euler(0, Vector3.Angle(Vector2.up, b_aN), 0));//create
            sceneMaster.photonMng.focusRobot.GetComponent<RobotAvatarSetting>().CallSetG(goal.tag);
        }
        //targetプレハブを使う時
        //goal.transform.position = new Vector3(0f, 0f, 0.3f);
        //goal.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void CheckGoal()
    {
        if (goal == null)
            Debug.Log("Goal Object does not exist.");
        else
        {
            //承認しない条件(できればrtabmap上にない座標にGoalがない場合に弾くようにしたい。)
            if (goal)
            {
                pubUnityControl.SetMoveToPose();
                return;
            }
            else
                Debug.Log("Cannot set target. Something went wrong.");
        }
        CreateOrResetGoal();
        sceneMaster.inputMng.semiAutoCmd = SemiAutomaticCommands.PlaceGoal;
        PhotonNetwork.CurrentRoom.SetSemiAutoCmd((int)sceneMaster.inputMng.semiAutoCmd);
    }

    /// <summary>
    /// childの座標と回転がtargetPoseと一致するように、parentを移動。
    /// parentとchildの間に他のTransformがあっても動作するが、parentとchildは実際に親子関係である必要あり。
    /// </summary>
    /// <param name="parent">位置合わせの為に動かすオブジェクト</param>
    /// <param name="child">位置を合わせるオブジェクト</param>
    /// <param name="targetPose">目標となる姿勢（ワールド空間）</param>
    private void AlignChildByMoveParent(Transform parent, Transform child, Pose targetPose)
    {
        var rotationDiff = Quaternion.Inverse(child.rotation) * parent.rotation;
        parent.rotation = targetPose.rotation * rotationDiff;
        var positionDiff = parent.position - child.position;
        parent.position = targetPose.position + positionDiff;
    }

    // Update is called once per frame
    /// <summary>
    ///
    /// switch構文　semiAutoCmdの数値によって実行処理を変更。
    /// </summary>
    void Update()
    {
        if (sceneMaster.inputMng == null)
            controlMode_Text.text = "Opetator is not in this Room. \nPlease wait for Operator to join this room.";
        else
        {
            controlMode_Text.text = "ControlMode: " + (ControlMode)sceneMaster.inputMng.controlMode + "\n";
            if(sceneMaster.inputMng.controlMode==ControlMode.ManualControl)
                controlMode_Text.text += "ControlingTarget :" + (ManualCommands)sceneMaster.inputMng.manualCmd + "\n";
            else if(sceneMaster.inputMng.controlMode == ControlMode.SemiAutomaticControl)
                controlMode_Text.text += "Controling :" + (SemiAutomaticCommands)sceneMaster.inputMng.semiAutoCmd + "\n";
        }
            //範囲内ならば緑、範囲外なら赤にUIを変更する。
        if (visualIndicator != null)
        {
            float angle = Vector2.Angle(a_eg, b_aN);
            Vector3 direction = eeGripper.transform.position - armBaseLinkTf.position;
            bool isInRange = direction.magnitude < 0.55f && 90 > angle && eeGripper.transform.position.y > 0;
            visualIndicator.GetComponent<MeshRenderer>().material.color = isInRange ? new Color(0.2f, 1f, 0f, 0.2f) : new Color(1f, 0f, 0.5f, 0.2f);
        }

        if (sceneMaster.userSettings.role == Role.Robot||sceneMaster.userSettings.role==Role.Operator)
        {
            if (sceneMaster.photonMng.focusRobot!=null)
            {
                Vector3 b_a3 = new Vector3(b_aN.x, 0, b_aN.y);
                Vector3 b_a3Cross = Vector3.Cross(b_a3, Vector3.down);
                if (sceneMaster.inputMng != null)
                {
                    switch (sceneMaster.inputMng.semiAutoCmd)
                    {
                        case SemiAutomaticCommands.Available:
                        case SemiAutomaticCommands.Disable:
                            //不要なUIを削除
                            if (visualIndicator != null&& visualIndicator.GetPhotonView().IsMine) PhotonNetwork.Destroy(visualIndicator);
                            if (target != null&&target.GetPhotonView().IsMine) PhotonNetwork.Destroy(target);
                            if (goal != null && goal.GetPhotonView().IsMine) PhotonNetwork.Destroy(goal);
                            break;
                        case SemiAutomaticCommands.PlaceTarget:
                            //コントローラからの入力値でターゲットを移動・回転
                            if (target != null && target.GetPhotonView().IsMine)
                            {
                                pose.position = eeGripper.transform.position;
                                pose.rotation = eeGripper.transform.rotation;
                                //ロボットの進行方向に対してグリッパーが移動するように処理
                                if (sceneMaster.inputMng.targetZ > 0.5)
                                    pose.position += b_a3 * targetMoveSpeed * Time.deltaTime;
                                else if (sceneMaster.inputMng.targetZ < -0.5)
                                    pose.position -= b_a3 * targetMoveSpeed * Time.deltaTime;
                                if (sceneMaster.inputMng.targetX > 0.5)
                                    pose.position += b_a3Cross * targetMoveSpeed * Time.deltaTime;
                                else if (sceneMaster.inputMng.targetX < -0.5)
                                    pose.position -= b_a3Cross * targetMoveSpeed * Time.deltaTime;
                                if (sceneMaster.inputMng.targetY > 0.5)
                                    pose.position += Vector3.up * targetMoveSpeed * Time.deltaTime;
                                else if (sceneMaster.inputMng.targetY < -0.5)
                                    pose.position -= Vector3.up * targetMoveSpeed * Time.deltaTime;
                                //移動する場合、グリッパーがロボットの中心を向くように処理
                                if (sceneMaster.inputMng.targetX != 0 || sceneMaster.inputMng.targetY != 0 || sceneMaster.inputMng.targetZ != 0)
                                {
                                    // ターゲットへの向きベクトル計算
                                    var dir = (sceneMaster.rosConnector.arm_base_link.transform.position - pose.position).normalized;
                                    // ターゲットの方向への回転
                                    var lookAtRotation = Quaternion.LookRotation(-dir, Vector3.up);
                                    // 回転補正
                                    var offsetRotation = Quaternion.FromToRotation(-sceneMaster.inputMng.localArrow.defEef_egN, Vector3.forward);
                                    pose.rotation = lookAtRotation * offsetRotation;
                                }
                                //X軸方向の移動なしの場合、グリッパーのロールとピッチを指定して軌道推定が成功する可能性がある
                                //グリッパーのロールとピッチを動かすための処理
                                Quaternion xRot = Quaternion.AngleAxis(0, sceneMaster.inputMng.localArrow.curL_rN);
                                Quaternion zRot = Quaternion.AngleAxis(0, sceneMaster.inputMng.localArrow.curEef_egN);
                                if (sceneMaster.inputMng.eePitch > 0.5)
                                    xRot = Quaternion.AngleAxis(-1, sceneMaster.inputMng.localArrow.curL_rN);
                                else if (sceneMaster.inputMng.eePitch < -0.5)
                                    xRot = Quaternion.AngleAxis(1, sceneMaster.inputMng.localArrow.curL_rN);
                                if (sceneMaster.inputMng.eeRoll > 0.5)
                                    zRot = Quaternion.AngleAxis(-1, sceneMaster.inputMng.localArrow.curEef_egN);
                                else if (sceneMaster.inputMng.eeRoll < -0.5)
                                    zRot = Quaternion.AngleAxis(1, sceneMaster.inputMng.localArrow.curEef_egN);
                                pose.rotation = xRot * zRot * pose.rotation;
                                AlignChildByMoveParent(target.transform, eeGripper.transform, pose);
                                if (goal != null && goal.GetPhotonView().IsMine)
                                    PhotonNetwork.Destroy(goal);
                            }
                            break;
                        case SemiAutomaticCommands.PublishTarget:
                            break;
                        case SemiAutomaticCommands.PlaceGoal:
                            if (visualIndicator != null && visualIndicator.GetPhotonView().IsMine) PhotonNetwork.Destroy(visualIndicator);
                            if (target != null && target.GetPhotonView().IsMine) PhotonNetwork.Destroy(target);
                            if (goal != null && goal.GetPhotonView().IsMine)
                            {
                                if (sceneMaster.inputMng.targetZ > 0.5)
                                {
                                    goal.transform.position += b_a3 * goalMoveSpeed * Time.deltaTime;
                                    goal.transform.rotation = Quaternion.Euler(0,Vector3.Angle(Vector3.forward , b_a3),0);
                                }

                                else if (sceneMaster.inputMng.targetZ < -0.5)
                                {
                                    goal.transform.position -= b_a3 * goalMoveSpeed * Time.deltaTime;
                                    goal.transform.rotation = Quaternion.Euler(0, Vector3.Angle(Vector3.forward, b_a3), 0);
                                }
                                if (sceneMaster.inputMng.targetX > 0.5)
                                {
                                    goal.transform.position += b_a3Cross * goalMoveSpeed * Time.deltaTime;
                                    goal.transform.rotation = Quaternion.Euler(0, Vector3.Angle(Vector3.forward, b_a3), 0);
                                }
                                else if (sceneMaster.inputMng.targetX < -0.5)
                                {
                                    goal.transform.position -= b_a3Cross * goalMoveSpeed * Time.deltaTime;
                                    goal.transform.rotation = Quaternion.Euler(0, Vector3.Angle(Vector3.forward, b_a3), 0);
                                }
                                if (sceneMaster.inputMng.baseRotate > 0.5 || sceneMaster.inputMng.baseRotate < -0.5)
                                    goal.transform.Rotate(Vector3.up * Time.deltaTime * goalRotateSpeed * sceneMaster.inputMng.baseRotate * 30, Space.World);
                            }
                            break;
                        case SemiAutomaticCommands.PublishGoal:
                            break;
                    }
                }
            }
        }



        if (sceneMaster.userSettings.role != Role.Robot)
        {
            sceneMaster.uIMng.rosConnectButton.interactable = false;
            if(sceneMaster.photonMng.focusRobot != null)
            {
                var ram = sceneMaster.photonMng.focusRobot.GetComponent<RobotAvatarSetting>();
                rosConnection_Text.text = $"FocusRobotID: {ram.photonView.ViewID}" +
                    $"\nROS Network: {(RosConnection)ram.robotRosConnection}";
            }
            else
                rosConnection_Text.text = "FocusRobotID: 0\nROS Network: Disconnect";
        }
    }
}
