using System.Collections;
using UnityEngine;
using Oculus.Interaction; // PointerEvent, PointerEventType, Grabbable

namespace Jobworld
{
    [RequireComponent(typeof(Grabbable))]
    public class Tool : MonoBehaviour
    {
        public float resetDuration = 0.2f;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Transform initialParent;

        private Grabbable _grabbable;
        private Coroutine _resetCoroutine;
        private Equipment _equipment;
        
        protected Grabbable Grabbable => _grabbable;

        private void Awake()
        {
            _grabbable = GetComponent<Grabbable>();
            if (_grabbable == null)
            {
                Debug.LogError("Tool requires a Grabbable component.");
                return;
            }

            _equipment = GetComponentInParent<Equipment>();
            if (_equipment == null)
            {
                Debug.LogError("Tool must be a child of an Equipment object.");
            }

            // 포인터 이벤트 구독
            _grabbable.WhenPointerEventRaised += HandlePointerEvent;
        }

        private void OnDestroy()
        {
            if (_grabbable != null)
            {
                _grabbable.WhenPointerEventRaised -= HandlePointerEvent;
            }
        }

        public void SetInitialTransform(Transform parent)
        {
            initialParent = parent;
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
        }

        public void ResetToInitialTransform()
        {
            if (_resetCoroutine != null)
            {
                StopCoroutine(_resetCoroutine);
            }
            _resetCoroutine = StartCoroutine(SmoothReset());
        }

        private IEnumerator SmoothReset()
        {
            float elapsedTime = 0f;
            Vector3 startPosition = transform.localPosition;
            Quaternion startRotation = transform.localRotation;

            while (elapsedTime < resetDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / resetDuration);

                transform.localPosition = Vector3.Lerp(startPosition, initialPosition, t);
                transform.localRotation = Quaternion.Slerp(startRotation, initialRotation, t);

                yield return null;
            }

            transform.localPosition = initialPosition;
            transform.localRotation = initialRotation;
        }

        // Grabbable에서 올리는 모든 포인터 이벤트를 여기서 처리
        private void HandlePointerEvent(PointerEvent evt)
        {
            switch (evt.Type)
            {
                case PointerEventType.Select:
                    OnGrabbed(evt);
                    break;
                case PointerEventType.Unselect:
                    OnReleased(evt);
                    break;
                // 필요하면 Move, Cancel 처리 가능
            }
        }

        private void OnGrabbed(PointerEvent evt)
        {
            if (_resetCoroutine != null)
            {
                StopCoroutine(_resetCoroutine);
            }

            _equipment?.OnToolGrabbed();
        }

        private void OnReleased(PointerEvent evt)
        {
            // 아직 잡고 있는 손이 있으면 복귀하지 않음
            if (_grabbable.SelectingPointsCount == 0)
            {
                _equipment?.OnToolReleased();
                ResetToInitialTransform();
            }
        }


    }
}
