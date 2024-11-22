using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject engineerPrefab;
    [SerializeField] private TCPServer tcpServer;

    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    void Awake(){
        //desactiver l'objet si je  suis  le client
        if (!Globals.IsServer){
            gameObject.SetActive(false);
        }
    }


    // Méthode pour démarrer le jeu
    void Start()
    {
        int serverPort = 25000;

        if (tcpServer.Listen(OnMessageReceived))
        {
            Debug.Log($"Jeu démarré. Serveur en écoute sur le port {serverPort}.");
        }
        else
        {
            Debug.LogError("Échec du démarrage du serveur.");
        }
    }

    // Méthode appelée lorsque des messages sont reçus par le serveur
    private void OnMessageReceived(string message)
    {
        Debug.Log($"Message reçu : {message}");

        string[] parts = message.Split(' ');

        if (parts.Length < 2)
        {
            Debug.LogWarning($"Message mal formé : {message}");
            return;
        }

        string action = parts[0];
        string playerID = parts[1];

        if (action == "connect")
        {
            OnPlayerConnected(playerID);
        }
        else if (action == "disconnect")
        {
            OnPlayerDisconnected(playerID);
        }
        else
        {
            Debug.LogWarning($"Action inconnue : {action}");
        }
    }

    // Gestion de la connexion d'un joueur
    private void OnPlayerConnected(string playerID)
    {
        if (players.ContainsKey(playerID))
        {
            Debug.LogWarning($"Le joueur {playerID} est déjà connecté !");
            return;
        }

        // Instancie le prefab du joueur dans la scène
        GameObject newPlayer = Instantiate(engineerPrefab);
        newPlayer.name = $"Player_{playerID}";
        players[playerID] = newPlayer;

        Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0.318f, Random.Range(-10, 10));
        Vector3 defaultRotation = Vector3.zero;
        newPlayer.transform.position = randomPosition;
        newPlayer.transform.eulerAngles = defaultRotation;

        Debug.Log($"Joueur connecté : {playerID}, Position : {randomPosition}, Rotation : {defaultRotation}");
    }

    // Gestion de la déconnexion d'un joueur
    private void OnPlayerDisconnected(string playerID)
    {
        if (!players.ContainsKey(playerID))
        {
            Debug.LogWarning($"Aucun joueur trouvé avec l'ID {playerID} !");
            return;
        }

        GameObject player = players[playerID];
        Destroy(player);
        players.Remove(playerID);

        Debug.Log($"Joueur déconnecté : {playerID}");
    }
}