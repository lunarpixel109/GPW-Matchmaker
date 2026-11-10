using System.Collections.Generic;
using PlayerMoveing;
using UnityEngine;

public class PlayerAvoid : SteeringBehaviour {
    [SerializeField] private float lookAhead = 3f;//how far ahead to check
    [SerializeField] private float agentRadius = 0.5f;//radius of each agent
    [SerializeField] private float padding = 0.2f;//extra personal space

    private static readonly List<PlayerAvoid> All = new List<PlayerAvoid>();

    private void OnEnable()  { All.Add(this); }
    private void OnDisable() { All.Remove(this); }

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        steeringVelocity = Vector3.zero;
        desiredVelocity = Vector3.zero;
        
        Vector3 heading = steeringAgent.currentVelocity;
        if (heading.sqrMagnitude < 0.01f)
            heading = steeringAgent.targetPosition - transform.position;
        heading.y = 0f;
        if (heading.sqrMagnitude < 0.0001f) return Vector3.zero;
        heading.Normalize();

        Vector3 side = Vector3.Cross(Vector3.up, heading);//right of heading
        float combinedRadius = agentRadius * 2f + padding;

        float closestAhead = float.MaxValue;
        float steerSign = 0f;

        foreach (var other in All) {
            if (other == this) continue;

            Vector3 rel = other.transform.position - transform.position;
            rel.y = 0f;

            float ahead = Vector3.Dot(rel, heading);//distance along heading
            if (ahead < 0f || ahead > lookAhead) continue;//behind us or too far

            float lateral = Vector3.Dot(rel, side);//+ = other is to our right
            if (Mathf.Abs(lateral) > combinedRadius) continue;

            if (ahead < closestAhead) {
                closestAhead = ahead;
                //Steer to the side the obstacle ISN'T on.
                //if dead centre then both go around, the great old side to side :)
                steerSign = lateral > 0.05f ? -1f : 1f;
            }
        }

        if (steerSign == 0f) return Vector3.zero;

        // Closer obstacle = stronger push
        float urgency = 1f - Mathf.Clamp01(closestAhead / lookAhead);

        desiredVelocity = side * steerSign * steeringAgent.maxCurrentSpeed;
        steeringVelocity = desiredVelocity * urgency;
        return steeringVelocity;
    }
}