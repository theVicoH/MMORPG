using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

public class ServerManager : MonoBehaviour
{
    public TCPServer tcpServer;
    public GameObject engineerPrefab;
    private Dictionary<string, GameObject> serverPlayers = new Dictionary<string, GameObject>();
    public UDPService UDP;
    public int ListenPort = 25001;

    public Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>(); 

    private bool isDisconnecting = false;
    private Queue<string> disconnectionQueue = new Queue<string>();

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
        UDP.OnMessageReceived += OnMessageReceived;
        Debug.Log("[ServerManager] Hallo");
        StartListening();
        UDP.Listen(ListenPort);
    }

    private void OnMessageReceived(string message, IPEndPoint sender)
    {
        // Utiliser un pool de threads ou des tâches asynchrones pour traiter les messages
        Task.Run(() => ProcessMessage(message, sender));
    }

    private void ProcessMessage(string message, IPEndPoint sender)
    {
        #if DEBUG
        Debug.Log("[SERVER] Message received from " + sender.Address.ToString() + ":" + sender.Port + " =>" + message);
        #endif

        string[] parts = message.Split('|');
        if (parts.Length < 2) return;

        string command = parts[0];
        string content = parts[1];

        switch (command)
        {
            case "CHAR_POS":
                CharacterState state = JsonUtility.FromJson<CharacterState>(content);
                UpdatePlayerPosition(state);
                BroadcastUDPMessage("CHAR_POS|" + content, sender);
                break;

            default:
                Debug.LogWarning("Unknown message type: " + command);
                break;
        }
    }

    // void Update()
    // {
    //     UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
    //         Debug.Log("[SERVER] Message received from " + sender.Address.ToString() + ":" + sender.Port + " =>" + message);

    //         string[] parts = message.Split('|');
    //         if (parts.Length < 2) return;

    //         string command = parts[0];
    //         string content = parts[1];

    //         switch (command) {
    //             case "CHAR_POS":
    //                 CharacterState state = JsonUtility.FromJson<CharacterState>(content);
    //                 UpdatePlayerPosition(state);
    //                 BroadcastUDPMessage("CHAR_POS|" + content, sender);
    //                 break;

    //             default:
    //                 Debug.LogWarning("Unknown message type: " + command);
    //                 break;
    //         }
    //     };
        // UDP.OnMessageReceived +=  
        //     (string message, IPEndPoint sender) => {
        //         Debug.Log("[SERVER] Message received from " + 
        //             sender.Address.ToString() + ":" + sender.Port 
        //             + " =>" + message);
                
        //         if (message == "coucou") {
        //             string addr = sender.Address.ToString() + ":" + sender.Port;
        //             if (!Clients.ContainsKey(addr)) {
        //                 Clients.Add(addr, sender);
        //             }
        //             Debug.Log("There are " + Clients.Count + " clients present.");
        //             UDP.SendUDPMessage("welcome!", sender);
        //             return;
        //         }
                
        //         string[] parts = message.Split('|');
        //         if (parts.Length < 2) return;

        //         string command = parts[0];
        //         string content = parts[1];

        //         switch (command) {
        //             case "POS":
        //                 CharacterState state = JsonUtility.FromJson<CharacterState>(content);
        //                 UpdatePlayerPosition(state);
        //                 BroadcastUDPMessage(message, sender);
        //                 break;

        //             default:
        //                 Debug.LogWarning("Unknown message type: " + command);
        //                 break;
        //         }
                
        //     };
    // }
    private void UpdatePlayerPosition(CharacterState state)
    {
        if (serverPlayers.ContainsKey(state.PlayerID))
        {
            GameObject playerInstance = serverPlayers[state.PlayerID];
            playerInstance.transform.position = state.Position;
            playerInstance.transform.rotation = state.Rotation;
        }
    }

    private void BroadcastUDPMessage(string message, IPEndPoint sender)
    {
        foreach (var client in Clients)
        {
            if (!client.Value.Equals(sender))
            {
                UDP.SendUDPMessage(message, client.Value);
            }
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
    private async Task HandlePlayerDisconnectAsync(string playerID)
    {
        if (isDisconnecting)
        {
            disconnectionQueue.Enqueue(playerID);
            Debug.Log($"[ServerManager] Joueur {playerID} mis en file d'attente pour déconnexion");
            return;
        }

        isDisconnecting = true;
        try
        {
            // Nettoyer l'ID du joueur en retirant tous les "disconnect" potentiels
            string cleanPlayerID = playerID;
            while (cleanPlayerID.Contains("disconnect"))
            {
                cleanPlayerID = cleanPlayerID.Replace("disconnect", "").Trim();
            }
            
            Debug.Log($"[ServerManager] Début de la déconnexion pour le joueur : {cleanPlayerID}");
            
            if (serverPlayers.ContainsKey(cleanPlayerID))
            {
                GameObject playerInstance = serverPlayers[cleanPlayerID];
                Debug.Log($"[ServerManager] Destruction de l'instance serveur du joueur : {cleanPlayerID}");
                
                // Retirer d'abord du dictionnaire
                serverPlayers.Remove(cleanPlayerID);
                
                // Attendre une frame pour s'assurer que tout est bien traité
                await Task.Yield();
                
                // Détruire l'objet
                if (playerInstance != null)
                {
                    Destroy(playerInstance);
                    Debug.Log($"[ServerManager] Instance du joueur détruite : ID = {cleanPlayerID}");
                }
                
                // Nettoyer les clients UDP
                string clientKey = Clients.Keys.FirstOrDefault(k => k.Contains(cleanPlayerID));
                if (!string.IsNullOrEmpty(clientKey))
                {
                    Clients.Remove(clientKey);
                    Debug.Log($"[ServerManager] Client UDP supprimé : {clientKey}");
                }
                
                // Attendre encore une frame pour s'assurer que tout est bien nettoyé
                await Task.Yield();
            }
            else
            {
                Debug.LogWarning($"[ServerManager] Aucune instance serveur trouvée pour le joueur : {cleanPlayerID}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ServerManager] Erreur lors de la déconnexion du joueur : {e.Message}\n{e.StackTrace}");
        }
        finally
        {
            isDisconnecting = false;
            Debug.Log($"[ServerManager] Déconnexion terminée pour le joueur");
            
            // Traiter la prochaine déconnexion en attente s'il y en a
            if (disconnectionQueue.Count > 0)
            {
                string nextPlayerID = disconnectionQueue.Dequeue();
                _ = HandlePlayerDisconnectAsync(nextPlayerID);
            }
        }
    }

    private void HandlePlayerDisconnect(string playerID)
    {
        _ = HandlePlayerDisconnectAsync(playerID);
    }

    private async void OnApplicationQuit()
    {
        Debug.Log("[ServerManager] Nettoyage des joueurs avant fermeture...");
        var players = serverPlayers.ToList();
        foreach (var player in players)
        {
            await HandlePlayerDisconnectAsync(player.Key);
        }
        serverPlayers.Clear();
        Clients.Clear();
    }
}


