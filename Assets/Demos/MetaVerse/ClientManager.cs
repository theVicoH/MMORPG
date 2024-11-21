using UnityEngine;

public class ClientManager : MonoBehaviour
{

    public UDPSender Sender;

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

    }

    // Update is called once per frame
    void Update()
    {
        Sender.SendUDPMessage("coucou");
    }
}
