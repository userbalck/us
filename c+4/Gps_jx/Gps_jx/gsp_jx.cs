using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using log4net;
namespace Gps_jx
{
	public partial class gsp_jx : Form
	{
		log4net.ILog log = log4net.LogManager.GetLogger("Gps_jx");

		Thread T;
		string CurFile;
		string[] datas;
		List<float> lngs = new List<float>();
		List<float> lats = new List<float>();
		List<float> Speeds = new List<float>();
		List<DateTime> dates = new List<DateTime>();
		List<float> Distances = new List<float>();

		public gsp_jx()
		{
			log.Info("-----------------Form1()--------------------------");
			InitializeComponent();
			open_Gps.Click += Open_Gps_Click;
			FormClosing += Form1_FormClosing;
			CheckForIllegalCrossThreadCalls = false;
			NewGps.Click += NewGps_Click;
		}

		private void NewGps_Click(object sender, EventArgs e)
		{
			//打开另外一个gps——jx
			GPS_jx_excel gps_excel = new GPS_jx_excel();
			gps_excel.Show();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			T.Abort();
		}

		private void Open_Gps_Click(object sender, EventArgs e)
		{
			log.Info("open_gps-click");
			OpenFileDialog openFile = new OpenFileDialog();
			openFile.FileName = "GpsData-000000002";

			//判断打开
			if (openFile.ShowDialog() == DialogResult.OK)
			{
				//读取
				//reader = new StreamReader(openFile.FileName,Encoding.Default);
				Logg($"File[{openFile.FileName}] opend !");
				CurFile = openFile.FileName;
				log.Info("CurFile=====" + CurFile);
				//集合移除所有元素
				lngs.Clear(); lats.Clear(); Speeds.Clear(); dates.Clear(); Distances.Clear();
				//开启线程
				T = new Thread(new ThreadStart(AnaysinsGpsFile));
				T.Start();
			}


		}

		private void AnaysinsGpsFile()
		{
			StreamReader sr = new StreamReader(CurFile);
			log.Info("sr  =" + sr);
			string gpsdata = sr.ReadToEnd();
			string[] gpsdatas = gpsdata.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			sr.Close();
			int lastpab = 0;
			for (int i = 0; i < gpsdatas.Length; i++)
			{
				int p = (i + 1) * 100 / gpsdatas.Length;

				if (p != lastpab)
				{
					log.Info("p ==" + p);
					psb_Gps.Value = p;
					lastpab = p;
					log.Info("lastpab====" + lastpab);
				}
				//以，分隔
				datas = gpsdatas[i].Split(new string[] { "," }, StringSplitOptions.None);
				if (datas[0] == "$GNRMC")
				{
					//$GNRMC,214756.000,A,3048.8798,N,10410.8829,E,3.51,281.00,131016,,,A * 75
					log.Info("datas[0]====" + datas[0]);
					if (datas[2] == "A")
					{
						log.Info("lngs====" + lngs);
						//lngs.Add(float.Parse(datas[3]));  //string 转换为flost


						Logg($"File[{datas[0] + " " + datas[1] + " " + datas[2] + " " + datas[3] + " " + datas[4] + " " + datas[5] + " " + datas[6] + " " + datas[7] + " " + datas[8] + " " + datas[9] + " " + datas[10] + " " + datas[11] + " " + datas[12]}]");
						

					}
					else
					{
						Logg("无效数据");

					}
				}

			}
		}


		private void Logg(string log)
		{
			//写gps到txt，获取起始点，滚动到结束的
			Gps_text.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {log}|\r\n");
			Gps_text.SelectionStart = Gps_text.Text.Length;
			Gps_text.ScrollToCaret();
		}
		//地球半径，单位米
		private const double EARTH_RADIUS = 6378137;

		/// <summary>
		/// 经纬度转化成弧度
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		/// 经纬度转化成弧度
		private double rad(double d)
		{
			return d * Math.PI / 180.0;
		}


		//计算经纬度的距离
		/// <summary>
		/// 计算两点位置的距离，返回两点的距离，单位 米
		/// 该公式为GOOGLE提供，误差小于0.2米
		/// </summary>
		/// <param name="lat1">第一点纬度</param>
		/// <param name="lng1">第一点经度</param>
		/// <param name="lat2">第二点纬度</param>
		/// <param name="lng2">第二点经度</param>
		/// <returns></returns>
		public double GetDistance(double lat1, double lng1, double lat2, double lng2)
		{
			double radLat1 = rad(lat1);
			double radLat2 = rad(lat2);
			double a = radLat1 - radLat2;
			double b = rad(lng1) - rad(lng2);

			double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
			 Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
			s = s * EARTH_RADIUS;
			s = Math.Round(s * 10000) / 10000;
			return s;
		}
	}
}
