using PlayerMoveing;
using UnityEngine;

public class Arrival : SteeringBehaviour
{
    /// <summary>
    /// Controls how far from the target position should the agent start to slow down
    /// NOTE: [SerializeField] exposes a C# variable to Unity's inspector without making it public. Useful for encapsulating code
    /// while still giving access to the Unity inspector
    /// </summary>
    [SerializeField]
    protected float arrivalRadius = 200.0f;

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        Vector3 targetPosition = steeringAgent.targetPosition;
        //Debug.Log(targetPosition + " Arrival");
		
        var targetOffset = targetPosition - this.transform.position;
        var distance = targetOffset.magnitude;
        var rampedSpeed = steeringAgent.maxCurrentSpeed * (distance / arrivalRadius);
        var clippedSpeed = Mathf.Min(rampedSpeed, steeringAgent.maxCurrentSpeed);
        desiredVelocity = (clippedSpeed / distance) * targetOffset;
        steeringVelocity = desiredVelocity - steeringAgent.currentVelocity;
		
        return steeringVelocity;
    }
}