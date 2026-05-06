using UnityEngine;

namespace Lean.Touch
{
	// This modifies LeanCameraMove to be smooth
	public class LeanCameraMoveSmooth : LeanCameraMove
	{
		[Tooltip("How quickly the zoom reaches the target value")]
		public float Dampening = 10.0f;

		public Vector3 remainingDelta;

		public bool IsBeezy { get; set; }

		public LeanTouch MainComponent { get; set; }

		protected override void LateUpdate()
		{
			// Store the current position
			var oldPosition = transform.localPosition;

			// Call LeanCameraMove.LateUpdate
			base.LateUpdate();

			// Add to remainingDelta
			remainingDelta += transform.localPosition - oldPosition;

			if (MainComponent.enabled)
				IsBeezy = remainingDelta.magnitude > 3.0f;

			// Get t value
			var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Dampen remainingDelta
			var newDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

			// Shift this position by the change in delta
			transform.localPosition = oldPosition + remainingDelta - newDelta;

			// Update remainingDelta with the dampened value
			remainingDelta = newDelta;
		}
	}
}