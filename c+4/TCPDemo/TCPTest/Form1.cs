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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Input;


namespace TCPTest
{
	public partial class clikBt : Form
	{
		private static TcpListener tcplistener;  //监听TCP网络客户端连接
		private static TcpClient tcpClien;  //提供tcp连接
		public BinaryReader br;
		public BinaryWriter bw;
		IPAddress ip;
		String st, setserver;
		NetworkStream sendStream;
		TcpClient client;
		string hostip;
	
		public clikBt()
		{
			InitializeComponent();
			butSTATR.Click += ButSTATR_Click;  //启动服务器
			butset.Click += Butset_Click;
			clikbtt.Click += Clikbtt_Click;
			settClit.Click += SettClit_Click;
			StopServer.Click += StopServer_Click;
			stopclik.Click += Stopclik_Click;
			
			Initialization();
		}
		

		private void Initialization()
		{
			string strHostName = Dns.GetHostName(); //得到本机的主机名
			IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
			hostip = ipEntry.AddressList[0].ToString();
			label3.Text = hostip;
			portText.Text = "5252";
			stateLab.Text = "未启动";
		}

		private void StopServer_Click(object sender, EventArgs e)
		{
			tcplistener.Stop();
			stateLab.Text = "服务器关闭";
			logs("stop" + hostip + portText.Text);
		}

		//发送消息
		private void SettClit_Click(object sender, EventArgs e)
		{
			st = "jklsadflk";
			st = tbClik.Text;
			logs("发送到服务端"+st);
			sendStream = client.GetStream();
			Byte[] sendBytes = Encoding.Default.GetBytes(st);
			sendStream.Write(sendBytes, 0, sendBytes.Length);
			sendStream.Flush();
		}

		//客户端
		private void Clikbtt_Click(object sender, EventArgs e)
		{
			string postip = hostip;
			client = new TcpClient(postip, 5252);
			logs("开启客户端端");


		}
		//关闭客户端
		private void Stopclik_Click(object sender, EventArgs e)
		{
			sendStream.Close();//关闭网络流  
			client.Close();//关闭客户端  
			logs("关闭客户端端");
		
		}


		//发送服务端
		private void Butset_Click(object sender, EventArgs e)
		{
		setserver=setServer.Text;


		}

		//启动服务器
		private void ButSTATR_Click(object sender, EventArgs e)
		{
			logs("ip" + hostip);
			ip = IPAddress.Parse(hostip);
			tcplistener = new TcpListener(new System.Net.IPEndPoint(ip, 5252));
			tcplistener.Start();
			stateLab.Text = "服务启动";
			logs("start====服务启动" + hostip + portText.Text);
			Thread t = new Thread(thTCP);



		}
		//收数据线程
		public void thTCP()
		{
			while (true)
			{
				tcpClien = tcplistener.AcceptTcpClient();
				byte[] buffer = new byte[tcpClien.ReceiveBufferSize];
				NetworkStream network = tcpClien.GetStream();   //获取网络流
				network.Read(buffer, 0, buffer.Length);  //获取流的总大小
				network.Close();
				tcpClien.Close();
				String receiveString = Encoding.Default.GetString(buffer).Trim('\0');//转换成字符串 
				getsetver.Text = receiveString;
				logs("while数据" + receiveString);
			}
		}
		public void logs(string log)
		{


			tblogs.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {log}\r\n");
		}


	}
}
