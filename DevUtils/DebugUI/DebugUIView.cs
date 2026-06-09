using System;
//using Firebase.Auth;
using Jobworld;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace JobworldDev
{

	public class DebugUIView : MonoBehaviour
	{
		public TextMeshProUGUI internetStateView;

		public TextMeshProUGUI photonConnectionView;
		
		public TextMeshProUGUI connectMethodView;
		
		public TextMeshProUGUI pingView;

		public TextMeshProUGUI firebaseUserView;
		
		public TextMeshProUGUI roomNameView;
		
		public TextMeshProUGUI authIdView;
		
		public TextMeshProUGUI authTokenStatusView;

		public void Update()
		{
			UpdateInternetStateView(internetStateView);
			UpdatePhotonConnectionView(photonConnectionView);
			UpdateConnectMethodView(connectMethodView);
			UpdatePingView(pingView);
			UpdateFirebaseUserView(firebaseUserView);
			UpdateRoomNameView(roomNameView);
			UpdateAuthIdView(authIdView);
			UpdateAuthTokenStatusView(authTokenStatusView);
		}

		private void UpdateInternetStateView(TextMeshProUGUI tmp)
		{
			switch (Application.internetReachability)
			{
				case NetworkReachability.NotReachable:
					tmp.text = "No Connection";
					break;
				
				case NetworkReachability.ReachableViaCarrierDataNetwork:
					tmp.text = "Connected Via Carrier Data Network";
					break;
				
				case NetworkReachability.ReachableViaLocalAreaNetwork:
					tmp.text = "Connected Via LAN";
					break;
			}
		}

		private void UpdatePhotonConnectionView(TextMeshProUGUI tmp)
		{
			if (PhotonNetwork.IsConnectedAndReady)
			{
				tmp.text = "Connected And Ready";
			}
			else if (PhotonNetwork.IsConnected)
			{
				tmp.text = "Connected, Not Ready";
			}
			else
			{
				tmp.text = "Not Connected";
			}
		}

		private void UpdateConnectMethodView(TextMeshProUGUI tmp)
		{
			switch (PhotonNetwork.ConnectMethod)
			{
				case ConnectMethod.NotCalled:
					tmp.text = "Not Called";
					break;
				
				case ConnectMethod.ConnectToMaster:
					tmp.text = "Connect To Master";
					break;
				
				case ConnectMethod.ConnectToRegion:
					tmp.text = "Connect To Region";
					break;
				
				case ConnectMethod.ConnectToBest:
					tmp.text = "Connect To Best(Auto Select)";
					break;
			}
		}

		private void UpdatePingView(TextMeshProUGUI tmp)
		{
			if (PhotonNetwork.IsConnected)
			{
				tmp.text = PhotonNetwork.GetPing().ToString();
			}
		}

		private void UpdateFirebaseUserView(TextMeshProUGUI tmp)
		{
//			var user = FirebaseAuth.DefaultInstance.CurrentUser;

//			tmp.text = user != null ? user.UserId : "No Login";
		}
		
		private void UpdateRoomNameView(TextMeshProUGUI tmp)
		{
			if (PhotonNetwork.InRoom)
			{
				tmp.text = PhotonNetwork.CurrentRoom.Name;
			}
			else
			{
				tmp.text = "No Room";
			}
		}
		
		private void UpdateAuthIdView(TextMeshProUGUI tmp)
		{
			tmp.text = LocalAuthorizationHolder.id != 0 
				? LocalAuthorizationHolder.id.ToString()
				: "No ID";
		}

		private void UpdateAuthTokenStatusView(TextMeshProUGUI tmp)
		{
			bool hasAccessToken = !string.IsNullOrEmpty(LocalAuthorizationHolder.accessToken);
			bool hasRefreshToken = !string.IsNullOrEmpty(LocalAuthorizationHolder.refreshToken);

			if (hasAccessToken && hasRefreshToken)
			{
				tmp.text = "Both Tokens";
			}
			else if (hasAccessToken)
			{
				tmp.text = "Only Access";
			}
			else if (hasRefreshToken)
			{
				tmp.text = "Only Refresh";
			}
			else
			{
				tmp.text = "No Tokens";
			}
		}
	}

}