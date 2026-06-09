using System.Collections.Generic;
using UnityEngine;

namespace Jobworld
{

	public class ComponentLocator : MonoBehaviourSingleton<ComponentLocator>
	{
		private Dictionary<string, ILocatableComponent> m_worldLocatables;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		protected static void InitializeOnLoad()
		{
			InitializeSingleton();
		}
		
		public T Get<T>() where T : MonoBehaviour, ILocatableComponent
		{
			if (m_worldLocatables == null) m_worldLocatables = new();
			
			string key = typeof(T).Name;
			if (!m_worldLocatables.ContainsKey(key) || m_worldLocatables[key] == null)
			{
				m_worldLocatables[key] = FindObjectOfType<T>();
			}
			
			ILocatableComponent result = m_worldLocatables[key];
			if (result != null)
			{
				return result.gameObjectSource.GetComponent<T>();
			}
			else
			{
				return null;
			}
		}
	}

}