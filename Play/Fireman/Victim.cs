using UnityEngine;
using System;
using Photon.Pun;

namespace Jobworld
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Animator))]
    public class Victim : MonoBehaviourPun
    {
        [Header("설정")]
        [Tooltip("플레이어의 손이 트리거 안에 머물러야 하는 시간 (초)")]
        [SerializeField] private float rescueTime = 1f;

        [Tooltip("구조 성공 시 재생할 사운드 이름")]
        [SerializeField] private string rescueSoundName = "VictimRescued";

        [Tooltip("구조 성공 시 활성화할 애니메이션 트리거")]
        [SerializeField] private string happyAnimationTrigger = "Happy";

        [Tooltip("구조 성공 시 생성할 파티클 프리팹")]
        [SerializeField] private ParticleSystem rescueParticlePrefab;

        private float _currentRescueTime = 0f;
        private bool _isHandInside = false;
        private bool _isMyHand = false; // 내 손인지 확인
        private bool _isRescued = false;
        private Animator _animator;
        private VictimGroup _victimGroup;
        private RescueProgressUI _connectedUI; // 현재 연결된 UI
        
        [Header("Water Damage Settings")]
        [SerializeField] private int damagePerSecond = 3;
        [SerializeField] private float damageInterval = 1f;
        
        private bool _isWaterHitting = false;
        private float _lastWaterDamageTime = 0f;
        private FiremanMissionManager _missionManager;

        // 구출 진행 이벤트
        public event Action<float> OnRescueProgress; // 진도율 (0.0 ~ 1.0)
        public event Action OnRescueStarted;
        public event Action OnRescueCancelled;
        public event Action OnRescueCompleted;

        // 공개 프로퍼티
        public bool IsRescued => _isRescued;
        public float CurrentRescueProgress => rescueTime > 0 ? _currentRescueTime / rescueTime : 0f;
        public bool IsBeingRescued => _isHandInside && !_isRescued;

        public void SetGroup(VictimGroup group)
        {
            _victimGroup = group;
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _missionManager = FindObjectOfType<FiremanMissionManager>();
        }

        private void Update()
        {
            if (_isRescued) return;

            // 구조 진행 (현재 플레이어만)
            if (_isHandInside && _isMyHand)
            {
                _currentRescueTime += Time.deltaTime;
                
                // 진도율 계산 (0.0 ~ 1.0)
                float progress = Mathf.Clamp01(_currentRescueTime / rescueTime);
                
                // 매 프레임 구출 진행 이벤트 호출
                OnRescueProgress?.Invoke(progress);
                
                if (_currentRescueTime >= rescueTime)
                {
                    Rescue();
                }
            }
            
            // 물 데미지 처리 (마스터 클라이언트만)
            if ((PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected) && _isWaterHitting && Time.time - _lastWaterDamageTime >= damageInterval)
            {
                ApplyWaterDamage();
                _lastWaterDamageTime = Time.time;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (_isRescued) return;

            if (other.CompareTag("RightHand"))
            {
                Debug.Log("손이 Victim 트리거에 진입");
                _isHandInside = true;
                
                // 내 손인지 확인 (PhotonView의 소유자가 나인지)
                var playerPhotonView = other.GetComponentInParent<PhotonView>();
                _isMyHand = !PhotonNetwork.IsConnected || (playerPhotonView != null && playerPhotonView.IsMine);
                
                Debug.Log($"손 소유자 확인: IsMine={_isMyHand}");
                
                // 손의 주인(플레이어)을 찾아서 UI 연결
                ConnectToPlayerUI(other.transform);
                
                // 구출 시작 이벤트 호출
                OnRescueStarted?.Invoke();
                
                Debug.Log("PlayerHand entered the victim trigger. Rescue started!");
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            // 마스터 클라이언트에서만 파티클 충돌 처리 (네트워크 연결 시)
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;
            
            Debug.Log($"Victim {name}: OnParticleCollision 호출됨, other={other.name}, tag={other.tag}");
            
            if (other.CompareTag("WaterParticle"))
            {
                Debug.Log($"Victim {name}: WaterParticle 태그 확인됨!");
                
                if (!_isWaterHitting)
                {
                    _isWaterHitting = true;
                    _lastWaterDamageTime = Time.time;
                    Debug.Log($"물이 피해자 {name}에게 닿기 시작!");
                }
                
                Invoke(nameof(StopWaterHitting), 0.1f);
            }
            else
            {
                Debug.Log($"Victim {name}: WaterParticle 태그가 아님. 실제 태그: '{other.tag}'");
            }
        }
        
        private void StopWaterHitting()
        {
            _isWaterHitting = false;
        }
        
        private void ApplyWaterDamage()
        {
            if (_missionManager != null)
            {
                _missionManager.DamageVictimSafety(damagePerSecond);
                Debug.Log($"피해자 {name} 물 데미지: {damagePerSecond} (총 안정성 지수 감소)");
            }
            else
            {
                Debug.LogWarning($"피해자 {name}: FiremanMissionManager를 찾을 수 없습니다!");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isRescued) return;

            if (other.CompareTag("RightHand") || other.CompareTag("LeftHand"))
            {
                _isHandInside = false;
                _isMyHand = false; // 손 소유권 리셋
                
                // 구출 취소 이벤트 호출 (진도가 있었다면)
                if (_currentRescueTime > 0f)
                {
                    OnRescueCancelled?.Invoke();
                    Debug.Log("Rescue cancelled!");
                }
                
                _currentRescueTime = 0f;
                
                // UI 연결 해제
                DisconnectFromPlayerUI();
                
                Debug.Log("PlayerHand exited the victim trigger.");
            }
        }

        /// <summary>
        /// 손의 주인 플레이어를 찾아서 UI와 연결
        /// </summary>
        private void ConnectToPlayerUI(Transform handTransform)
        {
            // 이미 연결되어 있으면 무시
            if (_connectedUI != null) return;

            // 손의 루트(최상위 부모)에서 RescueProgressUI를 찾음
            Transform rootTransform = handTransform.root;
            _connectedUI = rootTransform.GetComponentInChildren<RescueProgressUI>();
            
            if (_connectedUI != null)
            {
                _connectedUI.RegisterVictimEvents(this);
                Debug.Log($"Victim {name}이(가) {rootTransform.name}의 UI에 연결되었습니다.");
            }
            else
            {
                Debug.LogWarning($"Victim {name}: {rootTransform.name}에서 RescueProgressUI를 찾을 수 없습니다!");
            }
        }

        /// <summary>
        /// 플레이어 UI와의 연결 해제
        /// </summary>
        private void DisconnectFromPlayerUI()
        {
            if (_connectedUI != null)
            {
                _connectedUI.UnregisterVictimEvents(this);
                Debug.Log($"Victim {name}이(가) UI 연결 해제되었습니다.");
                _connectedUI = null;
            }
        }

        private void Rescue()
        {
            // 네트워크로 구조 완료 동기화
            if (PhotonNetwork.IsConnected && photonView != null)
            {
                photonView.RPC("NetworkRescue", RpcTarget.All);
            }
            else
            {
                ExecuteRescue();
            }
        }
        
        private void ExecuteRescue()
        {
            _isRescued = true;
            
            // 구출 완료 이벤트 호출
            OnRescueCompleted?.Invoke();
            
            Debug.Log("Victim has been rescued!");

            // 애니메이션 재생
            if (_animator != null)
            {
                _animator.CrossFade(happyAnimationTrigger, 0.1f);
            }

            // 사운드 재생
            if (!string.IsNullOrEmpty(rescueSoundName))
            {
                Debug.Log("Playing rescue sound: " + rescueSoundName);
                // SoundManager.singleton.PlaySFX(rescueSoundName, transform.position);
            }

            // 그룹에 구조되었음을 알림
            _victimGroup?.NotifyVictimRescued(this);
            
            // UI 연결 해제
            DisconnectFromPlayerUI();

            // 짧은 딜레이 후 파괴
            Invoke(nameof(DestroyVictimAndSpawnParticle), 0.5f);
        }

        private void DestroyVictimAndSpawnParticle()
        {
            Vector3 particlePosition = transform.position + Vector3.up * 1f;
            
            Destroy(gameObject);
            
            if (rescueParticlePrefab != null)
            {
                var particle = Instantiate(rescueParticlePrefab, particlePosition, Quaternion.identity);
                
                var particleSystem = particle.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    var main = particleSystem.main;
                    float particleDuration = main.duration + main.startLifetime.constantMax;
                    Destroy(particle, particleDuration + 1f);
                }
                else
                {
                    Destroy(particle, 3f);
                }
            }
        }

        private void OnDestroy()
        {
            // 파괴될 때도 UI 연결 해제
            DisconnectFromPlayerUI();
        }
        
        // 네트워크 RPC 메서드들
        [PunRPC]
        private void NetworkRescue()
        {
            ExecuteRescue();
            Debug.Log("네트워크: 피해자 구조 동기화 완료");
        }
    }
}