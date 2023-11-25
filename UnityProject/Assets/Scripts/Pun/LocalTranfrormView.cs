using Photon.Pun;
using UnityEngine;

public class LocalTransformView : MonoBehaviourPunCallbacks, IPunObservable
{
    private const float InterpolationPeriod = 0.1f; // 補間にかける時間

    private Vector3 p1;
    private Vector3 p2;
    private Vector3 v1;
    private Vector3 v2;
    private float elapsedTime;

    private void Start()
    {
        p1 = transform.position;
        p2 = p1;
        v1 = Vector3.zero;
        v2 = v1;
        elapsedTime = Time.deltaTime;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // 自身のネットワークオブジェクトは、毎フレームの移動量と経過時間を記録する
            p1 = p2;
            p2 = transform.position;
            elapsedTime = Time.deltaTime;
        }
        else
        {
            // 他プレイヤーのネットワークオブジェクトは、補間処理を行う
            elapsedTime += Time.deltaTime;
                        if (elapsedTime < InterpolationPeriod)
            {
                transform.position = HermiteSpline.Interpolate(p1, p2, v1, v2, elapsedTime / InterpolationPeriod);
                            }
            else
            {
                transform.position = Vector3.LerpUnclamped(p1, p2, elapsedTime / InterpolationPeriod);
                            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            // 毎フレームの移動量と経過時間から、秒速を求めて送信する
            stream.SendNext((p2 - p1) / elapsedTime);
        }
        else
        {
            var networkPosition = (Vector3)stream.ReceiveNext();
            var networkVelocity = (Vector3)stream.ReceiveNext();
            var lag = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - info.SentServerTimestamp) / 1000f);

            // 受信時の座標を、補間の開始座標にする
            p1 = transform.position;
            // 現在時刻における予測座標を、補間の終了座標にする
            p2 = networkPosition + networkVelocity * lag;
            // 前回の補間の終了速度を、補間の開始速度にする
            v1 = v2;
            // 受信した秒速を、補間にかける時間あたりの速度に変換して、補間の終了速度にする
            v2 = networkVelocity * InterpolationPeriod;
            // 経過時間をリセットする
            elapsedTime = 0f;
        }
    }
}

public static class HermiteSpline
{
    public static float Interpolate(float p1, float p2, float v1, float v2, float t)
    {
        float a = 2f * p1 - 2f * p2 + v1 + v2;
        float b = -3f * p1 + 3f * p2 - 2f * v1 - v2;
        return t * (t * (t * a + b) + v1) + p1;
    }

    public static Vector2 Interpolate(Vector2 p1, Vector2 p2, Vector2 v1, Vector2 v2, float t)
    {
        return new Vector2(
            Interpolate(p1.x, p2.x, v1.x, v2.x, t),
            Interpolate(p1.y, p2.y, v1.y, v2.y, t)
        );
    }

    public static Vector3 Interpolate(Vector3 p1, Vector3 p2, Vector3 v1, Vector3 v2, float t)
    {
        return new Vector3(
            Interpolate(p1.x, p2.x, v1.x, v2.x, t),
            Interpolate(p1.y, p2.y, v1.y, v2.y, t),
            Interpolate(p1.z, p2.z, v1.z, v2.z, t)
        );
    }
}
