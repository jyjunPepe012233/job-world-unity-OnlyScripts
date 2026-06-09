using UnityEngine;  

namespace Jobworld
{
    public class TaserProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 50f;
        [SerializeField] private float lifeTime = 5f;

        private Transform m_firePoint;
        private LineRenderer m_lineRenderer;
        private Rigidbody m_rigidbody;
        private TaserGun m_ownerGun;
        private string m_pinType; // "A" 또는 "B"

        public void SetFirePoint(Transform firePoint)
        {
            m_firePoint = firePoint;
        }

        public void SetOwner(TaserGun gun, string pinType)
        {
            m_ownerGun = gun;
            m_pinType = pinType;
        }

        private void Awake()
        {
            m_lineRenderer = GetComponent<LineRenderer>();
            m_rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (m_rigidbody != null)
            {
                m_rigidbody.velocity = transform.forward * speed;
            }
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
            // 줄 연결 효과
            if (m_lineRenderer != null && m_firePoint != null)
            {
                m_lineRenderer.positionCount = 2;
                m_lineRenderer.SetPosition(0, m_firePoint.position);
                m_lineRenderer.SetPosition(1, transform.position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            Suspect suspect = other.GetComponent<Suspect>();
            if (suspect != null)
            {
                suspect.OnTased();
                Debug.Log($"TaserProjectile: {other.name}에게 테이저건 명중");
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (m_ownerGun != null && !string.IsNullOrEmpty(m_pinType))
            {
                m_ownerGun.OnProjectileDestroyed(m_pinType);
            }
        }
    }
}
