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

namespace WindowsFormsApp1
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

		}

		public void Tree()
		{
			TreeNodeCollection roots = this.tv.Nodes;
			//First.Nodes
			TreeNode first = roots[0].Nodes.Add("First");
			first.Nodes.RemoveAt(0);
			first.Nodes.Remove(first);
			//first.Parent.Parent.Nodes

			this.tv.LabelEdit = true;
			this.tv.AfterLabelEdit += Tv_AfterLabelEdit;

		}

		private void Tv_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			e.CancelEdit = true;
			string newlabel = e.Label;
			string oldlabel = e.Node.Text;
		}

		public void ReadXML()
		{
			//http://www.cnblogs.com/a1656344531/archive/2012/11/28/2792863.html
			List<Book> books = new List<Book>();
			XmlDocument xml = new XmlDocument();
			xml.Load("filepath");
			XmlElement root = xml.DocumentElement;
			for (int i = 0; i < root.ChildNodes.Count; i++)
			{
				XmlNode booknode = root.ChildNodes[i];
				Book book = new Book();
				for (int j = 0; j < booknode.ChildNodes.Count; j++)
				{
					XmlNode p = booknode.ChildNodes[i];
					if (p.Name == "Title") book.Title = p.Value;
					XmlNode nd = xml.CreateElement("addr");
					nd.Value = "..&<>...";
					p.AppendChild(nd);
					p.RemoveChild(nd);
				}
				books.Add(book);
			}
			/////////////
			XmlNode node = xml.SelectSingleNode("/bookstore/book[2]/title");
			XmlNodeList lst = xml.SelectNodes("/bookstore/book/title");
			string allxml = xml.OuterXml;
			xml.Save("filepath");
		}

		public class Book
		{
			public string Title;
			public string Author;
			public string Price;
			public string Type;
			public string ISBN;
		}

		public void JSON()
		{
			JObject jo = new JObject();
			JArray ja = new JArray();
			string title;
			jo = JObject.Parse("json string");
			//JToken jt = jo["bookstore"];
			JArray bookstore = (JArray)jo["bookstore"];
			for (int i = 0; i < bookstore.Count; i++)
			{
				JObject book = (JObject)bookstore[i];
				title = book["title"].ToString();
			}

			title =
				((JArray)jo["bookstore"])[0]["title"].ToString();
			string name = ((JArray)jo["root"])[0]["person"]["sex"].ToString();

			jo.Add("addr", @"..""..");
			jo.Add("name", "新华\"书店");
			jo.Remove("addr");
			string str = $".{title}..\r\n.";
			str = @"
			{
				 book:{
					 title :""title string""
					 }
				}
";
		}
	}
}
