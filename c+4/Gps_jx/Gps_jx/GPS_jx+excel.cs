using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gps_jx
{
	public partial class GPS_jx_excel : Form
	{
		String Curle;
		List<GPSPoint> gps_list = new List<GPSPoint>();
		Thread T;
		log4net.ILog log4 = log4net.LogManager.GetLogger("GPS_jx_excel");
		public GPS_jx_excel()
		{
			InitializeComponent();
			open_gps.Click += Open_gps_Click;
			FormClosing += GPS_jx_excel_FormClosing;
		}

		private void GPS_jx_excel_FormClosing(object sender, FormClosingEventArgs e)
		{
			try { T.Abort(); }
			catch (Exception) { }
		}

		private void Open_gps_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.FileName = "gsp-000000000000000";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				Log($"File[{ofd.FileName}] opend ！");
				log4.Info("da	kai	wen j");
				Curle = ofd.FileName;
				gps_list.Clear();
				log4.Info("new thread   jin	ru	xian	cheng");
				T = new Thread(new ThreadStart(this.AnaysisGpsFile));
				T.Start();





			}
		}

		private void AnaysisGpsFile()
		{
			log4.Info("jing  ru  xian cheng");
			StreamReader srd = new StreamReader(Curle);
			string gpsdata = srd.ReadToEnd();
			gps_list = GPSPoint.Analysis(gpsdata);
			string excel = "时间,精度,纬度,速度,距离";
			int stati = 0;
			for (int i = 0; i < gps_list.Count; i++)
			{

				int p = (i + 1) * 100 / gps_list.Count;
				if (p != stati)
				{
					log4.Info("p ==" + p);
					pb_Gps.Value = p;
					stati = p;



					excel += $"\r\n{gps_list[i].Date.ToString("yyyy-MM-dd HH:mm:ss") }" +
						$",{ gps_list[i].Lng},{gps_list[i].Lat },{gps_list[i].Speed}" +
						$",{gps_list[i].Distance}";
				}
				StreamWriter sw = new StreamWriter(
					$"{Application.StartupPath}\\123.csv", false, Encoding.UTF8);
				sw.Write(excel);
				sw.Close();
			}
		}
		private void Log(string logs)
		{
			//写gps到txt，获取起始点，滚动到结束的
			text_jgs.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {logs}|\r\n");
			text_jgs.SelectionStart = text_jgs.Text.Length;
			text_jgs.ScrollToCaret();
		}

	}
}
