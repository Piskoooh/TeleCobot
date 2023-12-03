using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class VrAvatarSetting : AvatarSetting
{
    [SerializeField]
    private TeleportationProvider teleportationProvider;
    [SerializeField]
    private XROrigin xrOrigin;
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private XRUIInputModule xruiInputModule;
    [SerializeField]
    TrackedPoseDriver trackedPoseDriver;
    [SerializeField]
    private GameObject[] vRComponents;
    [SerializeField]
    private GameObject[] nonVRComponents;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            foreach (GameObject go in vRComponents)
            {
                go.SetActive(true);
            }
            eventSystem.enabled = true;
            xruiInputModule.enabled = true;
            trackedPoseDriver.enabled = true;
            foreach (GameObject go in nonVRComponents)
            {
                go.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject go in nonVRComponents)
            {
                go.SetActive(true);
            }
            foreach(GameObject go in vRComponents)
            {
                go.SetActive(false);
            }
            eventSystem.enabled = false;
            xruiInputModule.enabled = false;
            trackedPoseDriver.enabled = false;
        }
        // TeleportationProviderを各TeleportationAnchor、TeleportationAnchorに設定する
        // TeleportationAnchorとTeleportationAnchorが継承しているBaseTeleportationInteractableコンポーネントを取得し、設定。
        GameObject tp = GameObject.FindGameObjectWithTag("Teleport");
        if (tp != null)
        {
            BaseTeleportationInteractable[] btis = tp.GetComponentsInChildren<BaseTeleportationInteractable>();
            foreach (var bti in btis)
            {
                bti.teleportationProvider = teleportationProvider;
            }
        }

        if (photonView.IsMine)
        {
            GameObject.FindGameObjectWithTag("VRUIs").GetComponent<VRUIManager>().xrOrigin = xrOrigin.transform;
        }
        else
        {
            eventSystem.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
