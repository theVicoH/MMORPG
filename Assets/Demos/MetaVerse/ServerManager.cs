using UnityEngine;
using System.Collections.Generic;

public class ServerManager : MonoBehaviour
{
    public TCPServer tcpServer;
    public GameObject engineerPrefab;
    private Dictionary<string, GameObject> serverPlayers = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (!Globals.IsServer)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void Start()
    {
        if (tcpServer == null)
        {
            Debug.LogError("[ServerManager] TCPServer non assigné !");
            return;
        }
        Debug.Log("[ServerManager] Hallo");
        tcpServer.OnPlayerSpawnReceived += HandlePlayerSpawn;
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

    private void HandlePlayerSpawn(string playerID, Vector3 position, Quaternion rotation)
    {
        if (!serverPlayers.ContainsKey(playerID))
        {
            GameObject playerInstance = Instantiate(engineerPrefab, position, rotation);
            playerInstance.name = $"ServerPlayer_{playerID}";
            
            var characterController = playerInstance.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.playerID = playerID;
            }

            serverPlayers.Add(playerID, playerInstance);
            Debug.Log($"[ServerManager] Joueur instancié sur le serveur : ID = {playerID}, Position = {position}");
        }
    }
}


