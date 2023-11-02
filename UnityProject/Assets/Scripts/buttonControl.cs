using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class buttonControl : MonoBehaviour
{
    public Button sleepButton, homeButton, trackButton, pickButton;
    public CanvasGroup armUI, baseUI;
    public PubTelecobotArmControl pubTelecobotArmControl;

    public GameObject targetPrefab;
    public GameObject visualIndicatorPrefab;

    private GameObject targetObject;
    private GameObject visualIndicator;
    

    // Start is called before the first frame update
    void Start()
    {
        sleepButton.onClick.AddListener(() => slpbtn());
        homeButton.onClick.AddListener(() => hombtn());
        trackButton.onClick.AddListener(() => trcbtn());
        pickButton.onClick.AddListener(() => pictrgbtn());

    }
    //?????????????????
    void slpbtn()
    {
        pubTelecobotArmControl.armControlMode = ArmControlMode.Sleep;
    }

    void hombtn()
    {
        pubTelecobotArmControl.armControlMode = ArmControlMode.Home;
    }

    void trcbtn()
    {
        pubTelecobotArmControl.armControlMode = ArmControlMode.PlaceTarget;
        CreateOrResetTargetObject();
        if (visualIndicator == null)
        {
            VisualRange();
        }
    }

    void pictrgbtn()
    {
        pubTelecobotArmControl.armControlMode = ArmControlMode.PublishTarget;
        PickTarget();
    }

    void VisualRange()
    {
        // ??????????Cube???????????
        visualIndicator = Instantiate(visualIndicatorPrefab, pubTelecobotArmControl.armBaseLink.transform.position, Quaternion.Euler(0f,0f,0f));
        // armBaseLinkObject?????????????
        visualIndicator.transform.parent = pubTelecobotArmControl.armBaseLink.transform;
        // ?????????????
        visualIndicator.transform.localPosition = new Vector3(0f, 0f, 0f); // ?????
        visualIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void CreateOrResetTargetObject()
    {
        Vector3 armBaseLinkPosition = pubTelecobotArmControl.armBaseLink.transform.localPosition;
        Vector3 endEffectorPosition = pubTelecobotArmControl.endEffector.transform.localPosition;
        Vector3 direction = endEffectorPosition - armBaseLinkPosition;
        if (direction.magnitude <= 0.5f && endEffectorPosition.z > armBaseLinkPosition.z /*&&  endEffectorPosition.y > 0*/ )
        {
            if (targetObject == null)
            {
                targetObject = Instantiate(targetPrefab, pubTelecobotArmControl.armBaseLink.transform.position, Quaternion.Euler(0f, 0f, 0f)); //create
                targetObject.transform.parent = pubTelecobotArmControl.armBaseLink.transform;
                targetObject.transform.localPosition = new Vector3(0f, 0.1f, 0.3f);
                targetObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                targetObject.transform.localPosition = new Vector3(0f, 0.1f, 0.3f);
                targetObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            trackButton.GetComponentInChildren<TMP_Text>().text = "Reset Target";
        }
        else
        {
            Debug.Log("End effector is outside the specified range or with invalid y value.\nDirection Magnitude: " + direction.magnitude+"\nReturn to Home Position.");
            hombtn();
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
            Vector3 armBaseLinkPosition = pubTelecobotArmControl.armBaseLink.transform.localPosition;
            Vector3 direction = targetObject.transform.localPosition - armBaseLinkPosition;
            if (direction.magnitude <= 0.5f && targetObject.transform.localPosition.z > armBaseLinkPosition.z /*&& targetObject.transform.position.y > 0*/)
            {
                pubTelecobotArmControl.target = targetObject.transform;
                pubTelecobotArmControl.PublishTransform();
                trcbtn();
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
        switch (pubTelecobotArmControl.armControlMode)
        {
            case ArmControlMode.Sleep:
                sleepButton.interactable = false;
                homeButton.interactable = true;
                trackButton.interactable = false;
                pickButton.interactable = false;
                baseUI.interactable = true;
                if (targetObject != null) Destroy(targetObject);
                if (visualIndicator != null) Destroy(visualIndicator);
                trackButton.GetComponentInChildren<TMP_Text>().text = "Track \nMode";
                break;
            case ArmControlMode.Home:
                sleepButton.interactable = true;
                homeButton.interactable = false;
                trackButton.interactable = true;
                pickButton.interactable = false;
                baseUI.interactable = false;
                if (targetObject != null) Destroy(targetObject);
                if (visualIndicator != null) Destroy(visualIndicator);
                trackButton.GetComponentInChildren<TMP_Text>().text = "Track \nMode";
                break;
            case ArmControlMode.PlaceTarget:
                sleepButton.interactable = true;
                homeButton.interactable = true;
                trackButton.interactable = true;
                pickButton.interactable = true;
                baseUI.interactable = false;
                if (visualIndicator != null)
                {
                    Vector3 armBaseLinkPosition = pubTelecobotArmControl.armBaseLink.transform.localPosition;
                    Vector3 direction = targetObject.transform.localPosition - armBaseLinkPosition;
                    if (direction.magnitude <= 0.5f && targetObject.transform.localPosition.z > armBaseLinkPosition.z /*&& targetObject.transform.position.y > 0*/)
                    {
                        visualIndicator.GetComponent<MeshRenderer>().material.color = new Color(0.2f, 1f, 0f, 0.2f);
                    }
                    else visualIndicator.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0.5f, 0.2f);
                }
                break;
            case ArmControlMode.PublishTarget:
                sleepButton.interactable = true;
                homeButton.interactable = true;
                trackButton.interactable = true;
                pickButton.interactable = true;
                baseUI.interactable = false;
                break;
        }

    }
}
