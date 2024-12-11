using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour
{
    public TCPClient tcpClient;
    public GameObject engineerPrefab;

    public static string LocalPlayerID { get; private set; }

    private string playerID;
    private GameObject playerInstance;

    private List<Vector3> spawns = new List<Vector3>
    {
        new Vector3(253.32f, 1.34f, 248.2043f)
        // new Vector3(250.44f, 1.34f, 248.2043f),
        // new Vector3(247.38f, 1.34f, 248.2043f),
        // new Vector3(244.44f, 1.34f, 248.2043f),
        // new Vector3(241.58f, 1.34f, 248.2043f),
        // new Vector3(238.22f, 1.34f, 248.2043f),
        // new Vector3(235.35f, 1.34f, 248.2043f),
        // new Vector3(232.53f, 1.34f, 248.2043f),
    };
    // private float NextCoucouTimeout = -1;
    public UDPService UDP;
    public string ServerIP = "127.0.0.1";
    public int ServerPort = 25001;

    public IPEndPoint ServerEndpoint { get; private set; }

    void Awake() {
        if (Globals.IsServer) {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (tcpClient == null)
        {
            Debug.LogError("[ClientManager] TCPClient non assigné !");
            return;
        }

        playerID = System.Guid.NewGuid().ToString();
        LocalPlayerID = playerID;

        Debug.Log($"[ClientManager] Génération de l'ID joueur local : {playerID}");
        StartCoroutine(TryConnectToServer());

        UDP.InitClient();

        ServerEndpoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
            
        UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            Debug.Log("[CLIENT] Message received from " + 
                sender.Address.ToString() + ":" + sender.Port 
                + " =>" + message);
        };
    }

    void Update()
    {
        // if (Time.time > NextCoucouTimeout) {
        //     UDP.SendUDPMessage("coucou", ServerEndpoint);
        //     NextCoucouTimeout = Time.time + 0.5f;
        // }
    }

    private void OnApplicationQuit()
    {
        SendDisconnectMessage();
    }

    private void OnDisable()
    {
        SendDisconnectMessage();
    }

    private IEnumerator TryConnectToServer()
    {
        int maxRetries = 5;
        int retryCount = 0;
        float retryDelay = 2f;

        while (retryCount < maxRetries)
        {
            Debug.Log($"[ClientManager] Tentative de connexion au serveur (essai {retryCount + 1}/{maxRetries})...");

            bool isConnected = tcpClient.Connect((string message) =>
            {
                Debug.Log($"[ClientManager] Message du serveur : {message}");
            });

            if (isConnected)
            {
                Debug.Log("[ClientManager] Connexion réussie !");
                SendConnectMessage();
                yield return new WaitForSeconds(0.5f);
                InstantiateLocalPlayer();
                yield break;
            }

            retryCount++;
            Debug.LogWarning("[ClientManager] Connexion échouée. Nouvelle tentative...");
            yield return new WaitForSeconds(retryDelay);
        }

        Debug.LogError("[ClientManager] Impossible de se connecter au serveur après plusieurs tentatives !");
    }

    private void SendConnectMessage()
    {
        if (tcpClient != null && tcpClient.IsConnected)
        {
            string message = $"connect {playerID}";
            tcpClient.SendTCPMessage(message);
            Debug.Log($"[ClientManager] Message de connexion envoyé : {message}");
        }
        else
        {
            Debug.LogError("[ClientManager] TCPClient non connecté !");
        }
    }

     private void InstantiateLocalPlayer()
    {
        if (engineerPrefab == null)
        {
            Debug.LogError("[ClientManager] Prefab Engineer non assigné !");
            return;
        }

        Vector3 spawnPosition = spawns[Random.Range(0, spawns.Count - 1)];
        spawns.Remove(spawnPosition);
        Quaternion spawnRotation = Quaternion.identity;

        playerInstance = Instantiate(engineerPrefab, spawnPosition, spawnRotation);
        playerInstance.name = $"Player_{playerID}";

        var characterController = playerInstance.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.playerID = playerID;
        }

        // Utilisation de la nouvelle méthode recommandée
        var virtualCamera = FindFirstObjectByType<CinemachineCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerInstance.transform;
            virtualCamera.LookAt = playerInstance.transform;
            Debug.Log("[ClientManager] Caméra Cinemachine configurée pour suivre le joueur");
        }
        else
        {
            Debug.LogError("[ClientManager] Pas de CinemachineVirtualCamera trouvée dans la scène!");
        }

        string spawnMessage = $"spawn {playerID} {spawnPosition.x} {spawnPosition.y} {spawnPosition.z} {spawnRotation.eulerAngles.x} {spawnRotation.eulerAngles.y} {spawnRotation.eulerAngles.z}";
        Debug.Log($"[ClientManager] Envoi du message spawn : {spawnMessage}");
        tcpClient.SendTCPMessage(spawnMessage);

        Debug.Log($"[ClientManager] Joueur instancié et message d'instanciation envoyé : ID = {playerID}, Position = {spawnPosition}");
    }

    public void SendDisconnectMessage()
    {
        if (tcpClient != null && tcpClient.IsConnected)
        {
            string message = $"disconnect {playerID}";
            tcpClient.SendTCPMessage(message);
            Debug.Log($"[ClientManager] Message de déconnexion envoyé : {message}");
        }
    }
    

    public void DestroyPlayer()
    {
        SendDisconnectMessage();
        if (playerInstance != null)
        {
            Debug.Log($"[ClientManager] Suppression du joueur local : {playerInstance.name}");
            Destroy(playerInstance);
            playerInstance = null;
        }
        Debug.Log("[ClientManager] Retour au menu principal...");
        SceneManager.LoadScene("ServeurClientMenu");
    }
}