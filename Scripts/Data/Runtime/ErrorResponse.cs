namespace Jobworld
{

	public class ErrorResponse
	{
		public string message;

		public string status;

		public string error;

		public static ErrorResponse RequestTimeout()
		{
			return new ErrorResponse
			{
				message = "시간 초과",
				status = "408",
				error = "Request Timeout"
			};
		} 
	}

}