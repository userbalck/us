using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;

namespace N9MTest.Peripherals
{
    [TestFixture]
    class TestCase_Peripherals:TestCase_Basecase
    {
		[Ignore("1111")]
        [Test]
        public void InternalAccel()
        {
            H264FileLoader loader = new H264FileLoader("video.data");
            loader.Parse();
            loader.Dispose();
        }
		[Ignore("1111")]
		[Test]
        public void ExternalAccel()
        {

        }
    }
}
