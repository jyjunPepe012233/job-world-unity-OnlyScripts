// using UnityEngine;
//
// namespace Jobworld
// {
//     public class TestDollarSetup : MonoBehaviour
//     {
//         [Header("Test Settings")]
//         [SerializeField] private bool initializeOnStart = true;
//         [SerializeField] private long testAuthId = 12345;
//         [SerializeField] private string testSessionName = "TestSession";
//         [SerializeField] private string initialDollar = "1000";
//         
//         private void Start()
//         {
//             if (initializeOnStart)
//             {
//                 SetupTestData();
//             }
//         }
//         
//         private void Update()
//         {
//             // 키보드 단축키 (게임 실행 중)
//             if (Input.GetKeyDown(KeyCode.Alpha1))
//             {
//                 Add250Dollars();
//             }
//             else if (Input.GetKeyDown(KeyCode.Alpha2))
//             {
//                 Subtract100Dollars();
//             }
//             else if (Input.GetKeyDown(KeyCode.Alpha3))
//             {
//                 SetDollarTo500();
//             }
//             else if (Input.GetKeyDown(KeyCode.Alpha4))
//             {
//                 SetDollarTo2000();
//             }
//         }
//         
//         [ContextMenu("Setup Test Data")]
//         [System.Diagnostics.Conditional("UNITY_EDITOR")]
//         public void SetupTestData()
//         {
//             // LocalAuthorizationHolder.id 설정이 필요하다면 (있다고 가정)
//             // LocalAuthorizationHolder.id = testAuthId;
//             
//             // 테스트 세션 참가
//             GuestClientLocalInfoHolder.JoinSession(testSessionName);
//             
//             // 달러 설정
//             if (GuestClientLocalInfoHolder.playData != null)
//             {
//                 GuestClientLocalInfoHolder.playData.dollar = initialDollar;
//                 Debug.Log($"테스트 데이터 설정 완료: AuthID={testAuthId}, Dollar={initialDollar}");
//             }
//             else
//             {
//                 Debug.LogError("playData 생성 실패!");
//             }
//         }
//         
//         [ContextMenu("Clear Test Data")]
//         public void ClearTestData()
//         {
//             GuestClientLocalInfoHolder.ClearSession();
//             Debug.Log("테스트 데이터 정리 완료");
//         }
//         
//         [ContextMenu("Set Dollar to 500")]
//         public void SetDollarTo500()
//         {
//             if (GuestClientLocalInfoHolder.playData != null)
//             {
//                 GuestClientLocalInfoHolder.playData.dollar = "500";
//                 Debug.Log("달러를 500으로 설정");
//             }
//         }
//         
//         [ContextMenu("Set Dollar to 2000")]
//         public void SetDollarTo2000()
//         {
//             if (GuestClientLocalInfoHolder.playData != null)
//             {
//                 GuestClientLocalInfoHolder.playData.dollar = "2000";
//                 Debug.Log("달러를 2000으로 설정");
//             }
//         }
//         
//         [ContextMenu("Add 250 Dollars")]
//         public void Add250Dollars()
//         {
//             if (GuestClientLocalInfoHolder.playData != null)
//             {
//                 if (int.TryParse(GuestClientLocalInfoHolder.playData.dollar, out int current))
//                 {
//                     GuestClientLocalInfoHolder.playData.dollar = (current + 250).ToString();
//                     Debug.Log($"250달러 추가: {current} → {current + 250}");
//                 }
//             }
//         }
//         
//         [ContextMenu("Subtract 100 Dollars")]
//         public void Subtract100Dollars()
//         {
//             if (GuestClientLocalInfoHolder.playData != null)
//             {
//                 if (int.TryParse(GuestClientLocalInfoHolder.playData.dollar, out int current))
//                 {
//                     int newAmount = Mathf.Max(0, current - 100);
//                     GuestClientLocalInfoHolder.playData.dollar = newAmount.ToString();
//                     Debug.Log($"100달러 차감: {current} → {newAmount}");
//                 }
//             }
//         }
//     }
// }

using System;
using Jobworld;
using UnityEngine;

public class TestDollarSetup : MonoBehaviour
{
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            TestDollar();
        }
    }

    private async void TestDollar()
    {
            Debug.Log(GuestClientLocalInfoHolder.playData.dollar);
            await GuestClientService.SetDollar(10000);
            Debug.Log(GuestClientLocalInfoHolder.playData.dollar);
    }
}