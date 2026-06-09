using Jobworld;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Handle : MonoBehaviour
{
    [Header("핸들 오브젝트")]
    [SerializeField] private Transform m_handle;

    [Header("핸들 손잡이 설정")]
    [SerializeField] private InteractorFinder m_interactorFinder;
    
    [Header("핸들 더미 손 모델")]
    [SerializeField] private GameObject m_leftDummyHand;
    [SerializeField] private GameObject m_rightDummyHand;

    [Header("회전 기준")]
    [SerializeField] private Transform m_rotationReference; // 회전 기준이 될 Transform

    [Header("회전 제한")]
    [SerializeField] private float m_maxAngle = 90f;

    [Header("회전 속도")]
    [SerializeField] private float m_resetSpeed = 50f;  

    private bool m_isGrabbed = false;
    private bool m_isHovered = false;
    
    // 좌우 손 그랩 상태 플래그
    private bool m_isLeftGrabbed = false;
    private bool m_isRightGrabbed = false;

    private Transform m_leftHandAnchor;
    private Transform m_rightHandAnchor;
    private HandModelController m_leftHandController;
    private HandModelController m_rightHandController;

    private float m_currentAngle = 0f;
    private bool m_isReleased = false;

    private void Update()
    {
        // 호버 시 양손 더미 모두 활성화, 그랩 시에는 개별 제어
        m_leftDummyHand.SetActive(m_isHovered || m_isLeftGrabbed);
        m_rightDummyHand.SetActive(m_isHovered || m_isRightGrabbed);

        // 실제 손 모델 제어
        if (m_leftHandController != null)
            m_leftHandController.SetModelActive(!m_isLeftGrabbed);
        if (m_rightHandController != null)
            m_rightHandController.SetModelActive(!m_isRightGrabbed);

        float targetAngle = m_currentAngle;

        if (m_isGrabbed)
        {
            m_isReleased = false;

            if (m_isLeftGrabbed && m_isRightGrabbed)
            {
                float leftAngle = CalcHandleAngle(m_leftDummyHand.transform.position, m_leftHandAnchor.position);
                float rightAngle = CalcHandleAngle(m_rightDummyHand.transform.position, m_rightHandAnchor.position);
                targetAngle = (leftAngle + rightAngle) * 0.5f;
            }
            else if (m_isLeftGrabbed)
            {
                targetAngle = CalcHandleAngle(m_leftDummyHand.transform.position, m_leftHandAnchor.position);
            }
            else if (m_isRightGrabbed)
            {
                targetAngle = CalcHandleAngle(m_rightDummyHand.transform.position, m_rightHandAnchor.position);
            }

            // 목표값 Clamp
            targetAngle = Mathf.Clamp(targetAngle, -m_maxAngle, m_maxAngle);

            // MoveTowards로 즉시 반영
            float moveStep = 300 * Time.deltaTime;
            m_currentAngle = Mathf.MoveTowards(m_currentAngle, targetAngle, moveStep);

            m_handle.localRotation = Quaternion.Euler(20f, 0f, m_currentAngle);
        }
        else
        {
            m_isReleased = true;
            // 놓았을 때 원위치로 MoveTowards로 부드럽게 이동
            float moveStep = m_resetSpeed * Time.deltaTime * 1.5f;
            m_currentAngle = Mathf.MoveTowards(m_currentAngle, 0f, moveStep);
            m_handle.localRotation = Quaternion.Euler(20f, 0f, m_currentAngle);
        }
    }

    /// <summary>
    /// 더미 핸드와 실제 손 위치를 기준으로 핸들 회전 각 계산
    /// </summary>
    private float CalcHandleAngle(Vector3 dummyPos, Vector3 handPos)
    {
        // 기준 Transform이 없으면 자기 자신을 기준으로
        Transform reference = m_rotationReference ? m_rotationReference : transform;
        
        // 더미와 실제 손의 위치를 기준 Transform 좌표계로 변환
        Vector3 localDummy = reference.InverseTransformPoint(dummyPos);
        Vector3 localHand = reference.InverseTransformPoint(handPos);
        
        // 더미 핸들의 초기 방향을 기준으로 각도 계산
        float dummyAngle = Mathf.Atan2(localDummy.y, localDummy.x) * Mathf.Rad2Deg;
        float handAngle = Mathf.Atan2(localHand.y, localHand.x) * Mathf.Rad2Deg;
        
        // 각도 차이 계산
        float angleDiff = Mathf.DeltaAngle(dummyAngle, handAngle);
        
        // 현재 핸들 각도에서의 상대적인 회전값 반환
        return m_currentAngle + angleDiff;
    }

    /// <summary>
    /// 핸들 회전값을 -1 ~ 1 범위로 정규화
    /// </summary>
    public float GetHandleValue()
    {
        float angle = m_handle.localEulerAngles.z;
        if (angle > 180f) angle -= 360f;

        angle = Mathf.Clamp(angle, -m_maxAngle, m_maxAngle);

        return angle / m_maxAngle;
    }
    
    public void SetHandleValue(float value)
    {
        m_currentAngle = Mathf.Clamp(value * m_maxAngle, -m_maxAngle, m_maxAngle);
        if (m_handle != null)
            m_handle.localRotation = Quaternion.Euler(20f, 0f, m_currentAngle);
    }

    // ----- 이벤트 처리 -----
    public void OnHover()
    {
        m_isHovered = true;
    }

    public void OnHoverExit()
    {
        m_isHovered = false;
    }

    public void OnGrab()
    {
        // GrabInteractorFinder를 통해 어느 손이 그랩했는지 확인
        if (m_interactorFinder != null)
        {
            GameObject leftInteractor = m_interactorFinder.LeftHandInteractorGO;
            GameObject rightInteractor = m_interactorFinder.RightHandInteractorGO;
            
            // 왼손 그랩 처리
            if (leftInteractor != null && leftInteractor.CompareTag("LeftHand") && !m_isLeftGrabbed)
            {
                var ovrRig = leftInteractor.GetComponentInParent<OVRCameraRig>();
                if (ovrRig != null)
                {
                    m_leftHandAnchor = ovrRig.leftHandAnchor;
                    m_leftHandController = ovrRig.leftHandAnchor?.GetComponent<HandModelController>();
                }
                m_isLeftGrabbed = true;
            }
            
            // 오른손 그랩 처리
            if (rightInteractor != null && rightInteractor.CompareTag("RightHand") && !m_isRightGrabbed)
            {
                var ovrRig = rightInteractor.GetComponentInParent<OVRCameraRig>();
                if (ovrRig != null)
                {
                    m_rightHandAnchor = ovrRig.rightHandAnchor;
                    m_rightHandController = ovrRig.rightHandAnchor?.GetComponent<HandModelController>();
                }
                m_isRightGrabbed = true;
            }

            // 전체 그랩 상태 업데이트
            m_isGrabbed = m_isLeftGrabbed || m_isRightGrabbed;
        }
    }

    public void OnRelease()
    {
        // GrabInteractorFinder를 통해 어느 손이 해제되었는지 확인
        if (m_interactorFinder != null)
        {
            GameObject leftInteractor = m_interactorFinder.LeftHandInteractorGO;
            GameObject rightInteractor = m_interactorFinder.RightHandInteractorGO;

            // 왼손 해제 처리
            if ((leftInteractor == null || !leftInteractor.CompareTag("LeftHand")) && m_isLeftGrabbed)
            {
                m_isLeftGrabbed = false;
                m_leftHandAnchor = null;
                m_leftHandController = null;
            }

            // 오른손 해제 처리
            if ((rightInteractor == null || !rightInteractor.CompareTag("RightHand")) && m_isRightGrabbed)
            {
                m_isRightGrabbed = false;
                m_rightHandAnchor = null;
                m_rightHandController = null;
            }

            // 전체 그랩 상태 업데이트
            m_isGrabbed = m_isLeftGrabbed || m_isRightGrabbed;
        }
    }
}