using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TCPServer tcpServer;

    private void Start()
    {
        if (Globals.IsServer)
        {
            Debug.Log("[GameManager] Mode Serveur activé. En attente de connexions...");
            InitializeServer();
        }
    }

    private void InitializeServer()
    {
        if (tcpServer == null)
        {
            Debug.LogError("[GameManager] TCPServer non assigné !");
            return;
        }

        tcpServer.Listen((string message) =>
        {
            Debug.Log($"[GameManager] Message reçu du client : {message}");
        });
    }
}
