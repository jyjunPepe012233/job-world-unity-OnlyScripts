using Oculus.Interaction;
using UnityEngine;

namespace Jobworld
{

	public class TeacherMainCreateMenuPresenter : MonoBehaviour
	{
		[SerializeField]
		private TeacherMainMenuPageNavigator m_pageNav;
		
		[SerializeField]
		private TeacherMainCreateMenuView m_view;

		private ICreateSessionModel m_createModel = new CreateSessionModel();

		[SerializeField, Interface(typeof(IMyDataModel))]
		private Object m_myDataModelObject;
		
		private IMyDataModel m_myDataModel => m_myDataModelObject as IMyDataModel;
		
		public void OnEnable()
		{
			m_pageNav.m_createMenuOpened += UpdateMyData;
			
			m_view.createAttempted += CreateSession;
			
			m_createModel.createSucceed += OnCreateSucceed;
			m_createModel.createFailed += OnCreateFailed;
		}
		
		public void OnDisable()
		{
			m_pageNav.m_createMenuOpened -= UpdateMyData;
			
			m_view.createAttempted -= CreateSession;

			m_createModel.createSucceed -= OnCreateSucceed;
			m_createModel.createFailed -= OnCreateFailed;
		}
		
		private void OnCreateSucceed()
		{
			m_pageNav.OpenLobbyMenu();
		}
		
		private void OnCreateFailed()
		{
			m_pageNav.OpenCreateMenu();
		}

		private async void UpdateMyData()
		{
			var myData = await m_myDataModel.GetMyData();
			m_view.UpdateMyData(myData);
		}
		
		private void CreateSession()
		{
			m_pageNav.OpenLoadingScreen();
			var creationSetting = new SessionCreationSetting
			{
				maxPlayer = m_view.maxPlayer,
				playTime = m_view.playTime
			};
			m_createModel.CreateSession(creationSetting);
		}
	}

}