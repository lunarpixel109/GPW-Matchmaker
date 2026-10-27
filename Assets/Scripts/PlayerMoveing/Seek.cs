using PlayerMoveing;
using UnityEngine;

public class Seek : SteeringBehaviour
{
    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        // Get the target position from the mouse input
        Vector3 targetPosition = steeringAgent.targetPosition;
        //Debug.Log(targetPosition + " Seek");

        // Get the desired velocity for seek and limit to maxSpeed
        desiredVelocity = Vector3.Normalize(targetPosition - transform.position) * steeringAgent.maxCurrentSpeed;

        // Calculate steering velocity
        steeringVelocity = desiredVelocity - steeringAgent.currentVelocity;
        return steeringVelocity;
    }
}
