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
		public Form1()
		{
			InitializeComponent();
			btCreate.Click += BtCreate_Click;
			btOpen.Click += BtOpen_Click;
		}

		private void BtOpen_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
		
			if (ofd.ShowDialog()==DialogResult.OK)
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
			int i1 = Convert.ToInt32(tbi1.Text);  //父节点
			int i2 = Convert.ToInt32(tbi2.Text);
			for (int i = 0; i < i1; i++)
			{
				TreeNode node = tvXml.Nodes.Add("父节点"+(i+1).ToString());
				oulog("添加父节点 :");
				for (int j = 0; j< i2; j++)
				{
					TreeNode node2 = new TreeNode("子节点"+ (i + 1).ToString());
					node.Nodes.Add(node2);
				}

			}

			
		}




		private void oulog(string v)
		{
			if (tbLog.GetLineFromCharIndex(tbLog.Text.Length)>100)
			{
				tbLog.Text = "";
				tbLog.AppendText(DateTime.Now.ToString("HH:mm:ss")+v+"\r\n");
				
			}
		}
	}
}
