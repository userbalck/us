using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using log4net;

namespace Nopedemo2
{
    public partial class Form1 : Form
    {

		#region  字段 全局变量
		log4net.ILog log = log4net.LogManager.GetLogger("Nopedemo02");
        private bool isSaved = true; //是否保持标示符，true表示已保存。false=未保存
        private string curenFilepath;  //当前保存路径初始为null
        #endregion
        public Form1()
        {
            InitializeComponent();
			

        }
        private void tsmiNew_Click(object sender, EventArgs e)
		{  //新建
			bool isContinue = true;  //保存是否，true，新建保存	
			if (!isSaved)
            {
				
                //说明文本有内容，提示是否保持
                DialogResult result = MessageBox.Show("文件内容被修改，是否保持？", "保存提示",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
					//选择
                    case DialogResult.Cancel:
                        isContinue = false;
                        break;        
                    case DialogResult.Yes:
                        tsmiSave_Click(sender,e);
                        //写保存文档代码
                        break;

                }


            }
            if (isContinue)//  表示当前文件以保存
            {
                rtxtNotead.Text = string.Empty;  //清空
                this.Text = "新建文本文档-记事本";
                curenFilepath=null;
            }

        }
        private void tsmiOpen_Click(object sender, EventArgs e)
		{//打开
			bool isContinue = true;  //true，新建，
            
            if (!isSaved)
            {
                //说明文本有内容，提示是否保持
                DialogResult result = MessageBox.Show("文件内容被修改，是否保持？", "保存提示",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {


                    case DialogResult.Cancel:
                        isContinue = false;
                        break;
                    case DialogResult.Yes:
                        //写保存文档代码
                        tsmiSave_Click(sender, e);
                        break;
                }


            }
            if (isContinue)
            {
                OpenFileDialog openFileDilog = new OpenFileDialog();
                openFileDilog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
               
                openFileDilog.Title = "打开文件";
                //打开文件，点击OK后就是开始读取文件
                if (openFileDilog.ShowDialog()==DialogResult.OK)
                {
                    curenFilepath = openFileDilog.FileName;  // 获取打开的文件路径
                    //读取，设置编码
                    StreamReader reader = new StreamReader(openFileDilog.FileName,Encoding.Default);
                    rtxtNotead.Text = reader.ReadToEnd();  //读取所有文件
                    reader.Close();//关流
                    //读取文件标题
                    this.Text = Path.GetFileNameWithoutExtension(openFileDilog.FileName)+"记事本";
                }

            }
        }
		
        private void tsmiSave_Click(object sender, EventArgs e)
        {//保存
			savaPath();

		}
		//保存函数
		private  string GetFilePath() {
            //清空文本
            string filePath = string.Empty;
            SaveFileDialog savaFile = new SaveFileDialog();
            savaFile.Filter= "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            savaFile.Title = "保存文件";
            if (savaFile.ShowDialog()==DialogResult.OK)
            {
                filePath = savaFile.FileName;
            }
            return filePath;
        }

        private void tsmiFie_Click(object sender, EventArgs e)
        {
            //文件

        }
		//另存为保持函数
		private void savaPath() {
			if (curenFilepath == null)
			{
				curenFilepath = GetFilePath();
			}
			//判断路径，写入路径
			if (curenFilepath.Length > 0)
			{
				StreamWriter writer = new StreamWriter(curenFilepath, false, Encoding.Unicode);
				writer.Write(rtxtNotead.Text);
				writer.Close();
				isSaved = true;
			}
			else
			{
				//路径为空
				curenFilepath = null;
			}

		}


        private void rtxtNotead_TextChanged(object sender, EventArgs e)
        {

			//多格式文本框      文本被修改，表示未保存
			isSaved = false;

		}



        private void Form1_Load(object sender, EventArgs e)
        {
            //外大框
        }

        private void tsmiClose_Click(object sender, EventArgs e)
        {
			
            //退出
            Application.Exit();  // 程序退出
        }

        private void tsmiEdit_Click(object sender, EventArgs e)
        {
            //编辑
        }

        private void tsmlookup_Click(object sender, EventArgs e)
        {
            //查找
           
        }

        private void tsmiSeAs_Click(object sender, EventArgs e)
		{        //另存为	
				
			savaPath();//保持
		}

        private void tsmiUndo_Click(object sender, EventArgs e)
        {
            //撤销
            rtxtNotead.Undo();
        }

        private void tsmiCut_Click(object sender, EventArgs e)
        {
            //剪切
            rtxtNotead.Cut();

        }

        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            //复制
            rtxtNotead.Copy();
        }

        private void tsmiPaste_Click(object sender, EventArgs e)
        {
            //粘贴
            rtxtNotead.Paste();
        }

        private void tsmiSelectAll_Click(object sender, EventArgs e)
        {
            //全选
            rtxtNotead.SelectAll();
        }

        private void tsmiDate_Click(object sender, EventArgs e)
        {
            //时间日期
            rtxtNotead.AppendText(System.DateTime.Now.ToString());
        }

		private void tsmiAbout_Click(object sender, EventArgs e)
		{
			//关于
			//MessageBox.Show
			string stb = "版本: 2.2.01    \n版权所有: @2017-2099 粟敏保留所有权。   \n本产品使用权: \nhttp://www.cnblogs.com/hs22/";
			MessageBox.Show(stb);
		}

		private void tsmiHelpp_Click(object sender, EventArgs e)
		{
			string st = "仅供学习，\n严禁商业用途。\n";
			MessageBox.Show(st);
		}
	}
}
