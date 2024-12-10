using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public string playerID;    
    public string username;
    public float walkSpeed = 3f;
    public float rotateSpeed = 250f;
    public float smoothTime = 0.1f;

    private Rigidbody rb;
    private Animator animator;

    private Vector2 inputDirection;
    private Vector3 currentVelocity;
    private Vector3 targetVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (!Globals.IsServer) {
            string playerName = Globals.playerName;
            username = playerName;
        }
    }

    void Update()
    {
        if (!IsLocalPlayer()) return;

        inputDirection = GetZQSDInput();

        if (animator != null)
        {
            animator.SetFloat("Walk", inputDirection.y);
        }
    }

    void FixedUpdate()
    {
        if (!IsLocalPlayer()) return;

        targetVelocity = transform.forward * walkSpeed * inputDirection.y;
        
        Vector3 smoothVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, smoothTime);
        
        rb.linearVelocity = smoothVelocity;
        
        Quaternion rotation = Quaternion.AngleAxis(rotateSpeed * Time.fixedDeltaTime * inputDirection.x, Vector3.up);
        rb.MoveRotation(rb.rotation * rotation);
    }

    private Vector2 GetZQSDInput()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.Z)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;
        if (Input.GetKey(KeyCode.Q)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;

        return new Vector2(horizontal, vertical).normalized;
    }

    private bool IsLocalPlayer()
    {
        return playerID == ClientManager.LocalPlayerID;
    }
}