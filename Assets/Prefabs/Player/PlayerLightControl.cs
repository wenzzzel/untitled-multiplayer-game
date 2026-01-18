using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class PlayerLightControl : NetworkBehaviour
{
    private Light2D playerLight;

    void Start()
    {
        playerLight = GetComponent<Light2D>();

        if (!IsOwner)
        {
            playerLight.enabled = false;
        }
    }
}
