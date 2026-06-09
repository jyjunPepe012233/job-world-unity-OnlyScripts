using UnityEngine;
using UnityEngine.UI;

namespace Jobworld
{

	public class SliderLoadingProgressVisual : MonoBehaviour, ILoadingProgressVisual
	{
		public Slider slider;
		
		public void SetProgress(float progress)
		{
			slider.value = progress;
		}
	}

}