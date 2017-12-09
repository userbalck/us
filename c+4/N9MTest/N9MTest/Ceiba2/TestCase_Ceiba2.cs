using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace N9MTest.Maintenance
{
	[Ignore("11111")]
	[TestFixture]
    class TestCase_Ceiba2 : TestCase_Basecase
    {
        [Test]
        public void SendSMS()
        {
            Assert.IsTrue(false, "暂不支持短信内容查询");
        }
    }
}
