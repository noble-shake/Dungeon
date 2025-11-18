using Unity.Netcode;
using UnityEngine;

public class Reflectable : MonoBehaviour
{

    // [ServerRpc(RequireOwnership = false)]
    // public void RotateServerRpc(Vector3 reflectDirection)
    // {
    //     RotateClientRpc(reflectDirection);
    // }

    // // 서버에서 클라이언트로 회전값을 전달하는 RPC
    // [ClientRpc]
    // public void RotateClientRpc(Vector3 reflectDirection)
    // {
    //     Debug.Log("반사가 된거임?");
    //     Debug.Log(reflectDirection);
    //     transform.rotation = Quaternion.LookRotation(reflectDirection);
    // }
}
