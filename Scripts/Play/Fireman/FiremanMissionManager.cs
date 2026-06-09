using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using Photon.Pun;
//using Firebase.Auth;
using Random = UnityEngine.Random;

namespace Jobworld
{
    [Serializable]
    public class FiremanMissionLog
    {
        public int victimSafetyIndex = 100;
        public int damageManagementIndex = 100;
        public float fireExtinguishRatio = 0f;
        public float victimRescueRatio = 0f;
        public float completionTimePercent = 0f;
        public int resourceManagementIndex = 0;
        public float arrivalTimePercent = 0f;

        private float _waterSprayTime = 0f;
        private float _fireContactTime = 0f;

        public void AddWaterSprayTime(float time) => _waterSprayTime += time;
        public void AddFireContactTime(float time) => _fireContactTime += time;

        public void CalculateResourceManagement()
        {
            if (_fireContactTime > 0)
            {
                resourceManagementIndex = Mathf.RoundToInt((1f - (_waterSprayTime / _fireContactTime)) * 100f);
                resourceManagementIndex = Mathf.Clamp(resourceManagementIndex, 0, 100);
            }
        }

        public void LogResults()
        {
            Debug.Log($"=== 소방관 체험 로그 ===");
            Debug.Log($"피해자 안정성 지수: {victimSafetyIndex}");
            Debug.Log($"피해 관리 지수: {damageManagementIndex}");
            Debug.Log($"최종 소화 비율: {fireExtinguishRatio:F1}%");
            Debug.Log($"최종 구출 비율: {victimRescueRatio:F1}%");
            Debug.Log($"소화 완료 시간: {completionTimePercent:F1}%");
            Debug.Log($"소화 자원 관리 지수: {resourceManagementIndex}");
            Debug.Log($"도착 시간: {arrivalTimePercent:F1}%");
        }
    }

    public enum MissionType
    {
        Tutorial,
        Small,
        Medium,
        Large
    }

    public class FiremanMissionManager : MonoBehaviourPunCallbacks
    {
        [Header("Mission Settings")]
        public MissionType missionType = MissionType.Tutorial;
        public float goldenTime = 300f;
        
        [Header("Game Settings")]
        public int playerCount = 1;
        
        [Header("Tutorial Integration")]
        public bool autoDetectTutorial = true;
        
        [Header("Destination Settings")]
        [SerializeField] private bool requireArrival = true;
        
        [Header("Mission Cycle Settings")]
        [SerializeField] private float minRestTime = 120f;
        [SerializeField] private float maxRestTime = 300f;
        
        [Header("First Mission Settings")]
        [SerializeField] private float firstMissionDelay = 30f; // 첫 미션 시작 딜레이 (초)
        [SerializeField] private bool enableFirstMissionDelay = true; // 첫 미션 딜레이 활성화 여부
        
        [Header("Mission Rewards")]
        [SerializeField] private int smallMissionReward = 500;   // Small 미션 보상
        [SerializeField] private int mediumMissionReward = 1000; // Medium 미션 보상
        [SerializeField] private int largeMissionReward = 2000;  // Large 미션 보상

        private FireGroup[] _fireGroups;
        private VictimGroup[] _victimGroups;
        private FireSceneSpawner _fireSceneSpawner;
        private TutorialManager _tutorialManager;
        private DestinationSystem _destinationSystem;
        private PlayerTeleporter _playerTeleporter;
        private MinimapSystem _minimapSystem;

        private int _totalGroups = 0;
        private int _completedGroups = 0;
        private bool _missionCompleted = false;
        private bool _missionStarted = false;
        private bool _waitingForArrival = false;

        private float _missionStartTime;
        private float _currentTime;
        private bool _timerExpired = false;

        public FiremanMissionLog missionLog = new FiremanMissionLog();

        public event Action<float> OnTimerUpdate;
        public event Action OnMissionStart;
        public event Action OnMissionComplete;
        public event Action OnMissionFailed;
        public event Action OnArrivalRequired;
        public event Action OnArrivalComplete;
        public event Action<float> OnFirstMissionCountdown; // 첫 미션 카운트다운 이벤트
        
        public static event Action OnMissionStartGlobal;
        public static event Action OnMissionCompleteGlobal;
        public static event Action OnMissionFailedGlobal;

        private int _totalFires = 0;
        private int _extinguishedFires = 0;
        private int _totalVictims = 0;
        private int _rescuedVictims = 0;
        
        private bool _isCycleActive = false;
        private bool _isFirstMission = true;
        private bool _isInitialized = false;
        private string _lastSceneName = "";
        private Coroutine _nextMissionCoroutine;

        private void Start()
        {
            _tutorialManager = FindObjectOfType<TutorialManager>();
            _destinationSystem = FindObjectOfType<DestinationSystem>();
            _minimapSystem = FindObjectOfType<MinimapSystem>();

            if (autoDetectTutorial && _tutorialManager != null)
            {
                missionType = MissionType.Tutorial;
                Debug.Log("TutorialManager 감지: Tutorial 모드로 설정");
            }

            // 튜토리얼만 즉시 시작
            if (missionType == MissionType.Tutorial)
            {
                Debug.Log("튜토리얼 모드로 시작 - 기존 화재 현장 사용");
                InitializeMissionGroups();
                _isInitialized = true;
            }
            else
            {
                // 게임 미션은 OnJoinedRoom에서 시작
                _fireSceneSpawner = FindObjectOfType<FireSceneSpawner>();
                Debug.Log("게임 미션 모드 - Photon 방 입장 대기 중...");
            }
        }
        
        // Photon 콜백: 방 입장 완료 시 호출
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            
            // 이미 초기화됐거나 튜토리얼이면 무시
            if (_isInitialized || missionType == MissionType.Tutorial) return;
            
            Debug.Log($"✅ 방 입장 완료! IsMasterClient: {PhotonNetwork.IsMasterClient}");
            
            // 마스터 클라이언트만 미션 초기화
            if (PhotonNetwork.IsMasterClient)
            {
                _isInitialized = true;
                
                int playerCount = missionType == MissionType.Small ? 1 : 
                                 missionType == MissionType.Medium ? 2 : 3;
                string missionName = missionType == MissionType.Small ? "소형 미션" :
                                    missionType == MissionType.Medium ? "중형 미션" : "대형 미션";
                
                Debug.Log($"👑 [마스터] {missionName} 초기화");
                
                // 첫 미션 딜레이 적용
                if (enableFirstMissionDelay && _isFirstMission)
                {
                    Debug.Log($"👑 [마스터] 첫 미션 {firstMissionDelay}초 후 시작...");
                    StartCoroutine(StartFirstMissionWithDelay(playerCount));
                }
                else
                {
                    // 딜레이 없이 바로 시작
                    _isFirstMission = false;
                    
                    if (_fireSceneSpawner != null)
                    {
                        _fireSceneSpawner.SpawnRandomFireScene(playerCount);
                        InitializeAndStartMission();
                    }
                }
            }
            else
            {
                Debug.Log("👤 [클라이언트] 마스터의 명령 대기 중...");
            }
        }
        
        private IEnumerator StartFirstMissionWithDelay(int playerCount)
        {
            float remainingTime = firstMissionDelay;
            
            Debug.Log($"👑 [마스터] 카운트다운 시작: {firstMissionDelay}초");
            
            // 네트워크로 카운트다운 시작 알림
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("NetworkStartFirstMissionCountdown", RpcTarget.All, firstMissionDelay);
            }
            
            // 카운트다운
            while (remainingTime > 0)
            {
                Debug.Log($"👑 [마스터] 카운트다운: {remainingTime}초");
                
                // 네트워크로 카운트다운 동기화
                if (PhotonNetwork.IsConnected)
                {
                    photonView.RPC("NetworkFirstMissionCountdown", RpcTarget.All, remainingTime);
                }
                else
                {
                    OnFirstMissionCountdown?.Invoke(remainingTime);
                }
                
                yield return new WaitForSeconds(1f);
                remainingTime -= 1f;
            }
            
            Debug.Log($"👑 [마스터] 카운트다운 종료! 미션 시작!");
            
            // 딜레이 종료 후 첫 미션 시작
            _isFirstMission = false;
            
            if (_fireSceneSpawner != null)
            {
                _fireSceneSpawner.SpawnRandomFireScene(playerCount);
                InitializeAndStartMission();
            }
            else
            {
                InitializeMissionGroups();
            }
        }
        
        private void InitializeDestinationSystem()
        {
            if (_destinationSystem == null) return;
            
            _destinationSystem.UpdateDestination();
            
            if (requireArrival && missionType != MissionType.Tutorial)
            {
                _destinationSystem.OnDestinationArrived += OnDestinationArrived;
                _waitingForArrival = true;
                OnArrivalRequired?.Invoke();
                
                Debug.Log("목적지 도착 대기 중...");
            }
        }

        private void InitializeMissionGroups()
        {
            _fireGroups = FindObjectsOfType<FireGroup>();
            _totalGroups = _fireGroups.Length;
            _totalFires = 0;
            foreach (var group in _fireGroups)
            {
                group.OnAllFiresExtinguished += OnFireGroupCompleted;
                var fires = group.GetComponentsInChildren<Fire>();
                _totalFires += fires.Length;
            }

            _victimGroups = FindObjectsOfType<VictimGroup>();
            _totalGroups += _victimGroups.Length;
            _totalVictims = 0;
            foreach (var group in _victimGroups)
            {
                group.OnAllVictimsRescued += OnVictimGroupCompleted;
                var victims = group.GetComponentsInChildren<Victim>();
                _totalVictims += victims.Length;
            }

            Debug.Log($"미션 그룹 초기화 완료: FireGroup {_fireGroups.Length}개 (화재 {_totalFires}개), VictimGroup {_victimGroups.Length}개 (피해자 {_totalVictims}명), 총 그룹 {_totalGroups}개");

            if (_totalGroups == 0)
            {
                Debug.LogWarning("미션 그룹이 없어서 바로 완료 처리");
                CompleteMission();
                return;
            }

            if (!requireArrival || missionType == MissionType.Tutorial)
            {
                StartMission();
            }
        }
        
        private void InitializeAndStartMission()
        {
            if (_destinationSystem != null)
            {
                Invoke(nameof(InitializeDestinationSystem), 0.15f);
            }
            
            Invoke(nameof(InitializeMissionGroups), 0.2f);
            
            if (requireArrival)
            {
                Invoke(nameof(StartMissionTimer), 0.25f);
            }
        }
        
        private void StartMissionTimer()
        {
            _missionStartTime = Time.time;
    
            OnMissionStart?.Invoke();
            OnMissionStartGlobal?.Invoke();
        }
        
        private void OnDestinationArrived()
        {
            if (!_waitingForArrival) return;
            
            _waitingForArrival = false;
            
            if (_destinationSystem != null)
            {
                float arrivalTime = _destinationSystem.ArrivalTime;
                missionLog.arrivalTimePercent = (arrivalTime / goldenTime) * 100f;
                
                float remainingTime = goldenTime - arrivalTime;
                Debug.Log($"목적지 도착 완료! 도착 시간: {arrivalTime:F1}초 ({missionLog.arrivalTimePercent:F1}%)");
                Debug.Log($"남은 소방 활동 시간: {remainingTime:F1}초");
            }
            
            OnArrivalComplete?.Invoke();
        }

        private void StartMission()
        {
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) return;
            
            _missionStarted = true;
    
            if (!requireArrival || missionType == MissionType.Tutorial)
            {
                _missionStartTime = Time.time;
                
                // 네트워크로 미션 시작 알림
                if (PhotonNetwork.IsConnected)
                {
                    photonView.RPC("NetworkStartMission", RpcTarget.All, _missionStartTime);
                }
                else
                {
                    OnMissionStart?.Invoke();
                    OnMissionStartGlobal?.Invoke();
                }
            }
    
            Debug.Log($"소방관 활동 시작!");
        }

        private void Update()
        {
            if (missionType == MissionType.Tutorial) return;
            
            if (_missionStartTime == 0 || _missionCompleted) return;

            _currentTime = Time.time - _missionStartTime;
            float remainingTime = goldenTime - _currentTime;

            // 마스터 클라이언트에서만 시간 초과 체크
            if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            {
                if (remainingTime <= 0 && !_timerExpired)
                {
                    _timerExpired = true;
                    
                    if (_waitingForArrival)
                    {
                        Debug.Log("목적지 도착 실패! 골든타임 초과!");
                    }
                    
                    FailMission();
                }
            }

            // 모든 클라이언트에서 UI 업데이트
            OnTimerUpdate?.Invoke(remainingTime);
        }

        private void OnFireGroupCompleted()
        {
            Debug.Log("FireGroup 완료 이벤트 호출됨!");
            _extinguishedFires += _fireGroups.Where(g => g != null).SelectMany(g => g.GetComponentsInChildren<Fire>()).Count(f => f.IsExtinguished);
            OnGroupCompleted();
        }

        private void OnVictimGroupCompleted()
        {
            Debug.Log("VictimGroup 완료 이벤트 호출됨!");
            _rescuedVictims = _totalVictims - _victimGroups.Where(g => g != null).SelectMany(g => g.GetComponentsInChildren<Victim>()).Count();
            OnGroupCompleted();
        }

        private void OnGroupCompleted()
        {
            if (_missionCompleted) return;

            _completedGroups++;
            Debug.Log($"임무 그룹 완료 ({_completedGroups}/{_totalGroups}) - 미션 완료 여부: {_completedGroups >= _totalGroups}");

            if (_completedGroups >= _totalGroups)
            {
                Debug.Log("모든 그룹 완료! CompleteMission() 호출");
                CompleteMission();
            }
        }

        private void CompleteMission()
        {
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) return;
            
            _missionCompleted = true;
            float completionTime = Time.time - _missionStartTime;
            
            if (missionType != MissionType.Tutorial)
            {
                CalculateMissionLog(completionTime);
                missionLog.LogResults();
            }
            
            Debug.Log("모든 화재 진압 및 인명 구조 완료!");

            // 네트워크로 미션 완료 알림
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("NetworkCompleteMission", RpcTarget.All, completionTime);
            }
            else
            {
                ExecuteMissionComplete(completionTime);
            }
        }
        
        private void ExecuteMissionComplete(float completionTime)
        {
            if (missionType == MissionType.Tutorial && _tutorialManager != null)
            {
                Debug.Log("튜토리얼 다음 단계로 진행");
            }

            OnMissionComplete?.Invoke();
            
            if (_minimapSystem != null)
            {
                _minimapSystem.OnMissionCompleted();
            }
            
            if (missionType == MissionType.Tutorial)
            {
                CleanupMission();
            }
            else
            {
                Debug.Log("=== 미션 완료! 결과 요약 ===");
                missionLog.LogResults();
                
                // 미션 완료 보상 지급
                GiveReward();
                
                OnMissionCompleteGlobal?.Invoke();
                
                if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
                {
                    Debug.Log("미션 완료! 다음 사이클 시작");
                    StartMissionCycle();
                }
            }
        }

        private void FailMission()
        {
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) return;
            
            _missionCompleted = true;
            Debug.Log("골든타임 초과! 미션 실패!");
            
            CalculateMissionLog(goldenTime);
            
            Debug.Log("=== 미션 실패! 결과 요약 ===");
            missionLog.LogResults();

            // 네트워크로 미션 실패 알림
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("NetworkFailMission", RpcTarget.All);
            }
            else
            {
                OnMissionFailed?.Invoke();
                OnMissionFailedGlobal?.Invoke();
            }
            
            if (missionType == MissionType.Tutorial)
            {
                CleanupMission();
            }
            else
            {
                Debug.Log("미션 실패! 다음 사이클 시작");
                StartMissionCycle();
            }
        }

        private void CalculateMissionLog(float completionTime)
        {
            missionLog.fireExtinguishRatio = _totalFires > 0 ? (_extinguishedFires / (float)_totalFires) * 100f : 100f;
            missionLog.victimRescueRatio = _totalVictims > 0 ? (_rescuedVictims / (float)_totalVictims) * 100f : 100f;
            missionLog.completionTimePercent = (completionTime / goldenTime) * 100f;
            missionLog.CalculateResourceManagement();
        }

        private void StartMissionCycle()
        {
            Debug.Log($"StartMissionCycle 호출: _isCycleActive={_isCycleActive}");

            if (_isCycleActive)
            {
                Debug.Log("이미 사이클이 진행 중이므로 새로운 사이클 시작하지 않음");
                return;
            }

            _isCycleActive = true;
            Debug.Log("새로운 미션 사이클 시작!");
            
            if (_fireSceneSpawner != null)
            {
                var currentSceneInfo = _fireSceneSpawner.GetCurrentSceneInfo();
                if (currentSceneInfo != null)
                {
                    _lastSceneName = currentSceneInfo.sceneName;
                }
            }

            float restTime = Random.Range(minRestTime, maxRestTime);
            Debug.Log($"다음 화재 발생까지 {restTime / 60f:F1}분 대기...");

            _nextMissionCoroutine = StartCoroutine(StartNextMissionAfterDelay(restTime));
        }

        private IEnumerator StartNextMissionAfterDelay(float delay)
        {
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) yield break;
            
            if (_fireSceneSpawner != null)
            {
                _fireSceneSpawner.ClearActiveScenes();
                Debug.Log("화재 현장 정리: 평소 프랍으로 복구됨");
            }

            // 네트워크로 쉬는 시간 시작 알림
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("NetworkStartRestTime", RpcTarget.All, delay);
            }

            yield return new WaitForSeconds(delay);

            StartNewRandomMission();
        }

        private void StartNewRandomMission()
        {
            if (_fireSceneSpawner == null) return;

            ResetMissionVariables();

            int playerCount = missionType == MissionType.Small ? 1 : 
                             missionType == MissionType.Medium ? 2 : 3;
                             
            Debug.Log($"새로운 화재 발생! {playerCount}명용 미션 시작");

            SpawnDifferentFireScene(playerCount);
            
            InitializeAndStartMission();
        }

        private void SpawnDifferentFireScene(int playerCount)
        {
            var availableScenes = _fireSceneSpawner.GetAvailableScenes(playerCount);
            
            var differentScenes = availableScenes.FindAll(scene => scene.sceneName != _lastSceneName);
            
            if (differentScenes.Count == 0)
            {
                Debug.LogWarning("이전과 다른 화재 현장을 찾을 수 없어 랜덤 선택");
                _fireSceneSpawner.SpawnRandomFireScene(playerCount);
            }
            else
            {
                var selectedScene = differentScenes[Random.Range(0, differentScenes.Count)];
                _fireSceneSpawner.SpawnSpecificFireScene(selectedScene.sceneName);
                Debug.Log($"이전({_lastSceneName})과 다른 화재 현장 생성: {selectedScene.sceneName}");
            }
        }

        private void ResetMissionVariables()
        {
            CleanupPreviousMissionEvents();
            
            _totalGroups = 0;
            _completedGroups = 0;
            _missionCompleted = false;
            _missionStarted = false;
            _waitingForArrival = false;
            _missionStartTime = 0;
            _currentTime = 0;
            _timerExpired = false;
            _totalFires = 0;
            _extinguishedFires = 0;
            _totalVictims = 0;
            _rescuedVictims = 0;
            _isCycleActive = false;
            
            missionLog = new FiremanMissionLog();

            Debug.Log("미션 진행 변수 초기화 완료");
        }
        
        private void CleanupPreviousMissionEvents()
        {
            if (_fireGroups != null)
            {
                foreach (var group in _fireGroups)
                {
                    if(group != null) group.OnAllFiresExtinguished -= OnFireGroupCompleted;
                }
            }
            if (_victimGroups != null)
            {
                foreach (var group in _victimGroups)
                {
                    if(group != null) group.OnAllVictimsRescued -= OnVictimGroupCompleted;
                }
            }
            if (_destinationSystem != null)
            {
                _destinationSystem.OnDestinationArrived -= OnDestinationArrived;
            }
        }

        private void CleanupMission()
        {
            if (_nextMissionCoroutine != null)
            {
                StopCoroutine(_nextMissionCoroutine);
                _nextMissionCoroutine = null;
            }

            foreach (var group in _fireGroups)
            {
                if(group != null) group.OnAllFiresExtinguished -= OnFireGroupCompleted;
            }
            foreach (var group in _victimGroups)
            {
                if(group != null) group.OnAllVictimsRescued -= OnVictimGroupCompleted;
            }
            
            if (_destinationSystem != null)
            {
                _destinationSystem.OnDestinationArrived -= OnDestinationArrived;
            }
            
            if (missionType != MissionType.Tutorial && _fireSceneSpawner != null)
            {
                _fireSceneSpawner.ClearActiveScenes();
                Debug.Log("화재 현장 정리: 평소 프랍으로 복구됨");
            }

            if (missionType == MissionType.Tutorial)
            {
                enabled = false;
            }
        }

        public void DamageVictimSafety(int damage)
        {
            if (missionType != MissionType.Tutorial)
            {
                int oldValue = missionLog.victimSafetyIndex;
                missionLog.victimSafetyIndex = Mathf.Max(0, missionLog.victimSafetyIndex - damage);
                Debug.Log($"피해자 안정성 지수: {oldValue} → {missionLog.victimSafetyIndex} (데미지: {damage})");
            }
        }

        public void DamageDamageManagement(int damage)
        {
            if (missionType != MissionType.Tutorial)
            {
                int oldValue = missionLog.damageManagementIndex;
                missionLog.damageManagementIndex = Mathf.Max(0, missionLog.damageManagementIndex - damage);
                Debug.Log($"피해 관리 지수: {oldValue} → {missionLog.damageManagementIndex} (데미지: {damage})");
            }
        }

        public void AddWaterSprayTime(float time)
        {
            if (missionType != MissionType.Tutorial)
            {
                missionLog.AddWaterSprayTime(time);
            }
        }

        public void AddFireContactTime(float time)
        {
            if (missionType != MissionType.Tutorial)
            {
                missionLog.AddFireContactTime(time);
            }
        }
        
        private void GiveReward()
        {
            int rewardAmount = GetMissionReward();
            if (rewardAmount <= 0) return;
            
            // 모든 플레이어에게 보상 지급 RPC 전송
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("123123123");
                photonView.RPC("NetworkGiveReward", RpcTarget.All, rewardAmount);
            }
            else
            {
                // 네트워크 미연결 시 로컬에서만 처리
                ExecuteGiveReward(rewardAmount);
            }
        }
        
        private async void ExecuteGiveReward(int rewardAmount)
        {
//            Debug.Log($"=== ExecuteGiveReward 시작 ===");
//            Debug.Log($"보상 금액: {rewardAmount}");
//            Debug.Log($"현재 플랫폼: {Application.platform}");
//            Debug.Log($"Unity Android 빌드: {Application.platform == RuntimePlatform.Android}");
//            
//            try
//            {
//                // 현재 달러 가져오기
//                int currentDollars = GetCurrentDollars();
//                int newAmount = currentDollars + rewardAmount;
//                
//                Debug.Log($"현재 달러: {currentDollars}");
//                Debug.Log($"새로운 달러: {newAmount}");
//                
//                // GuestClientLocalInfoHolder 상태 확인
//                Debug.Log($"GuestClientLocalInfoHolder.playData != null: {GuestClientLocalInfoHolder.playData != null}");
//                if (GuestClientLocalInfoHolder.playData != null)
//                {
//                    Debug.Log($"PlayData AuthId: {GuestClientLocalInfoHolder.playData.authId}");
//                    Debug.Log($"PlayData Dollar: {GuestClientLocalInfoHolder.playData.dollar}");
//                }
//                
//                // GuestClientLocalInfoHolder가 세션에 연결되었는지 확인
//                Debug.Log($"GuestClientLocalInfoHolder.isJoined: {GuestClientLocalInfoHolder.isJoined}");
//                Debug.Log($"GuestClientLocalInfoHolder.sessionName: {GuestClientLocalInfoHolder.sessionName}");
//                
//                // Firebase 사용자 정보 디버깅
////                var firebaseAuth = FirebaseAuth.DefaultInstance;
//                if (firebaseAuth != null && firebaseAuth.CurrentUser != null)
//                {
//                    string firebaseUserId = firebaseAuth.CurrentUser.UserId;
//                    Debug.Log($"Firebase UID: {firebaseUserId}");
//                    Debug.Log($"LocalAuth ID: {LocalAuthorizationHolder.id}");
//                }
//                else
//                {
//                    Debug.LogWarning("Firebase CurrentUser is null!");
//                }
//                Debug.Log("Android 빌드에서 GuestClientService.SetDollar 호출 시도");
//                
//                // Android에서는 GuestClientService 사용
//                if (true)
//                {
//                    Debug.Log("GuestClientService.SetDollar 호출 중...");
//                    await GuestClientService.SetDollar(newAmount);
//                    Debug.Log("GuestClientService.SetDollar 호출 완료!");
//                }
//            }
//            catch (System.Exception e)
//            {
//                Debug.LogError($"보상 지급 실패: {e.Message}");
//                Debug.LogError($"StackTrace: {e.StackTrace}");
//                Debug.LogError($"플레이어 ID: {GuestClientLocalInfoHolder.playData?.authId}");
//                Debug.LogError($"현재 플랫폼: {Application.platform}");
//                
//                // Firebase 사용자 정보 디버깅
//                var firebaseAuth = FirebaseAuth.DefaultInstance;
//                if (firebaseAuth != null && firebaseAuth.CurrentUser != null)
//                {
//                    Debug.LogError($"Firebase UID: {firebaseAuth.CurrentUser.UserId}");
//                }
//                else
//                {
//                    Debug.LogError("Firebase CurrentUser is null!");
//                }
//            }
//            
//            Debug.Log($"=== ExecuteGiveReward 종료 ===");
        }
        
        private int GetMissionReward()
        {
            return missionType switch
            {
                MissionType.Small => smallMissionReward,
                MissionType.Medium => mediumMissionReward,
                MissionType.Large => largeMissionReward,
                MissionType.Tutorial => 0, // 튜토리얼은 보상 없음
                _ => 0
            };
        }
        
        private int GetCurrentDollars()
        {
            if (GuestClientLocalInfoHolder.playData != null)
            {
                return GuestClientLocalInfoHolder.playData.dollar;
            }
            return 0;
        }

        public float GetRemainingTime()
        {
            if (_missionStartTime == 0) return goldenTime;
            return Mathf.Max(0, goldenTime - (Time.time - _missionStartTime));
        }

        public float GetElapsedTime()
        {
            if (_missionStartTime == 0) return 0;
            return Time.time - _missionStartTime;
        }
        
        public bool IsWaitingForArrival()
        {
            return _waitingForArrival;
        }
        
        // 네트워크 RPC 메서드들
        [PunRPC]
        private void NetworkStartMission(float startTime)
        {
            _missionStartTime = startTime;
            _missionStarted = true;
            
            OnMissionStart?.Invoke();
            OnMissionStartGlobal?.Invoke();
            
            Debug.Log("네트워크: 미션 시작 동기화 완료");
        }
        
        [PunRPC]
        private void NetworkCompleteMission(float completionTime)
        {
            _missionCompleted = true;
            ExecuteMissionComplete(completionTime);
            Debug.Log("네트워크: 미션 완료 동기화 완료");
        }
        
        [PunRPC]
        private void NetworkFailMission()
        {
            _missionCompleted = true;
            
            OnMissionFailed?.Invoke();
            OnMissionFailedGlobal?.Invoke();
            
            Debug.Log("네트워크: 미션 실패 동기화 완료");
        }
        
        [PunRPC]
        private void NetworkStartFirstMissionCountdown(float totalTime)
        {
            Debug.Log($"네트워크: 첫 미션 카운트다운 시작 - {totalTime}초");
        }
        
        [PunRPC]
        private void NetworkFirstMissionCountdown(float remainingTime)
        {
            OnFirstMissionCountdown?.Invoke(remainingTime);
            Debug.Log($"네트워크: 첫 미션 카운트다운 - {remainingTime}초");
        }
        
        [PunRPC]
        private void NetworkStartRestTime(float restTime)
        {
            Debug.Log($"네트워크: 미션 쉬는 시간 시작 - {restTime / 60f:F1}분");
        }
        
        [PunRPC]
        private void NetworkGiveReward(int rewardAmount)
        {
            ExecuteGiveReward(rewardAmount);
            Debug.Log("네트워크: 미션 보상 동기화 완료");
        }
    }
}