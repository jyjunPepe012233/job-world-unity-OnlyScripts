using Oculus.Interaction;
using UnityEngine;

namespace Jobworld
{

	public class MainJoinMenuPresenter : MonoBehaviour
	{
		[SerializeField]
		private MainMenuPageNavigator m_pageNav;
		
		[SerializeField]
		private MainJoinMenuView m_view;
		
		[SerializeField, Interface(typeof(IJoinModel))]
		private Object m_joinModelObject;
		
		private IJoinModel m_joinModel => m_joinModelObject as IJoinModel;
		
		[SerializeField, Interface(typeof(IMyDataModel))]
		private Object m_myDataModelObject;
		
		private IMyDataModel m_myDataModel => m_myDataModelObject as IMyDataModel;

		void OnEnable()
		{
			m_pageNav.joinMenuOpened += UpdateMyData;
			
			m_view.joinSessionAttempted += JoinSession;

			m_joinModel.joinSucceed += OnJoinSucceed;
			m_joinModel.joinFailed += OnJoinFailed;
		}

		void OnDisable()
		{
			m_pageNav.joinMenuOpened -= UpdateMyData;

			m_view.joinSessionAttempted -= JoinSession;

			m_joinModel.joinSucceed -= OnJoinSucceed;
			m_joinModel.joinFailed -= OnJoinFailed;
		}

		private void JoinSession()
		{
			m_joinModel.JoinSession(m_view.sessionName);
			
			m_pageNav.OpenLoadingScreen();
		}

		private async void UpdateMyData()
		{
			var userData = await m_myDataModel.GetMyData();
			m_view.UpdateMyData(userData);
		}
		
		private void OnJoinSucceed()
		{
			// 세션 접속 성공 시 GuestClientService가 다른 씬을 로드함 
		}

		private void OnJoinFailed()
		{
			m_pageNav.OpenJoinMenu();
		}
	}
}