using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeTextureAlpha : MonoBehaviour
{
    GameObject[] objects;

    [SerializeField]
    List<LocomotionProvider> m_LocomotionProviders = new List<LocomotionProvider>();
    public void callSetAlpha0()
    {
        objects = GameObject.FindGameObjectsWithTag("VideoSurface");
        foreach (GameObject obj in objects)
        {
            var tac = obj.GetComponent<TextureAlphaControl>();
            if (tac == null)
                continue;
            tac.SetAlpha(true);
        }
    }

    public void callSetAlpha1()
    {
        objects = GameObject.FindGameObjectsWithTag("VideoSurface");
        foreach (GameObject obj in objects)
        {
            var tac= obj.GetComponent<TextureAlphaControl>();
            if (tac == null)
                continue;
            tac.SetAlpha(false);
        }
    }

    private void Update()
    {
                if (m_LocomotionProviders.Count > 0)
            {
                foreach (var locomotionProvider in m_LocomotionProviders)
                {
                    if (locomotionProvider == null)
                        continue;

                    switch (locomotionProvider.locomotionPhase)
                    {
                        case LocomotionPhase.Started:
                        case LocomotionPhase.Moving:
                            callSetAlpha0();
                            break;
                        case LocomotionPhase.Done:
                            callSetAlpha1();
                            break;
                    }
                }
            }
    }
}
