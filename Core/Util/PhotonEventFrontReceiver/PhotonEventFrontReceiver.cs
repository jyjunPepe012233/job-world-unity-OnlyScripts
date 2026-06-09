using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

namespace Jobworld
{

	public class PhotonEventFrontReceiver : MonoBehaviourSingleton<PhotonEventFrontReceiver>
	{
		private Dictionary<CustomPhotonEventCodes, Action> m_subscriptions = new();
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeOnLoad()
		{
			InitializeSingleton();
		}

		public static void Subscribe(CustomPhotonEventCodes eventCode, Action callback)
		{
			singleton.SubscribeInternal(eventCode, callback);
		}

		private void SubscribeInternal(CustomPhotonEventCodes eventCode, Action callback)
		{
			if (callback == null)
				return; // 빈 Action은 등록 의미 없음

			if (!m_subscriptions.ContainsKey(eventCode))
				m_subscriptions[eventCode] = delegate { }; // 기본 빈 델리게이트

			m_subscriptions[eventCode] += callback;
		}

		private void OnEnable()
		{
			PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
		}

		private void OnEvent(EventData data)
		{
			foreach (var eventCode in m_subscriptions.Keys)
			{
				var eventCodeByte = (byte)eventCode;
				if (eventCodeByte == data.Code)
				{
					m_subscriptions[eventCode]?.Invoke();
				}
			}
		}
		
	}

}