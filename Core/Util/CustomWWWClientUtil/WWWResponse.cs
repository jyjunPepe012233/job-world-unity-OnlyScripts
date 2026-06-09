using System;
using Newtonsoft.Json.Linq;

namespace Jobworld
{

	public class WWWResponse
	{
		private const string MESSAGE_KEY = "message";
		
		private const string STATUS_KEY = "status";
		
		private JObject jObject;

		public WWWResponse()
		{
		}

		public WWWResponse(string json)
		{
			this.jObject = JObject.Parse(json);
		}
		
		public WWWResponse(JObject jObject)
		{
			this.jObject = jObject;
		}

		public TData Parse<TData>()
		{
			return jObject.ToObject<TData>();
		}

		public WWWResponse Child(string child)
		{
			return new WWWResponse((JObject)jObject[child]);
		}
		
		public string GetMessage()
		{
			if (jObject.TryGetValue(MESSAGE_KEY, out var value))
			{
				return value.ToString();
			}
			else
			{
				return null;
			}
		}

		public int GetStatus()
		{
			if (jObject.TryGetValue(STATUS_KEY, out var value))
			{
				return int.Parse(value!.ToString());
			}
			else
			{
				return -1;
			}
		}
	}

}