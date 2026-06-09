using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Jobworld
{
    public class TestSession : MonoBehaviourPunCallbacks
    {
        public bool isHost;
        public Transform transform1;

        private void Awake()
        {
            // 씬 자동 동기화 설정 (중요!)
            PhotonNetwork.AutomaticallySyncScene = true;
            
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("✅ Photon 서버 연결 완료");
            Debug.Log($"닉네임: {PhotonNetwork.NickName}");
            
            if (isHost)
            {
                // 방 옵션 설정
                RoomOptions roomOptions = new RoomOptions
                {
                    MaxPlayers = 4,
                    IsVisible = true,
                    IsOpen = true
                };
                
                PhotonNetwork.CreateRoom("TestRoom", roomOptions);
                Debug.Log("🏠 방 생성 시도중... (호스트 = 마스터 클라이언트)");
            }
            else
            {
                PhotonNetwork.JoinRoom("TestRoom");
                Debug.Log("🚪 방 참가 시도중...");
            }
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"✅ 방 입장 완료: {PhotonNetwork.CurrentRoom.Name}");
            Debug.Log($"👥 현재 방 인원: {PhotonNetwork.CurrentRoom.PlayerCount}명");
            
            // 마스터 클라이언트 확인
            Debug.Log($"👑 마스터 클라이언트: {PhotonNetwork.MasterClient.NickName} (ActorNumber: {PhotonNetwork.MasterClient.ActorNumber})");
            Debug.Log($"🎯 내가 마스터?: {PhotonNetwork.IsMasterClient}");
            
            // 방에 있는 모든 플레이어 출력
            foreach (var player in PhotonNetwork.PlayerList)
            {
                string role = player.IsMasterClient ? " 👑 [마스터]" : "";
                Debug.Log($"  - Player {player.ActorNumber}: {player.NickName}{role}");
            }
            
            // 플레이어 생성
            Vector3 spawnPos = isHost ? transform1.position : transform.position;
            GameObject myPlayer = PhotonNetwork.Instantiate("FiremanPlayer", spawnPos, Quaternion.identity);
            
            Debug.Log($"🎮 내 플레이어 생성 완료 (ViewID: {myPlayer.GetComponent<PhotonView>().ViewID})");
        }

        // 다른 플레이어가 방에 들어왔을 때
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"🆕 새 플레이어 입장: {newPlayer.NickName} (총 {PhotonNetwork.CurrentRoom.PlayerCount}명)");
            
            // 마스터 클라이언트만 실행할 로직
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"👑 [마스터 권한] 새 플레이어 {newPlayer.NickName} 입장 처리");
                // 여기에 마스터만 할 수 있는 작업 추가 (예: 게임 상태 동기화)
            }
        }

        // 다른 플레이어가 방을 나갔을 때
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"👋 플레이어 퇴장: {otherPlayer.NickName}");
            
            // 마스터 클라이언트만 실행할 로직
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"👑 [마스터 권한] 플레이어 {otherPlayer.NickName} 퇴장 처리");
            }
        }

        // 마스터 클라이언트가 변경되었을 때
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log($"👑 마스터 클라이언트 변경: {newMasterClient.NickName}");
            Debug.Log($"🎯 내가 새 마스터?: {PhotonNetwork.IsMasterClient}");
            
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("👑 내가 새로운 마스터 클라이언트가 되었습니다!");
                // 마스터 권한 획득 시 처리할 로직
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"❌ 방 참가 실패 [{returnCode}]: {message}");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"❌ 방 생성 실패 [{returnCode}]: {message}");
        }

        // 디버그용 - 현재 상태 확인
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                PrintDebugInfo();
            }
        }

        private void PrintDebugInfo()
        {
            Debug.Log("========== 현재 상태 ==========");
            Debug.Log($"연결 상태: {PhotonNetwork.IsConnected}");
            Debug.Log($"방에 있음: {PhotonNetwork.InRoom}");
            
            if (PhotonNetwork.InRoom)
            {
                Debug.Log($"방 이름: {PhotonNetwork.CurrentRoom.Name}");
                Debug.Log($"총 인원: {PhotonNetwork.CurrentRoom.PlayerCount}명");
                Debug.Log($"내 ActorNumber: {PhotonNetwork.LocalPlayer.ActorNumber}");
                Debug.Log($"👑 마스터 클라이언트: {PhotonNetwork.MasterClient.NickName}");
                Debug.Log($"🎯 내가 마스터?: {PhotonNetwork.IsMasterClient}");
                
                // 씬에 있는 모든 PhotonView 오브젝트 출력
                PhotonView[] allViews = FindObjectsOfType<PhotonView>();
                Debug.Log($"씬의 PhotonView 오브젝트: {allViews.Length}개");
                foreach (var view in allViews)
                {
                    string ownerInfo = view.Owner != null ? 
                        $"{view.Owner.NickName}{(view.Owner.IsMasterClient ? " 👑" : "")}" : 
                        "없음";
                    Debug.Log($"  - ViewID: {view.ViewID}, Owner: {ownerInfo}, IsMine: {view.IsMine}");
                }
            }
            Debug.Log("==============================");
        }
    }
}