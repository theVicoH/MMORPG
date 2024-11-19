using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject engineerPrefab; // Prefab du joueur
    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>(); // Liste des joueurs connectés

    [SerializeField] private TCPServer tcpServer;

    void Start()
    {
        // Démarre le serveur et écoute les messages
        tcpServer.Listen(OnMessageReceived);
    }

    // Callback appelé quand un message est reçu
    private void OnMessageReceived(string message)
    {
        Debug.Log($"Message reçu : {message}");

        if (message.StartsWith("CONNECT:"))
        {
            string playerID = message.Split(':')[1];
            OnPlayerConnected(playerID);
        }
        else if (message.StartsWith("DISCONNECT:"))
        {
            string playerID = message.Split(':')[1];
            OnPlayerDisconnected(playerID);
        }
    }

    // Instancie un joueur lors de la connexion
    private void OnPlayerConnected(string playerID)
    {
        if (players.ContainsKey(playerID))
        {
            Debug.LogWarning($"Le joueur avec l'ID {playerID} est déjà connecté !");
            return;
        }

        // Instancier le joueur à une position aléatoire
        Vector3 spawnPosition = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        GameObject newPlayer = Instantiate(engineerPrefab, spawnPosition, Quaternion.identity);

        // Ajouter le joueur à la liste
        players[playerID] = newPlayer;

        Debug.Log($"Nouveau joueur connecté : {playerID}, Position : {spawnPosition}");
    }

    // Supprime un joueur lors de la déconnexion
    private void OnPlayerDisconnected(string playerID)
    {
        if (!players.ContainsKey(playerID))
        {
            Debug.LogWarning($"Aucun joueur trouvé avec l'ID {playerID} !");
            return;
        }

        // Supprime le GameObject du joueur
        GameObject player = players[playerID];
        Destroy(player);

        // Retirer le joueur de la liste
        players.Remove(playerID);

        Debug.Log($"Joueur déconnecté : {playerID}");
    }
}
