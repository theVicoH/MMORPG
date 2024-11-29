using UnityEngine;
using System.Collections.Generic;
using System.Net;

public class ServerManager : MonoBehaviour
{
    public TCPServer tcpServer;
    public GameObject engineerPrefab;
    private Dictionary<string, GameObject> serverPlayers = new Dictionary<string, GameObject>();
    public UDPService UDP;
    public int ListenPort = 25000;

    public Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>(); 

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
        tcpServer.OnPlayerSpawnReceived += HandlePlayerSpawn;
        tcpServer.OnPlayerDisconnectReceived += HandlePlayerDisconnect;
        Debug.Log("[ServerManager] Hallo");
        StartListening();
        UDP.Listen(ListenPort);
    }

    void Update()
    {
        UDP.OnMessageReceived +=  
            (string message, IPEndPoint sender) => {
                Debug.Log("[SERVER] Message received from " + 
                    sender.Address.ToString() + ":" + sender.Port 
                    + " =>" + message);
                
                if (message == "coucou") {
                    string addr = sender.Address.ToString() + ":" + sender.Port;
                    if (!Clients.ContainsKey(addr)) {
                        Clients.Add(addr, sender);
                    }
                    Debug.Log("There are " + Clients.Count + " clients present.");
                    UDP.SendUDPMessage("welcome!", sender);
                    return;
                }
                
                string[] parts = message.Split('|');
                if (parts.Length < 2) return;

                string command = parts[0];
                string content = parts[1];

                switch (command) {
                    case "POS":
                        BroadcastUDPMessage(message, sender);
                        break;

                    default:
                        Debug.LogWarning("Unknown message type: " + command);
                        break;
                }
                
            };
    }

    public void BroadcastUDPMessage(string message, IPEndPoint sender) {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients) {
            if (client.Value.Address.Equals(sender.Address) && client.Value.Port == sender.Port) {
                continue;
            }
            UDP.SendUDPMessage(message, client.Value);
        }
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
    private void HandlePlayerDisconnect(string playerID)
    {
        if (serverPlayers.ContainsKey(playerID))
        {
            GameObject playerInstance = serverPlayers[playerID];
            Destroy(playerInstance);
            serverPlayers.Remove(playerID);
            Debug.Log($"[ServerManager] Joueur déconnecté et supprimé du serveur : ID = {playerID}");
        }
    }
}


