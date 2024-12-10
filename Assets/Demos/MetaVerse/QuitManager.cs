using UnityEngine;

public class QuitManager : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    private ClientManager clientManager;

    private void Start()
    {
        if (pausePanel == null)
        {
            Debug.LogError("[QuitManager] Pause Panel non assigné !");
        }
        else
        {
            pausePanel.SetActive(false);
            Debug.Log("[QuitManager] Pause Panel initialisé et désactivé.");
        }

        clientManager = FindObjectOfType<ClientManager>();
        if (clientManager == null)
        {
            Debug.LogError("[QuitManager] ClientManager introuvable !");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (pausePanel == null) return;

        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Debug.Log(isPaused ? "[QuitManager] Menu ouvert." : "[QuitManager] Menu fermé.");
    }

    public void QuitGame()
    {
        Debug.Log("[QuitManager] Déconnexion et suppression du joueur...");

        if (clientManager != null)
        {
            clientManager.SendDisconnectMessage();
            clientManager.DestroyPlayer();
        }
        else
        {
            Debug.LogWarning("[QuitManager] pas trouvé ClientManager trouvé pour gérer la déconnexion.");
        }

        Debug.Log("[QuitManager] Fin de Game");
        Application.Quit();
    }
}
