using System;
using NUnit.Framework;
using RM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;



namespace NC_Test._002Nc_Alac
{

	public class Alac_Test01
	{
		String sIP = "192.168.1.1";
		


		[TestFixtureSetUp]
		public void Teststat() 
		{
			Console.WriteLine("(初始化)方法TestFixtureSetUp");
			

		}

	
		[SetUp]
		public void TestSetup()
		{
			Console.WriteLine("每次执行方法SetUp");
			
		}
	
		[Test]
		public void Channe()
		{

			Console.WriteLine("ip={0}", sIP);

			 


			Assert.AreEqual(2, 2);
			Assert.AreEqual(1, 2);
		}
		[Test]
		public void Channe2()
		{

			Console.WriteLine("ip={0}", sIP);
			
			Assert.AreEqual(2, 2);
			Assert.AreEqual(1, 2);
		}
       [TearDown]
		public void jsRepat()
		{
			Console.WriteLine("结束运行TearDown");

		}
		[TestFixtureTearDown]
		public void js()
		{
			Console.WriteLine("结束运行TestFixtureTearDown");

		}
	}
}
