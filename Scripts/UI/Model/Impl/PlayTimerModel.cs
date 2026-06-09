using System;

namespace Jobworld
{

	public class PlayTimerModel : IPlayTimerModel
	{
		public float? time
		{
			get
			{
				return PlayTimer.time;
			}
		}

		public bool? isPlaying
		{
			get
			{
				return PlayTimer.isPlaying;
			}
		}

		public float? elapsedTime
		{
			get
			{
				return PlayTimer.elapsedTime;
			}
		}
	}

}