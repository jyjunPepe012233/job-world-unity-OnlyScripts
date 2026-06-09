using Oculus.Interaction;
using UnityEngine;

namespace Jobworld
{

	public class MainLoginMenuPresenter : MonoBehaviour
	{
		[SerializeField]
		private MainMenuPageNavigator m_pageNav;
		
		[SerializeField]
		private MainLoginMenuView m_view;
		
		[SerializeField, Interface(typeof(ILoginModel))]
		private Object m_loginModelObject;
		
		private ILoginModel m_loginModel => m_loginModelObject as ILoginModel;

		void OnEnable()
		{
			m_view.loginAttempted += Login;
			
			m_loginModel.loginSucceed += OnLoginSucceed;
			m_loginModel.loginFailed += OnLoginFailed;
		}

		void OnDisable()
		{
			m_view.loginAttempted -= Login;

			m_loginModel.loginSucceed -= OnLoginSucceed;
			m_loginModel.loginFailed -= OnLoginFailed;
		}

		private void OnLoginSucceed()
		{
			m_pageNav.OpenJoinMenu();
		}

		private void OnLoginFailed(ErrorResponse errorResponse)
		{
			m_pageNav.OpenLoginMenu();
		}
		
		private void Login()
		{
			m_loginModel.Login(m_view.id, m_view.password);
			
			m_pageNav.OpenLoadingScreen();
		}
	}

}