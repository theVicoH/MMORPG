using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public UDPService UDP;
    public int ListenPort = 25000;
    public Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>(); 

    void Awake() {
        if (!Globals.IsServer) {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        UDP.Listen(ListenPort);

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
                }
                else if (message.StartsWith("CHAR_UPDATE")) {
                    BroadcastUDPMessage(message, sender);
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
}
