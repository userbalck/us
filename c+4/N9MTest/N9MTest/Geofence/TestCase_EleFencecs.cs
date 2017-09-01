using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using N9MTest.SDK.net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace N9MTest.Geofence
{
    [TestFixture]
    class TestCase_EleFencecs:TestCase_Basecase
    {
       // [Test]
        public void GeofenceCreateCircle()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //设定101个圆形
                JObject jparameter = new JObject();
                JArray jcircleArray = new JArray();

                for (int i = 1; i <=101; i++)
                {
                    JObject jcircle = new JObject();
                    jcircle["ID"] = i;
                    jcircle["NAME"] = String.Format("AT Circle {0}", i);

                    JArray jpointArray = new JArray();

                    JObject point = new JObject();
                    point["X"] = 113998288 + i;
                    point["Y"] = 22596859 + i;
                    jpointArray.Add(point);

                    jcircle["POINT"] = jpointArray;
                    jcircle["RADIUS"] = 400;
                    jcircleArray.Add(jcircle);
                }

                jparameter["CIRCLE9"] = jcircleArray;

                jparameter["CMD"] = (int)EleFenceCmd.UPDATE;

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.EDITAREA, jparameter);

                Assert.IsNotNull(jresp);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                //获取设定以后圆形的状态
                jparameter = new JObject();
                jparameter["CMD"] = 0;

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETAREA, jparameter);

                Assert.IsNotNull(jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["CIRCLE9"]);

                jcircleArray = JArray.Parse(jresp["CIRCLE9"].ToString());

                Assert.AreEqual(100, jcircleArray.Count);

            }
        }

       // [Test]
        public void GeofenceCreatePoly()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");


                //设定101个多边形
                JObject jparameter = new JObject();
                JArray jpolyArray = new JArray();
                JObject jresp = null;

                for (int i = 1; i <= 101; i++)
                {
                    jparameter = new JObject();
                    jpolyArray = new JArray();

                    JObject jpoly = new JObject();
                    jpoly["ID"] = i;
                    jpoly["NAME"] = String.Format("AT Poly {0}", i);

                    JArray jpointArray = new JArray();

                    for (int n = 0; n < 2048; n++)
                    {
                        JObject point = new JObject();
                        point["X"] = 113998288 + n + i;
                        point["Y"] = 22596859 + n + i;
                        jpointArray.Add(point);
                    }

                    jpoly["POINT"] = jpointArray;
                    jpolyArray.Add(jpoly);

                    jparameter["POLY9"] = jpolyArray;

                    jparameter["CMD"] = (int)EleFenceCmd.UPDATE;

                    jresp = session.SendCommand(Module.DEVEMM, Operation.EDITAREA, jparameter, 3000);

                    Assert.IsNotNull(jresp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                }

               
                Sleep(5000);

                //获取设定以后圆形的状态
                jparameter = new JObject();
                jparameter["CMD"] = 0;

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETAREA, jparameter);

                Assert.IsNotNull(jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["POLY9"]);

                jpolyArray = JArray.Parse(jresp["POLY9"].ToString());

                Assert.AreEqual(100, jpolyArray.Count);

                List<Rectangle> list = JsonConvert.DeserializeObject<List<Rectangle>>(jresp["POLY9"].ToString());
                for (int i = 0; i < list.Count; i++)
                {
                    Rectangle rectangle = list[i];
                    Assert.AreEqual(2048, rectangle.list.Count);
                }
            }
        }

      //  [Test]
        public void GeofenceCreateRect()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //设定101个矩形
                JObject jparameter = new JObject();
                JArray jrectArray = new JArray();

                for (int i = 1; i < 101; i++)
                {
                    JObject jrect = new JObject();
                    jrect["ID"] = i;
                    jrect["NAME"] = String.Format("AT Rect {0}", i);

                    JArray jpointArray = new JArray();

                    JObject pointA = new JObject();
                    pointA["X"] = 113998288 + i;
                    pointA["Y"] = 22596859 + i;
                    jpointArray.Add(pointA);

                    JObject pointB = new JObject();
                    pointB["X"] = 114998288 + i;
                    pointB["Y"] = 22596859 + i;
                    jpointArray.Add(pointB);

                    JObject pointC = new JObject();
                    pointC["X"] = 114998288 + i;
                    pointC["Y"] = 22496859 + i;
                    jpointArray.Add(pointC);

                    JObject pointD = new JObject();
                    pointD["X"] = 113998288 + i;
                    pointD["Y"] = 22496859 + i;
                    
                    jpointArray.Add(pointD);

                    jrect["POINT"] = jpointArray;
                    jrectArray.Add(jrect);
                }

                jparameter["RECT9"] = jrectArray;

                jparameter["CMD"] = (int)EleFenceCmd.UPDATE;

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.EDITAREA, jparameter);

                Assert.IsNotNull(jresp);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                //获取设定以后圆形的状态
                jparameter = new JObject();
                jparameter["CMD"] = 0;

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETAREA, jparameter);

                Assert.IsNotNull(jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["RECT9"]);

                jrectArray = JArray.Parse(jresp["RECT9"].ToString());

                Assert.AreEqual(100, jrectArray.Count);
            }
        }

       // [Test]
        public void GeofenceCreateLine()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                //设定101条线路 每条2028个点
                JObject jparameter = new JObject();
                JArray jlineArray = new JArray();
                JObject jresp = null;

                for (int i = 1; i <= 101; i++)
                {
                    jparameter = new JObject();
                    jlineArray = new JArray();

                    JObject jline = new JObject();
                    jline["ID"] = i;
                    jline["NAME"] = String.Format("AT Line {0}", i);

                    JArray jpointArray = new JArray();

                    for (int n = 0; n < 2048; n++)
                    {
                        JObject point = new JObject();
                        point["X"] = 113998288 + n + i;
                        point["Y"] = 22596859 + n + i;
                        jpointArray.Add(point);
                    }

                    jline["POINT"] = jpointArray;
                    jlineArray.Add(jline);

                    jparameter["LINE9"] = jpointArray;

                    jparameter["CMD"] = (int)EleFenceCmd.UPDATE;

                    jresp = session.SendCommand(Module.DEVEMM, Operation.EDITAREA, jparameter, 3000);

                    Assert.IsNotNull(jresp);

                    Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                }


                Sleep(5000);

                //获取设定以后圆形的状态
                jparameter = new JObject();
                jparameter["CMD"] = 0;

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETAREA, jparameter);

                Assert.IsNotNull(jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));
                Assert.IsNotNull(jresp["LINE9"]);

                jlineArray = JArray.Parse(jresp["LINE9"].ToString());

                Assert.AreEqual(100, jlineArray.Count);

                List<Rectangle> list = JsonConvert.DeserializeObject<List<Rectangle>>(jresp["LINE9"].ToString());
                for (int i = 0; i < list.Count; i++)
                {
                    Rectangle rectangle = list[i];
                    Assert.AreEqual(2048, rectangle.list.Count);
                }
            }
        }

        [Test]
        public void GeofenceEdit()
        {
            foreach (string ip in IPList)
            {
                Console.WriteLine("ip = {0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();
                jparameter["CMD"] = 0;

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.GETAREA, jparameter);

                JObject jparamater = new JObject();
                jparamater = jresp;

                if (jresp["RECT9"] != null)
                {
                    jparamater["RECT9"][0]["NAME"] = "AT-RECT9-NAME";
                }

                if (jresp["CIRCLE9"] != null)
                {
                    jparamater["CIRCLE9"][0]["NAME"] = "AT-CIRCLE9-NAME";
                }

                if (jresp["POLY9"] != null)
                {
                    jparamater["POLY9"][0]["NAME"] = "AT-POLY9-NAME";
                }

                if (jresp["LINE9"] != null)
                {
                    jparamater["LINE9"][0]["NAME"] = "AT-LINE9-NAME";
                }

                jresp = session.SendCommand(Module.DEVEMM, Operation.EDITAREA, jparamater);

                Assert.IsNotNull(jresp);

                if (jresp["RECT9"] != null)
                {
                    Assert.AreEqual(jresp["RECT9"][0]["NAME"].ToString(), "AT-RECT9-NAME");
                }

                if (jresp["CIRCLE9"] != null)
                {
                    Assert.AreEqual(jresp["CIRCLE9"][0]["NAME"].ToString(), "AT-CIRCLE9-NAME");
                }

                if (jresp["POLY9"] != null)
                {
                    Assert.AreEqual(jresp["POLY9"][0]["NAME"].ToString(), "AT-POLY9-NAME");
                }

                if (jresp["LINE9"] != null)
                {
					  Assert.AreEqual(jresp["LINE9"][0]["NAME"].ToString(), "AT-LINE9-NAME");
					
				}

            }
        }

       // [Test]
        public void GeofenceExport()
        {
            Assert.IsTrue(false,"目前协议不支持围栏文件的协议导出，只可以通过U盘导出");
        }

       // [Test]
        public void GeofenceImport()
        {
            Assert.IsTrue(false, "目前协议不支持围栏文件的协议导入，只可以通过U盘导入");
            string filename = this.CFG.gNode("Alarm/GeofenceImport/filename").InnerText;

            EleFenceLoader loader = new EleFenceLoader(filename);
            JObject jdata = loader.LoadData();

            Assert.IsNotNull(jdata);

            foreach (string ip in IPList)
            {
                Console.WriteLine("ip ={0}", ip);

                N9MSession session = new N9MSession(ip, port);
                Assert.AreEqual(0, session.Login(username, password), "登录设备失败");

                JObject jparameter = new JObject();

                if (jdata["POLY9"] != null)
                {
                    jparameter["POLY9"] = jdata["POLY9"];
                }

                if (jdata["RECT9"] != null)
                {
                    jparameter["RECT9"] = jdata["RECT9"];
                }

                if (jdata["CIRCLE9"] != null)
                {
                    jparameter["CIRCLE9"] = jdata["CIRCLE9"];
                }

                if (jdata["LINE9"] != null)
                {
                    jparameter["LINE9"] = jdata["LINE9"];
                }

                jparameter["CMD"] = (int)EleFenceCmd.UPDATE;

                JObject jresp = session.SendCommand(Module.DEVEMM, Operation.EDITAREA, jparameter);

                Assert.IsNotNull(jresp);

                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                //获取设定以后圆形的状态
                jparameter = new JObject();
                jparameter["CMD"] = 0;

                jresp = session.SendCommand(Module.DEVEMM, Operation.GETAREA, jparameter);

                Assert.IsNotNull(jresp);
                Assert.AreEqual(0, Convert.ToInt32(jresp["ERRORCODE"].ToString()));

                if (jparameter["POLY9"] != null)
                {
                    Assert.AreEqual(jparameter["POLY9"].ToString(), jresp["POLY9"].ToString());
                }

                if (jparameter["RECT9"] != null)
                {
                    Assert.AreEqual(jparameter["RECT9"].ToString(), jresp["RECT9"].ToString());
                }

                if (jparameter["CIRCLE9"] != null)
                {
                    Assert.AreEqual(jparameter["CIRCLE9"].ToString(), jresp["CIRCLE9"].ToString());
                }

                if (jparameter["LINE9"] != null)
                {
                    Assert.AreEqual(jparameter["LINE9"].ToString(), jresp["LINE9"].ToString());
                }
            }
        }
    }
}
