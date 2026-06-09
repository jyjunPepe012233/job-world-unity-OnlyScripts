using UnityEngine;

namespace Jobworld
{

	public static class CanvasGroupExtension
	{
		public static void SetVisible(this CanvasGroup canvasGroup, bool active)
		{
			canvasGroup.alpha = active ? 1 : 0;
			canvasGroup.interactable = active;
			canvasGroup.blocksRaycasts = active;
		}
	}

}