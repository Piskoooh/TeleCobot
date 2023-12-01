using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VrAvatarSetting : AvatarSetting
{
    [SerializeField]
    private TeleportationProvider teleportationProvider;
    [SerializeField]
    private XROrigin xrOrigin;
    // Start is called before the first frame update
    void Start()
    {
        // TeleportationProviderを各TeleportationAnchor、TeleportationAnchorに設定する
        // TeleportationAnchorとTeleportationAnchorが継承しているBaseTeleportationInteractableコンポーネントを取得し、設定。
        GameObject go = GameObject.FindGameObjectWithTag("Teleport");
        if(go != null)
        {
            BaseTeleportationInteractable[] btis = go.GetComponentsInChildren<BaseTeleportationInteractable>();
            foreach(var bti in btis)
            {
                bti.teleportationProvider = teleportationProvider;
            }
        }

       var  vrUIs = GameObject.FindGameObjectWithTag("VRUIs");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
