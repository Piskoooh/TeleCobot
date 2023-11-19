using UnityEngine;
using UnityEngine.InputSystem;

//HMDを使用せずに開発を進める際にMainCameraを制御するスクリプト
//使用するにはLocalUserのActionMapを有効化する必要がある。
public class CameraController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float zoomSpeed = 2.0f;
    public float minZoomFOV = 40.0f;
    public float maxZoomFOV = 60.0f;

    public float verticalRotationLimit = 60.0f;
    private float currentVerticalAngle = 0.0f;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float zoomInput;
    private bool mouseClicking = false;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>() * mouseSensitivity;

    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<Vector2>().y;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.action.triggered)
        {
            mouseClicking = context.ReadValueAsButton();
        }
        else if (context.canceled)
        {
            mouseClicking = false;
        }
    }

    private void Update()
    {
        Vector3 translation = new Vector3(moveInput.x, 0, moveInput.y) * movementSpeed * Time.deltaTime;
        transform.Translate(translation);

        if (mouseClicking)
        {
            float horizontal = lookInput.x;
            float vertical = lookInput.y;

            transform.Rotate(Vector3.up, horizontal);

            currentVerticalAngle -= vertical;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -verticalRotationLimit, verticalRotationLimit);
            transform.localEulerAngles = new Vector3(currentVerticalAngle, transform.localEulerAngles.y, 0);
        }

        float zoomAmount = -zoomInput * zoomSpeed * Time.deltaTime;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - zoomAmount, minZoomFOV, maxZoomFOV);

    }
}
