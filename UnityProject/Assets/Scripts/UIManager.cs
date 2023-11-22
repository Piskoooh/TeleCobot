﻿using System.Collections;
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
    public PlayerInput playerInput;
    public PubUnityControl pubUnityControl;
    public InputManager inputMng;
    public RosConnector rosConnector;

    //ネットワーク関連のUI
    public TMP_Text controlMode_Text, punConnection_Text, rosConnection_Text;
    public Button punConnectButton, rosConnectButton;

    //ロボットアームのターゲット指定に関連するUI
    //シーン上に生成するUIのプレハブ
    public GameObject targetPrefab;
    public GameObject visualIndicatorPrefab;
    [HideInInspector]
    public GameObject visualIndicator, eeGripper;
    [HideInInspector]
    public LocalArrow localArrow;
    public float targetMoveSpeed = 0.5f;
    GameObject target;
    Pose pose;

    //ロボットアームのターゲット指定で使用する読み取り専用の数値
    Transform baseLinkTf => rosConnector.base_link.transform;
    Transform armBaseLinkTf => rosConnector.arm_base_link.transform;
    Transform endEffectorTf => rosConnector.endEffector.transform;
    Vector2 b_a => new Vector2(armBaseLinkTf.transform.position.x - baseLinkTf.transform.position.x, armBaseLinkTf.transform.position.z - baseLinkTf.transform.position.z);
    Vector2 b_aN => b_a.normalized;
    Vector2 a_ee => new Vector2(endEffectorTf.transform.position.x - armBaseLinkTf.transform.position.x, endEffectorTf.transform.position.z - armBaseLinkTf.transform.position.z);
    Vector2 a_eg => new Vector2(eeGripper.transform.position.x - armBaseLinkTf.transform.position.x, eeGripper.transform.position.z - armBaseLinkTf.transform.position.z);

    //ロボットベースのゴール指定に関連するUI
    //シーン上に生成するUI
    public GameObject goalPrefab;
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
        visualIndicator = Instantiate(visualIndicatorPrefab, rosConnector.arm_base_link.transform.position, Quaternion.Euler(0f, 0f, 0f));
        visualIndicator.transform.parent = rosConnector.arm_base_link.transform;
        visualIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
        visualIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
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
        float angle = Vector2.Angle(a_ee, b_aN);
        Vector3 direction = endEffectorTf.position - armBaseLinkTf.position;
        if (direction.magnitude < 0.55f && 90 > angle && rosConnector.endEffector.transform.position.y > 0 )
        {
            if (target == null)
            {
                target = Instantiate(targetPrefab, endEffectorTf.position, Quaternion.Euler(0f, 0f, 0f)); //create
                target.transform.parent = baseLinkTf;
                eeGripper = GameObject.FindGameObjectWithTag("end_effector"); //gripperプレハブを使う時
            }

            //gripperプレハブを使う時
            Pose resetPose;
            //resetPose.position = new Vector3(baseLinkTf.position.x + 0.5f * b_aN.x, baseLinkTf.position.y + 0.1f, baseLinkTf.position.z + 0.5f * b_aN.y);
            //resetPose.rotation = baseLinkTf.rotation;
            resetPose.position = rosConnector.endEffector.transform.position;
            resetPose.rotation = rosConnector.endEffector.transform.rotation;
            pose = new Pose(resetPose.position,resetPose.rotation);
            AlignChildByMoveParent(target.transform, eeGripper.transform, pose);
        }
        else
        {
            Debug.LogWarning($"End effector is outside the specified range or with invalid y value.\n" +
            $"Direction Magnitude: {direction.magnitude}, Angle: {angle}, Gripper Position Y: {endEffectorTf.position.y}" +
            $"\nReset arm to 'Home Position'.");

            inputMng.semiAutoCmd = SemiAutomaticCommands.Available;
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
        inputMng.semiAutoCmd = SemiAutomaticCommands.PlaceTarget;
    }

    public void CreateOrResetGoal()
    {
        if (goal == null)
        {
            goal = Instantiate(goalPrefab, baseLinkTf.position, Quaternion.Euler(0f, 0f, 0f)); //create
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
        inputMng.semiAutoCmd = SemiAutomaticCommands.PlaceGoal;
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
        controlMode_Text.text = "ControlMode: "+playerInput.currentActionMap.name;
        if (rosConnector.rosConnection == RosConnection.Connect)
        {
            Vector3 b_a3 = new Vector3(b_aN.x, 0, b_aN.y);
            Vector3 b_a3Cross = Vector3.Cross(b_a3, Vector3.down);
            switch (inputMng.semiAutoCmd)
            {
                //不要なUIを削除
                case SemiAutomaticCommands.Available:
                case SemiAutomaticCommands.Disable:
                    if (visualIndicator != null) Destroy(visualIndicator);
                    if (target != null) Destroy(target);
                    if (goal != null) Destroy(goal);
                    break;
                case SemiAutomaticCommands.PlaceTarget:
                    //範囲内ならば緑、範囲外なら赤にUIを変更する。
                    if (visualIndicator != null)
                    {
                        float angle = Vector2.Angle(a_eg, b_aN);
                        Vector3 direction = eeGripper.transform.position - armBaseLinkTf.position;
                        bool isInRange = direction.magnitude < 0.55f && 90 > angle && eeGripper.transform.position.y > 0;
                        visualIndicator.GetComponent<MeshRenderer>().material.color = isInRange ? new Color(0.2f, 1f, 0f, 0.2f) : new Color(1f, 0f, 0.5f, 0.2f);
                    }
                    //コントローラからの入力値でターゲットを移動・回転
                    if (target != null)
                    {
                        pose.position = eeGripper.transform.position;
                        pose.rotation = eeGripper.transform.rotation;
                        //ロボットの進行方向に対してグリッパーが移動するように処理
                        if (inputMng.targetZ > 0.5)
                            pose.position += b_a3 * targetMoveSpeed * Time.deltaTime;
                        else if (inputMng.targetZ < -0.5)
                            pose.position -= b_a3 * targetMoveSpeed * Time.deltaTime;
                        if (inputMng.targetX > 0.5)
                            pose.position += b_a3Cross * targetMoveSpeed * Time.deltaTime;
                        else if (inputMng.targetX < -0.5)
                            pose.position -= b_a3Cross * targetMoveSpeed * Time.deltaTime;
                        if (inputMng.targetY > 0.5)
                            pose.position += Vector3.up * targetMoveSpeed * Time.deltaTime;
                        else if (inputMng.targetY < -0.5)
                            pose.position -= Vector3.up * targetMoveSpeed * Time.deltaTime;
                        //移動する場合、グリッパーがロボットの中心を向くように処理
                        if (inputMng.targetX != 0 || inputMng.targetY != 0 || inputMng.targetZ != 0)
                        {
                            // ターゲットへの向きベクトル計算
                            var dir = (rosConnector.arm_base_link.transform.position - pose.position).normalized;
                            // ターゲットの方向への回転
                            var lookAtRotation = Quaternion.LookRotation(-dir, Vector3.up);
                            // 回転補正
                            var offsetRotation = Quaternion.FromToRotation(-localArrow.defEef_egN,Vector3.forward);
                            pose.rotation = lookAtRotation * offsetRotation;
                        }
                        //X軸方向の移動なしの場合、グリッパーのロールとピッチを指定して軌道推定が成功する可能性がある
                        //グリッパーのロールとピッチを動かすための処理
                        Quaternion xRot = Quaternion.AngleAxis(0, localArrow.curL_rN);
                        Quaternion zRot = Quaternion.AngleAxis(0, localArrow.curEef_egN);
                        if (inputMng.eePitch > 0.5)
                            xRot = Quaternion.AngleAxis(-1, localArrow.curL_rN);
                        else if (inputMng.eePitch < -0.5)
                            xRot = Quaternion.AngleAxis(1, localArrow.curL_rN);
                        if (inputMng.eeRoll > 0.5)
                            zRot = Quaternion.AngleAxis(1, localArrow.curEef_egN);
                        else if (inputMng.eeRoll < -0.5)
                            zRot = Quaternion.AngleAxis(-1, localArrow.curEef_egN);
                        pose.rotation = xRot * zRot * pose.rotation;
                        AlignChildByMoveParent(target.transform, eeGripper.transform, pose);
                    }
                    if (goal != null) Destroy(goal);
                    break;
                case SemiAutomaticCommands.PublishTarget:
                    break;
                case SemiAutomaticCommands.PlaceGoal:
                    if (visualIndicator != null) Destroy(visualIndicator);
                    if (target != null) Destroy(target);
                    if (goal != null)
                    {
                        if (inputMng.targetZ > 0.5)
                        {
                            goal.transform.position += b_a3 * goalMoveSpeed * Time.deltaTime;
                            goal.transform.rotation = Quaternion.Euler(b_a3);
                        }

                        else if (inputMng.targetZ < -0.5)
                        {
                            goal.transform.position -= b_a3 * goalMoveSpeed * Time.deltaTime;
                            goal.transform.rotation = Quaternion.Euler(b_a3);
                        }
                        if (inputMng.targetX > 0.5)
                        {
                            goal.transform.position += b_a3Cross * goalMoveSpeed * Time.deltaTime;
                            goal.transform.rotation = Quaternion.Euler(b_a3);
                        }
                        else if (inputMng.targetX < -0.5)
                        {
                            goal.transform.position -= b_a3Cross * goalMoveSpeed * Time.deltaTime;
                            goal.transform.rotation = Quaternion.Euler(b_a3);
                        }
                        if (inputMng.baseRotate > 0.5 || inputMng.baseRotate < -0.5)
                            goal.transform.Rotate(Vector3.up * Time.deltaTime * goalRotateSpeed * inputMng.baseRotate * 30, Space.World);
                    }
                    break;
                case SemiAutomaticCommands.PublishGoal:
                    break;
            }
        }
    }
    /// <summary>
    /// グリッパーのロールとピッチを指定するための回転の基準となる軸の方向を取得する。
    /// InputSystemのロールとピッチのアクションにコールバックとして呼び出し設定。
    /// </summary>
    /// <param name="context"></param>
    public void OnEeCall(InputAction.CallbackContext context)
    {
        Debug.Log("OnEeCall called");
        if (localArrow)
        {
            if (context.started)
            {
                localArrow.UpdateEeArrow();
            }
        }
    }
}
