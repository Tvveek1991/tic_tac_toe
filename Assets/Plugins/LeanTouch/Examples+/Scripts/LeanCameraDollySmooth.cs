using UnityEngine;

namespace Lean.Touch
{
	// This modifies LeanDolly to be smooth
	[ExecuteInEditMode]
	public class LeanCameraDollySmooth : LeanCameraDolly
	{
		[Tooltip("How sharp the distance value change updates")]
		public float Dampening = 3.0f;

		private float currentDistance;

		protected virtual void OnEnable()
		{
			currentDistance = Distance;
		}

		protected override void LateUpdate()
		{
			// Use the LateUpdate code from LeanOrbitCamera
			base.LateUpdate();

			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Lerp the current value to the target one
			currentDistance = Mathf.Lerp(currentDistance, Distance, factor);

			// Reset position and dolly to distance
			transform.localPosition = Vector3.zero;

			transform.Translate(Direction.normalized * currentDistance);
		}
	}
}