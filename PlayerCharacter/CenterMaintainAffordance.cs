using UnityEngine;

namespace Jobworld
{

	public class CenterMaintainAffordance : MonoBehaviour
	{
		[Header("Affordance Settings")]
		[SerializeField]
		private Transform m_centerTransform;
		
		[SerializeField]
		private Transform m_headTransform;

		[SerializeField]
		private float m_guideStartDistance = 0.6f;

		[SerializeField]
		private float m_guideStopDistance = 0.5f;
		
		[Header("Visuals")]
		[SerializeField]
		private GameObject m_arrowPrefab;

		[SerializeField]
		private GameObject m_rangeVisualPrefab;

		[SerializeField]
		private GameObject m_blindPrefab;
		
		private GameObject m_arrowInstance;

		private GameObject m_rangeVisualInstance;

		private GameObject m_blindInstance;

		private bool m_isGuiding;

		void OnDrawGizmosSelected()
		{
			if (m_centerTransform != null)
			{
				Gizmos.color = Color.white;

				Gizmos.DrawWireSphere(m_centerTransform.position, m_guideStartDistance);
				Gizmos.DrawWireSphere(m_centerTransform.position, m_guideStopDistance);
			}
		}

		void Start()
		{
			m_arrowInstance = Instantiate(m_arrowPrefab, transform);
			m_rangeVisualInstance = Instantiate(m_rangeVisualPrefab, transform);
			m_blindInstance = Instantiate(m_blindPrefab, transform);
		}

		void Update()
		{
			Vector3 floorHeadTransform = m_headTransform.position;
			floorHeadTransform.y = m_centerTransform.position.y;
			float distanceToCenter = Vector3.Distance(m_centerTransform.position, floorHeadTransform);
			
			if (m_isGuiding)
			{
				// ArrowInstance를 headTransform과 같은 위치에 고정하되, y축만 centerTransform과 같게 함
				m_arrowInstance.transform.position =
					new Vector3(m_headTransform.position.x, m_centerTransform.position.y, m_headTransform.position.z);
				m_arrowInstance.transform.forward = m_centerTransform.position - m_arrowInstance.transform.position;

				m_blindInstance.transform.position = m_headTransform.position + m_headTransform.forward * 0.5f;
				m_blindInstance.transform.forward = m_headTransform.forward;

				if (distanceToCenter < m_guideStartDistance)
				{
					StopGuide();
				}
			}
			else
			{
				if (distanceToCenter > m_guideStartDistance)
				{
					StartGuide();
				}
			}

			if (m_isGuiding != m_arrowInstance.activeSelf)
				m_arrowInstance.SetActive(m_isGuiding);
			
			if (m_isGuiding != m_rangeVisualInstance.activeSelf)
				m_rangeVisualInstance.SetActive(m_isGuiding);
			
			if (m_isGuiding != m_blindInstance.activeSelf)
				m_blindInstance.SetActive(m_isGuiding);
		}
		
		private void StartGuide()
		{
			m_isGuiding = true;
		}

		private void StopGuide()
		{
			m_isGuiding = false;
		}
	}

}