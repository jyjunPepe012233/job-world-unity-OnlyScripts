using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oculus.Interaction;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Jobworld
{

	public class MasterClientService : IMatchmakingCallbacks
	{
		private static MasterClientService m_instance;

		private const string LOBBY_SCENE_NAME = "LobbyScene";

		private const string PLAY_SCENE_NAME = "MegaCityPlayScene";

		private const string END_SCENE_NAME = "EndScene";
		
		private static ISessionDataInterface m_sessionDI = DataInterfaceContainer.session;

		private static SessionCodeGenerator m_sessionCodeGenerator = new();
		
		public static event Action onSessionCreated;
		public static event Action onSessionCreateFailed;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			#if UNITY_STANDALONE
			m_instance = new MasterClientService();
			#endif
		}

		private MasterClientService()
		{
			PhotonNetwork.AddCallbackTarget(this);
		}

		~MasterClientService()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		public static void CreateSession(SessionCreationSetting creationSetting, out string sessionName)
		{
			sessionName = m_sessionCodeGenerator.GenerateCode();
			SessionData sessionData = new SessionData
			{
				ownerId = LocalAuthorizationHolder.id,
				sessionName = sessionName,
				maxPlayer = creationSetting.maxPlayer,
				playTime = creationSetting.playTime
			};
			MasterClientLocalInfoHolder.HostSession(sessionData);
			RoomOptions roomOptions = new RoomOptions
			{
				MaxPlayers = creationSetting.maxPlayer
			};
			PhotonNetwork.CreateRoom(sessionName, roomOptions);
		}

		public async void OnCreatedRoom()
		{
			// Room에 접속된 즉시 메세지 큐를 중지시킴(씬 전환 전에 메세지를 수신하면 동기화 누락이 발생할 수 있음)
			// 로비로 씬 전환이 완료된 후에 자동으로 메세지 큐가 활성화됨
			PhotonNetwork.IsMessageQueueRunning = false;

			var sessionData = MasterClientLocalInfoHolder.sessionData;
			m_sessionDI.SubscribeSessionEvents();
			await m_sessionDI.AddSession(sessionData);
			
			onSessionCreated?.Invoke();

			SafeLoading.ChangeActiveSceneAsync(LOBBY_SCENE_NAME);
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
			MasterClientLocalInfoHolder.ClearSession();
			
			onSessionCreateFailed?.Invoke();
		}
		
		public static async Task ChangeJob(long playerId, string jobName)
		{ 
			await m_sessionDI.UpdatePlayerJob(playerId, jobName);
		}

		public static async Task<bool> StartPlay()
		{
			int countsOfPlayerSelectedJob = await m_sessionDI.CountsOfPlayerSelectedJob();
			int countsOfAllPlayersInRoom = PhotonNetwork.CurrentRoom.PlayerCount - 1;
			bool isSuccess = countsOfPlayerSelectedJob == countsOfAllPlayersInRoom;
			if (isSuccess)
			{
				Debug.Log("Start Play Succeed");
				SessionNetwork.BroadcastEvent(CustomPhotonEventCodes.OnPlayStarted);
				await SafeLoading.ChangeActiveSceneAsync(PLAY_SCENE_NAME);
				// 위 코드에서 씬 교체가 완료된 뒤 코드 진행
				
				Debug.Log("Start Timer");
				var playTime = MasterClientLocalInfoHolder.sessionData.playTime;
				PlayTimer.StartTimer(playTime, FinishPlay, true);
			}
			return isSuccess;
		}

		public static void FinishPlay()
		{
			Debug.Log("Play Finished");
			
			if (PlayTimer.isPlaying != null)
			{
				PlayTimer.StopTimer();
			}
			else Debug.LogError("PlayTimerInstance hasn't been Initialized. You have to check the call timing of FinishPlay()");

			SessionNetwork.BroadcastEvent(CustomPhotonEventCodes.OnPlayFinished);
		}

		public static void StartBreak()
		{
			SessionNetwork.BroadcastEvent(CustomPhotonEventCodes.OnBreakStarted);
		}

		public static void FinishBreak()
		{
			SessionNetwork.BroadcastEvent(CustomPhotonEventCodes.OnBreakFinished);
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList) {}
		public void OnJoinedRoom() {}
		public void OnJoinRoomFailed(short returnCode, string message) {}
		public void OnJoinRandomFailed(short returnCode, string message) {}
		public void OnLeftRoom() {}
	}

}