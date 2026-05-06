using UnityEngine;

namespace Lean.Touch
{
	// This script allows you to change the color of the SpriteRenderer attached to the current GameObject
	public class LeanSelectableDrop : LeanSelectableBehaviour
	{
		public enum SelectType
		{
			Raycast3D,
			Overlap2D
		}

		public enum SearchType
		{
			GetComponent,
			GetComponentInParent,
			GetComponentInChildren
		}

		public SelectType SelectUsing;

		[Tooltip("This stores the layers we want the raycast/overlap to hit (make sure this GameObject's layer is included!)")]
		public LayerMask LayerMask = Physics.DefaultRaycastLayers;

		[Tooltip("How should the selected GameObject be searched for the LeanSelectable component?")]
		public SearchType Search;

		protected override void OnSelectUp(LeanFinger finger)
		{
			// Stores the component we hit (Collider or Collider2D)
			var component = default(Component);

			switch (SelectUsing)
			{
				case SelectType.Raycast3D:
				{
					// Get ray for finger
					var ray = finger.GetRay();

					// Stores the raycast hit info
					var hit = default(RaycastHit);

					// Was this finger pressed down on a collider?
					if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
					{
						component = hit.collider;
					}
				}
				break;

				case SelectType.Overlap2D:
				{
					// Find the position under the current finger
					var point = finger.GetWorldPosition(1.0f);

					// Find the collider at this position
					component = Physics2D.OverlapPoint(point, LayerMask);
				}
				break;
			}

			// Select the component
			Drop(finger, component);
		}

		private void Drop(LeanFinger finger, Component component)
		{
			// Stores the droppable we will search for
			var droppable = default(IDroppable);

			// Was a collider found?
			if (component != null)
			{
				switch (Search)
				{
					case SearchType.GetComponent:           droppable = component.GetComponent          <IDroppable>(); break;
					case SearchType.GetComponentInParent:   droppable = component.GetComponentInParent  <IDroppable>(); break;
					case SearchType.GetComponentInChildren: droppable = component.GetComponentInChildren<IDroppable>(); break;
				}
			}

			// Drop?
			if (droppable != null)
			{
				droppable.OnDrop(gameObject, finger);
			}
		}
	}
}