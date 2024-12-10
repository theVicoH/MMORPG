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
    public List<BonusData> bonus;
}

public class TCPServer : MonoBehaviour
{
    public int ListenPort = 25000;
    // public string OnConnectionMessage = "Welcome client!";

    private TcpListener tcp;
    private List<TcpClient> Connections = new List<TcpClient>();
    public List<EntityData> ConnectedClients = new List<EntityData>();
    private List<string> ConnectedClientsIds = new List<string>();

    public delegate void TCPMessageReceive(string message);
    private TCPMessageReceive OnMessageReceive;
    public event Action<string> OnPlayerDisconnectReceived;

    public BonusManager BonusMan;

    private List<BonusData> bonus = new List<BonusData>
    {
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(248.54f, 0.2789344f, 255.5f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(248.54f, 0.2789344f, 241f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(253.9f, 0.2789344f, 241f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(225.11f, 0.2789344f, 255.5f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(225.04f, 0.2789344f, 241f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(230.04f, 0.2789344f, 241f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(179.97f, 0.2789344f, 242.8f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(179.97f, 0.2789344f, 256.77f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(174.6f, 0.2789344f, 256.77f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(118.4f, 0.2789344f, 239.2f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(160.9f, 0.2789344f, 289.01f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(160.9f, 0.2789344f, 283.79f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(131.6f, 0.2789344f, 305.7f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(156.06f, 0.2789344f, 343.86f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(185.89f, 0.2789344f, 343.86f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(213.5f, 0.2789344f, 320.08f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(229.65f, 0.2789344f, 343.76f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(243.12f, 0.2789344f, 331.4f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(253.6f, 0.2789344f, 309.7f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(284.3f, 0.2789344f, 306.7f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(324.5f, 0.2789344f, 309.2f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(315.3f, 0.2789344f, 296.1f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(228.1f, 0.2789344f, 279.8f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(351f, 0.2789344f, 247.8f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(311.5f, 0.2789344f, 207.1f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(268.9f, 0.2789344f, 202.4f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(268.9f, 0.2789344f, 190.3f),
            isActive = true
        },
        new BonusData
        {
            ID = System.Guid.NewGuid().ToString(),
            position = new Vector3(251.3f, 0.2789344f, 170.5f),
            isActive = true
        },
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
            // byte[] bytes = Encoding.UTF8.GetBytes(OnConnectionMessage);
            // SendTCPBytes(tcpClient, bytes);
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
                        sendBonus();
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
                        string updateBonusMessage = "UPDATEBONUS|" + bonusId + "||";
                        BroadcastTCPMessage(updateBonusMessage);
                    }
                    else if (action == "incrementScore")
                    {
                        string clientId = parts[1];
                        IncrementClientScore(clientId);
                        sendConnectedClientsIds();
                    }
                    // else if (action == "getClientScore")
                    // {
                    //     string clientId = parts[1];
                    //     int clientScore = getClientScore(clientId);
                    //     string getClientScoreMessage = "CLIENTSCORE" + clientScore + "||";
                    // }
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
        // Supprimer le client de la liste des clients connectés
        ConnectedClients.RemoveAll(client => client.ID == clientId);
        sendConnectedClientsIds();

        // Supprimer le joueur du serveur
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
        string prefixedAndSuffixedData = "CONNECTED_CLIENTS|" + jsonData + "||";
        BroadcastTCPMessage(prefixedAndSuffixedData);
    }

    private void sendBonus()
    {
        string jsonBonus = JsonUtility.ToJson(new BonusListWrapper { bonus = bonus });
        string prefixedData = "BONUS|" + jsonBonus + "||";
        BroadcastTCPMessage(prefixedData);
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