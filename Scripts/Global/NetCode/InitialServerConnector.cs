using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Jobworld
{

	public class InitialServerConnector : MonoBehaviour, IConnectionCallbacks
	{
		[Header("Simulation")]
		[SerializeField] private bool enableSimulation = false;
		[SerializeField] private int inComingLag = 100;
		[SerializeField] private int outGoingLag = 100;

		public void OnEnable()
		{
			PhotonNetwork.AddCallbackTarget(this);
		}

		public void OnDisable()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		public void Start()
		{
			LoadBalancingPeer loadBalancingPeer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
			
			// 시뮬레이션 설정
			loadBalancingPeer.IsSimulationEnabled = enableSimulation;
			if (enableSimulation)
			{
				loadBalancingPeer.NetworkSimulationSettings.IncomingLag = inComingLag;
				loadBalancingPeer.NetworkSimulationSettings.OutgoingLag = outGoingLag;
			}

			PhotonNetwork.ConnectUsingSettings();
		}

		public void OnConnected()
		{
			Debug.Log("연결 성공");
		}

		public void OnConnectedToMaster() {}

		public void OnDisconnected(DisconnectCause cause)
		{
			Debug.Log($"연결 끊김: {cause}");
		}

		public void OnRegionListReceived(RegionHandler regionHandler) {}

		public void OnCustomAuthenticationResponse(Dictionary<string, object> data) {}

		public void OnCustomAuthenticationFailed(string debugMessage) {}
	}

}