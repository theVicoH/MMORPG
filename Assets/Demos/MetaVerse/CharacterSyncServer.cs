using UnityEngine;
using System.Net;

public class CharacterSyncServer : MonoBehaviour
{
    ServerManager ServerMan;
    Animator anim;
    DeadReckoning deadReckoning;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private float interpolationTime;
    private bool isInterpolating;
    private float lastUpdateTime;
    private Vector3 velocityBeforeInterpolation;
    private bool hasReceivedFirstUpdate = false;
    private Vector3 currentPredictedPosition;
    private Vector3 previousPosition;
    private float timeSinceLastUpdate;
    private Vector3 currentVelocity;

    private const float MOVEMENT_SPEED = 3f;
    private const float INTERPOLATION_PERIOD = 0.1f;
    private const float PREDICTION_WEIGHT = 0.8f;

    void Awake()
    {
        if (!Globals.IsServer)
        {
            enabled = false;
            return;
        }
        anim = GetComponent<Animator>();
        deadReckoning = gameObject.AddComponent<DeadReckoning>();
    }

    void Start()
    {
        ServerMan = FindFirstObjectByType<ServerManager>();
        deadReckoning.Initialize(transform.position);
        startPosition = transform.position;
        targetPosition = transform.position;
        startRotation = transform.rotation;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (isInterpolating)
        {
            interpolationTime += Time.deltaTime / INTERPOLATION_PERIOD;
            if (interpolationTime > 1.0f)
            {
                interpolationTime = 1.0f;
                isInterpolating = false;
                previousPosition = transform.position;
            }

            Vector3 predictedPosition = startPosition + (velocityBeforeInterpolation * timeSinceLastUpdate);
            
            float t = Mathf.SmoothStep(0, 1, interpolationTime);
            Vector3 interpolatedPosition = Vector3.Lerp(startPosition, targetPosition, t);
            transform.position = Vector3.Lerp(interpolatedPosition, predictedPosition, PREDICTION_WEIGHT);

            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t * 2f);
        }
        else
        {
            Vector3 predictedPosition = deadReckoning.PredictPosition();
            transform.position = Vector3.Lerp(transform.position, predictedPosition, Time.deltaTime * 10f);
        }
    }

    public void OnStateReceived(CharacterState state)
    {
        timeSinceLastUpdate = 0f;
        
        if (!hasReceivedFirstUpdate)
        {
            transform.position = state.Position;
            transform.rotation = state.Rotation;
            deadReckoning.Initialize(state.Position);
            previousPosition = state.Position;
            currentVelocity = state.Velocity;
            hasReceivedFirstUpdate = true;
        }
        else 
        {
            previousPosition = transform.position;
            startPosition = transform.position;
            targetPosition = state.Position;
            startRotation = transform.rotation;
            targetRotation = state.Rotation;
            velocityBeforeInterpolation = state.Velocity;
        }

        deadReckoning.UpdateState(state.Position, state.Velocity);
        interpolationTime = 0f;
        isInterpolating = true;
        
        if (anim != null)
        {
            anim.SetFloat("Walk", state.WalkAnimation);
        }

        lastUpdateTime = Time.time;
    }
}