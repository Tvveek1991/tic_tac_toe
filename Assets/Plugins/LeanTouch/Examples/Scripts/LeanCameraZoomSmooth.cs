using System;
using UnityEngine;

namespace Lean.Touch
{
	// This modifies LeanCameraZoom to be smooth
	public class LeanCameraZoomSmooth : LeanCameraZoom
	{
		[Tooltip("How quickly the zoom reaches the target value")]
		public float Dampening = 10.0f;

		private float currentZoom;

		public bool SaveOriginalZoom = true;
		
		private bool _isBeezy;
		private bool _isInteractionStarted;

		public bool IsBeezy
		{
			get => _isBeezy;
			set
			{
				_isBeezy = value;
				if (_isBeezy)
				{
					if(_isInteractionStarted)
						return;
					
					OnZoomStart?.Invoke();
					_isInteractionStarted = true;
				}
				else
					_isInteractionStarted = false;
			}
		}		
		public LeanTouch MainComponent { get; set; }

		public event Action OnZoomStart;
		
		protected virtual void OnEnable()
		{
			if (SaveOriginalZoom) 
				Zoom = Camera.main.orthographicSize;
			
			currentZoom = Zoom;
		}

		protected override void LateUpdate()
		{
			// Make sure the camera exists
			if (LeanTouch.GetCamera(ref Camera, gameObject) == true)
			{
				// Use the LateUpdate code from LeanCameraZoom
				base.LateUpdate();

				// Get t value
				var factor = LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

				// Lerp the current value to the target one
				currentZoom = Mathf.Lerp(currentZoom, Zoom, factor);

				if (MainComponent.enabled) 
					IsBeezy = Math.Abs(currentZoom - Zoom) > 0.4f;

				// Set the new zoom
				SetZoom(currentZoom);
			}
		}
		
		public void ClearBeezyStatus()
		{
			_isInteractionStarted = false;
			_isBeezy = false;
		}
	}
}