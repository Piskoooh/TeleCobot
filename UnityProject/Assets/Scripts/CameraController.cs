using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float mouseSensitivity = 2.0f;
    public float zoomSpeed = 2.0f;
    public float minZoomDistance = 2.0f;
    public float maxZoomDistance = 10.0f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float zoomInput;
    private bool mouseClicking = false;

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>() * mouseSensitivity;
    }

    private void OnZoom(InputValue value)
    {
        zoomInput = value.Get<Vector2>().y;
    }

    private void OnClick(InputValue value)
    {
        if (value.isPressed)
        {
            mouseClicking = true;
        }
        else
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
            Vector3 rotation = new Vector3(-lookInput.y, lookInput.x, 0);
            transform.eulerAngles += rotation;
        }

        float zoomAmount = zoomInput * zoomSpeed * Time.deltaTime;
        Vector3 currentPos = transform.position;
        currentPos.y = Mathf.Clamp(currentPos.y - zoomAmount, minZoomDistance, maxZoomDistance);
        transform.position = currentPos;
    }
}
