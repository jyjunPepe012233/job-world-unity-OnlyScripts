using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using Photon.Pun;

namespace Jobworld
{
    public class FireSceneSpawner : MonoBehaviourPun
    {
        [System.Serializable]
        public class FireScenePrefab
        {
            [Header("Prefab Info")]
            public GameObject prefab;
            public FireSceneType sceneType;
            public string sceneName;
            
            [Header("Spawn Transform")]
            [Tooltip("월드에 배치된 빈 오브젝트를 참조하세요")]
            public Transform spawnTransform;
            
            [Header("Location Info")]
            public string locationName = "도심";
            public string description = "";
            public Transform destinationPoint;
            
            [Header("Props Toggle")]
            [Tooltip("평소 상태의 프랍들 (화재 시 비활성화)")]
            public GameObject normalProps;
            [Tooltip("화재 상태의 프랍들 (화재 시 활성화)")]
            public GameObject fireProps;
        }

        public enum FireSceneType
        {
            Small,
            Medium,
            Large
        }

        [Header("Fire Scene Prefabs")]
        [SerializeField] private List<FireScenePrefab> fireScenePrefabs = new List<FireScenePrefab>();

        private List<GameObject> _activeFireScenes = new List<GameObject>();
        private List<FireScenePrefab> _activeSceneConfigs = new List<FireScenePrefab>();

        public void SpawnRandomFireScene(int playerCount)
        {
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) return;
            
            ClearActiveScenes();

            var availableScenes = GetAvailableScenes(playerCount);
            if (availableScenes.Count == 0)
            {
                Debug.LogWarning($"No fire scenes available for {playerCount} players");
                return;
            }

            var selectedScene = availableScenes[Random.Range(0, availableScenes.Count)];
            
            // 네트워크로 씬 스폰 동기화
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("NetworkSpawnScene", RpcTarget.All, selectedScene.sceneName, playerCount);
            }
            else
            {
                SpawnSelectedScene(selectedScene, playerCount);
            }
        }

        public List<FireScenePrefab> GetAvailableScenes(int playerCount)
        {
            var availableScenes = new List<FireScenePrefab>();

            foreach (var scene in fireScenePrefabs)
            {
                switch (playerCount)
                {
                    case 1:
                        if (scene.sceneType == FireSceneType.Small)
                            availableScenes.Add(scene);
                        break;
                    case 2:
                        if (scene.sceneType == FireSceneType.Small || scene.sceneType == FireSceneType.Medium)
                            availableScenes.Add(scene);
                        break;
                    case 3:
                    default:
                        availableScenes.Add(scene);
                        break;
                }
            }

            return availableScenes;
        }

        public void SpawnFireSceneAtLocation(int playerCount, string locationName)
        {
            ClearActiveScenes();

            var availableScenes = GetAvailableScenes(playerCount);
            var locationScenes = availableScenes.FindAll(scene => 
                scene.locationName.Equals(locationName, System.StringComparison.OrdinalIgnoreCase));

            if (locationScenes.Count == 0)
            {
                Debug.LogWarning($"No fire scenes available for {playerCount} players at location: {locationName}");
                SpawnRandomFireScene(playerCount);
                return;
            }

            var selectedScene = locationScenes[Random.Range(0, locationScenes.Count)];
            SpawnSelectedScene(selectedScene, playerCount);
        }

        public void SpawnSpecificFireScene(string sceneName)
        {
            ClearActiveScenes();

            var selectedScene = fireScenePrefabs.Find(scene => 
                scene.sceneName.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase));

            if (selectedScene == null)
            {
                Debug.LogWarning($"Fire scene not found: {sceneName}");
                return;
            }

            SpawnSelectedScene(selectedScene, 1);
        }

        private void SpawnSelectedScene(FireScenePrefab selectedScene, int playerCount)
        {
            // Props 토글 (평소 끄고, 화재 켜기)
            ToggleProps(selectedScene, true);

            GameObject spawnedScene;
            
            // 마스터 클라이언트에서만 PhotonNetwork.Instantiate 사용
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                if (selectedScene.spawnTransform == null)
                {
                    Debug.LogWarning($"Spawn Transform이 설정되지 않았습니다: {selectedScene.sceneName}");
                    spawnedScene = PhotonNetwork.Instantiate(selectedScene.prefab.name, Vector3.zero, Quaternion.identity);
                }
                else
                {
                    spawnedScene = PhotonNetwork.Instantiate(selectedScene.prefab.name, 
                        selectedScene.spawnTransform.position, 
                        selectedScene.spawnTransform.rotation);
                }
            }
            else
            {
                // 네트워크 미연결 시 일반 Instantiate 사용
                if (selectedScene.spawnTransform == null)
                {
                    Debug.LogWarning($"Spawn Transform이 설정되지 않았습니다: {selectedScene.sceneName}");
                    spawnedScene = Instantiate(selectedScene.prefab);
                }
                else
                {
                    spawnedScene = Instantiate(selectedScene.prefab, 
                        selectedScene.spawnTransform.position, 
                        selectedScene.spawnTransform.rotation);
                }
            }

            _activeFireScenes.Add(spawnedScene);
            _activeSceneConfigs.Add(selectedScene);
            
            Debug.Log($"화재 현장 생성: {selectedScene.sceneName} ({selectedScene.locationName}) - {playerCount}명용");
            if (!string.IsNullOrEmpty(selectedScene.description))
            {
                Debug.Log($"상황 설명: {selectedScene.description}");
            }
        }

        private void ToggleProps(FireScenePrefab scene, bool isFireActive)
        {
            if (scene.normalProps != null)
            {
                scene.normalProps.SetActive(!isFireActive);
                Debug.Log($"평소 프랍: {(!isFireActive ? "활성화" : "비활성화")}");
            }

            if (scene.fireProps != null)
            {
                scene.fireProps.SetActive(isFireActive);
                Debug.Log($"화재 프랍: {(isFireActive ? "활성화" : "비활성화")}");
            }
        }

        private void RestoreAllPropsToNormal()
        {
            foreach (var sceneConfig in _activeSceneConfigs)
            {
                ToggleProps(sceneConfig, false);
            }
        }

        public List<string> GetAvailableLocations(int playerCount)
        {
            var availableScenes = GetAvailableScenes(playerCount);
            var locations = new List<string>();
            
            foreach (var scene in availableScenes)
            {
                if (!locations.Contains(scene.locationName))
                {
                    locations.Add(scene.locationName);
                }
            }
            
            return locations;
        }

        public FireScenePrefab GetCurrentSceneInfo()
        {
            if (_activeFireScenes.Count > 0)
            {
                var currentScene = _activeFireScenes[0];
                return fireScenePrefabs.Find(scene => scene.prefab.name == currentScene.name.Replace("(Clone)", ""));
            }
            return null;
        }

        public void ClearActiveScenes()
        {
            if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) return;
            
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("NetworkClearScenes", RpcTarget.All);
            }
            else
            {
                ExecuteClearScenes();
            }
        }
        
        private void ExecuteClearScenes()
        {
            RestoreAllPropsToNormal();

            foreach (var scene in _activeFireScenes)
            {
                if (scene != null)
                {
                    // PhotonNetwork로 생성된 오브젝트는 PhotonNetwork.Destroy 사용
                    var photonView = scene.GetComponent<PhotonView>();
                    if (photonView != null && PhotonNetwork.IsConnected)
                    {
                        PhotonNetwork.Destroy(scene);
                    }
                    else
                    {
                        Destroy(scene);
                    }
                }
            }
            _activeFireScenes.Clear();
            _activeSceneConfigs.Clear();
            
            Debug.Log("화재 현장 정리 완료 - 평소 상태로 복구");
        }

        private void OnDrawGizmosSelected()
        {
            if (fireScenePrefabs != null)
            {
                for (int i = 0; i < fireScenePrefabs.Count; i++)
                {
                    var scene = fireScenePrefabs[i];
                    if (scene.prefab != null && scene.spawnTransform != null)
                    {
                        switch (scene.sceneType)
                        {
                            case FireSceneType.Small:
                                Gizmos.color = Color.green;
                                break;
                            case FireSceneType.Medium:
                                Gizmos.color = Color.yellow;
                                break;
                            case FireSceneType.Large:
                                Gizmos.color = Color.red;
                                break;
                        }

                        Vector3 position = scene.spawnTransform.position;
                        Gizmos.DrawWireCube(position, Vector3.one * 2f);
                        Gizmos.DrawLine(position, position + Vector3.up * 3f);
                        
                        Vector3 forward = scene.spawnTransform.forward;
                        Gizmos.DrawRay(position, forward * 2f);

                        #if UNITY_EDITOR
                        UnityEditor.Handles.Label(position + Vector3.up * 4f, scene.sceneName);
                        #endif
                    }
                }
            }
        }
        
        // 네트워크 RPC 메서드들
        [PunRPC]
        private void NetworkSpawnScene(string sceneName, int playerCount)
        {
            var selectedScene = fireScenePrefabs.Find(scene => 
                scene.sceneName.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase));
                
            if (selectedScene != null)
            {
                SpawnSelectedScene(selectedScene, playerCount);
                Debug.Log($"네트워크: 화재 현장 스폰 동기화 완료 - {sceneName}");
            }
        }
        
        [PunRPC]
        private void NetworkClearScenes()
        {
            ExecuteClearScenes();
            Debug.Log("네트워크: 화재 현장 정리 동기화 완료");
        }
    }
}