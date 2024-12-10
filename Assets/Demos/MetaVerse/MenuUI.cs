using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void SetRole(bool isServer)
    {
        Globals.IsServer = isServer;
    }

    public void StartGame()
    {
        if (Globals.IsServer){
            SceneManager.LoadScene("Metaverse");
        }else{
            SceneManager.LoadScene("PlayerData");
        }
    }
}
