using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TcpClik
{
	public partial class Form1 : Form
	{
		string hostip;
		TcpClient client;
		NetworkStream senStren;
		BinaryWriter bw;
		String st;
		public Form1()
		{
			InitializeComponent();
			StartClik.Click += StartClik_Click;
			StopClik.Click += StopClik_Click;
			Sendclik.Click += Sendclik_Click;
		}

		private void Sendclik_Click(object sender, EventArgs e)
		{

			
			st = "CS123";
		    st = textclik.Text;
		//	logs("发送到服务端" + st);
		//	senStren = client.GetStream();
		//	if (senStren.CanWrite)
		//	{
		//		Byte[] sendBytes = Encoding.Default.GetBytes(st);
		//		senStren.Write(sendBytes, 0, sendBytes.Length);
		//		senStren.Flush();
		//		senStren.Close();//关闭网络流  
		//						   //client.Close();//关闭客户端  

		//	}
		//	else
		//	{
		//		MessageBox.Show("无法写入数据流");

		//		senStren.Close();
		//		client.Close();

		//		return;




		//	}
		}

		private void StopClik_Click(object sender, EventArgs e)
		{
			senStren.Close();
		}
		//启动客户端
		private void StartClik_Click(object sender, EventArgs e)
		{
			string strHostName = Dns.GetHostName(); //得到本机的主机名
			IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
			hostip = ipEntry.AddressList[0].ToString();
			client = new TcpClient();
			client.Connect(hostip, 5252);
			if (client!=null)
			{
				logs("链接成功");
				NetworkStream networkStream = client.GetStream();
				BinaryReader br = new BinaryReader(networkStream);
				 bw = new BinaryWriter(networkStream);
				st = "CS123";
				st = textclik.Text;
				bw.Write(st);  //向服务器发送字符串
				while (true)
				{
					try
					{
						string brString = br.ReadString();     //接收服务器发送的数据  
						if (brString != null)
						{
							Console.WriteLine("接收到服务器发送的数据{0}", brString);
						}
					}
					catch
					{
						break;        //接收过程中如果出现异常，将推出循环  
					}
				}
			}
			Console.WriteLine("连接服务器失败");
		
		}
		public void logs(string log)
		{
			tblog.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {log}\r\n");
		}
	}
}
