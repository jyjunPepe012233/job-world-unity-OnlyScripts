using UnityEngine;
using System.Collections.Generic;

namespace Jobworld
{
    public class PlayerFireDamageDetector : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private int damagePerSecond = 5; // 초당 피해 관리 지수 감소량
        [SerializeField] private float damageInterval = 1f; // 데미지 적용 간격 (초)
        
        [Header("Player Detection")]
        [SerializeField] private LayerMask playerLayer = -1; // 플레이어 레이어
        
        private HashSet<Collider> _playersInFire = new HashSet<Collider>();
        private float _lastDamageTime = 0f;
        private FiremanMissionManager _missionManager;
        
        private void Start()
        {
            _missionManager = FindObjectOfType<FiremanMissionManager>();
            
            // 별도의 트리거 콜라이더 생성 (플레이어 감지용)
            var triggerCollider = gameObject.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            
            // 기존 콜라이더보다 약간 크게 설정
            var mainCollider = GetComponent<Collider>();
            if (mainCollider != null && mainCollider != triggerCollider)
            {
                triggerCollider.size = mainCollider.bounds.size * 1.2f;
            }
        }
        
        private void Update()
        {
            // 플레이어가 화재 영역에 있으면 주기적으로 데미지 적용
            if (_playersInFire.Count > 0 && Time.time - _lastDamageTime >= damageInterval)
            {
                ApplyFireDamage();
                _lastDamageTime = Time.time;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (IsPlayer(other))
            {
                _playersInFire.Add(other);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (IsPlayer(other))
            {
                _playersInFire.Remove(other);
            }
        }
        
        private bool IsPlayer(Collider other)
        {
            // 레이어 체크
            if (((1 << other.gameObject.layer) & playerLayer) == 0)
                return false;
                
            // 태그 체크 (추가 확인)
            return other.CompareTag("Player") || 
                   other.CompareTag("PlayerBody") || 
                   other.CompareTag("RightHand") || 
                   other.CompareTag("LeftHand") ||
                   other.name.ToLower().Contains("player");
        }
        
        private void ApplyFireDamage()
        {
            if (_missionManager == null) return;
            
            // 화재 영역에 있는 플레이어 수만큼 데미지 적용
            int totalDamage = damagePerSecond * _playersInFire.Count;
            _missionManager.DamageDamageManagement(totalDamage);
        }
        
        private void OnDestroy()
        {
            _playersInFire.Clear();
        }
    }
}