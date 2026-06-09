using Oculus.Interaction;
using UnityEngine;

namespace Jobworld
{
    [RequireComponent(typeof(Collider))]
    public class TaserGun : Tool
    {
        [Header("TaserGun Settings")]
        [SerializeField] private GameObject taserProjectilePrefab; // 발사할 탄환 프리팹
        [SerializeField] private Transform firePointA; // 첫 번째 핀 발사 위치
        [SerializeField] private Transform firePointB; // 두 번째 핀 발사 위치

        private TaserProjectile firedProjectileA;
        private TaserProjectile firedProjectileB;


        private void Update()
        {
            // Tool.cs의 Grabbable 프로퍼티 사용
            if (Grabbable == null || Grabbable.SelectingPointsCount == 0) return;
            // 두 발사체가 모두 없을 때만 발사 허용
            if (firedProjectileA != null || firedProjectileB != null) return;

            // XR 환경: 트리거 입력 시 발사
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) ||
                OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
            {
                FireTaser();
            }
            // PC 환경: 스페이스 버튼 클릭 시 발사
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FireTaser();
            }
        }

        private void FireTaser()
        {
            if (taserProjectilePrefab == null)
            {
                Debug.LogWarning("TaserProjectile 프리팹이 할당되지 않았습니다.");
                return;
            }
            // 두 개의 핀을 각각 발사
            firedProjectileA = FireSingleTaser(firePointA, "A");
            firedProjectileB = FireSingleTaser(firePointB, "B");
        }

        private TaserProjectile FireSingleTaser(Transform firePoint, string pinType)
        {
            if (firePoint == null) return null;
            GameObject projectile = Instantiate(taserProjectilePrefab, firePoint.position, firePoint.rotation);
            
            var taserProjectile = projectile.GetComponent<TaserProjectile>();
            if (taserProjectile != null)
            {
                taserProjectile.SetFirePoint(firePoint);
                taserProjectile.SetOwner(this, pinType);
            }
            return taserProjectile;
        }

        public void OnProjectileDestroyed(string pinType)
        {
            if (pinType == "A") firedProjectileA = null;
            else if (pinType == "B") firedProjectileB = null;
        }
    }
}
