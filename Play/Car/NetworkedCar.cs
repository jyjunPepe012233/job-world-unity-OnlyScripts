using Jobworld;
using Jobworld.CarSystems;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// 네트워크 멀티플레이어를 지원하는 자동차 클래스
/// </summary>
public class NetworkedCar : Car, IPunObservable
{
    [Header("네트워크 설정")]
    [SerializeField] private float networkLerpRate = 10f;
    [SerializeField] private bool syncPositionRotation = true;

    // 네트워크 동기화용 데이터
    private CarNetworkData localNetworkData;
    private CarNetworkData remoteNetworkData;

    protected override void ProcessCarLogic()
    {
        // 로컬 플레이어만 입력 처리
        if (photonView.IsMine || !photonView.enabled)
        {
            base.ProcessCarLogic();
        }
        else
        {
            // 원격 데이터 보간 적용
            ApplyRemoteNetworkData();
        }
    }

    protected override void ProcessCarPhysics()
    {
        // 로컬 플레이어만 물리 처리
        if (photonView.IsMine || !photonView.enabled)
        {
            base.ProcessCarPhysics();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 데이터 전송
            SendLocalData(stream);
        }
        else
        {
            // 원격 데이터 수신
            ReceiveRemoteData(stream);
        }
    }

    private void SendLocalData(PhotonStream stream)
    {
        // 움직임 데이터
        stream.SendNext(movementSystem?.CurrentSpeed ?? 0f);
        stream.SendNext(movementSystem?.CurrentSteeringAngle ?? 0f);
            
        // 바퀴 데이터
        var wheelData = wheelSystem?.GetWheelData() ?? new WheelNetworkData();
        stream.SendNext(wheelData.RotationAngle);
        stream.SendNext(wheelData.SteeringAngle);
            
        // 위치/회전 데이터
        if (syncPositionRotation)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
            
        // 탑승 상태
        stream.SendNext(seatingSystem?.IsPlayerSeated ?? false);
            
        // 핸들 값
        stream.SendNext(inputSystem?.CurrentSteeringValue ?? 0f);
    }

    private void ReceiveRemoteData(PhotonStream stream)
    {
        remoteNetworkData = new CarNetworkData
        {
            Speed = (float)stream.ReceiveNext(),
            SteeringAngle = (float)stream.ReceiveNext(),
            WheelRotationAngle = (float)stream.ReceiveNext(),
            WheelSteeringAngle = (float)stream.ReceiveNext()
        };

        if (syncPositionRotation)
        {
            remoteNetworkData.Position = (Vector3)stream.ReceiveNext();
            remoteNetworkData.Rotation = (Quaternion)stream.ReceiveNext();
        }

        remoteNetworkData.IsPlayerSeated = (bool)stream.ReceiveNext();
        remoteNetworkData.HandleValue = (float)stream.ReceiveNext();
    }

    private void ApplyRemoteNetworkData()
    {
        if (remoteNetworkData == null) return;

        // 움직임 데이터 적용
        movementSystem?.ApplyNetworkData(remoteNetworkData.Speed, remoteNetworkData.SteeringAngle);

        // 바퀴 데이터 적용
        var wheelNetworkData = new WheelNetworkData
        {
            RotationAngle = remoteNetworkData.WheelRotationAngle,
            SteeringAngle = remoteNetworkData.WheelSteeringAngle
        };
        wheelSystem?.ApplyNetworkData(wheelNetworkData);

        // 위치/회전 보간
        if (syncPositionRotation)
        {
            transform.position = Vector3.Lerp(transform.position, remoteNetworkData.Position, 
                Time.deltaTime * networkLerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, remoteNetworkData.Rotation, 
                Time.deltaTime * networkLerpRate);
        }

        // 입력 시스템에 네트워크 핸들 값 적용
        inputSystem?.ApplyNetworkHandleValue(remoteNetworkData.HandleValue);

        // 탑승 상태 동기화
        SynchronizeSeatingState(remoteNetworkData.IsPlayerSeated);
    }

    private void SynchronizeSeatingState(bool remotePlayerSeated)
    {
        var currentlySeated = seatingSystem?.IsPlayerSeated ?? false;
            
        if (remotePlayerSeated != currentlySeated)
        {
            if (remotePlayerSeated)
            {
                // 원격에서 누군가 탑승했다고 알림 (시각적 표시만)
                seatingSystem?.SetRemotePlayerSeated(true);
            }
            else
            {
                // 원격에서 하차했다고 알림
                seatingSystem?.SetRemotePlayerSeated(false);
            }
        }
    }
}