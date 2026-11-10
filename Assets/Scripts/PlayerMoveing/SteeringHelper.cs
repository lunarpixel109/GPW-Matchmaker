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

    static public Vector3 GetTargetPosition(GameObject floorSpace) {
        Vector3 fixedDebugTargetOffset = new Vector3(4, floorSpace.transform.position.y, 4);
        
        float randX = Random.Range(-4f, 4f);
        float randZ = Random.Range(-4f, 4f);
        
        Vector3 offset = new Vector3(randX, 1.0f, randZ);
        
        Vector3 targetPosition = floorSpace.transform.position + offset;
        
        //Debug.Log(targetPosition);
        return targetPosition;
    }

    static public bool IsAtTarget(Vector3 currentPos, Vector3 target) {
        var allowance = 0.5;
        
        var distance = Vector3.Distance(currentPos,  target);
        if (distance > allowance) return false;
        return true;
    }
}
