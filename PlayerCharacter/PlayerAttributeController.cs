using UnityEngine;
using Oculus.Interaction.Locomotion;

/// <summary>
/// FirstPersonLocomotor의 이동 속성(속도 등)을 제어하는 컨트롤러
/// </summary>
namespace Jobworld
{
    public class PlayerAttributeController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        [Tooltip("제어할 FirstPersonLocomotor 컴포넌트")]
        private FirstPersonLocomotor _locomotor;

        [Header("Speed Settings")]
        [SerializeField]
        [Tooltip("일반 이동 속도 배율")]
        private float _normalSpeed = 30.0f;

        [SerializeField]
        [Tooltip("웅크리기 속도 배율")]
        private float _crouchSpeed = 10.0f;

        [SerializeField]
        [Tooltip("달리기 속도 배율")]
        private float _runSpeed = 50.0f;

        [SerializeField]
        [Tooltip("가속도")]
        private float _acceleration = 5.0f;

        [Header("Speed Multiplier")]
        [SerializeField]
        [Range(0.1f, 3.0f)]
        [Tooltip("모든 속도에 적용되는 전역 배율 (0.1 ~ 3.0)")]
        private float _globalSpeedMultiplier = 1.0f;

        private void Start()
        {
            if (_locomotor == null)
            {
                _locomotor = GetComponent<FirstPersonLocomotor>();
            }

            if (_locomotor == null)
            {
                Debug.LogError("FirstPersonLocomotor를 찾을 수 없습니다!");
                return;
            }

            ApplySpeedSettings();
        }

        private void OnValidate()
        {
            // 에디터에서 값이 변경될 때마다 실시간으로 적용
            if (Application.isPlaying && _locomotor != null)
            {
                ApplySpeedSettings();
            }
        }

        /// <summary>
        /// 현재 설정된 속도 값들을 Locomotor에 적용
        /// </summary>
        public void ApplySpeedSettings()
        {
            if (_locomotor == null) return;

            _locomotor.SpeedFactor = _normalSpeed * _globalSpeedMultiplier;
            _locomotor.CrouchSpeedFactor = _crouchSpeed * _globalSpeedMultiplier;
            _locomotor.RunningSpeedFactor = _runSpeed * _globalSpeedMultiplier;
            _locomotor.Acceleration = _acceleration;
        }

        /// <summary>
        /// 일반 이동 속도 설정
        /// </summary>
        public void SetNormalSpeed(float speed)
        {
            _normalSpeed = speed;
            ApplySpeedSettings();
        }

        /// <summary>
        /// 웅크리기 속도 설정
        /// </summary>
        public void SetCrouchSpeed(float speed)
        {
            _crouchSpeed = speed;
            ApplySpeedSettings();
        }

        /// <summary>
        /// 달리기 속도 설정
        /// </summary>
        public void SetRunSpeed(float speed)
        {
            _runSpeed = speed;
            ApplySpeedSettings();
        }

        /// <summary>
        /// 가속도 설정
        /// </summary>
        public void SetAcceleration(float acceleration)
        {
            _acceleration = acceleration;
            ApplySpeedSettings();
        }

        /// <summary>
        /// 전역 속도 배율 설정 (모든 속도에 영향)
        /// </summary>
        public void SetGlobalSpeedMultiplier(float multiplier)
        {
            _globalSpeedMultiplier = Mathf.Clamp(multiplier, 0.1f, 3.0f);
            ApplySpeedSettings();
        }

        /// <summary>
        /// 모든 속도 값을 기본값으로 리셋
        /// </summary>
        public void ResetToDefault()
        {
            _normalSpeed = 30.0f;
            _crouchSpeed = 10.0f;
            _runSpeed = 50.0f;
            _acceleration = 5.0f;
            _globalSpeedMultiplier = 1.0f;
            ApplySpeedSettings();
        }

        // Getter 메서드들
        public float GetNormalSpeed() => _normalSpeed;
        public float GetCrouchSpeed() => _crouchSpeed;
        public float GetRunSpeed() => _runSpeed;
        public float GetAcceleration() => _acceleration;
        public float GetGlobalSpeedMultiplier() => _globalSpeedMultiplier;
    }
}

