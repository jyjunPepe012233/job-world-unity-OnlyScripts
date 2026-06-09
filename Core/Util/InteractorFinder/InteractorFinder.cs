using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Oculus.Interaction;

namespace Jobworld
{
    /// <summary>
    /// 어떤 종류의 Interactable이든 현재 선택중인 InteractorView들을 찾아주는 범용 유틸 클래스
    /// </summary>
    public class InteractorFinder : MonoBehaviour
    {
        private IInteractableView[] interactables;

        private void Awake()
        {
            // 이 GameObject에 있는 모든 IInteractableView 구현체들을 찾기
            interactables = GetComponents<IInteractableView>();
            
            if (interactables.Length == 0)
            {
                Debug.LogError($"{gameObject.name}: IInteractableView를 구현하는 컴포넌트가 없습니다.", this);
            }
        }

        /// <summary>
        /// 왼손 InteractorView 반환 (LeftHand 태그 기준)
        /// </summary>
        public GameObject LeftHandInteractorGO => FindHandInteractor("LeftHand");

        /// <summary>
        /// 오른손 InteractorView 반환 (RightHand 태그 기준)
        /// </summary>
        public GameObject RightHandInteractorGO => FindHandInteractor("RightHand");

        /// <summary>
        /// 현재 선택중인 모든 InteractorView 반환
        /// </summary>
        public IInteractorView[] GetSelectingInteractorViews()
        {
            var allInteractorViews = new List<IInteractorView>();
            
            foreach (var interactable in interactables)
            {
                if (interactable?.SelectingInteractorViews != null)
                {
                    allInteractorViews.AddRange(interactable.SelectingInteractorViews);
                }
            }
            
            return allInteractorViews.ToArray();
        }

        /// <summary>
        /// 현재 상호작용중인 모든 InteractorView 반환 (Hover + Select)
        /// </summary>
        public IInteractorView[] GetInteractorViews()
        {
            var allInteractorViews = new List<IInteractorView>();
            
            foreach (var interactable in interactables)
            {
                if (interactable?.InteractorViews != null)
                {
                    allInteractorViews.AddRange(interactable.InteractorViews);
                }
            }
            
            return allInteractorViews.ToArray();
        }

        /// <summary>
        /// 특정 손의 InteractorView가 현재 선택중인지 확인
        /// </summary>
        public bool IsHandSelecting(string handTag)
        {
            return FindHandInteractor(handTag) != null;
        }

        /// <summary>
        /// 양손 모두 선택중인지 확인
        /// </summary>
        public bool IsBothHandsSelecting()
        {
            return IsHandSelecting("LeftHand") && IsHandSelecting("RightHand");
        }

        /// <summary>
        /// 현재 선택중인 InteractorView 개수 반환
        /// </summary>
        public int SelectingInteractorCount => GetSelectingInteractorViews().Length;

        /// <summary>
        /// 현재 상호작용중인 InteractorView 개수 반환 (Hover + Select)
        /// </summary>
        public int InteractorCount => GetInteractorViews().Length;

        /// <summary>
        /// 아무 InteractorView라도 선택중인지 확인
        /// </summary>
        public bool IsAnyInteractorSelecting => SelectingInteractorCount > 0;

        /// <summary>
        /// 아무 InteractorView라도 상호작용중인지 확인 (Hover + Select)
        /// </summary>
        public bool IsAnyInteractorInteracting => InteractorCount > 0;

        /// <summary>
        /// SelectingInteractorViews 순회, 부모 계층에서 handTag 찾기
        /// </summary>
        private GameObject FindHandInteractor(string handTag)
        {
            var allInteractorViews = GetSelectingInteractorViews();
            
            foreach (var interactorView in allInteractorViews)
            {
                // IInteractorView는 Transform을 직접 제공하지 않으므로 MonoBehaviour로 캐스팅
                if (interactorView is MonoBehaviour mono)
                {
                    Transform t = mono.transform;
                    while (t != null)
                    {
                        if (t.CompareTag(handTag))
                            return mono.gameObject;
                        t = t.parent;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 특정 태그를 가진 선택중인 모든 InteractorView 반환
        /// </summary>
        public GameObject[] GetSelectingInteractorsByTag(string tag)
        {
            var result = new List<GameObject>();
            var allInteractorViews = GetSelectingInteractorViews();
            
            foreach (var interactorView in allInteractorViews)
            {
                if (interactorView is MonoBehaviour mono)
                {
                    Transform t = mono.transform;
                    while (t != null)
                    {
                        if (t.CompareTag(tag))
                        {
                            result.Add(mono.gameObject);
                            break;
                        }
                        t = t.parent;
                    }
                }
            }
            
            return result.ToArray();
        }

        /// <summary>
        /// 특정 태그를 가진 상호작용중인 모든 InteractorView 반환 (Hover + Select)
        /// </summary>
        public GameObject[] GetInteractorsByTag(string tag)
        {
            var result = new List<GameObject>();
            var allInteractorViews = GetInteractorViews();
            
            foreach (var interactorView in allInteractorViews)
            {
                if (interactorView is MonoBehaviour mono)
                {
                    Transform t = mono.transform;
                    while (t != null)
                    {
                        if (t.CompareTag(tag))
                        {
                            result.Add(mono.gameObject);
                            break;
                        }
                        t = t.parent;
                    }
                }
            }
            
            return result.ToArray();
        }
    }
}