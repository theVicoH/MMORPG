using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public UDPReceiver Receiver;

    void Awake(){
    //desactiver l'objet si je ne suis pas le serveur
        if (!Globals.IsServer){
        gameObject.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Receiver.Listen(
        (string message) => {
            Debug.Log("message received from" + message);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
