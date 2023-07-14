using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private TutorialInputActions action;
    TutorialInputActions.PlayerActions player;

    Vector2 horizontalInput;
    Vector2 mouseInput;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] FirstPersonCamera firstPersonCamera;

    private void Awake()
    {
        playerMovement.Spawned();

        //InputActionsの初期化
        action = new TutorialInputActions();
        player = action.Player;

        //各Actionsごとの処理を定義
        player.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        player.Jump.performed += _ => playerMovement.jumpPressed();

        player.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        player.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
    }
    private void Start()
    {
        firstPersonCamera = playerMovement.GetComponent<FirstPersonCamera>();
    }
    // Update is called once per frame
    void Update()
    {
        //更新された値を他スクリプトに伝達
        playerMovement.ReciveInput(horizontalInput);
        firstPersonCamera.ReciveInput(mouseInput);
    }

    //有効化
    private void OnEnable()
    {
        action?.Enable();
    }
    // 無効化
    private void OnDisable()
    {
        // 自身が無効化されるタイミングなどで
        // Actionを無効化する必要がある
        action?.Disable();
    }
}
