using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Jobworld
{

	public class NicknameLabel : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI m_nicknameTmp;

		public Transform overrideSource;

		public Vector3 offset;

		public void LateUpdate()
		{
			transform.position = overrideSource.position + offset;

			transform.forward = Camera.main.transform.position - transform.position;
		}
		
		public void SetNickname(string nickname)
		{
			m_nicknameTmp.text = nickname;
		}
	}

}