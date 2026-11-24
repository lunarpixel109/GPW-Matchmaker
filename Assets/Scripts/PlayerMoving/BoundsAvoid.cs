using PlayerMoveing;
using UnityEngine;

/// <summary>
/// Casts a fan of rays ("feelers") ahead of the agent. Any ray that hits a wall
/// pushes the agent away along the wall's surface normal, harder the closer the wall is.
/// Walls need a Collider and must be on a layer included in wallMask.
/// </summary>
public class BoundsAvoid : SteeringBehaviour
{
    [Header("Feelers")]
    [SerializeField] private LayerMask wallMask = ~0;          // which layers count as walls
    [SerializeField] private float feelerLength = 2.5f;        // how far ahead to look
    [SerializeField] private float fanAngle = 90f;             // total spread of the fan in degrees
    [SerializeField, Range(1, 15)] private int feelerCount = 5;// odd numbers keep one ray dead ahead
    [SerializeField] private float rayHeight = 0.0f;           // raise/lower rays relative to the agent pivot

    [Header("Response")]
    [SerializeField] private float minDistance = 0.6f;         // inside this the push is at full strength
    [SerializeField] private float pushStrength = 1.5f;        // multiplier on maxCurrentSpeed

    [Header("Debug")]
    [SerializeField] private bool drawFeelers = true;

    // stored so the debug draw shows what the last update actually saw
    private Vector3[] _feelerDirs;
    private float[] _feelerHitDist;
    
    protected override Color DebugColor { get { return Color.blue; } }

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        desiredVelocity = Vector3.zero;
        steeringVelocity = Vector3.zero;

        // Heading: where we're going, or where we're facing if we're nearly stopped
        Vector3 heading = steeringAgent.currentVelocity;
        heading.y = 0f;
        if (heading.sqrMagnitude < 0.01f)
        {
            heading = transform.forward;
            heading.y = 0f;
        }
        if (heading.sqrMagnitude < 0.0001f) return Vector3.zero;
        heading.Normalize();

        EnsureBuffers();

        Vector3 origin = transform.position + Vector3.up * rayHeight;
        Vector3 push = Vector3.zero;
        float strongestUrgency = 0f;

        for (int i = 0; i < feelerCount; i++)
        {
            // Spread rays from -fan/2 to +fan/2 (single feeler = straight ahead)
            float t = feelerCount == 1 ? 0.5f : (float)i / (feelerCount - 1);
            float angle = Mathf.Lerp(-fanAngle * 0.5f, fanAngle * 0.5f, t);
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * heading;

            _feelerDirs[i] = dir;
            _feelerHitDist[i] = -1f;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, feelerLength, wallMask,
                    QueryTriggerInteraction.Ignore))
            {
                _feelerHitDist[i] = hit.distance;

                // 0 at the end of the feeler, 1 at minDistance or closer
                float urgency = 1f - Mathf.Clamp01(
                    (hit.distance - minDistance) / Mathf.Max(0.0001f, feelerLength - minDistance));

                Vector3 normal = hit.normal;
                normal.y = 0f;
                if (normal.sqrMagnitude < 0.0001f) continue;
                normal.Normalize();

                // Away from the wall, scaled by how close it is
                push += normal * urgency;
                strongestUrgency = Mathf.Max(strongestUrgency, urgency);
            }
        }

        if (push.sqrMagnitude < 0.0001f) return Vector3.zero;

        desiredVelocity = push.normalized * steeringAgent.maxCurrentSpeed * pushStrength;
        steeringVelocity = desiredVelocity * strongestUrgency;
        return steeringVelocity;
    }

    private void EnsureBuffers()
    {
        if (_feelerDirs == null || _feelerDirs.Length != feelerCount)
        {
            _feelerDirs = new Vector3[feelerCount];
            _feelerHitDist = new float[feelerCount];
            for (int i = 0; i < feelerCount; i++) _feelerHitDist[i] = -1f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawFeelers || _feelerDirs == null) return;

        Vector3 origin = transform.position + Vector3.up * rayHeight;
        for (int i = 0; i < _feelerDirs.Length; i++)
        {
            bool hit = _feelerHitDist[i] >= 0f;
            Gizmos.color = hit ? Color.red : Color.green;
            float len = hit ? _feelerHitDist[i] : feelerLength;
            Gizmos.DrawLine(origin, origin + _feelerDirs[i] * len);
        }
    }
}