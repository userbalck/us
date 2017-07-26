using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NCTest.ncserver
{
	class ncServer
	{
		

		public void ncservic(string IP, string msg) {

			TcpListener listener = new TcpListener(IP,1234);
			listener.Start();

			while (true)

			{
				TcpClient client = listener.AcceptTcpClient();//接受一个Client  
				byte[] buffer = new byte[client.ReceiveBufferSize];
				NetworkStream stream = client.GetStream();//获取网络流  
				stream.Read(buffer, 0, buffer.Length);//读取网络流中的数据  
				stream.Close();//关闭流  
				client.Close();//关闭Client  
				receiveString = Encoding.Default.GetString(buffer).Trim('\0');//转换成字符串  
				Console.WriteLine(receiveString);
			}

			listener.Stop();
		}
		public void nckh(string ip,string msg) {
			TcpClient client = new TcpClient(ip, 1234);
			NetworkStream sendStream = client.GetStream();
			Byte[] sendBytes = Encoding.Default.GetBytes(msg);
			sendStream.Write(sendBytes, 0, sendBytes.Length);
			sendStream.Flush();
			sendStream.Close();//关闭网络流  
			client.Close();//关闭客户端
		}
		}
}
