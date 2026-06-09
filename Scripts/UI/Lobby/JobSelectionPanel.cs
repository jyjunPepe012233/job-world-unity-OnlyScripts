using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jobworld
{

	public class JobSelectionPanel : MonoBehaviour
	{
		public event Action onSelected;

		public event Action onClosed;

		public event Action onUnselected;

		[SerializeField] private TextMeshProUGUI m_jobName;

		[SerializeField] private TextMeshProUGUI m_description;

		[SerializeField] private Image m_banner;
		
		[SerializeField] private GameObject m_selectButton;

		[SerializeField] private GameObject m_unselectButton;

		/// <summary>
		/// Call by UI Button
		/// </summary>
		public void Select()
		{
			onSelected?.Invoke();
		}

		/// <summary>
		/// Call by UI Button
		/// </summary>
		public void Close()
		{
			onClosed?.Invoke();
		}

		/// <summary>
		/// Call by UI Button
		/// </summary>
		public void Unselect()
		{
			onUnselected?.Invoke();
		}

		public void UpdateInfo(JobSO job)
		{
			JobSelectionPanelInfo info = job.jobSelectionPanelInfo;
			
			m_jobName.text = info.jobName;
			m_description.text = info.description;
			m_banner.sprite = info.banner;
		}

		public void UpdateState(bool isThisJobSelected)
		{
			m_selectButton.SetActive(!isThisJobSelected);
			m_unselectButton.SetActive(isThisJobSelected);
		}
	}

}