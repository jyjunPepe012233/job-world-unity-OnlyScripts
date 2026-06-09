using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit.SceneDecorator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jobworld
{

    public class CoroutineHandler : MonoBehaviourSingleton<CoroutineHandler>
    {
        // 현재 진행중인 모든 코루틴
        // Value(boolean)는 씬 변경 시 중단할 지 여부
        private Dictionary<Coroutine, bool> m_stopCoroutinesOnSceneChanged = new Dictionary<Coroutine, bool>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeOnLoad()
        {
            InitializeSingleton();
        }
		
        /// <summary>
        /// 씬 변경 시 중단할 지 여부를 선택 가능
        /// </summary>
        public Coroutine StartCoroutineSafe(IEnumerator coroutine, bool stopOnSceneChanged)
        {
            Coroutine startedCoroutine = StartCoroutine(coroutine);
            m_stopCoroutinesOnSceneChanged[startedCoroutine] = stopOnSceneChanged;
            return startedCoroutine;
        }

        /// <summary>
        /// Coroutine Handler에서 진행 중인 특정 코루틴을 중지
        /// </summary>
        public void StopCoroutineSafe(Coroutine coroutine)
        {
            var temp = new Dictionary<Coroutine, bool>(m_stopCoroutinesOnSceneChanged);
			
            foreach (var c in temp)
            {
                if (c.Key == coroutine)
                {
                    StopCoroutine(c.Key);
                    m_stopCoroutinesOnSceneChanged.Remove(c.Key);
                }
            }
        }

        protected override void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            var temp = new Dictionary<Coroutine, bool>(m_stopCoroutinesOnSceneChanged);
			
            foreach (var c in temp)
            {
                if (c.Value)
                {
                    StopCoroutine(c.Key);
                    m_stopCoroutinesOnSceneChanged.Remove(c.Key);
                }
            }
        }
    }

}