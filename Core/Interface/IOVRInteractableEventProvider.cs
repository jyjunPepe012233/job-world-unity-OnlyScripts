using System;

namespace Jobworld
{

	public interface IOVRInteractableEventProvider
	{
		event Action onHoverEntered;
		
		event Action onHoverExited;
		
		event Action onSelectEntered;
		
		event Action onSelectExited;
	}

}