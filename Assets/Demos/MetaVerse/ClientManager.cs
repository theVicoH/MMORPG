using UnityEngine;

public class ClientManager : MonoBehaviour
{

    // public UDPSender Sender;
    public TCPClient TCP;

    private float NextCoucouTimeout = -1;
    void Awake(){
    //desactiver l'objet si je ne suis pas le client
        if (Globals.IsServer){
        gameObject.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Client");
        // TODO pour l'init des bonus faut juste stocké autre part les bonus et ajout le awake avec la vérif du serveur ou pas, si serveur alors instiaciate tous les bonus, mettre ça dans un script, l'assigné à un empty que tu met dans l'arborecense en à gauche dans unity, cad en bas de comms etc ou ailleurs
        // TODO a faire ça à l'init en gros tu ouvre tcp, tcp te dis welcome, tu lui dit connect 123, tu vérifies ce que tu as reçu et après tu traites le retour
        TCP.Connect(
            (string message) => {
                if (message.StartsWith("welcome")) {
                    TCP.SendTCPMessage("getBonus");
                } else if (message.StartsWith("setbonus")) {
                    string[] array = message.Split("|");
                    Debug.Log(array[1]);
                }
            }
        );
        // TODO Le TCP.Connect est à faire qu'une seule fois donc les prochaines requete au tcp sont à faire comme ceci  
        // TCP.SendTCPMessage(
        //     (string message) => {
        //         if (message.StartsWith("welcome")) {
        //             TCP.SendTCPMessage("getBonus");
        //         } else if (message.StartsWith("setbonus")) {
        //             string[] array = message.Split("|");
        //             Debug.Log(array[1]);
        //         }
        //     }
        // );
    }

    // Update is called once per frame
    void Update()
    {
        // TCP.SendTCPMessage("coucou");
    }
}
