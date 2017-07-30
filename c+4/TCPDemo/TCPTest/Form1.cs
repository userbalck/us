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
		private TcpListener tcplistener;  //监听TCP网络客户端连接
		private TcpClient tcpClien;  //提供tcp连接
		public BinaryReader br;
		public BinaryWriter bw;
		string hostip;

		public clikBt()
		{
			InitializeComponent();
			butSTATR.Click += ButSTATR_Click;  //启动服务器
			butset.Click += Butset_Click;   
			clikbtt.Click += Clikbtt_Click;  
			settClit.Click += SettClit_Click;
			StopServer.Click += StopServer_Click;
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
			logs("stop");
		}

		//发送消息
		private void SettClit_Click(object sender, EventArgs e)
		{
		
			
		}

		//客户端
		private void Clikbtt_Click(object sender, EventArgs e)
		{
		
		}

		//服务端

		//发送
		private void Butset_Click(object sender, EventArgs e)
		{
			


		}
	

		private void ButSTATR_Click(object sender, EventArgs e)
		{
			//tcpClien = new TcpClient(hostip,5252);
			long lgs = 15454.4564;
			logs("ip"+hostip);
			//long  host ='192.168.20.61';
			 long host = Convert.ToInt64(hostip);
			tcplistener = new TcpListener(new System.Net.IPEndPoint(host, 5252));
			tcplistener.Start();
			stateLab.Text = "服务启动";
			logs("start====服务启动" + hostip);
			while (true)
			{
				tcpClien = tcplistener.AcceptTcpClient();
				byte[] buffer = new byte[tcpClien.ReceiveBufferSize];
				NetworkStream network = tcpClien.GetStream();   //获取网络流
				network.Read(buffer,0,buffer.Length);  //获取流的总大小
				network.Close();
				tcpClien.Close();
			String	receiveString = Encoding.Default.GetString(buffer).Trim('\0');//转换成字符串 
				logs("数据"+receiveString);
			}

		
		}
		public void logs(string log) {


			tblogs.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {log}\r\n");
		}










		
	}
}
