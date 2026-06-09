using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Lobby
{

	public class LobbyMenuButtonController : MonoBehaviour
	{
		public GameObject controllerButton;

		[Range(15, 90)]
		public float controllerButtonEnableAngle = 60;

		public GameObject handButton;

		[Range(15, 90)]
		public float handButtonEnableAngle = 60;

		private OVRInput.Controller m_activeController;

		private void Update()
		{
			m_activeController = OVRInput.GetActiveController();
			
			SetActiveButton(controllerButton, controllerButtonEnableAngle, OVRInput.Controller.Touch);
			
			SetActiveButton(handButton, handButtonEnableAngle, OVRInput.Controller.Hands);
		}

		private void SetActiveButton(GameObject button, float enableAngle, OVRInput.Controller controller)
		{
			float angle = Vector3.Angle(button.transform.forward, Camera.main.transform.forward);
			
			button.SetActive(angle < enableAngle && m_activeController == controller);
		}
	}

}