using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class CameraFollowPun : MonoBehaviourPun
{
    public Transform target;  // 追従対象のオブジェクト
    public float rotationSpeed = 5f;
    public float height = 1.7f;  // カメラの高さ
    public float distance = 5f;  // カメラと対象の距離
    public float minDistance = 2f;  // カメラと対象の最小距離
    public float maxDistance = 10f; // カメラと対象の最大距離
    public float rotation = 45f;
    public Vector3 offset = new Vector3(0, -1, -1); // カメラ位置のオフセット

    void Start()
    {
        // カメラの初期回転を設定
        transform.rotation = Quaternion.Euler(30f, 0f, 0f);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (target != null)
            {
                // 左右のArrowキーの入力を取得
                float horizontalInput = 0f;
                if (Keyboard.current.leftArrowKey.isPressed)
                {
                    horizontalInput = 1f;
                }
                else if (Keyboard.current.rightArrowKey.isPressed)
                {
                    horizontalInput = -1f;
                }

                // キーボードの左右入力に応じてカメラを回転させる
                float horizontalRotation = horizontalInput * rotationSpeed * Time.deltaTime;
                Quaternion targetRotation = Quaternion.Euler(rotation, transform.rotation.eulerAngles.y + horizontalRotation, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.9f); // 第三引数の 0.5f はスムージングの強さを表しています

                // カメラの位置を対象の周りに配置
                Vector3 rotatedOffset = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * offset;
                transform.position = target.position + new Vector3(rotatedOffset.x, height, rotatedOffset.z);

                // カメラと対象の距離を制限する
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
            }
        }
    }
}
