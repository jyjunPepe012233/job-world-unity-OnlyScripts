using UnityEngine;
using UnityEngine.InputSystem;

namespace JobworldDev
{

	public class DebugUIController : MonoBehaviour
	{
		public InputActionProperty openInputs;
		
		public int pressTimesToOpen = 3;

		private int m_pressCount;
		private float m_lastPressTime;

		public GameObject m_viewGameObject;

		void Start()
		{
			openInputs.action.Enable();
			openInputs.action.started += OnPressOpenInput;
			CloseUI();
		}

		void OnPressOpenInput(InputAction.CallbackContext context)
		{
			if (m_viewGameObject.activeSelf)
			{
				CloseUI();
				m_pressCount = 0;
				return;
			}
			
			if (Time.time - m_lastPressTime < 0.2f)
			{
				m_pressCount += 1;
			}
			else
			{
				m_pressCount = 1;
			}
			
			if (m_pressCount == pressTimesToOpen)
			{
				OpenUI();
			}

			m_lastPressTime = Time.time;
		}

		void OpenUI()
		{ 
			m_viewGameObject.SetActive(true);
		}

		void CloseUI()
		{
			m_viewGameObject.SetActive(false);
		}
	}

}