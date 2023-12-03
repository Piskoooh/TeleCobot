using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class VrAvatarSetting : AvatarSetting
{
    [SerializeField]
    private TeleportationProvider teleportationProvider;
    [SerializeField]
    private XROrigin xrOrigin;
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private GameObject gazeInteractor;

    // Start is called before the first frame update
    void Start()
    {
        // TeleportationProviderを各TeleportationAnchor、TeleportationAnchorに設定する
        // TeleportationAnchorとTeleportationAnchorが継承しているBaseTeleportationInteractableコンポーネントを取得し、設定。
        GameObject go = GameObject.FindGameObjectWithTag("Teleport");
        if (go != null)
        {
            BaseTeleportationInteractable[] btis = go.GetComponentsInChildren<BaseTeleportationInteractable>();
            foreach (var bti in btis)
            {
                bti.teleportationProvider = teleportationProvider;
            }
        }

        if (photonView.IsMine)
        {
            GameObject.FindGameObjectWithTag("VRUIs").GetComponent<VRUIManager>().xrOrigin = xrOrigin.transform;
            eventSystem.enabled = true;
        }
        else
        {
            eventSystem.enabled = false;
            gazeInteractor.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
