using UnityEngine;

/// <summary>
/// A tweakable "feel" profile. Create assets via Assets > Create > Drag > Drag Feel
/// (Light, Normal, Heavy...) and assign one to an ItemData. Replaces HeavyDraggable's mass/baseSpeed.
/// </summary>
[CreateAssetMenu(menuName = "Drag/Drag Feel", fileName = "NewDragFeel")]
public class DragFeel : ScriptableObject
{
    [Header("Follow (spring)")]
    [Tooltip("How quickly the item chases the pointer. Low = heavy and laggy, high = snappy.")]
    [Min(0.1f)] public float frequency = 8f;

    [Tooltip("<1 overshoots and wobbles, 1 = no overshoot, >1 = sluggish and syrupy.")]
    [Range(0.1f, 2f)] public float damping = 0.85f;

    [Tooltip("2D ghost speed cap in px/s. 0 = unlimited.")]
    [Min(0f)] public float maxSpeed = 0f;

    [Tooltip("3D proxy speed cap in world units/s. 0 = unlimited.")]
    [Min(0f)] public float maxSpeed3D = 0f;

    [Header("Pickup (2D ghost)")]
    public float pickupScale = 1.1f;
    public float scaleSpeed = 12f;

    [Header("Tilt (2D ghost)")]
    [Tooltip("Degrees of tilt per px/s of horizontal velocity. 0 = no tilt.")]
    public float tiltPerSpeed = 0.02f;
    public float maxTilt = 15f;
    public float tiltSmoothing = 12f;
}

/// <summary>
/// Spring-damper that chases a target. Used for both the 2D ghost (screen px) and the 3D proxy (world units).
/// </summary>
public class DragMotion
{
    public Vector3 Position;
    public Vector3 Velocity;

    public void Reset(Vector3 position)
    {
        Position = position;
        Velocity = Vector3.zero;
    }

    public void Step(Vector3 target, DragFeel feel, float dt, bool world = false)
    {
        float omega = 2f * Mathf.PI * feel.frequency;
        float maxSpeed = world ? feel.maxSpeed3D : feel.maxSpeed;

        // Substep so stiff springs stay stable at low frame rates.
        const float maxStep = 1f / 240f;
        int steps = Mathf.Max(1, Mathf.CeilToInt(dt / maxStep));
        float h = dt / steps;

        for (int i = 0; i < steps; i++)
        {
            Vector3 accel = omega * omega * (target - Position) - 2f * feel.damping * omega * Velocity;
            Velocity += accel * h;

            if (maxSpeed > 0f)
                Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

            Position += Velocity * h;
        }
    }
}
