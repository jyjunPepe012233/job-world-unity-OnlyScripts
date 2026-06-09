using Oculus.Interaction;
using UnityEngine;

namespace Jobworld
{

	public class JobSelectionPick : RayInteractableEventProvider
	{
		[SerializeField] private JobSO m_job;

		public JobSO job => m_job;
	}

}