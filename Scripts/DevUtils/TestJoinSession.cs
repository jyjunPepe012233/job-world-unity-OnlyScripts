using Jobworld;
using Photon.Pun;
using UnityEngine;

public class TestJoinSession : MonoBehaviourPunCallbacks
{
    public override void OnConnectedToMaster()
    {
        var creationSetting = new SessionCreationSetting
        {
            maxPlayer = 100,
            playTime = 30
        };
        MasterClientService.CreateSession(creationSetting, out string sessionName);
    }

    public override void OnJoinedRoom()
    {
        SafeLoading.LoadSceneWithLoading("LobbyScene");
    }
}
