using UnityEngine;

public class UsernameController : MonoBehaviour
{
    public TMPro.TMP_Text TxtName;
    public CharacterController Controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Debug.Log($"Test 1 controller {TxtName.text}");
        Debug.Log($"Test 2 controller {Controller.username}");
        
    }

    // Update is called once per frame
    void Update()
    {
        TxtName.text = Controller.username;
        Debug.Log($"Test username controller {TxtName.text}");
    }
}
