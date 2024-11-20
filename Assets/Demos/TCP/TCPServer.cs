using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

public class EntityData {
    public string ID;
    public Vector3 position;
    public Vector3 rotation;
}

public class TCPServer : MonoBehaviour
{
    public int ListenPort = 25000;
    public string OnConnectionMessage = "Welcome client!";

    TcpListener tcp;
    IPEndPoint localEP;

    public delegate void TCPMessageReceive(string message);

    private TCPMessageReceive OnMessageReceive;

    private List<TcpClient> Connections = new List<TcpClient>();

    private List<EntityData> ConnectedClients = new List<EntityData>();

    public bool Listen(TCPMessageReceive handler) {
        if (tcp != null) {
            Debug.LogWarning("Socket already initialized! Close it first.");
            return false;
        }
        try {
            tcp = new TcpListener(IPAddress.Any, ListenPort);
            tcp.Start();
            Debug.Log("Server listening on port: " + ListenPort);
            OnMessageReceive = handler;
            return true;
        } catch (System.Exception ex)
        {
            Debug.LogWarning("Error creating TCP listener: " + ex.Message);
            CloseTCP();
            return false;
        }
    }

    public void BroadcastTCPMessage(string message) {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
        BroadcastTCPBytes(bytes);
    }

    public void Close() {
        CloseTCP();
    }

    public bool IsListening {
        get {
            return (tcp != null);
        }
    }

    public int ConnectionCount {
        get {
            return ConnectedClients.Count;
        }
    }

    private void BroadcastTCPBytes(byte[] bytes) {
        if (tcp == null) {
            return;
        }
        foreach (TcpClient client in Connections) {
            if (!client.Connected) {
                continue;
            }

            SendTCPBytes(client, bytes);

        }        
    }

    private void SendTCPBytes(TcpClient client, byte[] bytes) {
        if (client == null) {
            return;
        }

        try {
            client.GetStream().Write(bytes, 0, bytes.Length);            
        } catch (SocketException e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    void OnDisable() {
        CloseTCP();
    }

    void Update() {
        ReceiveTCP();
    }


    private void ReceiveTCP() {
        if (tcp == null) { return; }

        while (tcp.Pending()) {
            TcpClient tcpClient = tcp.AcceptTcpClient();       
            Debug.Log("New connection received from: " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address);
            Connections.Add(tcpClient);

            // Welcome message
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(OnConnectionMessage);
            SendTCPBytes(tcpClient, bytes);
        }

        foreach (TcpClient client in Connections) {
            if (!client.Connected) {
                Debug.Log("Client disconnected");
                Connections.Remove(client);
                return;
            }

            while (client.Available > 0)
            {   
                byte[] data = new byte[client.Available];
                client.GetStream().Read(data, 0, client.Available);

                string[] parts = ParseString(data).Split(' ');
                string action = parts[0].ToString();
                string clientId = parts[1].ToString();
                // TODO remplacer le randomSpawnPosition par des vrais positions sur la map pour faire apparaitres de gens au hasard sur la carte
                Vector3 randomSpawnPosition = new Vector3(Random.Range(-10, 10), 0.318325f, Random.Range(-10, 10));
                Vector3 initialRotation = new Vector3(0, 0, 0);

                try
                {
                    if (action == "connect")
                    {
                        EntityData newClient = new EntityData
                        {
                            ID = clientId,
                            position = randomSpawnPosition,
                            rotation = initialRotation 
                        };
                        ConnectedClients.Add(newClient);
                        string jsonData = JsonUtility.ToJson(newClient);
                        BroadcastTCPMessage(jsonData);
                    }
                    else if (action == "disconnect")
                    {
                        RemoveClient(clientId);
                    }
                    else
                    {
                        Debug.Log("Ni 'connect' ni 'disconnect' ne sont présents dans la chaîne.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("Error receiving TCP message: " + ex.Message);
                }
            }
        }
    }

    private string ParseString(byte[] bytes) {
        string message = System.Text.Encoding.UTF8.GetString(bytes);
        OnMessageReceive.Invoke(message);
        return message;
    }

    private void RemoveClient(string id)
    {
        for (int i = 0; i < ConnectedClients.Count; i++) 
            {
                if (id == ConnectedClients[i].ID)
                {
                    ConnectedClients.RemoveAt(i);
                }
            }
    }

    private void CloseTCP() {
        if (tcp != null) {
            tcp.Stop();            
            tcp = null;            
        }
        Connections.Clear();
        OnMessageReceive = null;
    }

}