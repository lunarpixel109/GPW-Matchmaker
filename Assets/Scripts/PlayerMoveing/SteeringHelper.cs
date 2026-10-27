using UnityEngine;

public class SteeringHelper : MonoBehaviour {
    static public Vector3 LimitVector(Vector3 vector, float magnitude) {
        if (vector.sqrMagnitude > magnitude * magnitude)
        {
            vector.Normalize();
            vector *= magnitude;
        }
        return vector;
    }
}
