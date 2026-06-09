using UnityEngine;
using Oculus.Interaction;

namespace Jobworld.CarSystems
{
    /// <summary>
    /// 입력 처리를 독립적으로 담당하는 시스템
    /// </summary>
    public class CarInputSystem : MonoBehaviour
    {
        [Header("핸들 시스템")] [SerializeField] private Handle handleScript;

        [Header("입력 설정")] [SerializeField] private float triggerDeadzone = 0.1f;
        [SerializeField] private float steeringSmoothingCar = 4f;

        private float currentSteeringAngle = 0f;

        public float CurrentSteeringValue => handleScript?.GetHandleValue() ?? 0f;

        public void Initialize()
        {
            if (handleScript == null)
                Debug.LogError("Handle 스크립트가 할당되지 않았습니다!", this);
        }

        public CarInputData GetInput()
        {
            var leftTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            var rightTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

            var accelerateInput = rightTrigger > triggerDeadzone ? rightTrigger : 0f;
            var brakeInput = leftTrigger > triggerDeadzone ? leftTrigger : 0f;

            // 핸들 조향 처리
            var rawSteeringInput = handleScript?.GetHandleValue() ?? 0f;
            currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, -rawSteeringInput,
                steeringSmoothingCar * Time.deltaTime);

            return new CarInputData
            {
                AccelerateInput = accelerateInput,
                BrakeInput = brakeInput,
                SteeringInput = currentSteeringAngle
            };
        }

        public void ResetState()
        {
            currentSteeringAngle = 0f;
        }

        public void ApplyNetworkHandleValue(float networkHandleValue)
        {
            // 원격 핸들 값을 현재 조향에 반영 (Handle 컴포넌트가 있다면)
            if (handleScript != null)
            {
                // Handle 스크립트에 SetHandleValue 메서드가 있다면 사용
                // handleScript.SetHandleValue(networkHandleValue);
            }
        }
    }
}