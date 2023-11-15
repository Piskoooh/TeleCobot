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
    public GameObject target,visualIndicator;

    // Start is called before the first frame update
    void Start()
    {
        punConnection_Text.text = "Photon : Not Connected";
        rosConnection_Text.text = "Ros : Not Connected";
        punConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        rosConnectButton.GetComponentInChildren<TMP_Text>().text = "Connect";
        rosConnectButton.interactable = false;
    }

    public void VisualRange()
    {
        visualIndicator = Instantiate(visualIndicatorPrefab, pubUnityControl.arm_base_link.transform.position, Quaternion.Euler(0f, 0f, 0f));
        visualIndicator.transform.parent = pubUnityControl.arm_base_link.transform;
        visualIndicator.transform.localPosition = new Vector3(0f, 0f, 0f);
        visualIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void CreateOrResetTarget()
    {
        Vector3 armBaseLinkPosition = pubUnityControl.arm_base_link.transform.localPosition;
        Vector3 endEffectorPosition = pubUnityControl.endEffector.transform.localPosition;
        Vector3 direction = endEffectorPosition - armBaseLinkPosition;
        if (direction.magnitude <= 0.55f && endEffectorPosition.z > armBaseLinkPosition.z &&  pubUnityControl.endEffector.transform.position.y > 0 )
        {
            if (target == null)
            {
                target = Instantiate(targetPrefab, pubUnityControl.base_link.transform.position, Quaternion.Euler(0f, 0f, 0f)); //create
                target.transform.parent = pubUnityControl.base_link.transform;
                target.transform.localPosition = new Vector3(0f, 0.1f, 0.3f);
                target.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                target.transform.localPosition = new Vector3(0f, 0.1f, 0.3f);
                target.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        else
        {
            Debug.Log("End effector is outside the specified range or with invalid y value.\nDirection Magnitude: " + direction.magnitude + "\nReturn to Home Position.");
            inputMng.semiAutoCmd=SemiAutomaticCommands.Home;
        }
    }

    public void PickTarget()
    {
        if (target == null)
        {
            Debug.Log("Target Object does not exist.");
        }
        else
        {
            Vector3 armBaseLinkPosition = pubUnityControl.arm_base_link.transform.localPosition;
            Vector3 direction = target.transform.localPosition - armBaseLinkPosition;
            if (direction.magnitude <= 0.55f && target.transform.localPosition.z > armBaseLinkPosition.z && target.transform.position.y > 0)
            {
                pubUnityControl.PubMoveitPose();
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
                    Vector3 armBaseLinkPosition = pubUnityControl.arm_base_link.transform.localPosition;
                    Vector3 direction = target.transform.localPosition - armBaseLinkPosition;
                    if (direction.magnitude <= 0.55f && target.transform.localPosition.z > armBaseLinkPosition.z && target.transform.position.y > 0)
                    {
                        visualIndicator.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 1f, 0f, 0.2f);
                    }
                    else visualIndicator.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0.5f, 0.2f);
                }
                break;
            case SemiAutomaticCommands.PublishTarget:
                break;
        }
    }
}
