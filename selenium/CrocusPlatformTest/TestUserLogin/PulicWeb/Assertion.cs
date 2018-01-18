using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Crocus.TestUI.PulicWeb
{
	class Assertion
	{
		public static bool flag = true;

		public static List<Exception> errors = new List<Exception>();

		public static void verifyEquals(Object actual, Object expected)
		{
			try
			{
				Assert.AreEqual(actual, expected);
			}
			catch (Exception e)
			{
				errors.Add(e);
				flag = false;
			}
		}

		public static void verifyEquals(Object actual, Object expected, String message)
		{
		

			try
			{
				Assert.AreEqual(actual, expected, message);
			}
			catch (Exception e)
			{
				errors.Add(e);
				flag = false;
			}
		}

		internal static void verifyEquals()
		{
			throw new NotImplementedException();
		}
	}
}
