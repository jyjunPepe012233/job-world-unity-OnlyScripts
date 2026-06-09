using System;
using TMPro;
using UnityEngine;

namespace Jobworld
{

	public class PlayTimeView : MonoBehaviour
	{
		private IPlayTimerModel m_playTimerModel;

		[SerializeField]
		private TextMeshProUGUI m_view;

		[SerializeField]
		private string m_viewFormat = "{0}:{1}";

		[SerializeField]
		private string m_holderWhenStopped = "";

		void Awake()
		{
			m_playTimerModel = new PlayTimerModel();
		}

		void Update()
		{
			var remainTimeNullable = m_playTimerModel.time - m_playTimerModel.elapsedTime;

			if (remainTimeNullable.HasValue)
			{
				var remainTime = remainTimeNullable.Value;
				var m = (int)(remainTime / 60);
				var s = (int)(remainTime % 60);

				m_view.text = String.Format(m_viewFormat, m.ToString("D2"), s.ToString("D2"));
			}
			else
			{
				m_view.text = m_holderWhenStopped;
			}
		}
	}

}