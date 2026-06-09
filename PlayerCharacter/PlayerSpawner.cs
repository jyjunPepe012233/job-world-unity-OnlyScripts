using Photon.Pun;
using UnityEngine;

namespace Jobworld
{

	public class PlayerSpawner : MonoBehaviour
	{
		[SerializeField] private string m_prefabName = "Player";
		
		public void Start()
		{ 
			PhotonNetwork.Instantiate(m_prefabName, transform.position, transform.rotation);
		}
	}

}