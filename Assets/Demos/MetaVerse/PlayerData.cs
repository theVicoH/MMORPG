using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerData : MonoBehaviour
{
    
    public TMP_InputField username;
    public Button validateButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        validateButton.onClick.AddListener(ValidateInput);
    }

    // Update is called once per frame
    void ValidateInput()
    {

        string playerInput = username.text;

        if (!string.IsNullOrWhiteSpace(playerInput))
        {
            Globals.playerName = playerInput;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Metaverse");
        }
        else
        {
            Debug.Log("Les informations du champ username sont incorrectes");
        }
    }
}

