using UnityEngine;

namespace Jobworld
{
    public class TutorialStater : MonoBehaviour
    {
        [SerializeField] TutorialManager tutorialManager;
        [SerializeField] ParticleSystem startEffect;
        [Header("Spawn & Trigger")]
        [SerializeField] private Transform effectSpawnPoint;
        [SerializeField] private float triggerDistance = 0.5f;
        [SerializeField] private bool destroyEffectOnTrigger = true;
        [SerializeField] private Transform cameraRig;
        
        private ParticleSystem m_spawnedEffectInstance;
        private bool m_triggered;
        private Bell m_bell;

        void Start()
        {
            Vector3 spawnPos = (effectSpawnPoint != null) ? effectSpawnPoint.position : transform.position;
            Quaternion spawnRot = (effectSpawnPoint != null) ? effectSpawnPoint.rotation : transform.rotation;

            if (startEffect != null)
            {
                m_spawnedEffectInstance = Instantiate(startEffect, spawnPos, spawnRot, null);
                m_spawnedEffectInstance.Play();
            }

            if (tutorialManager == null)
            {
                tutorialManager = FindObjectOfType<TutorialManager>();
            }
            
            m_bell = FindObjectOfType<Bell>();
            if (m_bell == null)
            {
                Debug.LogWarning("[TutorialStater] Bell not found in the scene.");
            }
        }

        void Update()
        {
            if (m_triggered) return;

            if (cameraRig == null)
            {
                Debug.LogWarning("[TutorialStater] Camera rig is not assigned.");
                return;
            }

            Vector3 targetPos = (effectSpawnPoint != null) ? effectSpawnPoint.position : transform.position;
            float dist = Vector3.Distance(cameraRig.position, targetPos);
            if (dist <= triggerDistance)
            {
                m_triggered = true;
                Debug.Log("Tutorial triggered");
                tutorialManager?.StartTutorialSequence();
                if (destroyEffectOnTrigger && m_spawnedEffectInstance != null)
                    Destroy(m_spawnedEffectInstance.gameObject);
                
                if (m_bell != null && m_bell.pointArrowParticle != null)
                {
                    m_bell.pointArrowParticle.Play();
                }

                Destroy(this);
            }
        }
    }
}
