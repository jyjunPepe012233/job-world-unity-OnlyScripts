using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Jobworld
{
	public static class SessionNetwork
	{
		public static event Action onPlayStarted;
		public static event Action onPlayFinished;
		public static event Action onBreakStarted;
		public static event Action onBreakFinished;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void SubscribePhotonEvents()
		{
			PhotonEventFrontReceiver.Subscribe(CustomPhotonEventCodes.OnPlayStarted, onPlayStarted);
			PhotonEventFrontReceiver.Subscribe(CustomPhotonEventCodes.OnPlayFinished, onPlayFinished);
			PhotonEventFrontReceiver.Subscribe(CustomPhotonEventCodes.OnBreakStarted, onBreakStarted);
			PhotonEventFrontReceiver.Subscribe(CustomPhotonEventCodes.OnBreakFinished, onBreakFinished);
		}

		public static void BroadcastEvent(CustomPhotonEventCodes eventCode)
		{
			BroadcastEvent(eventCode, null);
		}

		public static void BroadcastEvent(CustomPhotonEventCodes eventCode, object data)
		{
			RaiseEventOptions raiseEventOptions =
				new RaiseEventOptions { Receivers = ReceiverGroup.All };
			
			SendOptions sendOptions = SendOptions.SendReliable;

			PhotonNetwork.RaiseEvent(
				(byte)eventCode,
				data,
				raiseEventOptions,
				sendOptions
			);
		}
	}

}