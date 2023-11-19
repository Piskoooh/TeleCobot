using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Canvas配下のButtonを制御
//
public class buttonControl : MonoBehaviour
{
    public Button sleepButton, homeButton, setPoseButton, pubPoseButton, setGoalButton, pubGoalButton,
        enableTrqButton, disableTrqButton, rebootErrorButton, rebootAllButton, resetPanTiltButton;
    public CanvasGroup armUI, baseUI, coreUI, turretUI;

    public PubUnityControl pubUnityControl;

    //シーン上に生成するUIのプレハブ
    public GameObject targetPrefab;
    public GameObject visualIndicatorPrefab;
    private GameObject targetObject;
    private GameObject visualIndicator;

    //各モードを名称で管理できるようにするようにenumを定義。
    private SemiAutomaticCommands semiAutoCmd;
    
    // Start is called before the first frame update
    void Start()
    {
        //ボタンが押された際にコールバックする関数を登録。
        //コールバック：関数が実行された際にが呼び出される関数
        //今回の場合ボタンを押されたよと報告するUnityEngine.UIに用意されている関数OnClickが実行された際に
        //このスクリプトで定義している関数を実行するようにここで登録(AddListner())している。
        sleepButton.onClick.AddListener(() => sleepbtnPressed());
        homeButton.onClick.AddListener(() => homebtnPressed());
        setPoseButton.onClick.AddListener(() => setPosebtnPressed());
        pubPoseButton.onClick.AddListener(() => pubPosebtnPressed());
    }
    
    void sleepbtnPressed()
    {
        semiAutoCmd = SemiAutomaticCommands.Sleep;
    }

    void homebtnPressed()
    {
        semiAutoCmd = SemiAutomaticCommands.Home;

    }

    void setPosebtnPressed()
    {
        semiAutoCmd = SemiAutomaticCommands.PlaceTarget;
        CreateOrResetTargetObject();
        if (visualIndicator == null)
        {
            VisualRange();
        }
    }

    void pubPosebtnPressed()
    {
        semiAutoCmd = SemiAutomaticCommands.PublishTarget;
        PickTarget();
    }

    void VisualRange()
    {
        visualIndicator = Instantiate(visualIndicatorPrefab, pubUnityControl.arm_base_link.transform.position, Quaternion.Euler(0f,0f,0f));
        visualIndicator.transform.parent = pubUnityControl.arm_base_link.transform;
        visualIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
        visualIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void CreateOrResetTargetObject()
    {
        Vector3 armBaseLinkPosition = pubUnityControl.arm_base_link.transform.localPosition;
        Vector3 endEffectorPosition = pubUnityControl.endEffector.transform.localPosition;
        Vector3 direction = endEffectorPosition - armBaseLinkPosition;
        if (direction.magnitude <= 0.5f && endEffectorPosition.z > armBaseLinkPosition.z /*&&  endEffectorPosition.y > 0*/ )
        {
            if (targetObject == null)
            {
                targetObject = Instantiate(targetPrefab, pubUnityControl.arm_base_link.transform.position, Quaternion.Euler(0f, 0f, 0f)); //create
                targetObject.transform.parent = pubUnityControl.arm_base_link.transform;
                targetObject.transform.localPosition = new Vector3(0f, 0.1f, 0.3f);
                targetObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                targetObject.transform.localPosition = new Vector3(0f, 0.1f, 0.3f);
                targetObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            setPoseButton.GetComponentInChildren<TMP_Text>().text = "Reset Target";
        }
        else
        {
            Debug.Log("End effector is outside the specified range or with invalid y value.\nDirection Magnitude: " + direction.magnitude+"\nReturn to Home Position.");
            homebtnPressed();
        }
    }

    void PickTarget()
    {
        if (targetObject == null)
        {
            Debug.Log("Target Object does not exist.");
        }
        else
        {
            Vector3 armBaseLinkPosition = pubUnityControl.arm_base_link.transform.localPosition;
            Vector3 direction = targetObject.transform.localPosition - armBaseLinkPosition;
            if (direction.magnitude <= 0.5f && targetObject.transform.localPosition.z > armBaseLinkPosition.z /*&& targetObject.transform.position.y > 0*/)
            {
                var target = targetObject.transform;
                pubUnityControl.SetMoveitPose();
                setPosebtnPressed();
                return;
            }
            else
            {
                Debug.Log("Cannot set target outside the specified range or with invalid y value.\nDirection Magnitude: " + direction.magnitude);
            }
        }
        CreateOrResetTargetObject();

    }


    //Update?????interactable???
    private void Update()
    {
        switch (semiAutoCmd)
        {
            case SemiAutomaticCommands.Sleep:
                sleepButton.interactable = false;
                homeButton.interactable = true;
                setPoseButton.interactable = false;
                pubPoseButton.interactable = false;
                baseUI.interactable = true;
                if (targetObject != null) Destroy(targetObject);
                if (visualIndicator != null) Destroy(visualIndicator);
                setPoseButton.GetComponentInChildren<TMP_Text>().text = "Track \nMode";
                break;
            case SemiAutomaticCommands.Home:
                sleepButton.interactable = true;
                homeButton.interactable = false;
                setPoseButton.interactable = true;
                pubPoseButton.interactable = false;
                baseUI.interactable = false;
                if (targetObject != null) Destroy(targetObject);
                if (visualIndicator != null) Destroy(visualIndicator);
                setPoseButton.GetComponentInChildren<TMP_Text>().text = "Track \nMode";
                break;
            case SemiAutomaticCommands.PlaceTarget:
                sleepButton.interactable = true;
                homeButton.interactable = true;
                setPoseButton.interactable = true;
                pubPoseButton.interactable = true;
                baseUI.interactable = false;
                if (visualIndicator != null)
                {
                    Vector3 armBaseLinkPosition = pubUnityControl.arm_base_link.transform.localPosition;
                    Vector3 direction = targetObject.transform.localPosition - armBaseLinkPosition;
                    if (direction.magnitude <= 0.5f && targetObject.transform.localPosition.z > armBaseLinkPosition.z /*&& targetObject.transform.position.y > 0*/)
                    {
                        visualIndicator.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 1f, 0f, 0.2f);
                    }
                    else visualIndicator.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0.5f, 0.2f);
                }
                break;
            case SemiAutomaticCommands.PublishTarget:
                sleepButton.interactable = true;
                homeButton.interactable = true;
                setPoseButton.interactable = true;
                pubPoseButton.interactable = true;
                baseUI.interactable = false;
                break;
        }
    }
}
