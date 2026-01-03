using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class ServerBootstrap : MonoBehaviour
{
    void Start()
    {
        // Start server if either we're a Server Build or headless batch mode.
        bool isHeadless = Application.isBatchMode;

#if UNITY_SERVER
        bool isServerBuild = true;
#else
        bool isServerBuild = false;
#endif

        if (isHeadless || isServerBuild)
        {
            var transport = FindObjectOfType<UnityTransport>();
            if (transport != null)
            {
                ushort port = 7777;
                var args = System.Environment.GetCommandLineArgs();
                for (int i = 0; i < args.Length - 1; i++)
                    if (args[i] == "-port" && ushort.TryParse(args[i + 1], out var p))
                        port = p;

                transport.SetConnectionData("0.0.0.0", port);
                Debug.Log($"[BOOT] Transport bound to 0.0.0.0:{port}");
            }
            bool ok = NetworkManager.Singleton.StartServer();
            Debug.Log(ok ? "[NETCODE] Server started" : "[NETCODE] Server start FAILED");
        }
        else
        {
            // Debug.Log("[BOOT] Non-headless run; start client/host via UI.");
            Debug.Log("Non-headless run; Starting as client");
            bool ok = NetworkManager.Singleton.StartClient();
            Debug.Log(ok ? "[NETCODE] Client started" : "[NETCODE] Client start FAILED");
        }
    }
}
