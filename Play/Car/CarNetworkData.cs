using UnityEngine;

namespace Jobworld.CarSystems
{
    /// <summary>
    /// 네트워크 동기화를 위한 자동차 데이터 구조체
    /// </summary>
    [System.Serializable]
    public class CarNetworkData
    {
        public float Speed;
        public float SteeringAngle;
        public float WheelRotationAngle;
        public float WheelSteeringAngle;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool IsPlayerSeated;
        public float HandleValue;
    }

    /// <summary>
    /// 바퀴 네트워크 데이터 구조체
    /// </summary>
    [System.Serializable]
    public struct WheelNetworkData
    {
        public float RotationAngle;
        public float SteeringAngle;
    }
}