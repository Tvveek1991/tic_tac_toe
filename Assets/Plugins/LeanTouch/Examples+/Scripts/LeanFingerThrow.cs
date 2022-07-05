#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
	#define UNITY_OLD_LINE_RENDERER
#endif
using UnityEngine;
using System.Collections.Generic;

namespace Lean.Touch
{
	// This script will draw a line between the start and current finger points and change the thickness based on the distance
	public class LeanFingerThrow : MonoBehaviour
	{
		// This class will store an association between a Finger and a LineRenderer instance
		[System.Serializable]
		public class Link
		{
			// The finger associated with this link
			public LeanFinger Finger;

			// The LineRenderer instance associated with this link
			public LineRenderer Line;
		}

		[Tooltip("Ignore fingers with StartedOverGui?")]
		public bool IgnoreGuiFingers = true;

		[Tooltip("The line prefab")]
		public LineRenderer LinePrefab;

		[Tooltip("The distance from the camera the line points will be spawned in world space")]
		public float Distance = 1.0f;

		[Tooltip("The thickness scale per unit")]
		public float ThicknessScale = 0.1f;

		[Tooltip("Limit the length (0 = none)")]
		public float LengthLimit;

		[Tooltip("The maximum amount of fingers used")]
		public int MaxLines;

		[Tooltip("Invert the throw indicator direction?")]
		public bool Invert;

		[Tooltip("The prefab you want to throw")]
		public GameObject ThrowPrefab;

		[Tooltip("The strength of the throw")]
		public float ThrowForce = 1.0f;

		[Tooltip("The camera the translation will be calculated using (default = MainCamera)")]
		public Camera Camera;

		private List<Link> links = new List<Link>();

		protected virtual void OnEnable()
		{
			// Hook events
			LeanTouch.OnFingerDown += FingerDown;
			LeanTouch.OnFingerSet  += FingerSet;
			LeanTouch.OnFingerUp   += FingerUp;
		}

		protected virtual void OnDisable()
		{
			// Unhook events
			LeanTouch.OnFingerDown -= FingerDown;
			LeanTouch.OnFingerSet  -= FingerSet;
			LeanTouch.OnFingerUp   -= FingerUp;
		}

		// Override the WritePositions method from LeanDragLine
		protected virtual void WritePositions(LineRenderer line, LeanFinger finger)
		{
			// Get start and current world position of finger
			var start    = finger.GetStartWorldPosition(Distance);
			var end      = finger.GetWorldPosition(Distance);
			var distance = Vector3.Distance(start, end);

			// Limit the length?
			if (LengthLimit > 0.0f && distance > LengthLimit)
			{
				var direction = Vector3.Normalize(end - start);

				distance = LengthLimit;
				end      = start + direction * distance;
			}

			if (Invert == true)
			{
				end = start + (start - end);
			}

			// Write positions
			var thickness = distance * ThicknessScale;

#if UNITY_OLD_LINE_RENDERER
			line.SetVertexCount(2);

			line.SetWidth(thickness, thickness);
#else
			line.positionCount = 2;

			line.startWidth = thickness;
			line.endWidth   = thickness;
#endif
			line.SetPosition(0, start);
			line.SetPosition(1, end);
		}

		private void FingerDown(LeanFinger finger)
		{
			if (MaxLines > 0 && links.Count >= MaxLines)
			{
				return;
			}

			// Make new link
			var link = new Link();

			// Assign this finger to this link
			link.Finger = finger;

			// Create LineRenderer instance for this link
			link.Line = Instantiate(LinePrefab);

			// Add new link to list
			links.Add(link);
		}

		private void FingerSet(LeanFinger finger)
		{
			// Try and find the link for this finger
			var link = FindLink(finger);

			// Link exists?
			if (link != null && link.Line != null)
			{
				WritePositions(link.Line, link.Finger);
			}
		}

		private void FingerUp(LeanFinger finger)
		{
			// Try and find the link for this finger
			var link = FindLink(finger);

			// Link exists?
			if (link != null)
			{
				// Remove link from list
				links.Remove(link);

				// Destroy line GameObject
				if (link.Line != null)
				{
					Destroy(link.Line.gameObject);
				}

				Throw(finger);
			}
		}

		private Link FindLink(LeanFinger finger)
		{
			for (var i = 0; i < links.Count; i++)
			{
				var link = links[i];

				if (link.Finger == finger)
				{
					return link;
				}
			}

			return null;
		}

		private void Throw(LeanFinger finger)
		{
			// Start and end points of the drag
			var start = finger.GetStartWorldPosition(Distance, Camera);
			var end   = finger.GetWorldPosition(Distance, Camera);

			if (start != end)
			{
				// Vector between points
				var direction = end - start;

				// Angle between points
				var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

				// Instance the prefab, position it at the start point, and rotate it to the vector
				var instance = Instantiate(ThrowPrefab);

				instance.transform.position = start;
				instance.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -angle);

				// Apply 3D force?
				var rigidbody3D = instance.GetComponent<Rigidbody>();

				if (rigidbody3D != null)
				{
					rigidbody3D.velocity = direction * ThrowForce;
				}

				// Apply 2D force?
				var rigidbody2D = instance.GetComponent<Rigidbody2D>();

				if (rigidbody2D != null)
				{
					rigidbody2D.velocity = direction * ThrowForce;
				}
			}
		}
	}
}