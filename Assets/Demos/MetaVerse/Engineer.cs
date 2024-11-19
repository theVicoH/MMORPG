using UnityEngine;

public class Engineer : MonoBehaviour
{
    private string playerID; // ID unique pour ce joueur
    private Vector3 position;

    private TCPClient tcpClient;

    private void Start()
    {
        tcpClient = FindObjectOfType<TCPClient>();

        // Génère un ID unique pour ce joueur
        playerID = System.Guid.NewGuid().ToString();

        // Envoie un message de connexion au serveur
        SendMessageToServer($"CONNECT:{playerID}");

        // Log la position initiale
        position = transform.position;
        Debug.Log($"Joueur connecté - ID : {playerID}, Position : {position}");
    }

    private void Update()
    {
        // Met à jour et log la position en continu
        position = transform.position;
        Debug.Log($"Position actuelle - ID : {playerID}, Position : {position}");
    }

    private void OnDestroy()
    {
        // Envoie un message de déconnexion au serveur
        SendMessageToServer($"DISCONNECT:{playerID}");
        Debug.Log($"Joueur déconnecté - ID : {playerID}");
    }

    private void SendMessageToServer(string message)
    {
        if (tcpClient != null && tcpClient.IsConnected)
        {
            tcpClient.SendTCPMessage(message);
        }
        else
        {
            Debug.LogWarning("Impossible d'envoyer le message, TCPClient non connecté.");
        }
    }
}
