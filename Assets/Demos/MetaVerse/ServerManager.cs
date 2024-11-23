using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public TCPServer tcpServer;

    private void Awake()
    {
        /* // Désactiver l'objet si ce n'est pas le mode serveur
                if (!Globals.IsServer)
                {
                    gameObject.SetActive(false);
                    return;
                } */
    }

    private void Start()
    {
        if (tcpServer == null)
        {
            Debug.LogError("[ServerManager] TCPServer non assigné !");
            return;
        }

        StartListening();
    }

    private void StartListening()
    {
        bool started = tcpServer.Listen((string message) =>
        {
            Debug.Log($"[ServerManager] Message reçu : {message}");
        });

        if (started)
        {
            Debug.Log("[ServerManager] Serveur en attente de connexions...");
        }
        else
        {
            Debug.LogError("[ServerManager] Échec du démarrage du serveur !");
        }
    }
}


