using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _cc;

    [SerializeField] private Ball _prefabBall;
    [SerializeField] private Camera PlayerCamera;
    public Transform Target;
    [Networked] private TickTimer delay { get; set; }
    private Vector3 _forward;

    [SerializeField] float xClamp = 70f;
    float xRotation = 0f;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
        _forward = transform.forward;
    }

    private void Update()
    {
        //Spawned();
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            //player移動
            data.HorizontalDirection.Normalize();
            _cc.Move(data.HorizontalDirection);

            //jump
            if (data.isJumpPressed == true)
            {
                _cc.Jump();
                data.isJumpPressed = false;
            }
            //ball
            //if (data.HorizontalDirection.sqrMagnitude > 0)
            //    _forward = data.HorizontalDirection;
            //if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
            //{
            //    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
            //    Runner.Spawn(_prefabBall,
            //    transform.position + _forward, Quaternion.LookRotation(_forward),
            //    Object.InputAuthority, (runner, o) =>
            //    {
            //        // Initialize the Ball before synchronizing it
            //        o.GetComponent<Ball>().Init();
            //    });
            //}
            //Camera Control
            if (PlayerCamera == null) return;
            if (data.MousePosition== null) return;

            transform.Rotate(Vector3.up, data.MousePosition.x * Time.deltaTime);

            xRotation -= data.MousePosition.y;
            xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
            Vector3 targetRotation = transform.eulerAngles;
            targetRotation.x = xRotation;
            Target.eulerAngles = targetRotation;
            Debug.Log($"{data.MousePosition.x},{data.MousePosition.y}");
        }
    }
    private void LateUpdate()
    {
        if(GetInput(out NetworkInputData data))
        {
        }
    }
}