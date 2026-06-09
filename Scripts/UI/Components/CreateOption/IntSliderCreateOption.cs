using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Jobworld
{

	public class IntSliderCreateOption : ACreateOption<int>
	{
		[SerializeField]
		private Slider m_slider;

		[SerializeField]
		private TextMeshProUGUI m_valueView;

		void Start()
		{
			m_slider.onValueChanged.AddListener(UpdateValueView);
			UpdateValueView(m_slider.value);
		}

		private void UpdateValueView(float arg)
		{
			m_valueView.text = GetValue().ToString();
		}
		
		public override int GetValue()
		{
			return (int)m_slider.value;
		}
	}

}