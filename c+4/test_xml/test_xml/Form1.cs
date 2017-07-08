using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using System.IO;

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
			butJson.Click += ButJson_Click;
			butWr.Click += ButWr_Click;
		}

		private void ButWr_Click(object sender, EventArgs e)
		{
			jsWr();
		}

		//写json
		public void jsWr()
		{
			StringWriter sw = new StringWriter();
			JsonWriter writer = new JsonTextWriter(sw);
			String iput = "knoljdas";
			oulog("===iput=====" + iput);
			writer.WriteStartObject();
			writer.WritePropertyName(iput);
			writer.WriteValue("value");
			writer.WritePropertyName("output");
			writer.WriteValue("result");
			writer.WriteEndObject();
			writer.Flush();

			string jsonText = sw.GetStringBuilder().ToString();
			oulog("1111" + jsonText);
			Console.WriteLine(jsonText);
		}
		//打开json
		private void ButJson_Click(object sender, EventArgs e)
		{

			JsTest();
			//JOTest();
			//JoTest2();


		}
		//打开json并解析树
		public void JsTest()
		{
			tvXml.Nodes.Clear();
			String test_json = null;
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog()==DialogResult.OK)
			{
				String path=ofd.FileName;
				oulog("luj==="+path);
				StreamReader sr = new StreamReader(ofd.FileName,Encoding.Default);
				test_json = sr.ReadToEnd();
				sr.Close();
				oulog("var va==" + test_json);

				//test_json = "{\"name\":\"tom\",\"nickname\":\"tony\",\"sex\":\"male\",\"age\":20,\"email\":\"123@123.com\"}";
				var va = JObject.Parse(test_json);
				//遍历，添加到集合
				foreach (JToken child in va.Children())
				{
					var property1 = child as JProperty;
					TreeNode node = tvXml.Nodes.Add(property1.Name.ToString());
					TreeNode node2 = new TreeNode(property1.Value.ToString());
					node.Nodes.Add(node2);
					oulog("node" + node);
					oulog("" + node2);
				}
			}


			 
		

		}

		private void BtCreate_Click(object sender, EventArgs e)
		{

			oulog("添加节点");

			String  s = tbi1.Text.ToString();
			String s2 = tbi2.Text.ToString();
			oulog(""+s+"-------"+s2);
			if (s.Equals(null))
			{
				MessageBox.Show("请忽输入非法值");
			}
			else
			{
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


		}
			//读
			private void JoTest2()
		{
			string jsonText2 = @"{""input"" : ""value"", ""output"" : ""result""}";
			 jsonText2= @"{""input"" : ""value"", ""output"" : ""result""}";
			JsonReader reader = new JsonTextReader(new System.IO.StringReader(jsonText2));
			int i = 0;
			while (reader.Read())
			{
				i++;
				
			
				oulog(i + "Value======" + reader.Value);
				
			}
		}
		//直接解析值
		private void JOTest()
		{
			
			string jsonText = @"{""input"" : ""value"", ""output"" : ""result""}";
			JObject obj = JObject.Parse("{\"h\":\"Hello world!!!\"}");
			string jsH=((string)obj["h"]);
			oulog("H===" + jsH);

			JObject jobj2 = JObject.Parse(jsonText);
			jsH= ((string)jobj2["input"]);
			oulog("H===" + jsH);
			

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
			JsTest();
			OpenFileDialog ofd = new OpenFileDialog();

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				stPath = ofd.FileName;
				//xmldode();
				
				
			}
			
		}
		



		//xml -解析
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



		



		//添加日志
		private void oulog(string log)
		{

			
			
				//添加日
				txlog.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {log}\r\n");


			
		}
	}




}
