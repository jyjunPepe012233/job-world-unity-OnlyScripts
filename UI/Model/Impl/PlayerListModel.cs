using System;

namespace Jobworld
{

	public class PlayerListModel : IPlayerListModel
	{
		private static ISessionDataInterface m_sessionDI = DataInterfaceContainer.session;
		
		public event Action<long> memberAdded;
		
		public event Action<long> memberUpdated;
		
		public event Action<long> memberRemoved;
		
		public PlayerListModel()
		{
			m_sessionDI.memberAdded += InvokeMemberAddedEvent;
			m_sessionDI.memberUpdated += InvokeMemberUpdatedEvent;
			m_sessionDI.memberRemoved += InvokeMemberRemovedEvent;
		}

		~PlayerListModel()
		{
			m_sessionDI.memberAdded -= InvokeMemberAddedEvent;
			m_sessionDI.memberUpdated -= InvokeMemberUpdatedEvent;
			m_sessionDI.memberRemoved -= InvokeMemberRemovedEvent;
		}

		private void InvokeMemberAddedEvent(SessionPlayerData playerData)
		{
			memberAdded?.Invoke(playerData.authId);
		}

		private void InvokeMemberUpdatedEvent(SessionPlayerData playerData)
		{
			memberUpdated?.Invoke(playerData.authId);
		}
		
		private void InvokeMemberRemovedEvent(long id)
		{
			memberRemoved?.Invoke(id);
		}
	}

}