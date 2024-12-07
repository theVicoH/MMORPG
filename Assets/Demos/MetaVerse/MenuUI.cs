using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void SetRole(bool isServer)
    {
        Globals.IsServer = isServer;
        Debug.Log(isServer ? "Mode Serveur sélectionné." : "Mode Client sélectionné.");
    }

    public void StartGame()
    {
        if (Globals.IsServer){
            SceneManager.LoadScene("Metaverse");
            Debug.Log("Launching the scene with server...");
            Debug.Log("Chargement de la scène Metaverse...");

        }else{
            Debug.Log("Launching the scene with client...");
            SceneManager.LoadScene("PlayerData");
            
        }
    }
}
