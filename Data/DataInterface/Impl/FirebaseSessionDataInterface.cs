using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Jobworld
{

	public class FirebaseSessionDataInterface : ISessionDataInterface
	{
//		private readonly DatabaseReference m_root = FirebaseDatabase.DefaultInstance.RootReference;
//
//		public event Action<SessionPlayerData> memberAdded;
//		
//		public event Action<SessionPlayerData> memberUpdated;
//		
//		public event Action<long> memberRemoved;
//
//		public void SubscribeSessionEvents()
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				reference.Child("players").ChildAdded += OnPlayerListAdded;
//				reference.Child("players").ChildRemoved += OnPlayerListRemoved;
//			}
//		}
//
//		public void UnsubscribeSessionEvents()
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				reference.Child("players").ChildAdded -= OnPlayerListAdded;
//				reference.Child("players").ChildRemoved -= OnPlayerListRemoved;
//			}
//		}
//		
//		private void SubscribePlayerEvents()
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				reference.Child("players").ValueChanged += OnPlayerValueChanged;
//			}
//		}
//		
//		private void UnsubscribePlayerEvents()
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				reference.Child("players").ValueChanged -= OnPlayerValueChanged;
//			}
//		}
//		
//		private void OnPlayerListAdded(object _, ChildChangedEventArgs args)
//		{
//			SubscribePlayerEvents();
//			InvokeMemberAddedEvent(args);
//		}
//
//		private void InvokeMemberAddedEvent(ChildChangedEventArgs args)
//		{
//			var member = JsonUtility.FromJson<SessionPlayerData>(args.Snapshot.GetRawJsonValue());
//			memberAdded?.Invoke(member);
//		}
//		
//		private void OnPlayerListRemoved(object _, ChildChangedEventArgs args)
//		{
//			UnsubscribePlayerEvents();
//			InvokeMemberRemovedEvent(args);
//		}
//
//		private void InvokeMemberRemovedEvent(ChildChangedEventArgs args)
//		{
//			var memberId = (long)args.Snapshot.Child("authId").Value;
//			memberRemoved?.Invoke(memberId);
//		}
//
//		private void OnPlayerValueChanged(object _, ValueChangedEventArgs args)
//		{
//			Debug.Log("[FirebaseSessionDataInterface] Player Value Changed");
//			InvokeMemberUpdatedEvent(args);
//		}
//
//		private void InvokeMemberUpdatedEvent(ValueChangedEventArgs args)
//		{
//			var member = JsonUtility.FromJson<SessionPlayerData>(args.Snapshot.GetRawJsonValue());
//			memberUpdated?.Invoke(member);
//		}
//		
//		public async Task AddSession(SessionData sessionData)
//		{
//			string json = JsonUtility.ToJson(sessionData);
//			await m_root.Child(sessionData.sessionName).Child("info").SetRawJsonValueAsync(json);
//		}
//
//		public async Task RemoveSession(SessionData sessionData)
//		{
//			await m_root.Child(sessionData.sessionName).RemoveValueAsync();
//		}
//		
//		private bool TryGetCurrentSessionReference(out DatabaseReference reference)
//		{
//			bool inRoom = PhotonNetwork.InRoom;
//			
//			reference = inRoom ? m_root.Child(PhotonNetwork.CurrentRoom.Name) : null;
//			return inRoom;
//		}
//
//		public async Task<SessionPlayerData> GetPlayerDataById(long wasId)
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				var snapshot = await reference.Child("players").Child(wasId.ToString()).GetValueAsync();
//				return JsonUtility.FromJson<SessionPlayerData>(snapshot.GetRawJsonValue());
//			}
//
//			return null;
//		}
//
//		public async Task<SessionPlayerData[]> GetAllPlayerData()
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				var snapshot = await reference.Child("players").GetValueAsync();
//				if (snapshot.Exists)
//				{
//					var players = snapshot.Children
//						.Select(child => JsonUtility.FromJson<SessionPlayerData>(child.GetRawJsonValue()))
//						.ToArray();
//					return players;
//				}
//			}
//			
//			return Array.Empty<SessionPlayerData>();
//		}
//
//		public async Task<int> CountsOfPlayerSelectedJob()
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				var snapshot = await reference.Child("players").GetValueAsync();
//				
//				// jobName이 null이 아니거나 빈 문자열이 아닌 데이터의 개수를 셈 
//				return snapshot.Children.Count(child => String.IsNullOrEmpty(child.Child("jobName").Value?.ToString()));
//			}
//
//			return -1;
//		}
//
//		public async Task AddPlayer(SessionPlayerData playData)
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				string json = JsonUtility.ToJson(playData);
//				await reference.Child("players").Child(playData.authId.ToString()).SetRawJsonValueAsync(json);
//			}
//
//			return;
//		}
//
//		public async Task RemovePlayer(long wasId)
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				await reference.Child("players").Child(wasId.ToString()).RemoveValueAsync();
//			}
//
//			return;
//		}
//
//		public async Task UpdatePlayerDollar(long wasId, int dollar)
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				var updates = new Dictionary<string, object>
//				{
//					{ "dollar", dollar }
//				};
//				await reference.Child("players").Child(wasId.ToString()).UpdateChildrenAsync(updates);
//			}
//		}
//
//		public async Task UpdatePlayerJob(long wasId, string jobName)
//		{
//			if (TryGetCurrentSessionReference(out var reference))
//			{
//				var updates = new Dictionary<string, object>
//				{
//					{ "jobName", jobName }
//				};
//				await reference.Child("players").Child(wasId.ToString()).UpdateChildrenAsync(updates);
//			}
//
//			return;
//		}
public event Action<SessionPlayerData> memberAdded;
public event Action<SessionPlayerData> memberUpdated;
public event Action<long> memberRemoved;
public void SubscribeSessionEvents()
{
	throw new NotImplementedException();
}

public void UnsubscribeSessionEvents()
{
	throw new NotImplementedException();
}

public Task AddSession(SessionData sessionData)
{
	throw new NotImplementedException();
}

public Task RemoveSession(SessionData sessionData)
{
	throw new NotImplementedException();
}

public Task<SessionPlayerData> GetPlayerDataById(long wasId)
{
	throw new NotImplementedException();
}

public Task<SessionPlayerData[]> GetAllPlayerData()
{
	throw new NotImplementedException();
}

public Task<int> CountsOfPlayerSelectedJob()
{
	throw new NotImplementedException();
}

public Task AddPlayer(SessionPlayerData playData)
{
	throw new NotImplementedException();
}

public Task RemovePlayer(long wasId)
{
	throw new NotImplementedException();
}

public Task UpdatePlayerJob(long wasId, string jobName)
{
	throw new NotImplementedException();
}

public Task UpdatePlayerDollar(long wasId, int dollar)
{
	throw new NotImplementedException();
}
	}

}