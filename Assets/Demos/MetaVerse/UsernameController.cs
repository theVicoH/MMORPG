using UnityEngine;

public class UsernameController : MonoBehaviour
{
    public TMPro.TMP_Text TxtName;
    public CharacterController Controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TxtName.text = Controller.username;
    }
}
