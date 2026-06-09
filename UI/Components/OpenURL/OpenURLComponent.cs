using Oculus.Interaction;
using UnityEngine;

namespace Jobworld
{

	public class OpenURLComponent : MonoBehaviour
	{
		[SerializeField, Interface(typeof(IOpenURLModel))]
		private Object m_modelObject;
		
		private IOpenURLModel m_model => m_modelObject as IOpenURLModel;

		/// <summary>
		/// Called by Button
		/// </summary>
		public void OpenURL()
		{
			m_model.OpenURL();
		}
	}

}