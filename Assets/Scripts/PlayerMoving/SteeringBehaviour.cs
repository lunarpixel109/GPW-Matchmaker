using UnityEngine;

namespace PlayerMoveing {
    public abstract class SteeringBehaviour : MonoBehaviour{
	    
		/// <summary>
		/// How strongly this behaviour's steering force contributes to the total. Multiplied against J
		/// the returned steering velocity in CooperativeArbitration before summing. V
		/// </summary>
		[field: SerializeField]
		public float Weight { get; set; } = 1.0f;

		protected Vector3 desiredVelocity;
		protected Vector3 steeringVelocity;

		/// <summary>
		/// Do steering behaviour code here. At the end of this the desiredVelocity and steeringVelocity variables should be set
		/// </summary>
		/// <param name="steeringAgent">The agent this component is acting on</param>
		/// <returns>The steeringVelocity should always be returned here</returns>
		public abstract Vector3 UpdateBehaviour(SteeringAgent steeringAgent);
		
		[Header("Debug")]
		[SerializeField] protected bool drawDebug = true;

		protected virtual Color DebugColor => Color.white;

		public virtual void DrawDebug(SteeringAgent agent)
		{
			if (!drawDebug) return;
			Debug.DrawRay(transform.position + Vector3.up * 0.1f, desiredVelocity, DebugColor);
		}
    }
}