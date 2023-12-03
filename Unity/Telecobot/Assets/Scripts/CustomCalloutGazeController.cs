using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Unity.VRTemplate;

/// <summary>
/// Fires events when this object is is within the field of view of the gaze transform. This is currently used to
/// hide and show tooltip callouts on the controllers when the controllers are within the field of view.
/// </summary>
public class CustomCalloutGazeController : CalloutGazeController
{
    public Callout[] callouts;

    public void RegisterUnityEvents()
    {
        foreach (var item in callouts)
        {
            m_FacingEntered.AddListener(item.GazeHoverStart);
            m_FacingExited.AddListener(item.GazeHoverEnd);
        }
    }

}
