using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VrAvatarSetting : AvatarSetting
{
    [SerializeField]
    private TeleportationProvider teleportationProvider;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
