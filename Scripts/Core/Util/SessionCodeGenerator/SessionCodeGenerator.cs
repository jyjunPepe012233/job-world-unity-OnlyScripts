using System;

namespace Jobworld
{
	public class SessionCodeGenerator
	{
		private static readonly char[] chars = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
		private readonly Random random = new Random();

		public string GenerateCode(int length = 6)
		{
			char[] buffer = new char[length];
			for (int i = 0; i < length; i++)
			{
				buffer[i] = chars[random.Next(chars.Length)];
			}
			return new string(buffer);
		}
	}

}