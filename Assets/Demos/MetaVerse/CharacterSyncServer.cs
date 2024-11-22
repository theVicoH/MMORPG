using UnityEngine;
using System.Net;

[System.Serializable]
public class CharacterState {
    public Vector3 Position;
    public Quaternion Rotation;
    public float WalkAnimation;
}

public class CharacterSyncServer : MonoBehaviour
{
    ServerManager ServerMan;
    Animator anim;

    void Awake() {
        if (!Globals.IsServer) {
            enabled = false;
            return;
        }
        anim = GetComponent<Animator>();
    }

    void Start() {
        ServerMan = FindFirstObjectByType<ServerManager>();

        ServerMan.UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            if (message.StartsWith("CHAR_POS")) {
                string[] tokens = message.Split('|');
                string json = tokens[1];
                CharacterState state = JsonUtility.FromJson<CharacterState>(json);
                
                transform.position = state.Position;
                transform.rotation = state.Rotation;
                anim.SetFloat("Walk", state.WalkAnimation);
            }
        };
    }
}