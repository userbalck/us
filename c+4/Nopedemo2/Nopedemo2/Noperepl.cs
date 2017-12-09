using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nopedemo2
{
	public partial class Noperepl : Form
	{
		log4net.ILog log = log4net.LogManager.GetLogger("Noperepl");
		String st_Content = "";  //find
		String st_Rel = "";   //替换
		bool bo_CbSiz = false;  //勾选
		Nope nope;
		public Noperepl()
		{

			nope = new Nope();  //对象
			InitializeComponent();
			btfind2.Click += Btfind2_Click;
			tb_content.Click += Tb_content_Click;  //查找内容
			tb_content.TextChanged += Tb_content_TextChanged; //被改变
			FormClosing += Noperepl_FormClosing;

		}

		private void Noperepl_FormClosing(object sender, FormClosingEventArgs e)
		{//关闭窗体
			e.Cancel = true;
			Hide();  //隐藏

		}

		private void Tb_content_TextChanged(object sender, EventArgs e)
		{
			st_Content = tb_content.Text;
			if (st_Content == "")
			{
				log.Info("Tb_content_TextChange==" + st_Content);
				btfind2.Enabled = false;
				bt_Repl.Enabled = false;
				bt_Whole.Enabled = false;

			}
			else
			{
				log.Info("Tb_content_TextChange被改变==" + st_Content);
				btfind2.Enabled = true;
				bt_Repl.Enabled = true;
				bt_Whole.Enabled = true;
			}

		}

		private void Tb_content_Click(object sender, EventArgs e)
		{//输入内容框被点击
		}

		private void Btfind2_Click(object sender, EventArgs e)
		{  //查找被点击
			log.Info("查找的内容" + st_Content);

				nope.serch(st_Content, st_Rel, bo_CbSiz);



		}
	}


}
