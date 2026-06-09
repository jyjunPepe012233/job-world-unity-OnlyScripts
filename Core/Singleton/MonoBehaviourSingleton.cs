using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jobworld
{
	
	public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
	{
		private static T m_singleton;

		public static T singleton
		{
			get
			{
				if (m_singleton == null)
				{
					InitializeSingleton();
					Debug.Log($"{typeof(T).Name}이(가) 생성되지 않은 시점에서 싱글톤이 호출되어 임의로 생성함.");
				}

				return m_singleton;
			}
		}

		/// <summary>
		/// (RuntimeInitializeOnLoadMethod 어트리뷰트가 부착된 메서드에서 호출되어야 함)
		/// 게임 시작 시 싱글톤 객체를 자동 생성함.
		/// </summary>
		public static void InitializeSingleton()
		{
			T[] instances = FindObjectsOfType<T>();
			
			// 씬에 싱글톤 후보 객체가 존재한다면,
			// 첫 번째 객체를 싱글톤으로 만들고 나머지 객체는 제거
			if (instances.Length != 0)
			{
				ManageInstancesInThisScene();
			} 
			else
			{
				// 씬에 싱글톤 후보 객체가 없다면, 새로운 객체를 생성
				AssignSingletonIntoPublicVariable();
				Debug.Log(typeof(T).Name + "의 싱글톤 등록(새로운 객체 생성)");
			}
		}

		/// <summary>
		/// 싱글톤 저장 위치에 새로운 싱글톤 객체를 할당시킴
		/// </summary>
		private static void AssignSingletonIntoPublicVariable()
		{
			MakeInstanceToSingleton(new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>());
		}
		
		/// <summary>
		/// 객체를 싱글톤으로 만듬
		/// </summary>
		private static void MakeInstanceToSingleton(T instance)
		{
			m_singleton = instance;
			
			DontDestroyOnLoad(m_singleton);
			SceneManager.activeSceneChanged += m_singleton.OnActiveSceneChanged;
		}

		/// <summary>
		/// 씬이 변경될 때 해당 씬에 존재하는 동일한 타입의 오브젝트를 관리
		/// </summary>
		private static void ManageInstancesInThisScene()
		{
			T[] instances = FindObjectsOfType<T>();
			
			// 싱글톤이 존재하지 않는다면, 씬에 존재하는 동일한 타입의 오브젝트 중 첫 번째 오브젝트를 싱글톤으로 등록
			if (m_singleton == null)
			{ 
				MakeInstanceToSingleton(instances[0]);
			}
			
			// 싱글톤으로 등록되지 않은 나머지 객체는 모두 제거
			foreach (T instance in instances)
			{
				if (instance != m_singleton)
				{
					Destroy(instance.gameObject);
				}
			}
		}
		
		/// <summary>
		/// 씬이 변경될 때 호출됨
		/// (사용 시 base에서 선언된 함수를 반드시 호출할 것)
		/// </summary>
		protected virtual void OnActiveSceneChanged(Scene oldScene, Scene newScene)
		{
			ManageInstancesInThisScene();
		}

		private void OnDestroy()
		{
			if (m_singleton == this)
			{
				SceneManager.activeSceneChanged -= OnActiveSceneChanged;
			}
		}
	}

}