using System;
using Lean.Touch;
using UnityEngine;

namespace LeanTouch.Examples.Scripts
{
	// This modifies LeanPitchYaw to be smooth
	public class LeanPitchYawSmooth : LeanPitchYaw
	{
		[Tooltip("How sharp the rotation value changes update")]
		[Space(10.0f)]
		public float Dampening = 3.0f;

		public bool SaveOriginalRotation = true;
		
		private float currentPitch;
		private float currentYaw;

		private bool _isBeezy;
		private bool _isRotationStarted;
		
		public bool IsBeezy
		{
			get => _isBeezy;
			set
			{
				_isBeezy = value;
				if (_isBeezy)
				{
					if(_isRotationStarted)
						return;
					
					OnRotateStart?.Invoke();
					_isRotationStarted = true;
				}
				else
					_isRotationStarted = false;
			}
		}

		public Lean.Touch.LeanTouch MainComponent { get; set; }

		public Action OnRotateStart;

		protected virtual void OnEnable()
		{
			if (SaveOriginalRotation)
			{		
				Vector3 localRotationEulerAngles = transform.localRotation.eulerAngles;
				Pitch = localRotationEulerAngles.x;
				Yaw = localRotationEulerAngles.y;
			}

			currentPitch = Pitch;
			currentYaw   = Yaw;
		}

		protected override void LateUpdate()
		{
			// Call LeanPitchYaw.LateUpdate
			base.LateUpdate();

			// Get t value
			var factor = Lean.Touch.LeanTouch.GetDampenFactor(Dampening, Time.deltaTime);

			// Lerp the current values to the target ones
			currentPitch = Mathf.Lerp(currentPitch, Pitch, factor);
			currentYaw   = Mathf.Lerp(currentYaw  , Yaw  , factor);

			if(MainComponent.enabled)
				IsBeezy = Math.Abs(currentPitch - Pitch) > 0.4f || Math.Abs(currentYaw - Yaw) > 0.4f;
			
			// Rotate camera to pitch and yaw values
			transform.localRotation = Quaternion.Euler(currentPitch, currentYaw, 0.0f);
		}

		public void ClearBeezyStatus()
		{
			_isRotationStarted = false;
			_isBeezy = false;
		}
	}
}