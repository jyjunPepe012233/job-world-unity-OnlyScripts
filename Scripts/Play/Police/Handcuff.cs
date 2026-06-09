using UnityEngine;

namespace Jobworld
{
    [RequireComponent(typeof(Collider))]
    public class Handcuff : Tool
    {
        [Header("Handcuff Settings")]
        public string targetTag = "Suspect";

        private Suspect _currentSuspect;

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(targetTag))
            {
                Debug.Log("11");
                _currentSuspect = other.GetComponent<Suspect>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_currentSuspect != null && other.transform == _currentSuspect.transform)
            {
                Debug.Log("12");
                _currentSuspect = null;
            }
        }

        private void Update()
        {
            if (_currentSuspect == null)
            {
                Debug.Log("22");
                return;
            }

            // OculusInteraction 입력 체크 (왼손/오른손 상관없이 Trigger 입력)
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) ||
                OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
            {
                Debug.Log("33");
                _currentSuspect.OnHandcuffed();

                // 손에서 놓기
                if (Grabbable != null && Grabbable.SelectingPointsCount > 0)
                {
                    foreach (var interactor in Grabbable.SelectingPoints)
                    {
                        var type = interactor.GetType();
                        var releaseMethod = type.GetMethod("Release", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                        if (releaseMethod != null)
                        {
                            releaseMethod.Invoke(interactor, new object[] { Grabbable });
                        }
                    }
                }
                // 원래 자리로 복귀
                ResetToInitialTransform();
            }
            // PC 환경: 스페이스 버튼 클릭 시 수갑 채우기
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     _currentSuspect.OnHandcuffed();
            // }
        }
    }
}