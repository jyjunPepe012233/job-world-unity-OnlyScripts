using UnityEngine;
using Photon.Pun;

namespace Jobworld
{
    public class Fire : MonoBehaviourPun
    {
        [Header("Settings")]
        public float fireHealth = 3f;
        public float smokeDuration = 2f;
        public float smokeStopDelay = 0.2f; // 물을 안 맞은 후 연기 멈추는 시간

        [Header("Particles")]
        public ParticleSystem fireParticles;
        public ParticleSystem smokeParticles;

        private bool _isExtinguished = false;
        private float _lastWaterHitTime = -1f;
        private PlayerFireDamageDetector _damageDetector;
        
        public bool IsExtinguished => _isExtinguished;
        
        private void Start()
        {
            // 메인 콜라이더는 파티클 충돌용으로 설정 (isTrigger = false)
            var mainCollider = GetComponent<Collider>();
            if (mainCollider != null)
            {
                mainCollider.isTrigger = false; // 파티클 충돌 감지용
            }
            
            // PlayerFireDamageDetector 자동 추가 또는 기존 컴포넌트 가져오기
            _damageDetector = GetComponent<PlayerFireDamageDetector>();
            if (_damageDetector == null)
            {
                _damageDetector = gameObject.AddComponent<PlayerFireDamageDetector>();
            }
        }
        
        private void OnParticleCollision(GameObject other)
        {
            // 마스터 클라이언트에서만 파티클 충돌 처리 (네트워크 연결 시)
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient) return;
            
            if (other.CompareTag("WaterParticle"))
            {
                // FireHose에서 데미지 배율 가져오기
                float damageMultiplier = 1f;
                var fireHose = other.GetComponentInParent<FireHose>();
                if (fireHose != null)
                {
                    damageMultiplier = fireHose.GetDamageMultiplier();
                }
                
                Douse(Time.deltaTime * damageMultiplier);
            }
        }

        private void Update()
        {
            // 물을 맞지 않은 지 일정 시간이 지나면 연기 멈춤 (마스터 클라이언트에서만 처리)
            if ((PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected) && !_isExtinguished && _lastWaterHitTime > 0 && 
                Time.time - _lastWaterHitTime > smokeStopDelay)
            {
                if (smokeParticles != null && smokeParticles.isPlaying)
                {
                    if (PhotonNetwork.IsConnected && photonView != null)
                    {
                        photonView.RPC("StopSmokeEffect", RpcTarget.All);
                    }
                    else
                    {
                        StopSmokeEffect(); // 로컬에서만 실행
                    }
                }
                _lastWaterHitTime = -1f; // 리셋
            }
        }

        private void Douse(float amount)
        {
            if (_isExtinguished) return;

            // 물 맞는 시간 기록
            _lastWaterHitTime = Time.time;

            // 물 맞는 동안 연기 재생 (네트워크 동기화)
            if (smokeParticles != null && !smokeParticles.isPlaying)
            {
                if (PhotonNetwork.IsConnected && photonView != null)
                {
                    photonView.RPC("PlaySmokeEffect", RpcTarget.All);
                }
                else
                {
                    PlaySmokeEffect(); // 로컬에서만 실행
                }
            }

            fireHealth -= amount;
            if (fireHealth <= 0)
            {
                // 불 끄기는 네트워크로 동기화
                if (PhotonNetwork.IsConnected && photonView != null)
                {
                    photonView.RPC("NetworkExtinguish", RpcTarget.All);
                }
                else
                {
                    Extinguish(); // 로컬에서만 실행
                }
            }
        }

        private void Extinguish()
        {
            _isExtinguished = true;

            if (fireParticles != null && fireParticles.isPlaying)
                fireParticles.Stop();

            if (smokeParticles != null && smokeParticles.isPlaying)
            {
                smokeParticles.Stop();
            }

            // PlayerFireDamageDetector 비활성화 (불이 꺼지면 데미지 없음)
            if (_damageDetector != null)
            {
                _damageDetector.enabled = false;
            }

            var col = GetComponent<Collider>();
            if (col != null)
                col.enabled = false;

            Destroy(gameObject, smokeDuration);
        }

        // 네트워크 RPC 메서드들
        [PunRPC]
        private void PlaySmokeEffect()
        {
            if (smokeParticles != null && !smokeParticles.isPlaying)
            {
                smokeParticles.Play();
            }
        }

        [PunRPC]
        private void StopSmokeEffect()
        {
            if (smokeParticles != null && smokeParticles.isPlaying)
            {
                smokeParticles.Stop();
            }
        }

        [PunRPC]
        private void NetworkExtinguish()
        {
            Extinguish();
        }
    }
}