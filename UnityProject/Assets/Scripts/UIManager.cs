using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public PubUnityControl pubUnityControl;
    public InputManager inputMng;

    public TMP_Text controlMode_Text, punConnection_Text, rosConnection_Text;
    public Button punConnectButton, rosConnectButton;

    //シーン上に生成するUIのプレハブ
    public GameObject targetPrefab;
    public GameObject visualIndicatorPrefab;
    [HideInInspector]
    public GameObject visualIndicator, eeGripper;
    GameObject target;
    Pose pose;

    // Start is called before the first frame update
    void Start()
    {
        punConnection_Text.text = "Photon : Not Connected";
        rosConnection_Text.text = "Ros : Not Connected";
        punConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        rosConnectButton.interactable = false;
        pose = new Pose();
    }

    /// <summary>
    /// アームがリーチ可能な範囲を視覚化。ロボットの子オブジェクトとすることでロボットが移動しても追従。
    /// </summary>
    public void VisualRange()
    {
        visualIndicator = Instantiate(visualIndicatorPrefab, pubUnityControl.arm_base_link.transform.position, Quaternion.Euler(0f, 0f, 0f));
        visualIndicator.transform.parent = pubUnityControl.arm_base_link.transform;
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
        var baseLinkTf = pubUnityControl.base_link.transform;
        var armBaseLinkTf = pubUnityControl.arm_base_link.transform;
        var endEffectorTf = pubUnityControl.endEffector.transform;

        Vector2 b_a = new Vector2(armBaseLinkTf.transform.position.x - baseLinkTf.transform.position.x, armBaseLinkTf.transform.position.z - baseLinkTf.transform.position.z);
        Vector2 b_aN = b_a.normalized;
        Vector2 e_a = new Vector2(endEffectorTf.transform.position.x - armBaseLinkTf.transform.position.x, endEffectorTf.transform.position.z - armBaseLinkTf.transform.position.z);

        float angle = Vector2.Angle(e_a, b_aN);

        Vector3 direction = endEffectorTf.position - armBaseLinkTf.position;
        if (direction.magnitude < 0.55f && 90 > angle &&  pubUnityControl.endEffector.transform.position.y > 0 )
        {
            if (target == null)
            {
                target = Instantiate(targetPrefab, baseLinkTf.position, Quaternion.Euler(0f, 0f, 0f)); //create
                target.transform.parent = baseLinkTf;
                eeGripper = GameObject.FindGameObjectWithTag("end_effector"); //gripperプレハブを使う時
            }
            //targetプレハブを使う時
            //target.transform.localPosition = new Vector3(0f, 0.1f, 0.3f);
            //target.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            //gripperプレハブを使う時
            Pose resetPose;
            resetPose.position = new Vector3(baseLinkTf.position.x + 0.5f * b_aN.x, baseLinkTf.position.y + 0.1f, baseLinkTf.position.z + 0.5f * b_aN.y);
            resetPose.rotation = baseLinkTf.rotation;
            pose = new Pose(resetPose.position,resetPose.rotation);
        }
        else
        {
            Debug.Log("End effector is outside the specified range or with invalid y value.\nDirection Magnitude: " + direction.magnitude + "\nReturn to Home Position.");
            inputMng.semiAutoCmd = SemiAutomaticCommands.Home;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void PickTarget()
    {
        if (target == null)
        {
            Debug.Log("Target Object does not exist.");
        }
        else
        {
            var baseLinkTf = pubUnityControl.base_link.transform;
            var armBaseLinkTf = pubUnityControl.arm_base_link.transform;

            Vector2 b_a = new Vector2(armBaseLinkTf.transform.position.x - baseLinkTf.transform.position.x, armBaseLinkTf.transform.position.z - baseLinkTf.transform.position.z);
            Vector2 b_aN = b_a.normalized;
            Vector2 t_a = new Vector2(eeGripper.transform.position.x - armBaseLinkTf.transform.position.x, eeGripper.transform.position.z - armBaseLinkTf.transform.position.z);

            float angle = Vector2.Angle(t_a, b_aN);

            Vector3 direction = eeGripper.transform.position - armBaseLinkTf.position;
            if (direction.magnitude < 0.55f && 90 > angle && eeGripper.transform.position.y > 0)
            {
                //pubUnityControl.PubMoveitPose();
                pubUnityControl.PubTargetPose();
                inputMng.semiAutoCmd = SemiAutomaticCommands.Available;
                return;
            }
            else
            {
                Debug.Log("Cannot set target outside the specified range or with invalid y value.\nDirection Magnitude: " + direction.magnitude);
            }
        }
        CreateOrResetTarget();
        inputMng.semiAutoCmd = SemiAutomaticCommands.PlaceTarget;
    }


    /// <summary>
    /// childの座標と回転がtargetPoseと一致するように、parentを移動。
    /// parentとchildの間に他のTransformがあっても動作するが、parentとchildは実際に親子関係である必要があり。
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
    void Update()
    {
        controlMode_Text.text = "ControlMode: "+playerInput.currentActionMap.name;

        switch (inputMng.semiAutoCmd)
        {
            case SemiAutomaticCommands.Available:
            case SemiAutomaticCommands.Disable:
            case SemiAutomaticCommands.Sleep:
            case SemiAutomaticCommands.Home:
                if (target != null) Destroy(target);
                if (visualIndicator != null) Destroy(visualIndicator);
                
                break;
            case SemiAutomaticCommands.PlaceTarget:
                if (visualIndicator != null)
                {
                    var baseLinkTf = pubUnityControl.base_link.transform;
                    var armBaseLinkTf = pubUnityControl.arm_base_link.transform;

                    Vector2 b_a = new Vector2(armBaseLinkTf.transform.position.x - baseLinkTf.transform.position.x, armBaseLinkTf.transform.position.z - baseLinkTf.transform.position.z);
                    Vector2 b_aN = b_a.normalized;
                    Vector2 t_a = new Vector2(eeGripper.transform.position.x - armBaseLinkTf.transform.position.x, eeGripper.transform.position.z - armBaseLinkTf.transform.position.z);

                    float angle = Vector2.Angle(t_a, b_aN);

                    Vector3 armBaseLinkPosition = pubUnityControl.arm_base_link.transform.position;
                    Vector3 direction = eeGripper.transform.position - armBaseLinkPosition;
                    if (direction.magnitude < 0.55f && 90 > angle && eeGripper.transform.position.y > 0)
                    {
                        visualIndicator.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 1f, 0f, 0.2f);
                    }
                    else visualIndicator.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0.5f, 0.2f);
                }

                if (target != null)
                {
                    AlignChildByMoveParent(target.transform, eeGripper.transform, pose);
                }
                break;
            case SemiAutomaticCommands.PublishTarget:
                break;
        }
    }
}
