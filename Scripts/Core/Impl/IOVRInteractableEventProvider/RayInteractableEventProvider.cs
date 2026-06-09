using System;
using Oculus.Interaction;
using UnityEngine;

namespace Jobworld
{

	public class RayInteractableEventProvider : MonoBehaviour, IOVRInteractableEventProvider
	{
		[SerializeField] private RayInteractable m_rayInteractable;

		public event Action onHoverEntered;

		public event Action onHoverExited;

		public event Action onSelectEntered;

		public event Action onSelectExited;

		public void OnEnable()
		{
			m_rayInteractable.WhenInteractorAdded.Action += OnHoverEnter;
			m_rayInteractable.WhenInteractorRemoved.Action += OnHoverExit;

			m_rayInteractable.WhenSelectingInteractorAdded.Action += OnSelectEnter;
			m_rayInteractable.WhenSelectingInteractorRemoved.Action += OnSelectExit;
		}
		
		public void OnDisable()
		{
			m_rayInteractable.WhenInteractorAdded.Action -= OnHoverEnter;
			m_rayInteractable.WhenInteractorRemoved.Action -= OnHoverExit;

			m_rayInteractable.WhenSelectingInteractorAdded.Action -= OnSelectEnter;
			m_rayInteractable.WhenSelectingInteractorRemoved.Action -= OnSelectExit;
		}

		protected virtual void OnHoverEnter(RayInteractor interactor)
		{
			onHoverEntered?.Invoke();
		}

		protected virtual void OnHoverExit(RayInteractor interactor)
		{
			onHoverExited?.Invoke();
		}
    
		protected virtual void OnSelectEnter(RayInteractor interactor)
		{
			onSelectEntered?.Invoke();
		}

		protected virtual void OnSelectExit(RayInteractor interactor)
		{
			onSelectExited?.Invoke();
		}
	}

}