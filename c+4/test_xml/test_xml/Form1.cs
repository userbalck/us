using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using test_xml;
using System.Xml;

namespace test_xml
{
	public partial class Form1 : Form
	{
		String stPath;
		int i1;
		int i2;
		public Form1()
		{
			InitializeComponent();
			btCreate.Click += BtCreate_Click;
			btOpen.Click += BtOpen_Click;
			tbi1.KeyPress += Tbi1_KeyPress;
			tbi2.KeyPress += Tbi2_KeyPress;
		}
		//只允许输入数字
		private void Tbi2_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)8)
			{
				e.Handled = true;
			}
		}
		//只允许输入数字
		private void Tbi1_KeyPress(object sender, KeyPressEventArgs e)
		{
		
			if (!char.IsNumber(e.KeyChar) && e.KeyChar != (char)8)
			{
				e.Handled = true;
			}
		}

		private void BtOpen_Click(object sender, EventArgs e)
		{
			oulog("打开文件");
			OpenFileDialog ofd = new OpenFileDialog();

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				stPath = ofd.FileName;
				xmldode();
			}
		}

		private void xmldode()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(stPath);
			XmlNode xn = doc.SelectSingleNode("bookstore");
			XmlNodeList xnl = xn.ChildNodes;
			foreach (XmlNode xml in xnl)
			{

			}
		}


		/// <summary>
		/// 添加节点
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtCreate_Click(object sender, EventArgs e)
		{
			oulog("添加节点");
				i1 = Convert.ToInt32(tbi1.Text);  //父节点
				i2 = Convert.ToInt32(tbi2.Text);
			
				for (int i = 0; i < i1; i++)
				{
					TreeNode node = tvXml.Nodes.Add("父节点" + (i + 1).ToString());
					oulog("添加父节点 :");
					for (int j = 0; j < i2; j++)
					{
						TreeNode node2 = new TreeNode("子节点" + (i + 1).ToString());
						node.Nodes.Add(node2);
					}

				}
			


		}



		//添加日志
		private void oulog(string log)
		{
			
			if (txlog.GetLineFromCharIndex(txlog.Text.Length) > 100)
			{
				//添加日
				txlog.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {log}\r\n");


			}
		}
	}
}
