using System;
using UnityEngine;

namespace Jobworld
{

	public class TeacherMainMenuPageNavigator : MonoBehaviour
	{
		[SerializeField]
		private CanvasGroup m_loadingScreen;

		[SerializeField]
		private CanvasGroup m_loginMenu;

		[SerializeField]
		private CanvasGroup m_createMenu;

		[SerializeField]
		private CanvasGroup m_lobbyMenu;
		
		[SerializeField]
		private CanvasGroup m_playMenu;

		private CanvasGroup m_currentMenu; 
		
		public event Action m_loadingScreenOpened;
		
		public event Action m_loginMenuOpened;
		
		public event Action m_createMenuOpened;
		
		public event Action m_lobbyMenuOpened;
		
		public event Action m_playMenuOpened;

		void Start()
		{
			m_loadingScreen.SetVisible(false);
			m_createMenu.SetVisible(false);
			m_lobbyMenu.SetVisible(false);
			m_playMenu.SetVisible(false);

			ChangeMenuActive(m_loginMenu);
		}
		
		private void ChangeMenuActive(CanvasGroup menu)
		{
			m_currentMenu?.SetVisible(false);
			m_currentMenu = menu;
			m_currentMenu.SetVisible(true);
		}
		
		public void OpenLoadingScreen()
		{
			ChangeMenuActive(m_loadingScreen);
			
			m_loadingScreenOpened?.Invoke();
		}

		public void OpenLoginMenu()
		{
			ChangeMenuActive(m_loginMenu);
			
			m_loginMenuOpened?.Invoke();
		}
		
		public void OpenCreateMenu()
		{
			ChangeMenuActive(m_createMenu);
			
			m_createMenuOpened?.Invoke();
		}
		
		public void OpenLobbyMenu()
		{
			ChangeMenuActive(m_lobbyMenu);
			
			m_lobbyMenuOpened?.Invoke();
		}

		public void OpenPlayMenu()
		{
			ChangeMenuActive(m_playMenu);
			
			m_playMenuOpened?.Invoke();
		}
	}

}