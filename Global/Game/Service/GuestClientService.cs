using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Jobworld
{

	public class GuestClientService : IMatchmakingCallbacks
	{
		private static GuestClientService m_instance;
		
		private const string LOBBY_SCENE_NAME = "LobbyScene";
		
		private const string PLAY_SCENE_NAME = "MegaCityPlayScene";

		private const string END_SCENE_NAME = "EndScene";

		private static ISessionDataInterface m_sessionDI = DataInterfaceContainer.session;
		
		public static event Action onJoined;
		public static event Action onJoinFailed;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			#if UNITY_ANDROID
			m_instance = new GuestClientService();
			#endif
		}

		private GuestClientService()
		{
			PhotonNetwork.AddCallbackTarget(this);

			SessionNetwork.onPlayStarted += OnPlayStarted;
			SessionNetwork.onPlayFinished += OnPlayFinished;
			
			m_sessionDI.memberUpdated += UpdateLocalPlayerInfo;
		}

		~GuestClientService()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}
		
		public static bool JoinSession(string sessionName)
		{
			var isSuccess = PhotonNetwork.JoinRoom(sessionName);
			return isSuccess;
		}
		
		public async void OnJoinedRoom()
		{
			// Room에 접속된 즉시 메세지 큐를 중지시킴(씬 전환 전에 메세지를 수신하면 동기화 누락이 발생할 수 있음)
			// 로비로 씬 전환이 완료된 후에 자동으로 메세지 큐가 활성화됨
			PhotonNetwork.IsMessageQueueRunning = false;

			GuestClientLocalInfoHolder.JoinSession(PhotonNetwork.CurrentRoom.Name);
			m_sessionDI.SubscribeSessionEvents();
			await m_sessionDI.AddPlayer(GuestClientLocalInfoHolder.playData);
			
			onJoined?.Invoke();
			
			await SafeLoading.LoadSceneWithLoadingAsync(LOBBY_SCENE_NAME);
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
			onJoinFailed?.Invoke();
		}

		private static void UpdateLocalPlayerInfo(SessionPlayerData playerData)
		{
			var ownedData = GuestClientLocalInfoHolder.playData;
			ownedData.jobName = playerData.jobName;
			ownedData.dollar = playerData.dollar;
		}
		
		private static async void OnPlayStarted()
		{
			Debug.Log("On Play Started!");
			await SafeLoading.LoadSceneWithLoadingAsync(PLAY_SCENE_NAME);
		}

		private static async void OnPlayFinished()
		{
			Debug.Log("On Play Finished");
			await SafeLoading.LoadSceneWithLoadingAsync(END_SCENE_NAME);
		}

		public static async Task SetDollar(int dollar)
		{
			GuestClientLocalInfoHolder.playData.dollar = dollar;
			await m_sessionDI.UpdatePlayerDollar(GuestClientLocalInfoHolder.playData.authId, dollar);
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList) {}
		public void OnCreatedRoom() {}
		public void OnCreateRoomFailed(short returnCode, string message) {}
		public void OnJoinRandomFailed(short returnCode, string message) {}
		public void OnLeftRoom() {}
	}

}