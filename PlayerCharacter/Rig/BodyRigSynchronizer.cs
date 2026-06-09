using System;
using UnityEngine;

namespace Jobworld
{

	public class BodyRigSynchronizer : MonoBehaviour
	{
		[Serializable]
		public struct Offsets
		{
			public bool useWorldPosition;
			public Vector3 positionOffset;
			public Vector3 angleOffset;
		}
		
		public PlayerRig sourceRig;
		public PlayerRig targetRig;
		
		[Header("Offsets")]
		public Offsets rootOffsets;
		public Offsets headOffsets;
		public Offsets leftHandOffsets;
		public Offsets rightHandOffsets;

		private void Update()
		{
			if (sourceRig == null || targetRig == null) return;

			TryToCopyTransform(sourceRig.root, targetRig.root, rootOffsets);
			TryToCopyTransform(sourceRig.head, targetRig.head, headOffsets);
			TryToCopyTransform(sourceRig.leftHand, targetRig.leftHand, leftHandOffsets);
			TryToCopyTransform(sourceRig.rightHand, targetRig.rightHand, rightHandOffsets);
		}

		public void TryToCopyTransform(Transform source, Transform target, Offsets offset)
		{
			if (source != null && target != null)
			{
				Vector3 position = offset.useWorldPosition
					? source.position + offset.positionOffset
					: source.position + (source.rotation.normalized * offset.positionOffset);

				Quaternion rotation = source.rotation * Quaternion.Euler(offset.angleOffset);
				
				target.SetPositionAndRotation(position, rotation);
			}
		}
	}

}