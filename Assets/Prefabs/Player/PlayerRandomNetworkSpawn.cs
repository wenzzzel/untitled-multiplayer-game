using Unity.Netcode;
using UnityEngine;

public class RandomNetworkSpawn : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;
        
        transform.position = GetRandomPosition();
    }

    private static Vector2 GetRandomPosition() => new Vector2(Random.Range(-6f, 6f), Random.Range(-3f, 3f));
}
