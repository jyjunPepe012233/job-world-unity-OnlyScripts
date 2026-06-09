using System;
//using Firebase;
//using Firebase.Auth;
using UnityEngine;

namespace Jobworld
{

	public class FirebaseInitialAnonLogin : MonoBehaviour
	{
//		private FirebaseAuth m_auth;
//
//		[SerializeField]
//		private bool deleteUserOnQuitApplication;
//
//		void Start()
//		{ 
//			m_auth = FirebaseAuth.DefaultInstance;
//
//			FirebaseApp.CheckAndFixDependenciesAsync();
//		
//			SignInAnonymously();
//		}
//
//		void SignInAnonymously()
//		{
//			m_auth.SignInAnonymouslyAsync().ContinueWith(task =>
//			{
//				if (task.IsCanceled)
//				{
//					Debug.LogError("Anonymous sign-in was canceled.");
//					return;
//				}
//				if (task.IsFaulted)
//				{
//					Debug.LogError("Anonymous sign-in encountered an error: " + task.Exception);
//					return;
//				}
//
//				FirebaseUser newUser = task.Result.User;
//				Debug.LogFormat("[FirebaseInitialAnonLogin] User signed in anonymously: {0} ({1})",
//					newUser.DisplayName, newUser.UserId);
//			});
//		}
//
//		private void OnApplicationQuit()
//		{
//			m_auth.CurrentUser.DeleteAsync();
//		}
	}

}