using UnityEngine;

namespace Jobworld
{

	public class PlayerRig : MonoBehaviour
	{
		[SerializeField] private Transform m_root;

		public Transform root => m_root;
		
		[SerializeField] private Transform m_head;

		public Transform head => m_head;

		[SerializeField] private Transform m_leftHand;

		public Transform leftHand => m_leftHand;

		[SerializeField] private Transform m_rightHand;

		public Transform rightHand => m_rightHand;
	}

}