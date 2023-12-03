using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

// HMDを使用せずに開発を進める際にMainCameraを制御するスクリプト
public class CameraControllerPun : MonoBehaviourPun
{
    public float movementSpeed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float zoomSpeed = 2.0f;
    public float minZoomFOV = 40.0f;
    public float maxZoomFOV = 60.0f;

    public float verticalRotationLimit = 60.0f;
    private float currentVerticalAngle = 0.0f;
    private bool mouseClicking = false;

    private void Update()
    {
        if (photonView.IsMine)
        {

            // WASDキーでの移動
            Vector3 translation = new Vector3(
                (Keyboard.current.rightArrowKey.isPressed ? 1 : 0) - (Keyboard.current.leftArrowKey.isPressed ? 1 : 0),
                0,
                (Keyboard.current.upArrowKey.isPressed ? 1 : 0) - (Keyboard.current.downArrowKey.isPressed ? 1 : 0))
                * movementSpeed * Time.deltaTime;
            transform.Translate(translation);

            // マウスのポインタでの視点移動
            if (Mouse.current != null && mouseClicking)
            {
                float horizontal = Mouse.current.delta.x.ReadValue() * mouseSensitivity;
                float vertical = Mouse.current.delta.y.ReadValue() * mouseSensitivity;

                transform.Rotate(Vector3.up, horizontal);

                currentVerticalAngle -= vertical;
                currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -verticalRotationLimit, verticalRotationLimit);
                transform.localEulerAngles = new Vector3(currentVerticalAngle, transform.localEulerAngles.y, 0);
            }

            // マウスホイールでのズーム
            float zoomAmount = Mouse.current != null ? -Mouse.current.scroll.y.ReadValue() : 0;
            zoomAmount *= zoomSpeed * Time.deltaTime;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - zoomAmount, minZoomFOV, maxZoomFOV);

            // マウスの左クリックの取得
            if (Mouse.current != null)
            {
                mouseClicking = Mouse.current.leftButton.isPressed;
            }
        }
    }
}
