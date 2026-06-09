using UnityEngine;

namespace Jobworld
{

	public class PlayerAvatarIKController : MonoBehaviour
	{
		[Header("Targets")]
		public Transform characterRoot;

		[Space]
		public Transform rootIkTarget;
		
		public Transform spineIkTarget;

		public Transform leftArmIkTarget;

		public Transform rightArmIkTarget;

		[Header("Offsets")]
		[Range(0, 1.2f)]
		public float hipsDepth = 0.8f;

		public Vector3 hipsRotationOffset;

		public Vector3 headLocalPositionOffset = Vector3.zero;

		public Vector3 headRotationOffset;

		public Vector3 leftHandLocalPositionOffset = Vector3.zero;

		public Vector3 leftHandRotationOffset;

		public Vector3 rightHandLocalPositionOffset = Vector3.zero;

		public Vector3 rightHandRotationOffset;

		private PlayerRig m_sourceRig;

		public PlayerRig sourceRig
		{
			get => m_sourceRig;
			set => m_sourceRig = value;
		}

		public void Update()
		{
			if (sourceRig == null) return;
			
			characterRoot.SetPositionAndRotation(
				m_sourceRig.root.position,
				m_sourceRig.root.rotation
			);

			rootIkTarget.SetPositionAndRotation(
				m_sourceRig.head.position - (Vector3.up * hipsDepth) + (m_sourceRig.head.rotation.normalized * headLocalPositionOffset),
				Quaternion.LookRotation(GetParallelVector(m_sourceRig.head.forward)) * Quaternion.Euler(hipsRotationOffset)
			);

			spineIkTarget.SetPositionAndRotation(
				m_sourceRig.head.position + (m_sourceRig.head.rotation.normalized * headLocalPositionOffset),
				m_sourceRig.head.rotation * Quaternion.Euler(headRotationOffset)
			);

			leftArmIkTarget.SetPositionAndRotation(
				m_sourceRig.leftHand.position + (m_sourceRig.leftHand.rotation.normalized * leftHandLocalPositionOffset),
				m_sourceRig.leftHand.rotation * Quaternion.Euler(leftHandRotationOffset)
			);

			rightArmIkTarget.SetPositionAndRotation(
				m_sourceRig.rightHand.position + (m_sourceRig.rightHand.rotation.normalized * rightHandLocalPositionOffset),
				m_sourceRig.rightHand.rotation * Quaternion.Euler(rightHandRotationOffset)
			);

		}

		private Vector3 GetParallelVector(Vector3 vector)
		{
			vector.y = 0;
			return vector;
		}
	}

}