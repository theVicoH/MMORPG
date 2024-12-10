using UnityEngine;

public class DeadReckoning : MonoBehaviour 
{
    public float predictionThreshold = 0.1f;
    public float smoothingFactor = 10f;

    private Vector3 lastPosition;
    private Vector3 lastVelocity;
    private float lastUpdateTime;

    public Vector3 LastPosition => lastPosition;
    public Vector3 LastVelocity => lastVelocity;

    public void Initialize(Vector3 position)
    {
        lastPosition = position;
        lastVelocity = Vector3.zero;
        lastUpdateTime = Time.time;
    }

    public Vector3 PredictPosition()
    {
        float deltaTime = Time.time - lastUpdateTime;
        return lastPosition + (lastVelocity * deltaTime);
    }

    public void UpdateState(Vector3 newPosition, Vector3 newVelocity) 
    {
        lastPosition = newPosition;
        lastVelocity = newVelocity;
        lastUpdateTime = Time.time;
    }

    public void UpdateState(Vector3 newPosition) 
    {
        float deltaTime = Time.time - lastUpdateTime;
        if (deltaTime > 0)
        {
            lastVelocity = (newPosition - lastPosition) / deltaTime;
        }
        lastPosition = newPosition;
        lastUpdateTime = Time.time;
    }
}