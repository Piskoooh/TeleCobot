using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerIdentifier : MonoBehaviour
{
    public UserSettings userSettings;
    void Start()
    {
        // ビルドした実行ファイルで、起動時にVRコントローラーがアクティブになっている場合は
        // ここで検出される（onDeviceChangeは呼ばれない）。
        foreach (var device in InputSystem.devices)
        {
            IdentifyDevice(device);
        }
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void OnDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
    {
        if (inputDeviceChange == InputDeviceChange.Added)
        {
            // エディタで再生した場合、またはアプリを起動してから
            // VRコントローラーをアクティブにした場合はここで検出される。
            IdentifyDevice(inputDevice);
        }
    }

    void IdentifyDevice(InputDevice inputDevice)
    {
        // VRコントローラーかどうかをチェックする。
        if (!inputDevice.usages.Contains(CommonUsages.LeftHand) &&
            !inputDevice.usages.Contains(CommonUsages.RightHand)) return;

        if (inputDevice.displayName.Contains("Vive"))
        {
            userSettings.VRControllerType = VRControllerType.Vive;
        }
        else if (inputDevice.displayName.Contains("Touch"))
        {
            userSettings.VRControllerType = VRControllerType.OculusTouch;
        }
        else if (inputDevice.displayName.Contains("Pico"))
        {
            userSettings.VRControllerType = VRControllerType.Pico4;
        }
        else
            userSettings.VRControllerType = VRControllerType.Universal;
    }
}
