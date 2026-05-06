using UnityEngine;
using UnityEngine.UI;

namespace Lean.Touch
{
	public class LeanDropDestroy : MonoBehaviour, IDroppable
	{
		[Tooltip("The text element we will modify")]
		public Text NumberText;

		[Tooltip("The amount of times this component has destroyed dropped GameObjects")]
		public int DestroyCount;
		
		public void OnDrop(GameObject droppedGameObject, LeanFinger finger)
		{
			Destroy(droppedGameObject);

			DestroyCount += 1;

			NumberText.text = "Destroyed " + DestroyCount + "!";
		}
	}
}