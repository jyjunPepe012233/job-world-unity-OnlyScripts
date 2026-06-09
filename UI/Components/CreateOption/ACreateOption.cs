using UnityEngine;

namespace Jobworld
{

	public abstract class ACreateOption<T> : MonoBehaviour
	{
		public abstract T GetValue();
	}

}