using System;

namespace Jobworld
{

	public interface IPlayTimerModel
	{
		float? time { get; }
		bool? isPlaying { get; }
		float? elapsedTime { get; }
	}

}