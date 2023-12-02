using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerModelSelector : MonoBehaviour
{
    [SerializeField] private ActionBasedController LeftController;
    [SerializeField] private ActionBasedController RightController;

    [SerializeField] private CustomCalloutGazeController LeftCalloutGazeController;
    [SerializeField] private CustomCalloutGazeController RightCalloutGazeController;

    [SerializeField] private Transform OculusTouchController;
    [SerializeField] private Transform OculusTouchRightController;

    [SerializeField] private Transform ViveLeftController;
    [SerializeField] private Transform ViveRightController;

    [SerializeField] private Transform Pico4LeftController;
    [SerializeField] private Transform Pico4RightController;

    [SerializeField]
    private Transform UniversalLeftController;
    [SerializeField]
    private Transform UniversalRightController;

    private Transform InstatiatedLeftController;
    private Transform InstatiatedRightController;

    private UserSettings userSettings;
    bool flag;

    private void OnEnable()
    {
        flag = true;
    }
    private void Awake()
    {
        userSettings = GameObject.FindGameObjectWithTag("SceneMaster").GetComponent<SceneMaster>().userSettings;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(userSettings.controllerType == VRControllerType.OculusTouch)
        {
            LeftController.modelPrefab = OculusTouchController;
            RightController.modelPrefab = OculusTouchRightController;
        }
        else if(userSettings.controllerType == VRControllerType.Vive)
        {
            LeftController.modelPrefab = ViveLeftController;
            RightController.modelPrefab = ViveRightController;
        }
        else if(userSettings.controllerType == VRControllerType.Pico4)
        {
            LeftController.modelPrefab = Pico4LeftController;
            RightController.modelPrefab = Pico4RightController;
        }
        else if(userSettings.controllerType == VRControllerType.Universal)
        {
            LeftController.modelPrefab = UniversalLeftController;
            RightController.modelPrefab = UniversalRightController;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (flag == false)
            return;
        else if (InstatiatedLeftController == null)
        {
            var go = GameObject.FindGameObjectWithTag("LeftControllerPrefab");
            if (go != null)
                InstatiatedLeftController = go.transform;
        }
        else if(InstatiatedRightController == null)
        {
            var go = GameObject.FindGameObjectWithTag("RightControllerPrefab");
            if (go != null)
                InstatiatedRightController = go.transform;
        }
        if (InstatiatedLeftController != null && InstatiatedRightController != null && flag)
        {
            LeftCalloutGazeController.callouts = LeftController.GetComponentsInChildren<Callout>();
            RightCalloutGazeController.callouts = RightController.GetComponentsInChildren<Callout>();

            LeftCalloutGazeController.RegisterUnityEvents();
            RightCalloutGazeController.RegisterUnityEvents();
            Debug.LogWarning("Registered Callout UnityEvents");

            flag = false;
        }
    }
}
