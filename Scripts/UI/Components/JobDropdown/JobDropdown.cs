using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Jobworld
{

	public class JobDropdown : MonoBehaviour
	{
		public JobSO[] validJobList;

		[SerializeField]
		private TMP_Dropdown m_dropdown;
		
		[SerializeField]
		private TMP_Dropdown.OptionData m_defaultOption;

		public event Action onValueChanged;
		
		public string selectedJob { get; private set; }

		void Awake()
		{
			m_dropdown.onValueChanged.AddListener(OnValueChanged);
		}
		
		void OnEnable()
		{
			var optionList = validJobList.Select(
				i => new TMP_Dropdown.OptionData(i.jobSelectionPanelInfo.jobName, i.icon)
			).ToList();
			
			optionList.Insert(0, m_defaultOption);

			m_dropdown.options = optionList;
		}

		private void OnValueChanged(int changed)
		{
			selectedJob = changed == 0 ? null : validJobList[changed - 1].jobName;
			onValueChanged?.Invoke();
		}
	}

}