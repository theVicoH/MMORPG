using UnityEngine;
using System.Net;

public class CharacterSyncClient : MonoBehaviour
{
    ClientManager ClientManager;
    Animator anim;
    float NextUpdateTimeout = -1;
    DeadReckoning deadReckoning;
    CharacterController characterController;

    private Vector3 startPosition;
    private Vector3 targetPosition; 
    private float interpolationTime;
    private bool isInterpolating;
    private float lastMessageSentTime = 0f;

    private Vector2 lastInput = Vector2.zero;
    private bool inputChanged = false;

    private const float INTERPOLATION_PERIOD = 0.1f;
private const float UPDATE_INTERVAL = 0.1f;
private const float MIN_MESSAGE_INTERVAL = 0.1f; 

    void Awake() 
    {
        if (Globals.IsServer) {
            enabled = false;
            return;
        }
        anim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        deadReckoning = gameObject.AddComponent<DeadReckoning>();
    }

    void Start()
    {
        ClientManager = FindFirstObjectByType<ClientManager>();
        deadReckoning.Initialize(transform.position);

        ClientManager.UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            if (message.StartsWith("CHAR_POS|"))
            {
                string[] parts = message.Split('|');
                if (parts.Length == 2)
                {
                    CharacterState state = JsonUtility.FromJson<CharacterState>(parts[1]);
                    OnStateReceived(state);
                }
            }
        };
    }

    void Update()
    {
        if (Time.time > NextUpdateTimeout)
        {
            if (characterController.playerID != ClientManager.LocalPlayerID)
            {
                if (isInterpolating)
                {
                    interpolationTime += Time.deltaTime / INTERPOLATION_PERIOD;
                    if (interpolationTime > 1.0f)
                    {
                        interpolationTime = 1.0f;
                        isInterpolating = false;
                    }
                    transform.position = Vector3.Lerp(startPosition, targetPosition, interpolationTime);
                }
                else
                {
                    transform.position = deadReckoning.PredictPosition();
                }
            }
            else
            {
                Vector2 currentInput = GetPlayerInput();
                bool shouldSendUpdate = false;

                if (currentInput == Vector2.zero && lastInput != Vector2.zero)
                {
                    shouldSendUpdate = true;
                }
                else if (currentInput != lastInput)
                {
                    inputChanged = true;
                    shouldSendUpdate = true;
                }
                lastInput = currentInput;

                if (Vector3.Distance(transform.position, deadReckoning.LastPosition) > 0.1f &&
                    Time.time - lastMessageSentTime >= MIN_MESSAGE_INTERVAL)
                {
                    shouldSendUpdate = true;
                }

                if (shouldSendUpdate)
                {
                    CharacterState state = new CharacterState{
                        PlayerID = ClientManager.LocalPlayerID,
                        Position = transform.position,
                        Rotation = transform.rotation,
                        WalkAnimation = anim.GetFloat("Walk"),
                        Velocity = currentInput == Vector2.zero ? Vector3.zero : deadReckoning.LastVelocity
                    };

                    string json = JsonUtility.ToJson(state);
                    ClientManager.UDP.SendUDPMessage("CHAR_POS|" + json, ClientManager.ServerEndpoint);
                    
                    lastMessageSentTime = Time.time;
                    inputChanged = false;
                }

                deadReckoning.UpdateState(transform.position);
            }
            NextUpdateTimeout = Time.time + UPDATE_INTERVAL;
        }
    }

    private Vector2 GetPlayerInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        return new Vector2(horizontal, vertical);
    }

    public void OnStateReceived(CharacterState state)
    {
        if (characterController.playerID != ClientManager.LocalPlayerID)
        {
            deadReckoning.UpdateState(state.Position, state.Velocity);
            startPosition = transform.position;
            targetPosition = state.Position;
            transform.rotation = state.Rotation;
            anim.SetFloat("Walk", state.WalkAnimation);
            interpolationTime = 0f;
            isInterpolating = true;
        }
    }
}
