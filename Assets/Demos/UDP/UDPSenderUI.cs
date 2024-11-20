using UnityEngine;

public class UDPSenderUI : MonoBehaviour
{
    public UDPSender Sender;
    public TMPro.TMP_InputField InpMessage;
    public TMPro.TMP_InputField InpIP;
    public TMPro.TMP_InputField InpPort;
    public TMPro.TMP_Text TxtReceivedMessage; // Nouveau champ pour afficher les messages reçus

    void Start() {
        InpIP.text = Sender.DestinationIP;
        InpPort.text = Sender.DestinationPort.ToString();
        
        // S'abonne à l'événement de réception de message
        Sender.OnMessageReceived += HandleMessageReceived;
    }

    void OnDestroy() {
        // Se désabonne de l'événement
        if (Sender != null) {
            Sender.OnMessageReceived -= HandleMessageReceived;
        }
    }

    public void SendMessageViaUDP() {
        string IP = InpIP.text;
        int port = 0;
        if (!int.TryParse(InpPort.text, out port)) {
            Debug.LogWarning("Invalid port: " + InpPort.text);
            return;
        }

        string message = InpMessage.text;

        Sender.DestinationIP = IP;
        Sender.DestinationPort = port;
        Sender.SendUDPMessage(message);
    }

    // Nouvelle méthode pour gérer les messages reçus
    private void HandleMessageReceived(string message) {
        TxtReceivedMessage.text = message;
    }
}