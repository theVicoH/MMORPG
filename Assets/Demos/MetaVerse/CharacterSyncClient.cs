using UnityEngine;
using System.Net;
public class CharacterSyncClient : MonoBehaviour
{
    UDPService UDP;
    Animator anim;

    void Awake() {
        if (Globals.IsServer) {
            enabled = false;
        }
        anim = GetComponent<Animator>();
    }

    void Start() {
        UDP = FindFirstObjectByType<UDPService>();

        UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            if (!message.StartsWith("CHAR_UPDATE")) { return; }

            string[] tokens = message.Split('|');
            string json = tokens[1];

            CharacterState state = JsonUtility.FromJson<CharacterState>(json);
            
            transform.position = state.Position;
            transform.rotation = state.Rotation;
            anim.SetFloat("Walk", state.WalkAnimation);
        };
    }
}