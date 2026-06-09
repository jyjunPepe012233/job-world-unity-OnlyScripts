using System;
using UnityEngine;

namespace Jobworld
{

	public class PlayTimer : MonoBehaviourSingleton<PlayTimer>
	{
		public static float? time => singleton.m_timer?.time ?? null;

		public static float? elapsedTime => singleton.m_timer?.elapsedTime ?? null;

		public static bool? isPlaying => singleton.m_timer?.isPlaying ?? null;

		private PlayTimerInstance m_timer;
		
		public static void StartTimer(float time, bool autoStart = false)
		{
			StartTimer(time, null, autoStart);
		}
		
		public static void StartTimer(float time, Action onComplete = null, bool autoStart = false)
		{
			Debug.Log("Timer Start Attempted!");
			
			singleton.StartTimerInternal(time, onComplete, autoStart);
		}

		private void StartTimerInternal(float time, Action onComplete = null, bool autoStart = false)
		{
			var timer = new PlayTimerInstance(time, onComplete, autoStart);
			m_timer = timer;
		}

		public static void StopTimer()
		{ 
			singleton.StopTimerInternal();
		}

		private void StopTimerInternal()
		{
			m_timer = null;
		}

		void Update()
		{
			if (m_timer != null)
			{
				m_timer.Update(Time.deltaTime);
			} 
		}
	}

	public class PlayTimerInstance
	{
		public float time { get; private set; }

		public event Action onComplete;

		public float elapsedTime { get; private set; }
		
		public bool isPlaying { get; private set; }

		public PlayTimerInstance(float time, bool autoStart = false)
		{
			this.time = time;
			this.elapsedTime = 0f;
			this.isPlaying = false;

			if (autoStart) Start();
		}

		public PlayTimerInstance(float time, Action onComplete = null, bool autoStart = false) 
			: this(time, autoStart)
		{
			if (onComplete != null)
				this.onComplete += onComplete;
		}

		public void Start()
		{
			if (isPlaying)
				return;

			isPlaying = true;
			elapsedTime = 0f;
			
			Debug.Log("Timer Started isPlaying : " + isPlaying);
		}
		
		public void Update(float deltaTime)
		{
			if (!isPlaying)
				return;

			elapsedTime += deltaTime;
			if (elapsedTime >= time)
			{
				isPlaying = false;
				onComplete?.Invoke();
			}
			
			Debug.Log("Timer Updated" + isPlaying);
		}
	}

}