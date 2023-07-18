using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON1 = 0x01;

    public byte buttons;

    public Vector3 HorizontalDirection;
    public Vector3 VerticalDirection;
    public bool isJumpPressed;

    public Vector2 MousePosition;
}