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
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

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
    private XRInputModalityManager xrInputModalityManager;
    [SerializeField]
    private List<ActionBasedControllerManager> actionBasedControllerManagers;
    [SerializeField]
    private List <ActionBasedController> actionBasedControllers;
    [SerializeField]
    private List <XRInteractionGroup> xrInteractionGroups;
    [SerializeField]
    TrackedPoseDriver trackedPoseDriver;
    [SerializeField]
    private GameObject[] vRComponents;
    [SerializeField]
    private GameObject[] nonVRComponents;
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            foreach (GameObject go in vRComponents)
            {
                go.SetActive(true);
            }
            foreach (GameObject go in nonVRComponents)
            {
                go.SetActive(false);
            }
            xrOrigin.enabled = true;
            eventSystem.enabled = true;
            xruiInputModule.enabled = true;
            xrInputModalityManager.enabled = true;
            trackedPoseDriver.enabled = true;
            foreach (var abcm in actionBasedControllerManagers)
            {
                abcm.enabled = true;
            }
            foreach (var abc in actionBasedControllers)
            {
                abc.enabled = true;
            }
            foreach (var xig in xrInteractionGroups)
            {
                xig.enabled = true;
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
            xrOrigin.enabled = false;
            eventSystem.enabled = false;
            xruiInputModule.enabled = false;
            xrInputModalityManager.enabled = false;
            trackedPoseDriver.enabled = false;
            foreach (var abcm in actionBasedControllerManagers)
            {
                abcm.enabled = false;
            }
            foreach (var abc in actionBasedControllers)
            {
                abc.enabled = false;
            }
            foreach (var xig in xrInteractionGroups)
            {
                xig.enabled = false;
            }

            meshRenderer.material = material;
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
}
