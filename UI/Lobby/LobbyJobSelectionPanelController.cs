using UnityEngine;

namespace Jobworld
{

	public class LobbyJobSelectionPanelController : MonoBehaviour
	{
		[SerializeField] private Transform m_pivot;

		[SerializeField] private Transform m_panelTransform;

		[SerializeField] private Vector3 offset;

		public void LateUpdate()
		{
			Vector3 dirPivotToCam = Camera.main.transform.position - m_pivot.position; 
			dirPivotToCam.y = 0;
			dirPivotToCam.Normalize();

			m_panelTransform.position =
				m_pivot.position +
				Quaternion.LookRotation(dirPivotToCam) * offset;

			m_panelTransform.forward = -dirPivotToCam;
		}
	}

}