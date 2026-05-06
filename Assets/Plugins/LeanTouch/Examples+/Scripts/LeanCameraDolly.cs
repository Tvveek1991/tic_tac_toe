using UnityEngine;

namespace Lean.Touch
{
	// This script will keep this GameObject a certain distance away from the center with pinch or mouse wheel
	[ExecuteInEditMode]
	public class LeanCameraDolly : MonoBehaviour
	{
		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("Allows you to force input with a specific amount of fingers (0 = any)")]
		public int RequiredFingerCount;

		[Tooltip("The direction of the dolly")]
		public Vector3 Direction = -Vector3.forward;

		[Tooltip("The current orbit distance")]
		public float Distance = 10.0f;

		[Tooltip("Should the distance value get clamped?")]
		public bool DistanceClamp;

		[Tooltip("The minimum distance")]
		public float DistanceMin = 1.0f;

		[Tooltip("The maximum distance")]
		public float DistanceMax = 100.0f;

		[Tooltip("If you want the mouse wheel to simulate pinching then set the strength of it here")]
		[Range(-1.0f, 1.0f)]
		public float WheelSensitivity;

		protected virtual void LateUpdate()
		{
			// Get the fingers we want to use to rotate
			var fingers = LeanTouch.GetFingers(IgnoreGuiFingers, RequiredFingerCount);

			// Change the zoom based on pinch of all fingers
			Distance *= LeanGesture.GetPinchRatio(fingers, WheelSensitivity);

			// Limit distance to min/max values?
			if (DistanceClamp == true)
			{
				Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
			}

			// Reset position and dolly to distance
			transform.localPosition = Vector3.zero;

			transform.Translate(Direction.normalized * Distance);
		}
	}
}