using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Jobworld
{

	public class RemoveSessionOnDisconnected : MonoBehaviourPunCallbacks
	{
		private static RemoveSessionOnDisconnected m_instance;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void InitializeOnLoad()
		{
			m_instance = new GameObject(nameof(RemoveSessionOnDisconnected)).AddComponent<RemoveSessionOnDisconnected>();
			DontDestroyOnLoad(m_instance);
		}
		
		public override void OnDisconnected(DisconnectCause cause)
		{
			// TODO: 앱 종료로 인해 Disconnect될 RemoveSession이나 RemovePlayer Task가 제대로 진행될 수 없음
			
			if (PhotonNetwork.IsMasterClient)
			{
				DataInterfaceContainer.session.RemoveSession(MasterClientLocalInfoHolder.sessionData);
				MasterClientLocalInfoHolder.ClearSession();
			}
			else
			{
				DataInterfaceContainer.session.RemovePlayer(GuestClientLocalInfoHolder.playData.authId);
				GuestClientLocalInfoHolder.ClearSession();
			}
		}
	}

}