using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class UDPReceiver : MonoBehaviour
{
    public int ListenPort = 25000;
    UdpClient udp;
    IPEndPoint localEP;
    IPEndPoint sourceEP;
    IPEndPoint lastSenderEP; // Pour stocker l'adresse de l'expéditeur

    public delegate void UDPMessageReceive(string message);

    private UDPMessageReceive OnMessageReceive;

    public bool Listen(UDPMessageReceive handler) {
        if (udp != null) {
            Debug.LogWarning("Socket already initialized! Close it first.");
            return false;
        }

        try
        {
            localEP = new IPEndPoint(IPAddress.Any, ListenPort);
            sourceEP = new IPEndPoint(IPAddress.Any, 0);
            
            udp = new UdpClient();
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udp.ExclusiveAddressUse = false;
            udp.Client.Bind(localEP);

            Debug.Log("Server listening on port: " + ListenPort);

            OnMessageReceive = handler;
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Error creating UDP listener on port: " + ListenPort + ": " + ex.Message);
            CloseUDP();
            return false;
        }
    }

    // Nouvelle méthode pour envoyer un message à l'expéditeur
    public void SendUDPMessage(string message) {
        if (udp == null || lastSenderEP == null) return;

        try {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
            udp.Send(bytes, bytes.Length, lastSenderEP);
        }
        catch (SocketException e) {
            Debug.LogWarning("Error sending UDP message: " + e.Message);
        }
    }

    public void Close() {
        CloseUDP();
    }

    public bool IsListening {
        get {
            return (udp != null);
        }
    }

    void OnDisable() {
        CloseUDP();
    }

    void Update() {
        ReceiveUDP();
    }

    private void ReceiveUDP() {
        if (udp == null) { return; }

        while (udp.Available > 0)
        {
            byte[] data = udp.Receive(ref sourceEP);
            lastSenderEP = sourceEP; // Stocke l'adresse de l'expéditeur

            try
            {
                ParseString(data);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error receiving UDP message: " + ex.Message);
            }
        }
    }

    private void ParseString(byte[] bytes) {
        string message = System.Text.Encoding.UTF8.GetString(bytes);
        OnMessageReceive?.Invoke(message);
    }

    private void CloseUDP() {
        if (udp != null) {
            udp.Close();
            udp = null;
        }
        OnMessageReceive = null;
        lastSenderEP = null;
    }
}
