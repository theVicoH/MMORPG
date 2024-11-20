using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterPlayer {
    Player1,
    Player2
}

public class CharacterController : MonoBehaviour
{
    public CharacterPlayer Player = CharacterPlayer.Player1;
    public float WalkSpeed = 3;
    public float RotateSpeed = 250;
    public UDPSender Sender; // Référence au UDPSender
    public float updateInterval = 0.1f; // Intervalle d'envoi UDP en secondes

    private Animator Anim;
    private MetaverseInput inputs;
    private InputAction PlayerAction;
    private Rigidbody rb;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private float timer = 0f;
    private int messageCount = 0;

    void Start()
    {
        // Initialisation standard
        Anim = GetComponent<Animator>();
        inputs = new MetaverseInput();
        switch (Player) {
            case CharacterPlayer.Player1:
                PlayerAction = inputs.Player1.Move;
                break;
            case CharacterPlayer.Player2:
                PlayerAction = inputs.Player2.Move;
                break;
        }

        PlayerAction.Enable();
        rb = GetComponent<Rigidbody>();

        // Initialisation pour l'UDP
        if (Sender == null)
        {
            Debug.LogError($"[CharacterController] UDPSender non assigné pour le {Player}!");
            return;
        }

        lastPosition = rb.position;
        lastRotation = rb.rotation;
        Debug.Log($"[CharacterController] {Player} initialisé à la position: {lastPosition}");
    }

    void FixedUpdate()
    {
        // Gestion du mouvement standard
        Vector2 vec = PlayerAction.ReadValue<Vector2>();
        Anim.SetFloat("Walk", vec.y);

        rb.MovePosition(rb.position + transform.forward * WalkSpeed * Time.fixedDeltaTime * vec.y);
        rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(RotateSpeed * Time.fixedDeltaTime * vec.x, Vector3.up));

        // Gestion de l'envoi UDP
        timer += Time.fixedDeltaTime;
        if (timer >= updateInterval && Sender != null)
        {
            if (HasMoved())
            {
                SendPositionAndRotation();
                lastPosition = rb.position;
                lastRotation = rb.rotation;
            }
            timer = 0f;
        }
    }

    private bool HasMoved()
    {
        return Vector3.Distance(lastPosition, rb.position) > 0.001f || 
               Quaternion.Angle(lastRotation, rb.rotation) > 0.1f;
    }

    private void SendPositionAndRotation()
    {
        messageCount++;
        string message = string.Format("{0}:POS:{1:F2},{2:F2},{3:F2}:ROT:{4:F2},{5:F2},{6:F2}",
            Player,                    // Identifiant du joueur
            rb.position.x,            // Position X
            rb.position.y,            // Position Y
            rb.position.z,            // Position Z
            rb.rotation.eulerAngles.x, // Rotation X
            rb.rotation.eulerAngles.y, // Rotation Y
            rb.rotation.eulerAngles.z  // Rotation Z
        );

        Sender.SendUDPMessage(message);
        Debug.Log($"[CharacterController] {Player} Message #{messageCount}: {message}");
    }

    void OnDisable()
    {
        PlayerAction.Disable();
        Debug.Log($"[CharacterController] {Player} désactivé - Total messages envoyés: {messageCount}");
    }
}
