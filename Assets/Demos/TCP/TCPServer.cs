using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class EntityData
{
    public string ID;
    public Vector3 position;
    public Vector3 rotation;
    public int score;
}

[System.Serializable]
public class EntityDataList
{
    public List<EntityData> entities;
}

[System.Serializable]
public class BonusData
{
    public string ID;
    public Vector3 position;
    public bool isActive;
}

[System.Serializable]
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
    private List<EntityData> ConnectedClients = new List<EntityData>();

    public delegate void TCPMessageReceive(string message);
    private TCPMessageReceive OnMessageReceive;

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
                Connections.Remove(client);
                return;
            }

            while (client.Available > 0)
            {
                byte[] data = new byte[client.Available];
                client.GetStream().Read(data, 0, client.Available);

                string[] parts = ParseString(data).Split(' ');
                string action = parts[0];

                // TODO remplacer le randomSpawnPosition par des vrais positions sur la map pour faire apparaitres de gens au hasard sur la carte
                Vector3 randomPosition = new Vector3(Random.Range(-10, 10), 0.318325f, Random.Range(-10, 10));
                Vector3 initialRotation = new Vector3(0, 0, 0);
                Vector3 defaultRotation = Vector3.zero;

                try
                {
                    if (action == "connect")
                    {
                        string clientId = parts[1];
                        HandleConnect(clientId, randomPosition, defaultRotation, 0);
                    }
                    else if (action == "disconnect")
                    {
                        string clientId = parts[1];
                        HandleDisconnect(clientId);
                    }
                    else if(action == "getConnectedClients")
                    {
                        EntityDataList dataList = new EntityDataList { entities = ConnectedClients };
                        string jsonData = JsonUtility.ToJson(dataList);
                        BroadcastTCPMessage(jsonData);
                    }
                    else if (action == "getBonus")
                    {
                        BonusListWrapper wrapper = new BonusListWrapper { bonuses = bonus };
                        string jsonData = JsonUtility.ToJson(wrapper);
                        BroadcastTCPMessage(jsonData);
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
                        BonusListWrapper wrapper = new BonusListWrapper { bonuses = bonus };
                        string jsonData = JsonUtility.ToJson(wrapper);
                        BroadcastTCPMessage(jsonData);
                    }
                    else if (action == "incrementScore")
                    {
                        string clientId = parts[1];
                        IncrementClientScore(clientId);
                        EntityDataList dataList = new EntityDataList { entities = ConnectedClients };
                        string jsonData = JsonUtility.ToJson(dataList);
                        BroadcastTCPMessage(jsonData);
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

    private void HandleConnect(string clientId, Vector3 position, Vector3 rotation, int score)
    {
        EntityData newClient = new EntityData
        {
            ID = clientId,
            position = position,
            rotation = rotation,
            score = score
        };
        ConnectedClients.Add(newClient);

        BroadcastPlayerState(clientId, position, rotation, score);
        Debug.Log($"Client connected: {clientId}, Position: {position}, Rotation: {rotation}");
    }

    private void HandleDisconnect(string clientId)
    {
        ConnectedClients.RemoveAll(client => client.ID == clientId);
        EntityDataList dataList = new EntityDataList { entities = ConnectedClients };
        string jsonData = JsonUtility.ToJson(dataList);
        BroadcastTCPMessage(jsonData);
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

    public void BroadcastPlayerState(string playerID, Vector3 position, Vector3 rotation, int score)
    {
        EntityData playerData = new EntityData
        {
            ID = playerID,
            position = position,
            rotation = rotation,
            score = score
        };

        EntityDataList dataList = new EntityDataList { entities = ConnectedClients };
        string jsonData = JsonUtility.ToJson(dataList);
        BroadcastTCPMessage(jsonData);
    }
}
