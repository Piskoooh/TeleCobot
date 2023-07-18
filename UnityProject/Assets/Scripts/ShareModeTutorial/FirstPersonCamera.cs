using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform Target;
    [SerializeField] float xClamp = 70f;
    float xRotation = 0f;

    [SerializeField] float sensitivityX = 8f;
    [SerializeField] float sensitivityY = 0.5f;


    private Vector2 mouseInput;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (Target == null)　return;
        if (mouseInput == null) return;

        transform.Rotate(Vector3.up, mouseInput.x * Time.deltaTime);

        xRotation -= mouseInput.y;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        Target.eulerAngles = targetRotation;
    }

    public void ReciveInput(Vector2 msInput)
    {
        mouseInput.x = msInput.x*sensitivityX;
        mouseInput.y = msInput.y * sensitivityY;
    }
}