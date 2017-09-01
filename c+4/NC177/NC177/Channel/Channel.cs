using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NC177.Channel
{
	class Channel
	{
		string sIP = "192.168.1.1";
		System.Diagnostics.Process p;
		string ncip = "nc.exe -l -s 192.168.20.61 -p 9999";

		[Test]
		public void Channe()
		{
			
			ncip = "114.114.114.114";
			Console.WriteLine("1111111111");
			p = new System.Diagnostics.Process();
			p.StartInfo.FileName = "cmd.exe";
			Console.WriteLine("cmd");
			p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
			p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
			p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
			p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
			p.StartInfo.CreateNoWindow = true;//不显示程序窗口
			Console.WriteLine("不显示程序窗口");
			p.Start();//启动程序

			//向cmd窗口发送输入信息
			//p.StandardInput.WriteLine("ping " + ip + "&exit");
			p.StandardInput.WriteLine("pin"+ncip);
			Console.WriteLine("ncip==="+ncip);
			//p.StandardInput.WriteLine("ping " + ip + " -l " + package + " -w 1000 -n " + seconds + " &exit"); //一秒钟ping一次，一分钟则ping 60次
			p.StandardInput.AutoFlush = true;
			//p.StandardInput.WriteLine("exit");
			Console.WriteLine("AutoFlush = true;");
			string output = p.StandardOutput.ReadToEnd();
			Console.WriteLine("output==" + output);
			p.WaitForExit();//等待程序执行完退出进程
			p.Close();

			Console.WriteLine(output);



			Console.WriteLine("ip={0}", sIP);

			Assert.AreEqual(2, 2);
			
		}
		[Test]
		public void Multichannel()
		{

		

			Console.WriteLine("ip={0}", sIP);

			Assert.AreEqual(2, 2);

		}
		public void PingIPisLosspage(string ip, int seconds, int package)
		{
			p = new System.Diagnostics.Process();
			p.StartInfo.FileName = "cmd.exe";
			p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
			p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
			p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
			p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
			p.StartInfo.CreateNoWindow = true;//不显示程序窗口
			p.Start();//启动程序

			//向cmd窗口发送输入信息
			//p.StandardInput.WriteLine("ping " + ip + "&exit");
			p.StandardInput.WriteLine("ping " + ip + " -l " + package + " -w 1000 -n " + seconds + " &exit"); //一秒钟ping一次，一分钟则ping 60次
			p.StandardInput.AutoFlush = true;
			//p.StandardInput.WriteLine("exit");
			string output = p.StandardOutput.ReadToEnd();

			p.WaitForExit();//等待程序执行完退出进程
			p.Close();

			Console.WriteLine(output);


		}
	}

}
