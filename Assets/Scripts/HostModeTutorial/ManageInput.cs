using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ManageInput : MonoBehaviour
{
    [SerializeField] private TutorialInputActions action;
    TutorialInputActions.PlayerActions playerActions;
    Vector2 horizontalInput;
    Vector2 mouseInput;

    [SerializeField] private BasicSpawner basicSpawner;

    private void Awake()
    {
        action = new TutorialInputActions();
        playerActions = action.Player;
        playerActions.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        playerActions.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        playerActions.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
    }

    void Update()
    {
        //更新された値を他スクリプトに伝達
        basicSpawner.ReciveInput(horizontalInput,mouseInput);
        //Debug.Log($"ManageInput {horizontalInput}");

        //button入力の伝達
        if (playerActions.Jump.IsPressed())
            basicSpawner.jumpInput = true;
        else basicSpawner.jumpInput = false;
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
