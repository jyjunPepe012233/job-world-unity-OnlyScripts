using UnityEngine;

namespace Jobworld.CarSystems
{
    /// <summary>
    /// 바퀴 애니메이션을 독립적으로 처리하는 시스템
    /// </summary>
    public class CarWheelSystem : MonoBehaviour
    {
        [Header("바퀴 Transform")]
        [SerializeField] private Transform frontLeftWheel;
        [SerializeField] private Transform frontRightWheel;
        [SerializeField] private Transform rearLeftWheel;
        [SerializeField] private Transform rearRightWheel;

        [Header("조향용 Transform")]
        [SerializeField] private Transform frontLeftWheelSteering;
        [SerializeField] private Transform frontRightWheelSteering;

        [Header("바퀴 설정")]
        [SerializeField] private float wheelRotationSpeed = 50f;
        [SerializeField] private float maxSteeringAngle = 30f;
        [SerializeField] private float steeringSmoothing = 8f;

        private float wheelRotationAngle = 0f;
        private float currentWheelSteeringAngle = 0f;

        public void Initialize()
        {
            ValidateAndSetDefaults();
        }

        public void UpdateWheels(float carSpeed, float steeringAngle)
        {
            UpdateWheelRotation(carSpeed);
            UpdateWheelSteering(steeringAngle);
        }

        public void ResetState()
        {
            wheelRotationAngle = 0f;
            currentWheelSteeringAngle = 0f;
        }

        public WheelNetworkData GetWheelData()
        {
            return new WheelNetworkData
            {
                RotationAngle = wheelRotationAngle,
                SteeringAngle = currentWheelSteeringAngle
            };
        }

        public void ApplyNetworkData(WheelNetworkData networkData)
        {
            wheelRotationAngle = networkData.RotationAngle;
            currentWheelSteeringAngle = networkData.SteeringAngle;
            
            // 바퀴 시각적 업데이트
            ApplyWheelVisuals();
        }

        private void ValidateAndSetDefaults()
        {
            if (frontLeftWheelSteering == null) frontLeftWheelSteering = frontLeftWheel;
            if (frontRightWheelSteering == null) frontRightWheelSteering = frontRightWheel;
        }

        private void UpdateWheelRotation(float carSpeed)
        {
            wheelRotationAngle += carSpeed * wheelRotationSpeed * Time.deltaTime;
            var rotation = Quaternion.Euler(wheelRotationAngle, 0f, 0f);
            
            if (frontLeftWheel != null) frontLeftWheel.localRotation = rotation;
            if (frontRightWheel != null) frontRightWheel.localRotation = rotation;
            if (rearLeftWheel != null) rearLeftWheel.localRotation = rotation;
            if (rearRightWheel != null) rearRightWheel.localRotation = rotation;
        }

        private void UpdateWheelSteering(float steeringAngle)
        {
            var targetSteeringAngle = steeringAngle * maxSteeringAngle;
            currentWheelSteeringAngle = Mathf.Lerp(currentWheelSteeringAngle, targetSteeringAngle, 
                steeringSmoothing * Time.deltaTime);

            var steeringRotation = Quaternion.Euler(0f, currentWheelSteeringAngle, 0f);
            
            if (frontLeftWheelSteering != null) 
                frontLeftWheelSteering.localRotation = steeringRotation;
            if (frontRightWheelSteering != null) 
                frontRightWheelSteering.localRotation = steeringRotation;
        }

        private void ApplyWheelVisuals()
        {
            var rotation = Quaternion.Euler(wheelRotationAngle, 0f, 0f);
            var steeringRotation = Quaternion.Euler(0f, currentWheelSteeringAngle, 0f);
            
            // 바퀴 회전 적용
            if (frontLeftWheel != null) frontLeftWheel.localRotation = rotation;
            if (frontRightWheel != null) frontRightWheel.localRotation = rotation;
            if (rearLeftWheel != null) rearLeftWheel.localRotation = rotation;
            if (rearRightWheel != null) rearRightWheel.localRotation = rotation;
            
            // 조향 적용
            if (frontLeftWheelSteering != null) 
                frontLeftWheelSteering.localRotation = steeringRotation;
            if (frontRightWheelSteering != null) 
                frontRightWheelSteering.localRotation = steeringRotation;
        }
    }
}