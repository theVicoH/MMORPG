using UnityEngine;

public class Engineer : MonoBehaviour
{
    private string playerID;
    private TCPClient tcpClient;

    private void Start()
    {
       /* tcpClient = Object.FindFirstObjectByType<TCPClient>();
        if (tcpClient == null)
        {
            Debug.LogError("TCPClient non trouvé. Assure-toi qu'il est ajouté à la scène.");
            return;
        } */

        playerID = System.Guid.NewGuid().ToString();

        ConnectToServer();
        Debug.Log($"Joueur connecté avec l'ID : {playerID}");
    }

    private void OnDestroy()
    {
        DisconnectFromServer();
        Debug.Log($"Joueur déconnecté - ID : {playerID}");
    }

    // Méthode pour envoyer le message de connexion au serveur
    private void ConnectToServer()
    {
        if (tcpClient != null && tcpClient.IsConnected)
        {
            tcpClient.SendTCPMessage($"connect {playerID}");
        }
        else
        {
            // Debug.LogError("Impossible de se connecter au serveur. TCPClient non connecté.");
        }
    }

    // Méthode pour envoyer le message de déconnexion au serveur
    private void DisconnectFromServer()
    {
        if (tcpClient != null && tcpClient.IsConnected)
        {
            tcpClient.SendTCPMessage($"disconnect {playerID}");
        }
        else
        {
            Debug.LogWarning("Impossible d'envoyer le message de déconnexion. TCPClient non connecté.");
        }
    }

    // Permet de récupérer l'ID du joueur si nécessaire
    public string GetPlayerID()
    {
        return playerID;
    }
}
