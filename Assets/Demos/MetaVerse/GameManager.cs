using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject engineerPrefab;
    [SerializeField] private TCPServer tcpServer;

    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    void Start()
    {
        // Démarre l'écoute et configure le callback
       // tcpServer.Listen(OnMessageReceived);
    }

    // Callback pour traiter les messages du serveur
    private void OnMessageReceived(string message)
    {
        Debug.Log($"Message reçu : {message}");

        // Découper le message en parties
        string[] parts = message.Split(' ');
        if (parts.Length < 2)
        {
            Debug.LogWarning("Message mal formé : " + message);
            return;
        }

        string action = parts[0];
        string clientId = parts[1];

        if (action == "connect")
        {
            OnPlayerConnected(clientId);
        }
        else if (action == "disconnect")
        {
            OnPlayerDisconnected(clientId);
        }
    }

    // Connexion d'un joueur
    private void OnPlayerConnected(string playerID)
    {
        if (players.ContainsKey(playerID))
        {
            Debug.LogWarning($"Le joueur avec l'ID {playerID} est déjà connecté !");
            return;
        }


        // Instancier le joueur
        GameObject newPlayer = Instantiate(engineerPrefab);
        players[playerID] = newPlayer;

        Debug.Log($"Nouveau joueur connecté : {playerID}");
    }

    // Déconnexion d'un joueur
    private void OnPlayerDisconnected(string playerID)
    {
        if (!players.ContainsKey(playerID))
        {
            Debug.LogWarning($"Aucun joueur trouvé avec l'ID {playerID} !");
            return;
        }

        // Supprimer le GameObject du joueur
        GameObject player = players[playerID];
        Destroy(player);
        players.Remove(playerID);

        Debug.Log($"Joueur déconnecté : {playerID}");
    }
}

