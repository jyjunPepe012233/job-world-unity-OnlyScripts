using UnityEngine;
using Oculus.Interaction;
using Photon.Pun;

namespace Jobworld
{
    [RequireComponent(typeof(Grabbable))]
    public class FireHose : Tool, ILevelingObject, IPunObservable
    {
        [Header("Water Particles")]
        public ParticleSystem sprayParticles;  // 넓게 뿌리는 스프레이
        public ParticleSystem streamParticles; // 집중 분사 스트림

        [Header("Mode Settings")]
        public WaterMode currentMode = WaterMode.Spray;
        public float streamDamageMultiplier = 2f; // 스트림 모드 데미지 배율

        [Header("Leveling Settings")]
        [SerializeField] private LevelingObjectSetting m_levelingSetting;
        [SerializeField] private int m_currentLevel = 1;

        private bool _isUsing = false;
        private FiremanMissionManager _missionManager;
        private ParticleSystem _currentParticles;
        private PlayerLevelingManager _levelingManager;
        
        // Photon 동기화용
        private PhotonView _photonView;
        private Vector3 _networkPosition;
        private Quaternion _networkRotation;
        private bool _networkIsUsing;
        private WaterMode _networkMode;

        public enum WaterMode
        {
            Spray,   // 일반 스프레이
            Stream   // 집중 스트림 (2배 데미지)
        }

        // ILevelingObject 구현
        public LevelingObjectSetting levelingSetting => m_levelingSetting;
        public int currentLevel => m_currentLevel;

        private void Start()
        {
            if (sprayParticles != null)
                sprayParticles.Stop();
            
            if (streamParticles != null)
                streamParticles.Stop();
            
            // 초기 모드 설정
            _currentParticles = currentMode == WaterMode.Spray ? sprayParticles : streamParticles;
            
            _missionManager = FindObjectOfType<FiremanMissionManager>();
            
            // PlayerLevelingManager에 등록
            _levelingManager = FindObjectOfType<PlayerLevelingManager>();
            if (_levelingManager != null)
            {
                Debug.Log("FireHose가 PlayerLevelingManager에 등록되었습니다.");
                _levelingManager.AddLevelingObject(this);
            }
            
            // Grabbable 이벤트 구독
            if (Grabbable != null)
            {
                Grabbable.WhenPointerEventRaised += OnPointerEvent;
            }
            
            // PhotonView 컴포넌트 가져오기
            _photonView = GetComponent<PhotonView>();
            if (_photonView == null)
            {
                Debug.LogWarning("FireHose에 PhotonView 컴포넌트가 없습니다!");
            }
            
            // 네트워크 초기값 설정
            _networkPosition = transform.position;
            _networkRotation = transform.rotation;
            _networkIsUsing = _isUsing;
            _networkMode = currentMode;
        }
        
        private void OnDestroy()
        {
            // PlayerLevelingManager에서 제거
            if (_levelingManager != null)
            {
                _levelingManager.RemoveLevelingObject(this);
            }
            
            // 이벤트 구독 해제
            if (Grabbable != null)
            {
                Grabbable.WhenPointerEventRaised -= OnPointerEvent;
            }
        }
        
        private void OnPointerEvent(PointerEvent evt)
        {
            if (evt.Type == PointerEventType.Select)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);  // 로컬 회전
            }
        }

        private void Update()
        {
            // 물 사용 시간 추적
            if (_isUsing && _missionManager != null)
            {
                _missionManager.AddWaterSprayTime(Time.deltaTime);
            }

            // 내가 소유한 경우에만 입력 처리
            if (_photonView != null && _photonView.IsMine)
            {
                // 컨트롤러로 잡고 있는지 확인
                bool isGrabbed = Grabbable != null && Grabbable.SelectingPointsCount > 0;

                if (isGrabbed)
                {
                    // 모드 전환 (A/X 버튼)
                    if (OVRInput.GetDown(OVRInput.RawButton.B) || 
                        OVRInput.GetDown(OVRInput.RawButton.Y))
                    {
                        ToggleMode();
                    }

                    // 인덱스 트리거 입력 감지
                    if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) ||
                        OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
                    {
                        StartUsing();
                    }
                    else if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger) ||
                             OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
                    {
                        StopUsing();
                    }
                }
                else
                {
                    // 놓으면 자동 정지
                    StopUsing();
                }
            }
            else
            {
                // 네트워크에서 받은 데이터로 업데이트
                UpdateFromNetwork();
            }
        }

        private void ToggleMode()
        {
            // 사용 중이면 먼저 정지
            if (_isUsing)
            {
                StopUsing();
            }

            // 모드 전환
            currentMode = currentMode == WaterMode.Spray ? WaterMode.Stream : WaterMode.Spray;
            _currentParticles = currentMode == WaterMode.Spray ? sprayParticles : streamParticles;
            
            Debug.Log($"물 모드 전환: {currentMode}");
        }

        private void StartUsing()
        {
            if (_currentParticles != null && !_isUsing)
            {
                _currentParticles.Play();
                _isUsing = true;
            }
        }

        private void StopUsing()
        {
            if (_currentParticles != null && _isUsing)
            {
                _currentParticles.Stop();
                _isUsing = false;
            }
        }

        // Fire 스크립트에서 데미지 배율 확인용
        public float GetDamageMultiplier()
        {
            float baseDamage = currentMode == WaterMode.Stream ? streamDamageMultiplier : 1f;
            float levelMultiplier = GetLevelDamageMultiplier();
            return baseDamage * levelMultiplier;
        }

        // ILevelingObject 메서드 구현
        public int GetLevel()
        {
            return m_currentLevel;
        }

        public void SetLevel(int level)
        {
            if (level >= 1 && m_levelingSetting != null && level <= m_levelingSetting.MaxLevel)
            {
                m_currentLevel = level;
                Debug.Log($"소방 호스 레벨 {m_currentLevel}로 업그레이드!");
            }
        }

        private float GetLevelDamageMultiplier()
        {
            // 레벨당 10% 데미지 증가
            return 1f + (m_currentLevel - 1) * 0.1f;
        }

        // 네트워크에서 받은 데이터로 업데이트
        private void UpdateFromNetwork()
        {
            // 위치와 회전 보간
            transform.position = Vector3.Lerp(transform.position, _networkPosition, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, _networkRotation, Time.deltaTime * 10f);
            
            // 사용 상태 동기화
            if (_networkIsUsing != _isUsing)
            {
                if (_networkIsUsing)
                {
                    _isUsing = true;
                    if (_currentParticles != null && !_currentParticles.isPlaying)
                        _currentParticles.Play();
                }
                else
                {
                    _isUsing = false;
                    if (_currentParticles != null && _currentParticles.isPlaying)
                        _currentParticles.Stop();
                }
            }
            
            // 모드 동기화
            if (_networkMode != currentMode)
            {
                currentMode = _networkMode;
                _currentParticles = currentMode == WaterMode.Spray ? sprayParticles : streamParticles;
            }
        }

        // IPunObservable 구현
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // 데이터 전송
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(_isUsing);
                stream.SendNext((int)currentMode);
            }
            else
            {
                // 데이터 수신
                _networkPosition = (Vector3)stream.ReceiveNext();
                _networkRotation = (Quaternion)stream.ReceiveNext();
                _networkIsUsing = (bool)stream.ReceiveNext();
                _networkMode = (WaterMode)stream.ReceiveNext();
            }
        }
    }
}