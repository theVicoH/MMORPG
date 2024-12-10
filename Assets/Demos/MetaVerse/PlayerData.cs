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
            PlayerPrefs.SetString("PlayerName", playerInput); // Save input
            PlayerPrefs.Save();
            Debug.Log($"Player name saved: {playerInput}");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Metaverse");
        }
        else
        {
            Debug.Log("Input is empty or invalid!");
        }

    // *************************
        //  string playerInput = username.text;

        // if (string.IsNullOrWhiteSpace(playerInput))
        // {
        //     Debug.Log("Input is empty or invalid!");
        // }
        // else
        // {
        //     Debug.Log($"Player entered: {playerInput}");
        //     // Add your custom validation logic here
        //     SceneManager.LoadScene("Metaverse");
        // }
    }
}

