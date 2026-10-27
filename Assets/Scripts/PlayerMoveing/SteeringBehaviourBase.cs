using System;
using UnityEngine;

public class SteeringBehaviourBase : MonoBehaviour {
    public float maxCurrentSpeed;
    
    public float maxSteeringSpeed;
    
    public Vector3 currentVelocity;
    
    public Vector3 steeringVelocity;

    private void Update() {
        UpdateDirection();
        UpdatePosition();
    }

    protected virtual void UpdatePosition()
    {
        // Limit steering velocity to supplied maximum so it can be used to adjust current velocity. Ensure to subtract this limnited
        // amount from the current value of the steering velocity so that it decreases as over multiple game frames to reach the target
        var limitedsteeringVelocity = SteeringHelper.LimitVector(steeringVelocity, maxSteeringSpeed * Time.deltaTime);
        steeringVelocity -= limitedsteeringVelocity;

        // Set final velocity
        currentVelocity += limitedsteeringVelocity;
        currentVelocity = SteeringHelper.LimitVector(currentVelocity, maxCurrentSpeed);

        transform.position += currentVelocity * Time.deltaTime;
    }
    
    protected virtual void UpdateDirection()
    {
        // Don't set the direction if no direction
        if (currentVelocity.sqrMagnitude > 0.0f)
        {
            transform.up = Vector3.Normalize(new Vector3(currentVelocity.x, currentVelocity.y, 0.0f));
        }
    }
}
