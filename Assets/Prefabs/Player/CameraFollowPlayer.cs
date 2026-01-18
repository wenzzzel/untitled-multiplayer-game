using Unity.Netcode;
using UnityEngine;

public class CameraFollowPlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0f, 0f, -10f);
        }
    }
}
