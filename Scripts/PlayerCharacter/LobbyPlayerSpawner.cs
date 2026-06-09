using Oculus.Interaction;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Jobworld
{

	public class LobbyPlayerSpawner : MonoBehaviour
	{
		public string prefabName = "LobbyPlayer";

		[Optional]
		public string spawnFxPrefab = "FX_PlayerSpawn";

		public Transform[] spawnPointCandidates;

		private int m_lastSpawnPoint = -1;

		public void Start()
		{
			#if !UNITY_ANDROID
			return;
			#endif
			
			if (spawnPointCandidates.Length > 0)
			{
				var spawnPoint = ComputeSpawnPoint();
				PhotonNetwork.Instantiate(prefabName, spawnPoint.position, spawnPoint.rotation);
				if (!string.IsNullOrEmpty(spawnFxPrefab))
				{
					PhotonNetwork.Instantiate(spawnFxPrefab, spawnPoint.position, Quaternion.identity);
				}
			}
			else
			{
				Debug.LogError("LobbyPlayerSpawner: Spawn Point Candidates가 설정되지 않음.");
			}
		}

		private Transform ComputeSpawnPoint()
		{
			var randomIndex = Random.Range(0, spawnPointCandidates.Length);;
			while (spawnPointCandidates.Length > 1 && randomIndex == m_lastSpawnPoint)
			{
				randomIndex = Random.Range(0, spawnPointCandidates.Length);;
				m_lastSpawnPoint = randomIndex;
			}
			var spawnPoint = spawnPointCandidates[randomIndex]; 
			return spawnPoint;
		}
	}
}