using UnityEngine;

public class CharacterSyncClient : MonoBehaviour
{
    ClientManager ClientManager;
    Animator anim;
    float NextUpdateTimeout = -1;

    void Awake() {
        if (Globals.IsServer) {
            enabled = false;
            return;
        }
        anim = GetComponent<Animator>();
    }

    void Start() {
        ClientManager = FindFirstObjectByType<ClientManager>();
    }

    void Update() {
        if (Time.time > NextUpdateTimeout) {
            // Envoie l'état au serveur
            CharacterState state = new CharacterState{
                Position = transform.position,
                Rotation = transform.rotation,
                WalkAnimation = anim.GetFloat("Walk")
            };

            string json = JsonUtility.ToJson(state);
            ClientManager.UDP.SendUDPMessage("CHAR_UPDATE|" + json, ClientManager.ServerEndpoint);
            NextUpdateTimeout = Time.time + 0.03f;
        }
    }
}
