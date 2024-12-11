using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

[Serializable]
public class SerializableEndPoint
{
    public string Address;
    public int Port;

    public SerializableEndPoint(IPEndPoint endPoint)
    {
        Address = endPoint.Address.ToString();
        Port = endPoint.Port;
    }

    public IPEndPoint ToIPEndPoint()
    {
        return new IPEndPoint(IPAddress.Parse(Address), Port);
    }
}

[System.Serializable]
public class EntityData
{
    public string ID;
    public SerializableEndPoint RemoteEndPoint;
    public Vector3 position;
    public Vector3 rotation;
    public int score;
}

[Serializable]
public class EntityDataList
{
    public List<string> entities;
}

[Serializable]
public class BonusData
{
    public string ID;
    public Vector3 position;
    public bool isActive;
}

[Serializable]
public class BonusListWrapper
{
    public List<BonusData> bonuses;
}

public class TCPServer : MonoBehaviour
{
    public int ListenPort = 25000;
    public string OnConnectionMessage = "Welcome client!";

    private TcpListener tcp;
    private List<TcpClient> Connections = new List<TcpClient>();
    public List<EntityData> ConnectedClients = new List<EntityData>();
    private List<string> ConnectedClientsIds = new List<string>();

    public delegate void TCPMessageReceive(string message);
    private TCPMessageReceive OnMessageReceive;
    public event Action<string> OnPlayerDisconnectReceived;

    private List<BonusData> bonus = new List<BonusData>
    {
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(246.910385f, 0.785560608f, 254.503769f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(253.199997f, 0.785560608f, 252.179993f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(243.020004f, 0.785560608f, 252.179993f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(248.919998f, 0.785560608f, 238.419998f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(239.669998f, 0.785560608f, 242.979996f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(228.899994f, 0.785560608f, 252.580002f),
            isActive = true
        }
    };
    public delegate void PlayerSpawnHandler(string playerID, Vector3 position, Quaternion rotation);
    public event PlayerSpawnHandler OnPlayerSpawnReceived;


    public bool IsListening
    {
        get { return tcp != null; }
    }

    public int ConnectionCount
    {
        get { return ConnectedClients.Count; }
    }

    public bool Listen(TCPMessageReceive handler)
    {
        if (tcp != null)
        {
            Debug.LogWarning("Socket already initialized! Close it first.");
            return false;
        }
        try
        {
            tcp = new TcpListener(IPAddress.Any, ListenPort);
            tcp.Start();
            Debug.Log($"Server listening on port: {ListenPort}");
            OnMessageReceive = handler;
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Error creating TCP listener: {ex.Message}");
            CloseTCP();
            return false;
        }
    }

    public void Close()
    {
        CloseTCP();
    }

    public void BroadcastTCPMessage(string message)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        BroadcastTCPBytes(bytes);
    }

    private void BroadcastTCPBytes(byte[] bytes)
    {
        if (tcp == null) return;

        foreach (var client in Connections)
        {
            if (client.Connected)
            {
                SendTCPBytes(client, bytes);
            }
        }
    }

    private void SendTCPBytes(TcpClient client, byte[] bytes)
    {
        if (client == null) return;

        try
        {
            client.GetStream().Write(bytes, 0, bytes.Length);
        }
        catch (SocketException e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    void OnDisable()
    {
        CloseTCP();
    }

    void Update()
    {
        ReceiveTCP();
    }

    private void ReceiveTCP()
    {
        if (tcp == null) return;

        while (tcp.Pending())
        {
            TcpClient tcpClient = tcp.AcceptTcpClient();
            Debug.Log($"New connection received from: {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}");
            Connections.Add(tcpClient);

            // Welcome message
            byte[] bytes = Encoding.UTF8.GetBytes(OnConnectionMessage);
            SendTCPBytes(tcpClient, bytes);
        }

        foreach (var client in Connections)
        {
            if (!client.Connected)
            {
                Debug.Log("Client disconnected");
                HandleClientDisconnection(client);
                Connections.Remove(client);
                return;
            }

            while (client.Available > 0)
            {
                byte[] data = new byte[client.Available];
                client.GetStream().Read(data, 0, client.Available);

                string message = ParseString(data);
                Debug.Log($"Message TCP reçu : {message}");
                string[] parts = message.Split(' ');
                string action = parts[0];

                try
                {
                    if (action == "connect")
                    {
                        string clientId = parts[1];
                        HandleConnect(clientId, (IPEndPoint)client.Client.RemoteEndPoint, Vector3.zero, Vector3.zero, 0);
                    }
                    else if (action == "disconnect")
                    {
                        string clientId = parts[1];
                        HandleDisconnect(clientId);
                        OnPlayerDisconnectReceived?.Invoke(clientId);
                    }
                    else if(action == "getConnectedClientsIds")
                    {
                        sendConnectedClientsIds();
                    }
                    else if (action == "getBonus")
                    {
                        sendBonus();
                    }
                    else if (action == "updateBonus")
                    {
                        string bonusId = parts[1];
                        string newIsActive = parts[2];
                        if (newIsActive == "true")
                        {
                            UpdateBonusIsActive(bonusId, true);
                        } else if (newIsActive == "false")
                        {
                            UpdateBonusIsActive(bonusId, false);
                        }
                        sendBonus();
                    }
                    else if (action == "incrementScore")
                    {
                        string clientId = parts[1];
                        IncrementClientScore(clientId);
                        sendConnectedClientsIds();
                    }
                    else if (action == "spawn")
                    {
                        Debug.Log($"Message spawn reçu avec {parts.Length} parties"); // Log de debug
                        if (parts.Length >= 8)
                        {
                            string playerID = parts[1];
                            Vector3 position = new Vector3(
                                float.Parse(parts[2]),
                                float.Parse(parts[3]),
                                float.Parse(parts[4])
                            );
                            Quaternion rotation = Quaternion.Euler(
                                float.Parse(parts[5]),
                                float.Parse(parts[6]),
                                float.Parse(parts[7])
                            );
                            
                            Debug.Log($"Traitement du spawn pour le joueur {playerID} à la position {position}");
                            OnPlayerSpawnReceived?.Invoke(playerID, position, rotation);
                        }
                        else
                        {
                            Debug.LogError($"Format de message spawn invalide : {message}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown action: {action}");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Error processing message: {ex.Message}");
                }
            }
        }
    }

    private void HandleConnect(string clientId, IPEndPoint RemoteEndPoint, Vector3 position, Vector3 rotation, int score)
    {
        EntityData newClient = new EntityData
        {
            ID = clientId,
            RemoteEndPoint = new SerializableEndPoint(RemoteEndPoint),
            position = position,
            rotation = rotation,
            score = score
        };
        ConnectedClients.Add(newClient);
        sendConnectedClientsIds();
    }

    private void HandleClientDisconnection(TcpClient client)
    {
        IPEndPoint remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
        if (remoteEndPoint != null)
        {
            string clientId = ConnectedClients.FirstOrDefault(c => c.RemoteEndPoint.Address.ToString() == remoteEndPoint.Address.ToString())?.ID;
            if (clientId != null)
            {
                HandleDisconnect(clientId);
                OnPlayerDisconnectReceived?.Invoke(clientId);
            }
        }
    }

    private void HandleDisconnect(string clientId)
    {
    Debug.Log($"[Server] Tentative de déconnexion du joueur : {clientId}");
    
    // Supprimer le client de la liste des clients connectés
    ConnectedClients.RemoveAll(client => client.ID == clientId);
    sendConnectedClientsIds();
    Debug.Log($"[Server] Joueur déconnecté et nettoyé : {clientId}");
    BroadcastTCPMessage($"disconnect_success {clientId}");
    Debug.Log($"[Server] Déclenchement de l'événement OnPlayerDisconnectReceived pour {clientId}");
    OnPlayerDisconnectReceived?.Invoke(clientId);
    }

    private void UpdateBonusIsActive(string bonusId, bool newIsActive)
    {
        for(int i = 0; i < bonus.Count; i++)
        {
            if(bonusId == bonus[i].ID)
            {
                bonus[i].isActive = newIsActive;
            }
        }
    }

    private void IncrementClientScore(string clientId)
    {
        for(int i = 0; i < ConnectedClients.Count; i++)
        {
            if(clientId == ConnectedClients[i].ID)
            {
                ConnectedClients[i].score++;
            }
        }
    }

    private void sendConnectedClientsIds()
    {
        UpdateConnectedClientsIds();
        EntityDataList dataList = new EntityDataList { entities = ConnectedClientsIds };
        string jsonData = JsonUtility.ToJson(dataList);
        BroadcastTCPMessage(jsonData);
    }

    private void sendBonus()
    {
        BonusListWrapper wrapper = new BonusListWrapper { bonuses = bonus };
        string jsonData = JsonUtility.ToJson(wrapper);
        BroadcastTCPMessage(jsonData);
    }

    private void UpdateConnectedClientsIds()
    {
        ConnectedClientsIds.Clear();
        foreach (var client in ConnectedClients)
        {
            ConnectedClientsIds.Add(client.ID);
        }
    }

    private string ParseString(byte[] bytes)
    {
        string message = Encoding.UTF8.GetString(bytes);
        OnMessageReceive?.Invoke(message);
        return message;
    }

    private void CloseTCP()
    {
        if (tcp != null)
        {
            tcp.Stop();
            tcp = null;
        }
        Connections.Clear();
        OnMessageReceive = null;
    }
    
}