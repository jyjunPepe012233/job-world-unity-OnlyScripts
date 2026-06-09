using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jobworld
{

	public class TestJoinUI : MonoBehaviour
	{
		public TMP_InputField sessionNameField;
		public TMP_InputField nicknameField;
		
		public Button joinRoomButton;

		void Awake()
		{
			GuestClientService.onJoinFailed += () => joinRoomButton.interactable = true;
		}
		
		public void JoinRoom()
		{
			PhotonNetwork.NickName = nicknameField.text;
			
			GuestClientService.JoinSession(sessionNameField.text);

			joinRoomButton.interactable = false;
		}
	}

}