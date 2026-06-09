using System;
using UnityEngine;

namespace Jobworld
{

	public class MainMenuPageNavigator : MonoBehaviour
	{
		[SerializeField]
		private CanvasGroup m_loadingScreen;
		
		[SerializeField]
		private CanvasGroup m_loginMenu;

		[SerializeField]
		private CanvasGroup m_joinMenu;

		[SerializeField]
		private CanvasGroup m_tutorialSelectionMenu;

		private CanvasGroup m_currentMenu;

		public event Action loadingScreenOpened;

		public event Action loginMenuOpened;

		public event Action joinMenuOpened;

		public event Action tutorialSelectionMenuOpened;

		void Start()
		{
			m_loadingScreen.SetVisible(false);
			m_joinMenu.SetVisible(false);
			m_tutorialSelectionMenu.SetVisible(false);
			m_loginMenu.SetVisible(false);
			
			//ChangeMenuActive(m_loginMenu);
			ChangeMenuActive(m_joinMenu);
		}

		public void OpenLoadingScreen()
		{
			ChangeMenuActive(m_loadingScreen);

			loadingScreenOpened?.Invoke();
		}

		public void OpenLoginMenu()
		{
			ChangeMenuActive(m_loginMenu);
			
			loginMenuOpened?.Invoke();
		}

		public void OpenJoinMenu()
		{
			ChangeMenuActive(m_joinMenu);
			
			joinMenuOpened?.Invoke();
		}

		public void OpenTutorialSelectionMenu()
		{
			ChangeMenuActive(m_tutorialSelectionMenu);
			tutorialSelectionMenuOpened?.Invoke();
		}

		private void ChangeMenuActive(CanvasGroup menu)
		{
			m_currentMenu?.SetVisible(false);
			m_currentMenu = menu;
			m_currentMenu.SetVisible(true);
		}
	}

}