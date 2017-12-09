using System;
using NUnit.Framework;
using N9MTest.SDK.http;

namespace N9MTest.Config
{
	[Ignore ("用例错误")]
 // [TestFixture]
    public class TestCase_ParameterConfig:TestCase_Basecase
    {
        [Test]
        public void ConfigDefault()
        {
            int port = 80;
            string username = "admin";
            string password = "120223";

            N9MHttpSession session = new N9MHttpSession("192.168.61.202", port);
            session.Login("/logincheck.rsp?type=1", username, password);
    
            string path = Environment.CurrentDirectory;

            session.GetFile("/download.rsp", ref path);

            Console.WriteLine("ref path = {0}", path);

            string resp = session.PostFile("/upload.rsp?filetype=importparam", path);

            Console.WriteLine("ref resp = {0}", resp);

            session.Logout();
        }

        [Test]
        public void ParameterProtect()
        {

        }
    }

}
