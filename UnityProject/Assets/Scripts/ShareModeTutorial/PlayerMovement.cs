using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController Controller;
    public float PlayerSpeed = 6f;

    Vector2 horizontalInput;

    public Camera Camera;
    public float JumpForce = 10f;
    private bool isJumpPressed;

    public float GravityValue = -9.81f;
    private Vector3 verticalVelocity=Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Spawned();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false) return;
        if (Controller.isGrounded)
        {
            verticalVelocity = new Vector3(0, -1, 0);
        }

        Vector3 horizontalVelocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * PlayerSpeed;
        Controller.Move(horizontalVelocity * Runner.DeltaTime);

        verticalVelocity.y += GravityValue * Runner.DeltaTime;
        if (isJumpPressed && !Controller.isGrounded)
        {
            Debug.Log($"jump method is called.");
            verticalVelocity.y = Mathf.Sqrt(-2f * JumpForce * GravityValue);
         }

        Controller.Move(verticalVelocity * Runner.DeltaTime);
        isJumpPressed = false;

    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            if (Camera == null)
            {
                Camera = GetComponent<Camera>();
            }

            //GetComponent<FirstPersonCamera>().Target = GetComponent<NetworkTransform>().InterpolationTarget;
            GetComponent<FirstPersonCamera>().Target = Camera.gameObject.transform;
            //Camera.transform.position = transform.position;
        }
    }

    public void ReciveInput(Vector2 hzInput)
    {
        horizontalInput = hzInput;
    }

    public void jumpPressed()
    {
        isJumpPressed = true;
        Debug.Log($"space was pressed{isJumpPressed}");
    }
}
