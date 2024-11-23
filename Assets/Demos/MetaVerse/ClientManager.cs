using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientManager : MonoBehaviour
{
    public TCPClient tcpClient;
    public GameObject engineerPrefab;

    public static string LocalPlayerID { get; private set; }

    private string playerID;
    private GameObject playerInstance;

    private List<Vector3> spawns = new List<Vector3>
    {
        new Vector3(253.32f, 1.34f, 248.2043f),
        new Vector3(250.44f, 1.34f, 248.2043f),
        new Vector3(247.38f, 1.34f, 248.2043f),
        new Vector3(244.44f, 1.34f, 248.2043f),
        new Vector3(241.58f, 1.34f, 248.2043f),
        new Vector3(238.22f, 1.34f, 248.2043f),
        new Vector3(235.35f, 1.34f, 248.2043f),
        new Vector3(232.53f, 1.34f, 248.2043f),
    };

    private void Start()
    {
        if (Globals.IsServer)
        {
            gameObject.SetActive(false);
            return;
        }

        if (tcpClient == null)
        {
            Debug.LogError("[ClientManager] TCPClient non assigné !");
            return;
        }

        playerID = System.Guid.NewGuid().ToString();
        LocalPlayerID = playerID;

        Debug.Log($"[ClientManager] Génération de l'ID joueur local : {playerID}");
        StartCoroutine(TryConnectToServer());
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
            Debug.LogError("[ClientManager] Prefab Engineer non assignée !");
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
        Debug.Log($"[ClientManager] Joueur instancié : ID = {playerID}, Position = {spawnPosition}");
    }
}