using Oculus.Interaction;
using UnityEngine;

namespace Jobworld
{

	public class MonoBehaviourInteractableCallbacks : MonoBehaviour
	{
		[SerializeField, Interface(typeof(IOVRInteractableEventProvider))]
		private Object m_interactableEventProvider;
	
		public IOVRInteractableEventProvider interactableEventProvider { get; private set; }

		public void OnEnable()
		{
			if (m_interactableEventProvider != null)
			{
				interactableEventProvider = m_interactableEventProvider as IOVRInteractableEventProvider;
			
				interactableEventProvider.onHoverEntered += OnHoverEnter;
				interactableEventProvider.onHoverExited += OnHoverExit;
				interactableEventProvider.onSelectEntered += OnSelectEnter;
				interactableEventProvider.onSelectExited += OnSelectExit;
			}
		}

		protected virtual void OnHoverEnter() {}
	
		protected virtual void OnHoverExit() {}
		
		protected virtual void OnSelectEnter() {}
		
		protected virtual void OnSelectExit() {}
	}

}