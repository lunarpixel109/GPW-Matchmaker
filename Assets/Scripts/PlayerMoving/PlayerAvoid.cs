using System.Collections.Generic;
using PlayerMoveing;
using UnityEngine;

public class PlayerAvoid : SteeringBehaviour {
    [Header("Look ahead")]
    [SerializeField] private float lookAhead = 3f;      // how far ahead to check
    [SerializeField] private float agentRadius = 0.5f;  // radius of each agent
    [SerializeField] private float padding = 0.2f;      // extra personal space

    [Header("Bubble")]
    [SerializeField] private float bubbleRadius = 1.0f;   // personal space, centre to centre
    [SerializeField] private float bubbleStrength = 3f;   // max push speed when fully overlapping
    [SerializeField, Range(0f, 1f)] private float idleYield = 0.3f; // how much a settled agent gives way

    private static readonly List<PlayerAvoid> All = new List<PlayerAvoid>();

    private SteeringAgent _self;
    private float _committedSign = 0f; // side we've chosen to pass on, held until the obstacle is gone

    protected override Color DebugColor => Color.magenta;

    private void OnEnable()
    {
        All.Add(this);
        _self = GetComponent<SteeringAgent>();
        _committedSign = 0f;
    }

    private void OnDisable() { All.Remove(this); }

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        Vector3 bubble = GetBubblePush();
        Vector3 total = LookAheadSteer(steeringAgent) + bubble;

        desiredVelocity += bubble;
        steeringVelocity = total;
        return total;
    }

    /// <summary>
    /// Steers sideways around agents in front of us. Commits to one side so it doesn't flip every frame.
    /// </summary>
    private Vector3 LookAheadSteer(SteeringAgent steeringAgent)
    {
        steeringVelocity = Vector3.zero;
        desiredVelocity = Vector3.zero;

        Vector3 heading = steeringAgent.currentVelocity;
        if (heading.sqrMagnitude < 0.01f)
            heading = steeringAgent.targetPosition - transform.position;
        heading.y = 0f;
        if (heading.sqrMagnitude < 0.0001f) { _committedSign = 0f; return Vector3.zero; }
        heading.Normalize();

        Vector3 side = Vector3.Cross(Vector3.up, heading); // right of heading
        float combinedRadius = agentRadius * 2f + padding;

        float closestAhead = float.MaxValue;
        float closestLateral = 0f;
        bool found = false;

        foreach (var other in All) {
            if (other == this) continue;

            Vector3 rel = other.transform.position - transform.position;
            rel.y = 0f;

            float ahead = Vector3.Dot(rel, heading);       // distance along heading
            if (ahead < 0f || ahead > lookAhead) continue; // behind us or too far

            float lateral = Vector3.Dot(rel, side);        // + = other is to our right
            if (Mathf.Abs(lateral) > combinedRadius) continue;

            if (ahead < closestAhead) {
                closestAhead = ahead;
                closestLateral = lateral;
                found = true;
            }
        }

        if (!found) { _committedSign = 0f; return Vector3.zero; }

        // Pick a side once, then hold it until the way ahead is clear
        if (_committedSign == 0f) {
            if (Mathf.Abs(closestLateral) > 0.05f)
                _committedSign = closestLateral > 0f ? -1f : 1f;          // go to the side it ISN'T on
            else
                _committedSign = (GetInstanceID() % 2 == 0) ? 1f : -1f;   // dead centre: differs per agent
        }

        // Closer obstacle = stronger push
        float urgency = 1f - Mathf.Clamp01(closestAhead / lookAhead);

        desiredVelocity = side * _committedSign * steeringAgent.maxCurrentSpeed;
        steeringVelocity = desiredVelocity * urgency;
        return steeringVelocity;
    }

    /// <summary>
    /// Soft personal-space bubble. Public so SteeringAgent can also apply it while idle.
    /// Settled agents ignore each other and only give way a little to moving ones.
    /// </summary>
    public Vector3 GetBubblePush()
    {
        bool meIdle = _self != null && _self.IsIdle;
        Vector3 push = Vector3.zero;

        foreach (var other in All) {
            if (other == this) continue;

            bool otherIdle = other._self != null && other._self.IsIdle;
            if (meIdle && otherIdle) continue; // settled agents don't shove each other

            Vector3 d = transform.position - other.transform.position;
            d.y = 0f;
            float dist = d.magnitude;
            if (dist >= bubbleRadius) continue;

            if (dist < 0.0001f) { // exactly stacked, pick a random direction
                Vector2 r = Random.insideUnitCircle.normalized;
                d = new Vector3(r.x, 0f, r.y);
                dist = 0f;
            }

            float t = 1f - dist / bubbleRadius;
            t *= t; // soft edge, strong only when really overlapping
            float weight = meIdle ? idleYield : 1f;

            push += d.normalized * t * weight;
        }

        return push * bubbleStrength;
    }
}