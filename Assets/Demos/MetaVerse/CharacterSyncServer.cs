using UnityEngine;

[System.Serializable]
public class CharacterState {
    public Vector3 Position;
    public Quaternion Rotation;
    public float WalkAnimation;
}

public class CharacterSyncServer : MonoBehaviour
{
    ServerManager ServerMan;
    float NextUpdateTimeout = -1;
    Animator anim;

    void Awake() {
        if (!Globals.IsServer) {
            enabled = false;
        }
        anim = GetComponent<Animator>();
    }

    void Start() {
        ServerMan = FindFirstObjectByType<ServerManager>();
    }

    void Update() {
        if (Time.time > NextUpdateTimeout) {
            CharacterState state = new CharacterState{
                Position = transform.position,
                Rotation = transform.rotation,
                WalkAnimation = anim.GetFloat("Walk")
            };

            string json = JsonUtility.ToJson(state);
            ServerMan.BroadcastUDPMessage("CHAR_UPDATE|" + json);
            NextUpdateTimeout = Time.time + 0.03f;
        }
    }
}