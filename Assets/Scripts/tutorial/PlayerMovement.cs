using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    public Camera Camera;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;
    public CharacterController Controller;

    public float PlayerSpeed = 6f;
    private Vector3 velocity;
    private bool _jumpPressed;

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
        if(HasStateAuthority == false)
        {
            return;
        }
        if (Controller.isGrounded)
        {
            velocity = new Vector3(0, -1, 0);
        }

        var cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;
        //Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;

        velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && Controller.isGrounded)
        {
            velocity.y += JumpForce;
        }

        Controller.Move(move + velocity * Runner.DeltaTime);
        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
        _jumpPressed = false;

    }

    public override void Spawned()
    {
        if (HasStateAuthority&&Camera==null)
        {
            Camera = Camera.main;
            Camera.GetComponent<FirstPersonCamera>().Target = GetComponent<NetworkTransform>().InterpolationTarget;
        }
    }
}
