using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Jobworld
{

	public sealed class SimpleInteractableCallbacks : MonoBehaviourInteractableCallbacks
	{
		public UnityEvent onHoverEntered;

		public UnityEvent onHoverExited;

		public UnityEvent onSelectEntered;

		public UnityEvent onSelectExited;

		protected override void OnHoverEnter()
		{
			onHoverEntered?.Invoke();
		}

		protected override void OnHoverExit()
		{
			onHoverExited?.Invoke();
		}

		protected override void OnSelectEnter()
		{
			onSelectEntered?.Invoke();
		}

		protected override void OnSelectExit()
		{
			onSelectExited?.Invoke();
		}
	}

}