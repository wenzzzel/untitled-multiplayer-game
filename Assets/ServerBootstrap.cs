using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ServerBootstrap : MonoBehaviour
{
    void Start()
    {
        bool isBatchMode = Application.isBatchMode;

        if (isBatchMode)
        {
            Debug.Log("Application started in batch mode. Starting headless server.");

            var transport = FindObjectOfType<UnityTransport>();

            if (transport == null)
            {
                Debug.LogError("No UnityTransport found in the scene. Cannot start headless server.");
                return;
            }

            ushort port = 7777;
            var ip = "0.0.0.0";
            transport.SetConnectionData(ip, port);
            Debug.Log($"Transport bound to {ip}:{port}");

            bool serverStartedSuccessfully = NetworkManager.Singleton.StartServer();

            if (serverStartedSuccessfully)
                Debug.Log("Headless server started successfully.");
            else
                Debug.LogError("Headless server failed to start.");
        }
        else
        {
            Debug.Log("Application not started in batch mode. Starting as client.");
            bool clientStartedSuccessfully = NetworkManager.Singleton.StartClient();

            if (clientStartedSuccessfully)
                Debug.Log("Client started successfully.");
            else
                Debug.LogError("Client failed to start.");
        }
    }
}
