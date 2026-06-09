using UnityEngine;

namespace Play.Lobby
{

	public class LobbyObserverCameraController : MonoBehaviour
	{
		[SerializeField]
		private Camera m_camera;

		[SerializeField]
		private Transform m_pivot;

		[SerializeField]
		private Vector3 m_positionOffset;

		[SerializeField]
		private Vector3 m_rotationOffset;

		[SerializeField]
		private float m_rotateSpeed = 60;

		void Start()
		{
			#if !UNITY_STANDALONE
			this.enabled = false;
			#endif
		}

		public void TurnLeft()
		{
			m_pivot.eulerAngles = new Vector3(
				m_pivot.eulerAngles.x,
				m_pivot.eulerAngles.y - m_rotateSpeed * Time.deltaTime,
				m_pivot.eulerAngles.z);
		}

		public void TurnRight()
		{
			m_pivot.eulerAngles = new Vector3(
				m_pivot.eulerAngles.x,
				m_pivot.eulerAngles.y + m_rotateSpeed * Time.deltaTime,
				m_pivot.eulerAngles.z);
		}

		void LateUpdate()
		{
			m_camera.transform.position = m_pivot.TransformVector(m_positionOffset);
			m_camera.transform.rotation = m_pivot.rotation * Quaternion.Euler(m_rotationOffset);
		}

		void OnValidate()
		{
			LateUpdate();
		}
	}

}