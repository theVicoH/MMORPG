using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public string playerID;
    public float walkSpeed = 3f;
    public float rotateSpeed = 250f;

    private Rigidbody rb;
    private Animator animator;

    private Vector2 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
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

        Vector3 move = transform.forward * walkSpeed * Time.fixedDeltaTime * inputDirection.y;
        rb.MovePosition(rb.position + move);

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