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
        Debug.Log("Chargement de la scène Metaverse...");
        SceneManager.LoadScene("Metaverse");
    }
}
