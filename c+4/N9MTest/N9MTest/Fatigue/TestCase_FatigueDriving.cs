using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.Tooling;
using N9MTest.SDK.net;

namespace N9MTest.Fatigue
{
    [TestFixture]
    public class TestCase_FatigueDriving:TestCase_Basecase
    {
		[Ignore("1111111")]
        [Test]
        public void FatigueDrivingAlarm()
        {
            N9MSession session = new N9MSession("192.168.20.155", 9006);
            session.Login("admin", "");

            Sleep(3600 * 1000);
            /*
            ToolingSession session = new ToolingSession();
            session.StartSession("192.168.60.138", 7000);

            for (int i = 0; i < 60; i++)
            {
                session.SendMessage(DateTime.Now.ToString());
                Sleep(1000);
            }
            */
        }
    }
}
