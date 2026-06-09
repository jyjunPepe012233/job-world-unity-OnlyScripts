using UnityEngine;

namespace Jobworld
{
    public class Equipment : MonoBehaviour
    {
        public EquipmentSlot[] slots;
        public Vector3 offset;
        public float fanRadius = 0.5f;
        public float fanAngle = 90f;
        public float lookDownThreshold = 0.7f; // dot값, 0.7 ≈ 45도
        public float lookTime = 2f;            // 몇 초 동안 유지해야 켜질지
        public float scaleSpeed = 3f;          // 스케일 변화 속도

        [SerializeField] private OVRCameraRig _ovrCameraRig;
        private float _lookTimer;
        private bool _isVisible;
        private int _heldToolCount = 0;

        private void Awake()
        {
            transform.localScale = Vector3.zero; // 처음엔 숨김
            ArrangeSlots();
        }

        private void Update()
        {
            if (_ovrCameraRig == null) return;

            // 위치 & 회전 업데이트
            transform.position = _ovrCameraRig.trackerAnchor.position + _ovrCameraRig.trackerAnchor.rotation * offset;
            transform.rotation = _ovrCameraRig.trackerAnchor.rotation;

            // 시선 방향 계산
            Vector3 lookDir = _ovrCameraRig.centerEyeAnchor.forward;
            float dot = Vector3.Dot(lookDir.normalized, Vector3.down);

            if (dot > lookDownThreshold)
            {
                _lookTimer += Time.deltaTime;
                if (_lookTimer >= lookTime) _isVisible = true;
            }
            else
            {
                _lookTimer = 0f;
                if (_heldToolCount == 0) // 잡고 있는 툴이 없으면 숨김
                {
                    _isVisible = false;
                }
            }

            // 스케일 보간
            Vector3 targetScale = _isVisible ? Vector3.one : Vector3.zero;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
        }

        public void OnToolGrabbed()
        {
            _heldToolCount++;
            _isVisible = true; // 툴을 잡으면 즉시 보이도록
        }

        public void OnToolReleased()
        {
            _heldToolCount--;
        }

        private void ArrangeSlots()
        {
            if (slots == null || slots.Length == 0) return;

            float angleStep = slots.Length > 1 ? fanAngle / (slots.Length - 1) : 0f;
            float startAngle = -fanAngle / 2;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null) continue;

                float angle = startAngle + (i * angleStep);
                float angleRad = angle * Mathf.Deg2Rad;

                float x = fanRadius * Mathf.Sin(angleRad);
                float z = fanRadius - fanRadius * Mathf.Cos(angleRad);
                Vector3 position = new Vector3(x, 0, z);

                slots[i].transform.localPosition = position;
                slots[i].transform.localRotation = Quaternion.Euler(0, angle, 0);
            }
        }
    }
}
